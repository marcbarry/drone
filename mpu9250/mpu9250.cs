using System;
using System.Threading.Tasks;
using System.Device.I2c;
using System.Diagnostics;
using System.Collections;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using Iot.Device.Board;
using Iot.Device.Magnetometer;

namespace mpu9250
{
    /// <summary>
    /// Source https://raw.githubusercontent.com/miniben-90/MPU9250-IoT/master/mpu9250.cs
    /// </summary> 
    class mpu9250
    {
        public delegate void mpuCallbackData(Vector3 accel, Vector3 gyro, Vector3 mag, double temperature);
        public delegate void mpuCallbackTemp(Double data);

        private static byte BUS_ID = 1,
                            WIA = 0x00, // WIA: Device ID
                            MPU6050_ADDRESS = 0x68,
                            AK8963_ADDRESS = 0x0c,
                            AK8963_ST2 = 0x09,
                            AK8963_CNTL = 0x0A,
                            HXH = 0x04,
                            HYH = 0x06,
                            HZH = 0x08,
                            SELF_TEST_X_GYRO = 0x00,
                            SELF_TEST_Y_GYRO = 0x01,
                            SELF_TEST_Z_GYRO = 0x02,
                            SELF_TEST_X_ACCEL = 0x0D,
                            SELF_TEST_Y_ACCEL = 0x0E,
                            SELF_TEST_Z_ACCEL = 0x0F,
                            XG_OFFSET_H = 0x13,
                            XG_OFFSET_L = 0x14,
                            YG_OFFSET_H = 0x15,
                            YG_OFFSET_L = 0x16,
                            ZG_OFFSET_H = 0x17,
                            ZG_OFFSET_L = 0x18,
                            SMPLRT_DIV = 0x19,
                            CONFIG = 0x1A,
                            GYRO_CONFIG = 0x1B,
                            ACCEL_CONFIG = 0x1C,
                            ACCEL_CONFIG_2 = 0x1D,
                            LP_ACCEL_ODR = 0x1E,
                            WOM_THR = 0x1F,
                            FIFO_EN = 0x23,
                            I2C_MST_CTRL = 0x24,
                            I2C_SLV0_ADDR = 0x25,
                            I2C_SLV0_REG = 0x26,
                            I2C_SLV0_CTRL = 0x27,
                            I2C_SLV1_ADDR = 0x28,
                            I2C_SLV1_REG = 0x29,
                            I2C_SLV1_CTRL = 0x2A,
                            I2C_SLV2_ADDR = 0x2B,
                            I2C_SLV2_REG = 0x2C,
                            I2C_SLV2_CTRL = 0x2D,
                            I2C_SLV3_ADDR = 0x2E,
                            I2C_SLV3_REG = 0x2F,
                            I2C_SLV3_CTRL = 0x30,
                            I2C_SLV4_ADDR = 0x31,
                            I2C_SLV4_REG = 0x32,
                            I2C_SLV4_DO = 0x33,
                            I2C_SLV4_CTRL = 0x34,
                            I2C_SLV4_DI = 0x35,
                            I2C_MST_STATUS = 0x36,
                            INT_PIN_CFG = 0x37,
                            INT_ENABLE = 0x38,
                            INT_STATUS = 0x3A,
                            ACCEL_XOUT_H = 0x3B,
                            ACCEL_XOUT_L = 0x3C,
                            ACCEL_YOUT_H = 0x3D,
                            ACCEL_YOUT_L = 0x3E,
                            ACCEL_ZOUT_H = 0x3F,
                            ACCEL_ZOUT_L = 0x40,
                            TEMP_OUT_H = 0x41,
                            TEMP_OUT_L = 0x42,
                            GYRO_XOUT_H = 0x43,
                            GYRO_XOUT_L = 0x44,
                            GYRO_YOUT_H = 0x45,
                            GYRO_YOUT_L = 0x46,
                            GYRO_ZOUT_H = 0x47,
                            GYRO_ZOUT_L = 0x48,
                            EXT_SENS_DATA_00 = 0x49,
                            EXT_SENS_DATA_01 = 0x4A,
                            EXT_SENS_DATA_02 = 0x4B,
                            EXT_SENS_DATA_03 = 0x4C,
                            EXT_SENS_DATA_04 = 0x4D,
                            EXT_SENS_DATA_05 = 0x4E,
                            EXT_SENS_DATA_06 = 0x4F,
                            EXT_SENS_DATA_07 = 0x50,
                            EXT_SENS_DATA_08 = 0x51,
                            EXT_SENS_DATA_09 = 0x52,
                            EXT_SENS_DATA_10 = 0x53,
                            EXT_SENS_DATA_11 = 0x54,
                            EXT_SENS_DATA_12 = 0x55,
                            EXT_SENS_DATA_13 = 0x56,
                            EXT_SENS_DATA_14 = 0x57,
                            EXT_SENS_DATA_15 = 0x58,
                            EXT_SENS_DATA_16 = 0x59,
                            EXT_SENS_DATA_17 = 0x5A,
                            EXT_SENS_DATA_18 = 0x5B,
                            EXT_SENS_DATA_19 = 0x5C,
                            EXT_SENS_DATA_20 = 0x5D,
                            EXT_SENS_DATA_21 = 0x5E,
                            EXT_SENS_DATA_22 = 0x5F,
                            EXT_SENS_DATA_23 = 0x60,
                            I2C_SLV0_DO = 0x63,
                            I2C_SLV1_DO = 0x64,
                            I2C_SLV2_DO = 0x65,
                            I2C_SLV3_DO = 0x66,
                            I2C_MST_DELAY_CTRL = 0x67,
                            SIGNAL_PATH_RESET = 0x68,
                            MOT_DETECT_CTRL = 0x69,
                            USER_CTRL = 0x6A,
                            PWR_MGMT_1 = 0x6B,
                            PWR_MGMT_2 = 0x6C,
                            WHOAMI_VALUE = 0x71,
                            FIFO_COUNTH = 0x72,
                            FIFO_COUNTL = 0x73,
                            FIFO_R_W = 0x74,
                            WHO_AM_I = 0x75,
                            XA_OFFSET_H = 0x77,
                            XA_OFFSET_L = 0x78,
                            YA_OFFSET_H = 0x7A,
                            YA_OFFSET_L = 0x7B,
                            ZA_OFFSET_H = 0x7D,
                            ZA_OFFSET_L = 0x7E;


        public static int ACCEL_FS_2 = 0, GYRO_FS_250 = 0,
                            ACCEL_FS_4 = 1, GYRO_FS_500 = 1,
                            ACCEL_FS_8 = 2, GYRO_FS_1000 = 2,
                            ACCEL_FS_16 = 3, GYRO_FS_2000 = 3;

        public static int MAG_SENS = 4900; // magnetometer sensitivity: 4800 uT

        private I2cDevice mpu;
        private I2cDevice ak8963;
        private Boolean deviceReady = false;
        private bool debug = false;
        private Ak8963 magnetometer = null;
        
        /**
         * default config : accel FS 4, gyro FS 500, speed 400MHz auto clock source
         */
        public mpu9250()
        {
            this.debug = true;
            this.connection(ACCEL_FS_4, GYRO_FS_500, 13, 2);
        }

        private async void connection(int accel_fs, int gyro_fs, int i2cspeed, int clocksource)
        {
            try
            {
                //var i2CBus = I2cBus.Create(BUS_ID);

                //var connectedDevices = i2CBus.PerformBusScan();

                //Console.WriteLine($"Found {connectedDevices.Count} device{(connectedDevices.Count != 1 ? "s" : "")} on bus {BUS_ID}");

                //if (connectedDevices.Contains(MPU6050_ADDRESS) == false)
                //{
                //    Console.WriteLine($"MPU6050 Inertial Measurement Unit (IMU) not detected on bus {BUS_ID}");
                //    return;
                //}

                //if (connectedDevices.Contains(AK8963_ADDRESS) == false)
                //{
                //    Console.WriteLine($"AK8963 magnetometer not detected on bus {BUS_ID}");
                //    return;
                //}

                //Console.WriteLine($"Found MPU6050 Inertial Measurement Unit (IMU) on bus {BUS_ID} at address 0x{MPU6050_ADDRESS:X2}");
                //Console.WriteLine($"Found AK8963 Magnetometer on bus {BUS_ID} at address 0x{AK8963_ADDRESS:X2}");


                //
                //
                // 
                
                
                mpu = I2cDevice.Create(new I2cConnectionSettings(BUS_ID, MPU6050_ADDRESS));
                ak8963 = I2cDevice.Create(new I2cConnectionSettings(BUS_ID, AK8963_ADDRESS));

                // alter sample rate (stability)
                Console.WriteLine("SMPLRT_DIV");
                mpu.Write(new byte[] { SMPLRT_DIV, 0x00 });
                Task.Delay(100).Wait();

                // reset all sensors
                Console.WriteLine("PWR_MGMT_1");
                mpu.Write(new byte[] { PWR_MGMT_1, 0x00 });
                Task.Delay(100).Wait();

                // power management and crystal settings
                Console.WriteLine("PWR_MGMT_1");
                mpu.Write(new byte[] { PWR_MGMT_1, 0x01 });
                Task.Delay(100).Wait();

                // write to Configuration register
                Console.WriteLine("CONFIG");
                mpu.Write(new byte[] { CONFIG, 0x00 });
                Task.Delay(300).Wait();

                // interrupt register (related to overflow of data [FIFO])
                Console.WriteLine("INT_ENABLE");
                mpu.Write(new byte[] { INT_ENABLE, 0x01 });
                Task.Delay(100).Wait();

                // init AK8963
                Console.WriteLine("AK8963_CNTL");
                ak8963.Write(new byte[] { AK8963_CNTL, 0x00 });
                Task.Delay(100).Wait();

                // set ContinuousMeasurement100Hz and Output16bit
                var AK8963_bit_res = 0b0001; // 0b0001 = 16-bit
                var AK8963_samp_rate = 0b0110; // 0b0010 = 8 Hz, 0b0110 = 100 Hz
                var AK8963_mode = (AK8963_bit_res << 4) + AK8963_samp_rate; // bit conversion
                Console.WriteLine($"AK8963_CNTL {AK8963_mode}");
                ak8963.Write(new[] { AK8963_CNTL, (byte)AK8963_mode });
                Task.Delay(100).Wait();




                var mpuWIA = new byte[1];
                var ak8963WIA = new byte[1];

                mpu.WriteRead(new[] { WIA }, mpuWIA);
                ak8963.WriteRead(new[] { WIA }, ak8963WIA);

                Console.WriteLine($"WIA MPU6050: 0x{mpuWIA[0]:X2}");
                Console.WriteLine($"WIA AK8963: 0x{ak8963WIA[0]:X2}");

                Console.WriteLine($"whoami MPU6050: 0x{whoami(mpu):X2}");
                Console.WriteLine($"whoami AK8963: 0x{whoami(ak8963):X2}");
                Task.Delay(100).Wait();







                //this.resetDeviceConfiguration(mpu);
                //if (this.debug) Console.WriteLine("RESET DEVICE");

                //this.setI2C_MST_CLK(i2cspeed);
                //if (this.debug) Console.WriteLine("SET SPEED I2C");

                //this.setGyrosSensibility(gyro_fs);
                //if (this.debug) Console.WriteLine("SET GYROS FS");

                //this.setAccelSensibility(accel_fs);
                //if (this.debug) Console.WriteLine("SET ACCEL FS");

                //this.setClockSource(clocksource);
                //if (this.debug) Console.WriteLine("SET CLOCK SOURCE");

                //this.setSleep(false);
                //if (this.debug) Console.WriteLine("SET SLEEP MODE TO FALSE");

                //if (this.whoami(mpu) == WHOAMI_VALUE)
                //{
                //    Console.WriteLine("whoami");
                this.enableMagnetometer(ak8963);

                    this.deviceReady = true;
                //}

                Console.WriteLine("OK");
            }
            catch (Exception err)
            {
                Console.WriteLine($"general fault: {err.Message}");
            }
        }


        /**
         * Convert table of bits to byte
         **/
        public static byte ConvertToByte(BitArray bits)
        {
            if (bits.Length != 8)
            {
                throw new ArgumentException("illegal number of bits");
            }
            byte b = 0;
            if (bits.Get(0)) b++;
            if (bits.Get(1)) b += 2;
            if (bits.Get(2)) b += 4;
            if (bits.Get(3)) b += 8;
            if (bits.Get(4)) b += 16;
            if (bits.Get(5)) b += 32;
            if (bits.Get(6)) b += 64;
            if (bits.Get(7)) b += 128;
            return b;
        }

        /**
         * return if device is ready to work 
         **/
        public bool isReady()
        {
            return this.deviceReady;
        }

        /**
         * set Sleep on/off
         **/
        public void setSleep(bool value)
        {
            if (this.mpu != null)
            {
                byte[] ReadBuf = new byte[1];
                this.mpu.WriteRead(new byte[] { PWR_MGMT_1 }, ReadBuf);
                var bits = new BitArray(ReadBuf);
                bits.Set(6, value);
                var byteval = ConvertToByte(bits);
                this.mpu.Write(new byte[] { PWR_MGMT_1, byteval });
                Task.Delay(10).Wait();
            }
        }

        /**
         * turn on AK8963
         **/
        public bool enableMagnetometer(I2cDevice mpu)
        {
            bool val = false;
            
            if (this.mpu != null)
            {
                try
                {
                    //this.setI2C_MST_EN(false);
                    //this.setBYPASS(true);
                    Console.WriteLine("set bypass");
                    //if (this.ByPassEnabled())
                    {
                        //resetDeviceConfiguration(magnewotsit);
                        //Console.WriteLine("RESET MAGNEWOTSIT DEVICE");

                        //Console.WriteLine("INIT MAGNETOMETER");
                        //if (this.debug) Debug.WriteLine("INIT MAGNETOMETER");
                        //this.magnetometer = new Ak8963(ak8963);

                        //this.magnetometer.OutputBitMode = OutputBitMode.Output16bit;
                        //this.magnetometer.MeasurementMode = MeasurementMode.ContinuousMeasurement100Hz;

                        //Task.Delay(1000).Wait();

                        //getMotion6(0);

                        //while (true)
                        //{
                        //    Console.WriteLine("mag available?");

                        //    while (true)
                        //    {
                        //        ak8963.WriteRead(new[] { AK8963_ST2 }, st2);

                        //        Console.WriteLine($"AK8963_ST2: 0x{st2[0]:X2}");

                        //        vector3 = this.magnetometer.ReadMagnetometerWithoutCorrection();

                        //        var x = vector3.X;
                        //        var y = vector3.Y;
                        //        var z = vector3.Z;

                        //        Console.WriteLine($"{DateTime.UtcNow}: {x} {y} {z}");

                        //        //Console.WriteLine($"{this.magnetometer.HasDataToRead}");
                        //        //Task.Delay(100).Wait();
                        //    }

                        //    Task.Delay(10).Wait();
                        //}
                    }
                }
                catch(Exception err)
                {
                    Console.WriteLine($"fek: {err.Message}");
                }
            }
            return val;
        }
        

        /**
         * Set clock source
         **/
        public void setClockSource(int value)
        {
            if (value >= 0 && value <= 7)
            {
                byte[] ReadBuf = new byte[1];
                this.mpu.WriteRead(new byte[] { PWR_MGMT_1 }, ReadBuf);

                var bits = new BitArray(ReadBuf);
                var bitsTab = new bool[][]
                {
                        new bool[] { false, false, false},
                        new bool[] { false, false, true},
                        new bool[] { false, true, false},
                        new bool[] { false, true, true},
                        new bool[] { true, false, false},
                        new bool[] { true, false, true},
                        new bool[] { true, true, false},
                        new bool[] { true, true, true},
                };
                Array.Reverse(bitsTab);
                for (int i = 0; i < 3; i++)
                {
                    bits.Set(i, bitsTab[value][i]);
                }

                var byteval = ConvertToByte(bits);
                this.mpu.Write(new byte[] { PWR_MGMT_1, byteval });
                Task.Delay(10).Wait();
            }
        }

        /**
         * Reset device configuration
         **/
        private void resetDeviceConfiguration(I2cDevice device)
        {
            if (device != null)
            {
                byte[] ReadBuf = new byte[1];
                device.WriteRead(new byte[] { PWR_MGMT_1 }, ReadBuf);
                var bits = new BitArray(ReadBuf);
                bits.Set(7, true);
                var byteval = ConvertToByte(bits);
                device.Write(new byte[] { PWR_MGMT_1, byteval });
                Task.Delay(10).Wait();
            }
        }

        /**
         * set speed clock of I2C
         **/
        public void setI2C_MST_CLK(int value)
        {
            if (this.mpu != null)
            {
                if (value >= 0 && value <= 15)
                {
                    byte[] ReadBuf = new byte[1];
                    this.mpu.WriteRead(new byte[] { I2C_MST_CTRL }, ReadBuf);

                    var bits = new BitArray(ReadBuf);

                    var bitsTab = new bool[][] {
                        new bool[]{ false,false,false,false },
                        new bool[]{ false,false,false,true },
                        new bool[]{ false,false, true, false },
                        new bool[]{ false,false, true, true },
                        new bool[]{ false, true, false,false },
                        new bool[]{ false, true, false,true },
                        new bool[]{ false, true, true, false },
                        new bool[]{ false, true, true, true },
                        new bool[]{ true, false,false,false },
                        new bool[]{ true, false,false,true },
                        new bool[]{ true, false, true, false },
                        new bool[]{ true, false, true, true },
                        new bool[]{ true, true, false,false },
                        new bool[]{ true, true, false,true },
                        new bool[]{ true, true, true, false },
                        new bool[]{ true, true, true, true },
                    };
                    Array.Reverse(bitsTab);

                    for (int t = 0; t < 4; t++)
                    {
                        bits.Set(t, bitsTab[value][t]);
                    }
                    var byteval = ConvertToByte(bits);
                    this.mpu.Write(new byte[] { I2C_MST_CTRL, byteval });
                    Task.Delay(10).Wait();

                }
            }
        }

        public void setBYPASS(bool value)
        {
            if (this.mpu != null)
            {
                byte[] ReadBuf = new byte[1];
                this.mpu.WriteRead(new byte[] { INT_PIN_CFG }, ReadBuf);
                var bits = new BitArray(ReadBuf);
                bits.Set(1, value);
                var byteval = ConvertToByte(bits);
                this.mpu.Write(new byte[] { INT_PIN_CFG, byteval });
                Task.Delay(10).Wait();
            }
        }
        
        public bool ByPassEnabled()
        {
            var val = false;

            if (this.mpu != null)
            {
                byte[] ReadBuf = new byte[1];
                this.mpu.WriteRead(new byte[] { INT_PIN_CFG }, ReadBuf);
                var bits = new BitArray(ReadBuf);
                val = bits.Get(1);
            }

            return val;
        }

        public byte getBYPASS()
        {
            byte data = 0x00;

            if (this.mpu != null)
            {
                byte[] ReadBuf = new byte[1];
                this.mpu.WriteRead(new byte[] { INT_PIN_CFG }, ReadBuf);
                Debug.WriteLine(BitConverter.ToString(ReadBuf));
                data = ReadBuf[0];
            }

            return data;
        }

        /**
         * set I2C MASTER MODE ENABLED
         **/
        public void setI2C_MST_EN(bool value)
        {
            if (this.mpu != null)
            {
                byte[] ReadBuf = new byte[1];
                this.mpu.WriteRead(new byte[] { USER_CTRL }, ReadBuf);
                var bits = new BitArray(ReadBuf);
                bits.Set(5, value);
                var byteval = ConvertToByte(bits);
                this.mpu.Write(new byte[] { USER_CTRL, byteval });
                Task.Delay(10).Wait();
            }
        }

        /**
         * set sensibility of gyroscop
         **/
        public void setGyrosSensibility(int value)
        {
            if (this.mpu != null)
            {
                if (value >= 0 && value <= 3)
                {
                    byte[] ReadBuf = new byte[1];
                    this.mpu.WriteRead(new byte[] { GYRO_CONFIG }, ReadBuf);

                    var bits = new BitArray(ReadBuf);

                    var bitsTab = new bool[][]
                    {
                        new bool[] { false, false},
                        new bool[] { false, true},
                        new bool[] { true, false},
                        new bool[] { true, true},
                    };
                    Array.Reverse(bitsTab);

                    for (int i = 3; i < 5; i++)
                    {
                        bits.Set(i, bitsTab[value][i-3]);
                    }

                    var byteval = ConvertToByte(bits);
                    this.mpu.Write(new byte[] { GYRO_CONFIG, byteval });
                    Task.Delay(10).Wait();
                }
            }
        }

        /**
         * set sensibility of accel
         **/
        public void setAccelSensibility(int value)
        {
            if (this.mpu != null)
            {
                if (value >= 0 && value <= 3)
                {
                    byte[] ReadBuf = new byte[1];
                    this.mpu.WriteRead(new byte[] { ACCEL_CONFIG }, ReadBuf);

                    var bits = new BitArray(ReadBuf);

                    var bitsTab = new bool[][]
                    {
                        new bool[] { false, false},
                        new bool[] { false, true},
                        new bool[] { true, false},
                        new bool[] { true, true},
                    };
                    Array.Reverse(bitsTab);

                    for (int i = 3; i < 5; i++)
                    {
                        bits.Set(i, bitsTab[value][i - 3]);
                    }

                    var byteval = ConvertToByte(bits);
                    this.mpu.Write(new byte[] { ACCEL_CONFIG, byteval });
                }
            }
        }
        
        /**
         * return device ID if it work it wll be = to 113dec (0x71hex)
         **/
        public short whoami(I2cDevice device)
        {
            short value = 0x00;

            if (device != null)
            {
                try
                {
                    byte[] ReadBuf = new byte[1];
                    device.WriteRead(new byte[] { WHO_AM_I }, ReadBuf);
                    value = (short) ReadBuf[0];
                }
                catch (Exception err)
                {
                    Console.WriteLine($"whoami error: {err.Message}");
                }
            }

            return value;
        }

        /**
         * return temperature into °C
         **/
        public void getTemperature(mpuCallbackTemp callback)
        {
            this.getTemperature(callback, 1000);
        }

        public void getTemperature(mpuCallbackTemp callback, int millisecondsDelay)
        {
            double value = 0;

            if (this.deviceReady && this.mpu != null)
            {
                try
                {
                    for (;;)
                    {
                        byte[] ReadBuf = new byte[2];
                        this.mpu.WriteRead(new byte[] { TEMP_OUT_H }, ReadBuf);
                        Array.Reverse(ReadBuf);
                        var temperature2 = BitConverter.ToUInt16(ReadBuf, 0);
                        value = (temperature2 / 333.87) + 21.0;
                        callback(value);
                        Task.Delay(millisecondsDelay).Wait();
                    }
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                }
            }
        }


        /**
         * return temperature into °C
         **/
        public double getTemperature()
        {
            double value = 0;

            if (this.deviceReady && this.mpu != null)
            {
                try
                {
                    byte[] ReadBuf = new byte[2];
                    this.mpu.WriteRead(new byte[] { TEMP_OUT_H }, ReadBuf);
                    Array.Reverse(ReadBuf);
                    var temperature2 = BitConverter.ToUInt16(ReadBuf, 0);
                    value = (temperature2 / 333.87) + 21.0;
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                }
            }

            return value;
        }

        /*********************/

        /**
        * Return ACCEL AND GYROS VALUES ax,ay,az,gx,gy,gz
        */
        public float[] getMotion6()
        {
            float[] data = new float[6];

            if (this.deviceReady && this.mpu != null)
            {
                try
                {
                    byte[] ReadBuf = new byte[14];
                    this.mpu.WriteRead(new byte[] { ACCEL_XOUT_H }, ReadBuf);

                    data[0] = BitConverter.ToInt16(new byte[] { ReadBuf[1], ReadBuf[0] }, 0);
                    data[1] = BitConverter.ToInt16(new byte[] { ReadBuf[3], ReadBuf[2] }, 0);
                    data[2] = BitConverter.ToInt16(new byte[] { ReadBuf[5], ReadBuf[4] }, 0);

                    data[3] = BitConverter.ToInt16(new byte[] { ReadBuf[7], ReadBuf[8] }, 0);
                    data[4] = BitConverter.ToInt16(new byte[] { ReadBuf[9], ReadBuf[10] }, 0);
                    data[5] = BitConverter.ToInt16(new byte[] { ReadBuf[13], ReadBuf[12] }, 0);

                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                }
            }
            return data;
        }

        public void getMotion6(mpuCallbackData callback)
        {
            this.getMotion6(callback, 1000);
        }

        private float ReadAk8963Register(byte register)
        {
            var low = new byte[1];
            var high = new byte[1];

            // read magnetometer values
            ak8963.WriteRead(new[] { (byte)(register - 1) }, low);
            ak8963.WriteRead(new[] { register }, high);

            // combine high and low for unsigned bit value
            var value = (high[0] << 8) | low[0];

            // convert to +- value
            if (value > 32768)
            {
                value -= 65536;
            }

            return value;
        }

        public void getMotion6(mpuCallbackData callback, int millisecondsDelay)
        {
            Task.Delay(1000).Wait();

            Console.WriteLine("entering");

            try
            {
                //if (this.deviceReady && this.mpu != null)
                {
                    Vector3 accel = default;
                    Vector3 gyro = default;
                    Vector3 mag = default;

                    while (true)
                    {
                        double[] data = new double[9];
                        byte[] ReadBuf = new byte[14];
                        this.mpu.WriteRead(new [] { ACCEL_XOUT_H }, ReadBuf);

                        // acceleration raw data
                        var accel_X = BitConverter.ToInt16(new[] { ReadBuf[1], ReadBuf[0] }, 0);
                        var accel_Y = BitConverter.ToInt16(new[] { ReadBuf[3], ReadBuf[2] }, 0);
                        var accel_Z = BitConverter.ToInt16(new[] { ReadBuf[5], ReadBuf[4] }, 0);
                        var accel_sens = 2; // [2.0, 4.0, 8.0, 16.0] // g (g = 9.81 m/s^2)

                        // convert to acceleration into g
                        accel.X = (float)(accel_X / Math.Pow(2.0, 15.0) * accel_sens);
                        accel.Y = (float)(accel_Y / Math.Pow(2.0, 15.0) * accel_sens);
                        accel.Z = (float)(accel_Z / Math.Pow(2.0, 15.0) * accel_sens);

                        // gyroscope raw data
                        var gyro_X = BitConverter.ToInt16(new[] { ReadBuf[7], ReadBuf[8] }, 0);
                        var gyro_Y = BitConverter.ToInt16(new[] { ReadBuf[9], ReadBuf[10] }, 0);
                        var gyro_Z = BitConverter.ToInt16(new[] { ReadBuf[13], ReadBuf[12] }, 0);
                        var gyro_sens = 250; // [250.0, 500.0, 1000.0, 2000.0] # degrees/sec

                        // convert to gyro into dps
                        gyro.X = (float)(gyro_X / Math.Pow(2.0, 15.0) * gyro_sens);
                        gyro.Y = (float)(gyro_Y / Math.Pow(2.0, 15.0) * gyro_sens);
                        gyro.Z = (float)(gyro_Z / Math.Pow(2.0, 15.0) * gyro_sens);

                        // magnetometer
                        if (ak8963 != null)
                        {
                            // convert to acceleration in g and gyro dps
                            mag.X = (float)(ReadAk8963Register(HXH) / Math.Pow(2.0, 15.0) * MAG_SENS);
                            mag.Y = (float)(ReadAk8963Register(HYH) / Math.Pow(2.0, 15.0) * MAG_SENS);
                            mag.Z = (float)(ReadAk8963Register(HZH) / Math.Pow(2.0, 15.0) * MAG_SENS);

                            var st2 = new byte[1];
                            ak8963.WriteRead(new[] { AK8963_ST2 }, st2);

                            //Console.WriteLine($"x: {x} y: {y} z:{z} AK8963_ST2:0x{st2[0]:X2}");
                        }


                        callback(accel, gyro, mag, getTemperature());

                        Task.Delay(millisecondsDelay).Wait();
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        /*********************/

        /**
        * Return ACCEL VALUES ax,ay,az
        */
        public float[] getAccel()
        {
            float[] data = new float[3];

            if (this.deviceReady && this.mpu != null)
            {
                try
                {
                    byte[] ReadBuf = new byte[6];
                    this.mpu.WriteRead(new byte[] { ACCEL_XOUT_H }, ReadBuf);

                    data[0] = BitConverter.ToInt16(new byte[] { ReadBuf[1], ReadBuf[0] }, 0);
                    data[1] = BitConverter.ToInt16(new byte[] { ReadBuf[3], ReadBuf[2] }, 0);
                    data[2] = BitConverter.ToInt16(new byte[] { ReadBuf[5], ReadBuf[4] }, 0);
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                }
            }
            return data;
        }

        public void getAccel(mpuCallbackData callback)
        {
            this.getAccel(callback, 1000);
        }

        public void getAccel(mpuCallbackData callback, int millisecondsDelay)
        {
            if (this.deviceReady && this.mpu != null)
            {
                try
                {
                    for (;;)
                    {
                        double[] data = new double[3];
                        byte[] ReadBuf = new byte[6];
                        this.mpu.WriteRead(new byte[] { ACCEL_XOUT_H }, ReadBuf);

                        Vector3 accel;
                        accel.X = BitConverter.ToInt16(new byte[] { ReadBuf[1], ReadBuf[0] }, 0);
                        accel.Y = BitConverter.ToInt16(new byte[] { ReadBuf[3], ReadBuf[2] }, 0);
                        accel.Z = BitConverter.ToInt16(new byte[] { ReadBuf[5], ReadBuf[4] }, 0);

                        callback(accel, default, default, default);
                        Task.Delay(millisecondsDelay).Wait();
                    }
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                }
            }
        }

        /*********************/

        /**
        * Return GYROS VALUES ,gx,gy,gz
        */
        public float[] getGyros()
        {
            float[] data = new float[3];

            if (this.deviceReady && this.mpu != null)
            {
                try
                {
                    byte[] ReadBuf = new byte[6];
                    this.mpu.WriteRead(new byte[] { GYRO_XOUT_H }, ReadBuf);

                    data[0] = BitConverter.ToInt16(new byte[] { ReadBuf[1], ReadBuf[0] }, 0);
                    data[1] = BitConverter.ToInt16(new byte[] { ReadBuf[3], ReadBuf[2] }, 0);
                    data[2] = BitConverter.ToInt16(new byte[] { ReadBuf[5], ReadBuf[4] }, 0);
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                }
            }
            return data;
        }

        public void getGyros(mpuCallbackData callback)
        {
            this.getGyros(callback, 1000);
        }

        public void getGyros(mpuCallbackData callback, int millisecondsDelay)
        {
            if (this.deviceReady && this.mpu != null)
            {
                try
                {
                    for (;;)
                    {
                        double[] data = new double[3];
                        byte[] ReadBuf = new byte[6];
                        this.mpu.WriteRead(new byte[] { GYRO_XOUT_H }, ReadBuf);

                        Vector3 gyro;
                        gyro.X = BitConverter.ToInt16(new byte[] { ReadBuf[1], ReadBuf[0] }, 0);
                        gyro.Y = BitConverter.ToInt16(new byte[] { ReadBuf[3], ReadBuf[2] }, 0);
                        gyro.Z = BitConverter.ToInt16(new byte[] { ReadBuf[5], ReadBuf[4] }, 0);

                        callback(default, gyro, default, default);
                        Task.Delay(millisecondsDelay).Wait();
                    }
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                }
            }
        }

        /*********************/
        public double getPitch(double x, double z)
        {
            return (Math.Atan2(x, z) + Math.PI) * (180 / Math.PI) - 180;
        }

        public double getRoll(double y, double z)
        {
            return (Math.Atan2(y, z) + Math.PI) * (180 / Math.PI) - 180;
        }

        public double getPitch(float x, float z)
        {
            return (Math.Atan2(x, z) + Math.PI) * (180 / Math.PI) - 180;
        }

        public double getRoll(float y, float z)
        {
            return (Math.Atan2(y, z) + Math.PI) * (180 / Math.PI) - 180;
        }

        /** end of class **/
    }
}