using System;
using System.IO.Ports;
using System.Threading.Tasks;
using ArduinoMonitor.Common.Constants;

namespace ArduinoMonitor.Common.Controllers
{
    public static class SerialController
    {
        public static event IrCommandHandler IrCommandReceived;
        public static event DisplayStatusHandler DisplayStatusChanged;

        private const string COM_PORT = "COM5";
        private const int BAUD_RATE = 9600;

        private static int _openPortDelayMilliseconds = 5 * 1000;

        private static readonly SerialPort Port = new SerialPort(COM_PORT) {BaudRate = BAUD_RATE};

        public static void Init()
        {
            while (!Port.IsOpen)
                try
                {
                    Port.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine($"Next try to open Arduino port in {_openPortDelayMilliseconds / 1000} sec.");
                    Task.Delay(_openPortDelayMilliseconds).Wait();

                    if (_openPortDelayMilliseconds < 240 * 1000) _openPortDelayMilliseconds += 5 * 1000;
                }

            Port.DataReceived += SerialPortOnDataReceived;
        }

        private static void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs e) =>
            DecodeCommand(ReadFromSerial());

        public static string ReadFromSerial() => Port.ReadLine();
        public static void WriteToSerial(string text) => Port.Write(text);

        public static void DecodeCommand(string command)
        {
            command = command
                .Replace("\r\n", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .ToUpper();

            var splittedCommand = command.Split(':');

            var header = splittedCommand[0];
            var body = splittedCommand[1];

            switch (header)
            {
                case DeviceHeaders.SCREEN_ON_STATUS:
                    DisplayStatusChanged?.Invoke(body == "1");
                    return;
                case DeviceHeaders.IR_COMMAND:
                    IrCommandReceived?.Invoke(body);
                    return;
            }
        }
    }
}