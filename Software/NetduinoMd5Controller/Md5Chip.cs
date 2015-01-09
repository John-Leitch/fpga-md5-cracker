using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace NetduinoMd5Controller
{
    public sealed class Md5Chip : IDisposable
    {
        public static Md5Chip Instance { get; private set; }

        private SPI.Configuration _spiConfig;

        private SPI _spi;

        public SPI Spi
        {
            get { return _spi; }
            set { _spi = value; }
        }

        public Md5Chip()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException();
            }

            Instance = this;

            _spiConfig = new SPI.Configuration(
                SecretLabs.NETMF.Hardware.Netduino.Pins.GPIO_PIN_D10,
                false,
                0,
                0,
                false,
                true,
                5000,
                SPI_Devices.SPI1);

            _spi = new SPI(_spiConfig);
        }

        public ulong ReadCount()
        {
            _spi.WriteRead(Md5ChipCommands.GetCountHigh);
            var high = _spi.WriteRead(Md5ChipCommands.GetCountLow);
            var low = _spi.WriteRead(Md5ChipCommands.Nop);
            
            return((ulong)high << 32) + low;
        }

        public void ResetGenerator()
        {
            _spi.WriteRead(Md5ChipCommands.ResetGenerator);
        }

        public void StartGenerator()
        {
            _spi.WriteRead(Md5ChipCommands.StartGenerator);
        }

        public void SetExpectedValues(uint a, uint b, uint c, uint d)
        {
            _spi.WriteRead(Md5ChipCommands.SetExpectedA);
            _spi.WriteRead(a);
            _spi.WriteRead(Md5ChipCommands.SetExpectedB);
            _spi.WriteRead(b);
            _spi.WriteRead(Md5ChipCommands.SetExpectedC);
            _spi.WriteRead(c);
            _spi.WriteRead(Md5ChipCommands.SetExpectedD);
            _spi.WriteRead(d);
        }

        public void SetExpectedA(uint a)
        {
            _spi.WriteRead(Md5ChipCommands.SetExpectedA);
            _spi.WriteRead(a);
        }

        public void SetExpectedB(uint b)
        {
            _spi.WriteRead(Md5ChipCommands.SetExpectedB);
            _spi.WriteRead(b);
        }

        public void SetExpectedC(uint c)
        {
            _spi.WriteRead(Md5ChipCommands.SetExpectedC);
            _spi.WriteRead(c);
        }

        public void SetExpectedD(uint d)
        {
            _spi.WriteRead(Md5ChipCommands.SetExpectedD);
            _spi.WriteRead(d);
        }

        private uint GetUInt(byte[] buffer, int offset)
        {
            return
                (uint)buffer[offset] |
                ((uint)buffer[offset + 1]) << 8 |
                ((uint)buffer[offset + 2]) << 16 |
                ((uint)buffer[offset + 3]) << 24;
        }

        public void SetRange(byte[] buffer)
        {
            _spi.WriteRead(Md5ChipCommands.SetRange);
            _spi.WriteRead(GetUInt(buffer, 1));            
        }

        public void Dispose()
        {
            if (_spi != null)
            {
                _spi.Dispose();
            }

            if (Instance != null)
            {
                Instance = null;
            }
        }
    }
}
