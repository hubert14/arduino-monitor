using OpenHardwareMonitor.Hardware;

namespace ArduinoMonitor.Common.Controllers
{
    public class MainController
    {
        private static readonly Computer Computer = new Computer
            {CPUEnabled = true, GPUEnabled = true, MainboardEnabled = true, RAMEnabled = true};

        public static void Start()
        {
            Computer.Open();
            Init();
        }

        private static void Init()
        {
            MediaController.Init();
            SerialController.Init();
            IrController.Init();
            SensorController.Init(Computer);
            FanController.Init(Computer);
            DisplayController.Init();
        }
    }
}