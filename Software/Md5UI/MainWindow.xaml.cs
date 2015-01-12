using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Md5UI
{
    public class KeyspaceCalculator : Bindable
    {
        private byte _startByte;

        public byte StartByte
        {
            get { return _startByte; }
            set 
            { 
                _startByte = value;
                UpdateCalculations();
                InvokePropertyChanged();
            }
        }

        private byte _endByte;

        public byte EndByte
        {
            get { return _endByte; }
            set 
            { 
                _endByte = value;
                UpdateCalculations();
                InvokePropertyChanged();
            }
        }

        private byte _byteCount;

        public byte ByteCount
        {
            get { return _byteCount; }
            private set 
            { 
                _byteCount = value;
                InvokePropertyChanged();
            }
        }

        private double _testsPerSecond;

        public double TestsPerSecond
        {
            get { return _testsPerSecond; }
            set 
            {
                _testsPerSecond = value;
                InvokePropertyChanged();
            }
        }

        private void UpdateCalculations()
        {
            ByteCount = (byte)(EndByte - StartByte + 1);
        }

        public PermutationInfo GetCombinationCount(int length)
        {
            return new PermutationInfo(
                length,
                Math.Pow(ByteCount, length),
                TestsPerSecond);
        }

        public PermutationInfo[] GetCombinationTable()
        {
            return Enumerable
                .Range(1, 18)
                .Select(GetCombinationCount)
                .Where(x => x.TestsPerSecond > x.Count || x.TimeNeeded != default(TimeSpan))
                .ToArray();
        }
    }

    public class PermutationInfo
    {
        public double Length { get; private set; }

        public double Count { get; private set; }

        public string CountFormatted { get; private set; }

        public double TestsPerSecond { get; private set; }

        public TimeSpan TimeNeeded { get; private set; }

        public PermutationInfo(double length, double count, double testsPerSecond)
        {
            Length = length;
            Count = count;
            CountFormatted = string.Format("{0:n0}", Count);
            TestsPerSecond = testsPerSecond;

            try
            {
                TimeNeeded = TimeSpan.FromSeconds(Count / TestsPerSecond);
            }
            catch (OverflowException) { }
        }

        public override string ToString()
        {
            return CountFormatted;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient _client;

        private NetworkStream _stream;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;            
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_client == null)
            {
                return;
            }

            SendCommand(CommandOpcode.Close);
            _client.Close();
        }

        private void WriteUInt32(CommandOpcode opcode, uint value)
        {
            var buffer = new byte[5];
            buffer[0] = (byte)opcode;
            BitConverter.GetBytes(value).CopyTo(buffer, 1);
            _stream.Write(buffer, 0, buffer.Length);
        }

        private void SetExpectedValues(uint a, uint b, uint c, uint d)
        {
            lock (_stream)
            {
                WriteUInt32(CommandOpcode.SetExpectedA, a);
                WriteUInt32(CommandOpcode.SetExpectedB, b);
                WriteUInt32(CommandOpcode.SetExpectedC, c);
                WriteUInt32(CommandOpcode.SetExpectedD, d);
            }
        }

        private void SendCommand(CommandOpcode opcode)
        {
            _stream.Write(new byte[] { (byte)opcode }, 0, 1);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _client = new TcpClient();
            _client.Connect("192.168.1.200", 5230);
            _stream = _client.GetStream();
            new Thread(() =>
            {
                while (true)
                {
                    ulong count;

                    lock (_stream)
                    {
                        try
                        {
                            _stream.Write(
                                new byte[] 
                                {
                                    (byte)CommandOpcode.GetCount,
                                    0,
                                    0,
                                    0,
                                    0,
                                },
                                0,
                                5);
                        }
                        catch
                        {
                            break;
                        }

                        var buffer = new byte[8];
                        _stream.Read(buffer, 0, buffer.Length);
                        count = BitConverter.ToUInt64(buffer, 0);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                        CountTextBox.Text = string.Format("{0:##,##}", count));

                    Thread.Sleep(10);
                }
            }) { IsBackground = true }.Start();
            
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            lock (_stream)
            {
                SendCommand(CommandOpcode.ResetGenerator);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            SendCommand(CommandOpcode.StartGenerator);
        }

        private static byte[] GetBytes(string hexString)
        {
            return Enumerable
                .Range(0, hexString.Length / 2)
                .Select(x => Convert.ToByte(hexString.Substring(x * 2, 2), 16))
                .ToArray();
        }

        private uint[] ParseHash(string text)
        {
            var digest = GetBytes(text);

            var parts = new uint[]
            {
                BitConverter.ToUInt32(digest, 0),
                BitConverter.ToUInt32(digest, 4),
                BitConverter.ToUInt32(digest, 8),
                BitConverter.ToUInt32(digest, 12),
            };

            parts[0] -= 0x67452301;
            parts[1] -= 0xefcdab89;
            parts[2] -= 0x98badcfe;
            parts[3] -= 0x10325476;

            return parts;
        }

        private void ProgramButton_Click(object sender, RoutedEventArgs e)
        {
            lock (_stream)
            {
                ResetButton_Click(sender, e);
                var values = ParseHash(Md5TextBox.Text);
                SetExpectedValues(values[0], values[1], values[2], values[3]);
                var min = GetBytes(MinTextBox.Text)[0];
                var max = GetBytes(MaxTextBox.Text)[0];
                var range = BitConverter.ToUInt32(new byte[] { min, max, 0, 0 }, 0);
                WriteUInt32(CommandOpcode.SetRange, range);
                StartButton_Click(sender, e);
            }
            //SetExpectedValues(0x40fcc7e8, 0xa8285a73, 0xb90903ae, 0x4eb4d049);
        }

        private void TextTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var provider = new MD5CryptoServiceProvider();
            Md5TextBox.Text =
                string.Join(
                "",
                provider.ComputeHash(ASCIIEncoding.ASCII.GetBytes(TextTextBox.Text)).Select(x => string.Format("{0:X2}", x)));
        }
    }
}
