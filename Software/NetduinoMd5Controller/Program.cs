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
    public class Program
    {
        private static Md5Chip _md5Chip = new Md5Chip();        

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
            while (true)
            {
                var networkInterface = NetworkInterface.GetAllNetworkInterfaces()[0];
                Debug.Print(networkInterface.IPAddress.ToString());
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
}
