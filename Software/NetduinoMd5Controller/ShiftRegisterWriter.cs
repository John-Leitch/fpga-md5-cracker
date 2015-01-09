using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace NetduinoMd5Controller
{
    public class ShiftRegisterWriter : IDisposable
    {
        private OutputPort _clockPin;
        private OutputPort _resetPin;
        private OutputPort _dataPin;
        private OutputPort _commitPin;
        
        //private OutputPort _commitPin;

        public ShiftRegisterWriter(Cpu.Pin clock, Cpu.Pin reset, Cpu.Pin data, Cpu.Pin commit)
        {
            _clockPin = new OutputPort(clock, false);
            _dataPin = new OutputPort(data, false);
            _resetPin = new OutputPort(reset, false);
            _commitPin = new OutputPort(commit, false);
        }

        public void Clear()
        {
            _resetPin.Pulse();
        }

        public void Write(bool bit)
        {
            _dataPin.Write(bit);
            _clockPin.Pulse();
        }

        public void Write(byte b)
        {
            Write((b & 128) == 128);
            Write((b & 64) == 64);
            Write((b & 32) == 32);
            Write((b & 16) == 16);
            Write((b & 8) == 8);
            Write((b & 4) == 4);
            Write((b & 2) == 2);
            Write((b & 1) == 1);
        }

        public void Write(uint u)
        {
            Write((byte)((u & 0xFF000000) >> 24));
            Write((byte)((u & 0x00FF0000) >> 16));
            Write((byte)((u & 0x0000FF00) >> 8));
            Write((byte)((u & 0x000000FF) >> 0));
        }

        public void WriteAndCommit(uint u)
        {
            Write(u);
            Commit();
        }

        public void Commit()
        {
            _commitPin.Pulse();
        }

        public void Dispose()
        {
            if (_clockPin != null)
            {
                _clockPin.Dispose();
            }

            if (_dataPin != null)
            {
                _dataPin.Dispose();
            }

            if (_resetPin != null)
            {
                _resetPin.Dispose();
            }

            if (_commitPin != null)
            {
                _commitPin.Dispose();
            }
        }
    }
}
