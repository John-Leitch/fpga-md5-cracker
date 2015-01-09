using System;
using Microsoft.SPOT;

namespace NetduinoMd5Controller
{
    public static class BigEndianBitConverter
    {
        public static uint ToUInt32(byte[] bytes)
        {
            return (uint)
                ((bytes[0] << 24) |
                (bytes[1] << 16) |
                (bytes[2] << 8) |
                bytes[3]);
        }

        public static byte[] GetBytes(uint x)
        {
            return new[] 
            { 
                (byte)((x & 0xFF000000) >> 24),
                (byte)((x & 0x00FF0000) >> 16),
                (byte)((x & 0x0000FF00) >> 8),
                (byte)x,
            };
        }
    }
}
