using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Microsoft.SPOT.Net.NetworkInformation;

namespace NetduinoMd5Controller
{
    public enum CpuCommand : uint
    {
        Nop = 0x00000000,
        Break = 0x52300000,
        Continue = 0x52300001,
        Restart = 0x52300002,
        Reset = 0x52300005,
        ReadAddress = 0x52300010,
        Read4 = 0x52300020,
        WriteAddress = 0x52300030,
        Write4 = 0x52300040,
        GetProgramCounter = 0x52300050,
        GetInstructionAddress = 0x52300051,
        GetError = 0x52300052,
        GetErrorCode = 0x52300053,
        GetOpcode = 0x52300054,
        GetOperands = 0x52300055,
    }

    public sealed class CpuProgrammer : IDisposable
    {
        public static CpuProgrammer Instance { get; private set; }

        private SPI.Configuration _spiConfig;

        private SPI _spi;

        public CpuProgrammer()
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

        public void Break()
        {
            _spi.WriteRead(CpuCommand.Break);
        }

        public void Continue()
        {
            _spi.WriteRead(CpuCommand.Continue);
        }

        public void Restart()
        {
            _spi.WriteRead(CpuCommand.Restart);
        }

        public void Reset()
        {
            _spi.WriteRead(CpuCommand.Reset);
        }

        private uint Nop()
        {
            return _spi.WriteRead(CpuCommand.Nop);
        }

        public uint Read4(uint address)
        {
            _spi.WriteRead(CpuCommand.ReadAddress);
            _spi.WriteRead(address);
            _spi.WriteRead(CpuCommand.Read4);

            return Nop();
        }

        public byte[] ReadBuffer(uint address, uint length)
        {
            var buffer = new byte[length];
            var end = address + length;

            for (uint a = address; a < end; a += 4)
            {
                var b = BigEndianBitConverter.GetBytes(Read4(a));
                b.CopyTo(buffer, (int)(a - address));
                
            }            

            return buffer;
        }

        public void Write4(uint address, uint data)
        {
            _spi.WriteRead(CpuCommand.WriteAddress);
            _spi.WriteRead(address);

            _spi.WriteRead(CpuCommand.Write4);
            _spi.WriteRead(data);
        }

        private uint GetValue(CpuCommand command)
        {
            _spi.WriteRead(command);

            return Nop();
        }

        public uint GetInstructionAddress()
        {
            return GetValue(CpuCommand.GetInstructionAddress);
        }

        public uint GetProgramCounter()
        {
            return GetValue(CpuCommand.GetProgramCounter);
        }

        public uint GetOpcode()
        {
            return GetValue(CpuCommand.GetOpcode);
        }

        public uint GetOperands()
        {
            return GetValue(CpuCommand.GetOperands);
        }

        public uint GetError()
        {
            return GetValue(CpuCommand.GetError);
        }

        public uint GetErrorCode()
        {
            return GetValue(CpuCommand.GetErrorCode);
        }

        public CpuContext GetContext()
        {
            return new CpuContext(
                GetInstructionAddress(),
                GetProgramCounter(),
                GetOpcode(),
                GetOperands(),
                GetError(),
                GetErrorCode());
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class Program
    {
        private static Md5Chip _md5Chip;// = new Md5Chip();        

        private static Socket client;

        private static bool clientActive;

        static void InterpretCommand(byte[] buffer)
        {
            switch ((TcpCommandOpcode)buffer[0])
            {
                case TcpCommandOpcode.ResetGenerator:
                    _md5Chip.ResetGenerator();
                    break;

                case TcpCommandOpcode.StartGenerator:
                    _md5Chip.StartGenerator();
                    break;

                case TcpCommandOpcode.SetExpectedA:
                    
                    _md5Chip.SetExpectedA(BitConverter.ToUInt32(buffer, 1));
                    break;

                case TcpCommandOpcode.SetExpectedB:
                    _md5Chip.SetExpectedB(BitConverter.ToUInt32(buffer, 1));
                    break;

                case TcpCommandOpcode.SetExpectedC:
                    _md5Chip.SetExpectedC(BitConverter.ToUInt32(buffer, 1));
                    break;

                case TcpCommandOpcode.SetExpectedD:
                    _md5Chip.SetExpectedD(BitConverter.ToUInt32(buffer, 1));
                    break;

                case TcpCommandOpcode.SetRange:
                    _md5Chip.SetRange(buffer);                    
                    break;

                case TcpCommandOpcode.GetCount:
                    var count = _md5Chip.ReadCount();
                    client.Send(BitConverter.GetBytes(count));
                    break;

                case TcpCommandOpcode.Close:
                    clientActive = false;
                    client.Close();
                    return;
            }
        }

        public static void TestCount()
        {
            while (true)
            {
                var value = _md5Chip.ReadCount();
                Debug.Print("Count: " + value);
            }
        }

        public static void Main()
        {
            var programmer = new CpuProgrammer();
            programmer.Break();
            var instructions = programmer.ReadBuffer(0, 332);
            programmer.Continue();
            var inst = programmer.Read4(0x0);
            var contetx = programmer.GetContext();

            while (true)
            {
                //_md5Chip.Spi.WriteRead(0x52300002);

                var a = _md5Chip.Spi.WriteRead(0x52300000);
                var aasd = _md5Chip.Spi.WriteRead((uint)0x00000);
                var a2 = _md5Chip.Spi.WriteRead(0x52300001);
                var aasd2 = _md5Chip.Spi.WriteRead((uint)0x00000);

                _md5Chip.Spi.WriteRead(0x52300050);
                var programCounter = _md5Chip.Spi.WriteRead((uint)0x00000);

                _md5Chip.Spi.WriteRead(0x52300051);
                var instructionAddress = _md5Chip.Spi.WriteRead((uint)0x00000);

                _md5Chip.Spi.WriteRead(0x52300052);
                var error = _md5Chip.Spi.WriteRead((uint)0x00000);

                _md5Chip.Spi.WriteRead(0x52300053);
                var errorCode = _md5Chip.Spi.WriteRead((uint)0x00000);

                //var b = _md5Chip.Spi.WriteRead(0x52300030);
                //var c = _md5Chip.Spi.WriteRead((uint)0x00000000);

                //var d = _md5Chip.Spi.WriteRead(0x52300040);
                //var e = _md5Chip.Spi.WriteRead(0xAABBCCDD);

                uint address = 0;

                //for (address = 0; address < 100; address += 4)
                //{
                    var f = _md5Chip.Spi.WriteRead(0x52300010);
                    //var g = _md5Chip.Spi.WriteRead((uint)0x00000000);
                    var g = _md5Chip.Spi.WriteRead(address);


                    var h = _md5Chip.Spi.WriteRead(0x52300020);

                    var i = _md5Chip.Spi.WriteRead((uint)0x00000000);
                //    break;
                //}

                var z = _md5Chip.Spi.WriteRead(0x52300002);
                var crap = _md5Chip.Spi.WriteRead((uint)0x00000000);
                
            }
            //TestCount();
            //TestTransfer();
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces()[0];

            Debug.Print(networkInterface.IPAddress.ToString());
            //networkInterface.IsDynamicDnsEnabled = false;
            // 
            networkInterface.EnableStaticIP("192.168.1.200", "255.255.255.0", "192.168.1.1");
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Any, 5230));
                socket.Listen(1);

                while (true)
                {
                    using (client = socket.Accept())
                    {
                        clientActive = true;

                        while (clientActive)
                        {
                            var buffer = new byte[5];
                            client.Receive(buffer);
                            InterpretCommand(buffer);
                        }
                    }
                }
            }
        }
    }
}
