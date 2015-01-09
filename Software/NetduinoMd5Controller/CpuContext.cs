using System;
using Microsoft.SPOT;

namespace NetduinoMd5Controller
{
    public class CpuContext
    {
        public uint InstructionAddress { get; set; }

        public uint ProgramCounter { get; set; }

        public uint Opcode { get; set; }

        public uint Operands { get; set; }

        public uint Error { get; set; }

        public uint ErrorCode { get; set; }

        public CpuContext()
        {
        }

        public CpuContext(
            uint instructionAddress,
            uint programCounter,
            uint opcode,
            uint operands,
            uint error,
            uint errorCode)
        {
            InstructionAddress = instructionAddress;
            ProgramCounter = programCounter;
            Opcode = opcode;
            Operands = operands;
            Error = error;
            ErrorCode = errorCode;
        }
    }
}
