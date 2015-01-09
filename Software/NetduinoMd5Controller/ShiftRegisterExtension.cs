using System;
using Microsoft.SPOT;

namespace NetduinoMd5Controller
{
    public static class ShiftRegisterExtension
    {
        public static void WriteAndCommit(this ShiftRegisterWriter register, Md5ChipCommands cmd)
        {
            register.WriteAndCommit((uint)cmd);
        }
    }
}
