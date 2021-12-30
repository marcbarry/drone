using System;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

namespace mpu9250
{
    /// <summary>
    /// https://github.com/dotnet/iot/blob/main/Documentation/raspi-i2c.md
    /// 
    /// https://github.com/marcbarry/drone/blob/main/mpu9250_py/mpu9250.py#L10
    /// https://github.com/marcbarry/drone/blob/main/mpu9250_py/mpu9250_i2c.py#L38
    /// https://github.com/kplindegaard/smbus2/blob/70be77db7542625cfe7ea238283482c69fb34646/smbus2/smbus2.py#L433
    /// https://github.com/kplindegaard/smbus2/blob/70be77db7542625cfe7ea238283482c69fb34646/smbus2/smbus2.py#L340
    ///
    /// https://github.com/dotnet/iot/blob/main/src/System.Device.Gpio/System/Device/I2c/UnixI2cBus.cs#L88
    /// https://github.com/dotnet/iot/blob/main/src/System.Device.Gpio/System/Device/I2c/Devices/UnixI2cDevice.cs#L6
    /// 
    /// https://stackoverflow.com/questions/17317317/using-python-smbus-on-a-raspberry-pi-confused-with-syntax
    /// </summary>
    public static class Program
    {
        public static byte BusId = 1;
        public static byte MPU6050_ADDR = 0x68;

        // MPU6050 Registers
        public static byte PWR_MGMT_1 = 0x6B;
        public static byte SMPLRT_DIV = 0x19;
        public static byte CONFIG = 0x1A;
        public static byte GYRO_CONFIG = 0x1B;
        public static byte ACCEL_CONFIG = 0x1C;
        public static byte INT_ENABLE = 0x38;
        public static byte ACCEL_XOUT_H = 0x3B;
        public static byte ACCEL_YOUT_H = 0x3D;
        public static byte ACCEL_ZOUT_H = 0x3F;
        public static byte TEMP_OUT_H = 0x41;
        public static byte GYRO_XOUT_H = 0x43;
        public static byte GYRO_YOUT_H = 0x45;
        public static byte GYRO_ZOUT_H = 0x47;

        // AK8963 registers
        public static byte AK8963_ADDR = 0x0C;
        public static byte AK8963_ST1 = 0x02;
        public static byte AK8963_ST2 = 0x09;
        public static byte AK8963_CNTL = 0x0A;
        public static double mag_sens = 4900.0; // # magnetometer sensitivity: 4800 uT

        public static InfluxDBClient client;


        public static void Main(string[] args)
        {
            // You can generate an API token from the "API Tokens Tab" in the UI
            const string token = "";
            const string bucket = "";
            const string org = "";

            //const string data = "mem,host=host1 used_percent=23.43234543";

            //using (var writeApi = client.GetWriteApi())
            //{
            //    writeApi.WriteRecord(bucket, org, WritePrecision.Ns, data);
            //}


            client = InfluxDBClientFactory.Create("http://localhost:8086", token); // home nas

            KalmanTest();

            var mpu = new mpu9250();

            var counter = 0;

            try
            {
                KalmanFilter gyroYFilter = null;

                mpu.getMotion6((accel, gyro, mag, temperature) =>
                {
                    if (gyroYFilter == null)
                    {
                        gyroYFilter = new KalmanFilter(gyro.Y);
                    }

                    var kalmanGyroY = gyroYFilter.Smooth(gyro.Y);

                    // Sample every 25th of a second. Update four times a second
                    if (counter % 10 == 0)
                    {
                        try
                        {
                            Console.WriteLine($"accel [g]: x:{accel.X:f2} y:{accel.Y:f2} z:{accel.Z:f2}\t" +
                                              $"gyro [dps]: x:{gyro.X:f2} y:{kalmanGyroY:f2} z:{gyro.Z:f2}\t" +
                                              $"mag [uT]: x:{mag.X:f2} y:{mag.Y:f2} z:{mag.Z:f2}\t" +
                                              $"roll: {mpu.getRoll(gyro.Y, gyro.Z):f2} " +
                                              $"pitch: {mpu.getPitch(gyro.X, gyro.Z):f2} " +
                                              $"yaw: {mpu.getYaw(mag.Y, mag.X):f2} " +
                                              $"temp [c]: {temperature:f2}");

                            var accelerationPoints = PointData
                                .Measurement("acceleration")
                                .Tag("host", "rpi4b")
                                .Field("X", accel.X)
                                .Field("Y", accel.Y)
                                .Field("Z", accel.Z)
                                .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

                            var gyroscopePoints = PointData
                                .Measurement("gyroscope")
                                .Tag("host", "rpi4b")
                                .Field("X", gyro.X)
                                .Field("Y", gyro.Y)
                                .Field("Z", gyro.Z)
                                .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

                            var magnetometerPoints = PointData
                                .Measurement("magnetometer")
                                .Tag("host", "rpi4b")
                                .Field("X", mag.X)
                                .Field("Y", mag.Y)
                                .Field("Z", mag.Z)
                                .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

                            var orientationPoints = PointData
                                .Measurement("orientation")
                                .Tag("host", "rpi4b")
                                .Field("Roll", mpu.getRoll(gyro.Y, gyro.Z))
                                .Field("Pitch", mpu.getPitch(gyro.X, gyro.Z))
                                .Field("Yaw", mpu.getYaw(mag.Y, mag.X))
                                .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

                            var temperaturePoints = PointData
                                .Measurement("temperature")
                                .Tag("host", "rpi4b")
                                .Field("Celsius", temperature)
                                .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

                            using var writeApi = client.GetWriteApi();

                            writeApi.WritePoint(bucket, org, accelerationPoints);
                            writeApi.WritePoint(bucket, org, gyroscopePoints);
                            writeApi.WritePoint(bucket, org, magnetometerPoints);
                            writeApi.WritePoint(bucket, org, temperaturePoints);
                            writeApi.WritePoint(bucket, org, orientationPoints);

                            counter = 0;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }

                    counter++;
                }, 25);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void KalmanTest()
        {
            var filter = new KalmanFilter(0.4, 0);

            double[] noisySine = { 40, 41, 38, 40, 45, 42, 43, 44, 40, 38, 160, 45, 40, 39, 37, 41, 42, 70, 44, 42 };

            foreach (var t in noisySine)
            {
                Console.WriteLine(t + " " + filter.Smooth(t));
            }
        }
    }
}
