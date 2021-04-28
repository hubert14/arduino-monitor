using System;
using System.IO.Ports;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;

namespace ArduinoMonitor.Common.Controllers
{
    public class MainController
    {
        private const string COM_PORT = "COM5";
        private const int BAUD_RATE = 9600;
        private static int _openPortDelayMilliseconds = 5 * 1000;

        private static readonly SerialPort ArduinoPort = new SerialPort(COM_PORT) {BaudRate = BAUD_RATE};
        private static readonly Computer Computer = new Computer
            {CPUEnabled = true, GPUEnabled = true, MainboardEnabled = true, RAMEnabled = true};

        public static void Start()
        {
            while (!ArduinoPort.IsOpen)
                try
                {
                    ArduinoPort.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine($"Next try to open Arduino port in {_openPortDelayMilliseconds / 1000} sec.");
                    Task.Delay(_openPortDelayMilliseconds).Wait();

                    if (_openPortDelayMilliseconds < 240 * 1000) _openPortDelayMilliseconds += 5 * 1000;
                }

            Computer.Open();
            ArduinoPort.DataReceived += ArduinoPortOnDataReceived;

            Init();
        }

        private static void Init()
        {
            MediaController.Init();
            SensorController.Init(Computer);
            FanController.Init(Computer);
            DisplayController.StartDisplay(ArduinoPort);
        }

        private static void ArduinoPortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var command = ArduinoPort.ReadLine();
            IrController.DecodeCommand(command);
        }
    }
}