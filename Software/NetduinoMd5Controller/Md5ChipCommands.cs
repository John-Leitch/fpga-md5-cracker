using System;
using Microsoft.SPOT;

namespace NetduinoMd5Controller
{
    public enum Md5ChipCommands : uint
    {
        Nop =               0x00000000,
        ResetGenerator =    0x52300000,
        StartGenerator =    0x52300001,
        SetExpectedA =      0x52301000,
        SetExpectedB =      0x52301001,
        SetExpectedC =      0x52301002,
        SetExpectedD =      0x52301003,
        SetRange =          0x52302000,
        GetCountLow =       0x52303000,
        GetCountHigh =      0x52303001,
    }
}
