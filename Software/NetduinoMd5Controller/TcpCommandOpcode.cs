using System;
using Microsoft.SPOT;

namespace NetduinoMd5Controller
{
    public enum TcpCommandOpcode : byte
    {
        ResetGenerator = 0x1,
        StartGenerator = 0x2,
        SetExpectedA = 0x10,
        SetExpectedB = 0x11,
        SetExpectedC = 0x12,
        SetExpectedD = 0x13,
        SetRange = 0x20,
        GetCount = 0x30,
        Close = 0xFF,
    }
}
