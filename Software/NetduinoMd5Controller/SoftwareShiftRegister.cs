using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace NetduinoMd5Controller
{
    public delegate void ShiftRegisterCallback(bool[] bits);

    public class SoftwareShiftRegister
    {
        private InputPort _clockPin;
        private InputPort _resetPin;
        private InputPort _dataPin;
        private InputPort _commitPin;
        private ushort _bitNumber;
        private bool[] _bits;

        private bool _clockPosEdge;
        private ShiftRegisterCallback _callback;

        public ushort Size { get; private set; }

        public SoftwareShiftRegister(ushort size, Cpu.Pin clock, Cpu.Pin reset, Cpu.Pin data, Cpu.Pin commit, ShiftRegisterCallback callback)
        {
            Size = size;
            _bits = new bool[size];
            _clockPin = new InputPort(clock, true, Port.ResistorMode.Disabled);
            _resetPin = new InputPort(reset, true, Port.ResistorMode.Disabled);
            _dataPin = new InputPort(data, true, Port.ResistorMode.Disabled);
            _commitPin = new InputPort(commit, true, Port.ResistorMode.Disabled);
            _callback = callback;
        }

        public void Tick()
        {
            var clk = _clockPin.Read();

            if (!_clockPosEdge && clk)
            {
                _clockPosEdge = true;
            }
            else if (_clockPosEdge && !clk)
            {
                _clockPosEdge = false;
            }

            if (!_resetPin.Read())
            {
                if (_clockPosEdge)
                {
                    _bits[_bitNumber] = _dataPin.Read();

                    if (_commitPin.Read())
                    {
                        _callback(_bits);
                    }

                    if (++_bitNumber == Size)
                    {
                        _bitNumber = 0;
                    }

                }
            }
            else
            {
                _bits = new bool[Size];
                _bitNumber = 0;
            }
        }
    }
}
