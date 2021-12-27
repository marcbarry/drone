using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.IO;
using System.Threading;

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

        public static void Main(string[] args)
        {
            Console.WriteLine("hello");

            var mpu = new mpu9250();


            //var consoleLock = new object();

            //Console.WriteLine(mpu.isReady() ? "Device Ready": "Failure");

            //mpu.getGyros(data =>
            //{
            //    lock (consoleLock)
            //    {
            //        Console.Write("Gyros:\t");
            //        foreach (var item in data)
            //        {
            //            Console.Write($"{item} ");
            //        }
            //        Console.WriteLine();
            //    }
            //}, 100);

            //mpu.getAccel(data =>
            //{
            //    lock (consoleLock)
            //    {
            //        Console.Write("Accel:\t");
            //        foreach (var item in data)
            //        {
            //            Console.Write($"{item} ");
            //        }
            //        Console.WriteLine();
            //    }
            //}, 100);

            //mpu.getTemperature(data => { Console.WriteLine(data); }, 100);

            int counter = 0;

            try
            {
                mpu.getMotion6((accel, gyro, mag, temperature) =>
                {
                    Console.WriteLine($"{counter}: Motion6:\t" +
                                      $"accel [g]: x:{accel.X:f2} y:{accel.Y:f2} z:{accel.Z:f2}\t" +
                                      $"gyro [dps]: x:{gyro.X:f2} y:{gyro.Y:f2} z:{gyro.Z:f2}\t" +
                                      $"mag [uT]: x:{mag.X:f2} y:{mag.Y:f2} z:{mag.Z:f2}\t" +
                                      $"temp [c]: {temperature:f2}");

                    counter++;
                }, 25);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            return;

            using (var i2c = I2cDevice.Create(new I2cConnectionSettings(BusId, MPU6050_ADDR)))
            {
                byte samp_rate_div = 0;

                // alter sample rate (stability)
                i2c.WriteByte(SMPLRT_DIV);
                i2c.WriteByte(samp_rate_div);
                Thread.Sleep(10);
                Console.WriteLine($"{i2c.ReadByte():X2}");

                // reset all sensors
                i2c.WriteByte(PWR_MGMT_1);
                i2c.WriteByte(0x00);
                Thread.Sleep(10);
                Console.WriteLine($"{i2c.ReadByte():X2}");

                // power management and crystal settings
                i2c.WriteByte(PWR_MGMT_1);
                i2c.WriteByte(0x01);
                Thread.Sleep(10);
                Console.WriteLine($"{i2c.ReadByte():X2}");

                // write to configuration register
                i2c.WriteByte(CONFIG);
                i2c.WriteByte(0x00);
                Thread.Sleep(10);
                Console.WriteLine($"{i2c.ReadByte():X2}");

                // write to gyro configuration register
                var gyro_config_sel = new byte[] {0x00, 0x10, 0x10, 0x24}; // [0b00000, 0b010000, 0b10000, 0b11000]}; // byte registers
                var gyro_config_vals = new[] { 250.0, 500.0, 1000.0, 2000.0 }; // [250.0, 500.0, 1000.0, 2000.0]; // degrees/sec
                var gyro_indx = 0;

                i2c.WriteByte(GYRO_CONFIG);
                i2c.WriteByte(gyro_config_sel[gyro_indx]);
                Thread.Sleep(10);
                Console.WriteLine($"{i2c.ReadByte():X2}");

                // write to Accel configuration register
                var accel_config_sel = new byte[] { 0x00, 0x10, 0x10, 0x24 }; // [0b00000, 0b01000, 0b10000, 0b11000] # byte registers
                var accel_config_vals = new[] { 2.0, 4.0, 8.0, 16.0 }; // [2.0, 4.0, 8.0, 16.0] # g (g = 9.81 m/s^2)
                var accel_indx = 0;

                i2c.WriteByte(ACCEL_CONFIG);
                i2c.WriteByte(accel_config_sel[gyro_indx]);
                Thread.Sleep(10);
                Console.WriteLine($"{i2c.ReadByte():X2}");

                // interrupt register (related to overflow of data [FIFO])
                i2c.WriteByte(INT_ENABLE);
                i2c.WriteByte(1);
                Thread.Sleep(10);

                // start
                i2c.WriteByte(AK8963_ADDR);
                i2c.WriteByte(AK8963_CNTL);
                i2c.WriteByte(0x00);
                Thread.Sleep(10);
                Console.WriteLine($"{i2c.ReadByte():X2}");

                var AK8963_bit_res = 0x01; // 0b0001 # 0b0001 = 16-bit
                var AK8963_samp_rate = 0x06; // 0b0110 # 0b0010 = 8 Hz, 0b0110 = 100 Hz
                var AK8963_mode = (byte)((AK8963_bit_res << 4) + AK8963_samp_rate); // # bit conversion

                i2c.WriteByte(AK8963_ADDR);
                i2c.WriteByte(AK8963_CNTL);
                i2c.WriteByte(AK8963_mode);
                Thread.Sleep(10);
                Console.WriteLine($"{i2c.ReadByte():X2}");

                while (true)
                {
                    var acc_x = i2c.ReadByte(); //read_raw_bits(ACCEL_XOUT_H)
                    var acc_y = i2c.ReadByte(); //read_raw_bits(ACCEL_YOUT_H)
                    var acc_z = i2c.ReadByte(); //read_raw_bits(ACCEL_ZOUT_H)

                    Console.WriteLine($"ACCEL_XOUT_H: {i2c.ReadByte()}:X2");
                    Console.WriteLine($"ACCEL_YOUT_H: {i2c.ReadByte()}:X2");
                    Console.WriteLine($"ACCEL_ZOUT_H: {i2c.ReadByte()}:X2");

                    //Console.WriteLine($"{i2c.ReadByte():X2}");
                    Thread.Sleep(10);
                }
            }

            Console.WriteLine("done");
        }

        /// <summary>
        /// Scan I2C bus for devices. First 8 I2C addresses are reserved, last one is 0x7F.
        /// </summary>
        /// <returns>Active device addresses</returns>
        public static List<int> Scan(int busId)
        {
            var validAddresses = new List<int>();

            for (var i = 8; i < 0x80; i++)
            {
                try
                {
                    var i2c = I2cDevice.Create(new I2cConnectionSettings(busId, i));
                    var read = i2c.ReadByte();
                    validAddresses.Add(i);
                }
                catch (IOException)
                {
                    // do nothing, there is just no device
                }
            }

            return validAddresses;
        }
    }
}
