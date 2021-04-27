using System;
using System.IO.Ports;
using ArduinoMonitor.Controllers;
using OpenHardwareMonitor.Hardware;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ArduinoMonitor
{
    internal class Program
    {
        private const string COM_PORT = "COM5";
        private const int BAUD_RATE = 9600;
        private static int _openPortDelay = 5000;

        public static readonly SerialPort ArduinoPort = new SerialPort(COM_PORT) {BaudRate = BAUD_RATE};

        public static readonly Computer Computer = new Computer
            {CPUEnabled = true, GPUEnabled = true, MainboardEnabled = true, RAMEnabled = true};

        public static readonly DisplayController Display = new DisplayController(ArduinoPort, Computer);
        public static readonly IrController IrDecoder = new IrController();

        private static void Main(string[] args)
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);

                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    Console.WriteLine("To work correctly with sensors, you need administrator rights.\n" +
                                      "Please, press any keys and open this app with admin rights.");
                    Console.ReadKey();
                    return;
                }
            }

            while (!ArduinoPort.IsOpen)
            {
                try
                {
                    ArduinoPort.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine($"Next try to open Arduino port in {_openPortDelay / 1000} sec.");
                    Task.Delay(_openPortDelay).Wait();

                    if (_openPortDelay < 60000) _openPortDelay += 2500;
                }
            }

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