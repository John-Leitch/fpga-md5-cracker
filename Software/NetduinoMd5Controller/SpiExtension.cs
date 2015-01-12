using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace NetduinoMd5Controller
{
    public static class SpiExtension
    {
        public static uint WriteRead(this SPI spi, uint value)
        {
            var bytes = BigEndianBitConverter.GetBytes(value);
            var readBuffer = new byte[4];
            spi.WriteRead(bytes, readBuffer);
            
            return BigEndianBitConverter.ToUInt32(readBuffer);
        }

        public static uint WriteRead(this SPI spi, Md5ChipCommands command)
        {
            return spi.WriteRead((uint)command);
        }
    }
}
