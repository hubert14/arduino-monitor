using System;
using System.IO.Ports;
using System.Linq;
using OpenHardwareMonitor.Hardware;

namespace ArduinoMonitor.Controllers
{
    public enum FanOperation
    {
        Default,
        Up,
        Down
    }

    internal class FanController
    {
        private const float FAN_STEP = 5;
        private const string GPU_FAN_ID = "/nvidiagpu/0/control/0";

        public static void ChangeGPUFan(FanOperation operation)
        {
            var gpu = Program.Computer.Hardware.First(x => x.HardwareType == HardwareType.GpuNvidia);

            gpu.Update();

            var fan = gpu.Sensors.First(x => x.Identifier.ToString() == GPU_FAN_ID);

            var value = fan.Value.GetValueOrDefault(50);
            var outputValue = string.Empty;

            switch (operation)
            {
                case FanOperation.Up:
                    outputValue = (value + FAN_STEP).ToString("##") + "%";
                    fan.Control.SetSoftware(value + FAN_STEP);
                    break;
                case FanOperation.Down:
                    outputValue = (value - FAN_STEP).ToString("##") + "%";
                    fan.Control.SetSoftware(value - FAN_STEP);
                    break;
                case FanOperation.Default:
                    outputValue = "Default";
                    fan.Control.SetDefault();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }

            Program.Display.DisplayMessage("FAN CONTROL", $"{fan.Name} {outputValue}");
            Console.WriteLine($"FAN CONTROL | {fan.Name} set to {outputValue}");
        }
    }
}