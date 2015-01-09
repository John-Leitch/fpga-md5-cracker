using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace NetduinoMd5Controller
{
    public static class OutputPortExtension
    {
        public static void Pulse(this OutputPort port)
        {
            port.Write(true);
            port.Write(false);
        }
    }
}
