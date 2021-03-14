using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.IO;

namespace mpu9250
{
    /// <summary>
    /// https://github.com/dotnet/iot/blob/main/Documentation/raspi-i2c.md
    /// </summary>
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var busId = 1;
            var deviceAddress = 0x12;
            var validAddresses = new List<int>();

            // First 8 I2C addresses are reserved, last one is 0x7F
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
                    // Do nothing, there is just no device
                }
            }


            foreach (var valid in validAddresses)
            {
                Console.WriteLine($"Address: 0x{valid:X}");
            }

            Console.ReadLine();

            using (var i2c = I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress)))
            {
                i2c.WriteByte(0x42);
                var read = i2c.ReadByte();
            }
        }
    }
}
