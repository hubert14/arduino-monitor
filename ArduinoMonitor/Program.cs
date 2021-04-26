using System.IO.Ports;
using ArduinoMonitor.Controllers;
using OpenHardwareMonitor.Hardware;

namespace ArduinoMonitor
{
    internal class Program
    {
        private const string COM_PORT = "COM5";
        private const int BAUD_RATE = 9600;

        public static readonly SerialPort ArduinoPort = new SerialPort(COM_PORT) {BaudRate = BAUD_RATE};
        public static readonly Computer Computer = new Computer {CPUEnabled = true, GPUEnabled = true, MainboardEnabled = true, RAMEnabled = true};
        public static readonly DisplayController Display = new DisplayController(ArduinoPort, Computer);
        public static readonly IrController IrDecoder = new IrController();

        private static void Main(string[] args)
        {
            ArduinoPort.Open();
            Computer.Open();
            ArduinoPort.DataReceived += ArduinoPortOnDataReceived;
            Display.StartDisplay();
        }

        private static void ArduinoPortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var command = ArduinoPort.ReadLine();
            IrDecoder.DecodeCommand(command);
        }
    }
}