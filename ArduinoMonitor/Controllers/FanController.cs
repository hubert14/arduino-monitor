using System;
using System.Collections.Generic;
using System.Linq;
using OpenHardwareMonitor.Hardware;

namespace ArduinoMonitor.Controllers
{
    public enum FanType
    {
        GPU,
        CPU,
        Front1,
        Front2,
        Front3,
        Rear,
    }

    public enum FanOperation
    {
        Default,
        Up,
        Down
    }

    internal class FanController
    {
        private const float FAN_STEP = 10;

        private const string GPU_FAN_ID = "/nvidiagpu/0/control/0";
        private const string CPU_FAN_ID = "/lpc/nct6795d/control/1/control";
        private const string FRONT1_FAN_ID = "/lpc/nct6795d/control/0/control";
        private const string FRONT2_FAN_ID = "/lpc/nct6795d/control/4/control";
        private const string FRONT3_FAN_ID = "/lpc/nct6795d/control/5/control";
        private const string REAR_FAN_ID = "/lpc/nct6795d/control/2/control";

        private static readonly Dictionary<FanType, string> Fans = new Dictionary<FanType, string>()
        {
            {FanType.CPU, CPU_FAN_ID},
            {FanType.GPU, GPU_FAN_ID},
            {FanType.Front1, FRONT1_FAN_ID},
            {FanType.Front2, FRONT2_FAN_ID},
            {FanType.Front3, FRONT3_FAN_ID},
            {FanType.Rear, REAR_FAN_ID},
        };

        public static void ChangeFan(FanType fan, FanOperation operation)
        {
            ISensor fanSensor;

            if (fan == FanType.GPU)
            {
                var gpu = Program.Computer.Hardware.First(x => x.HardwareType == HardwareType.GpuNvidia);
                gpu.Update();

                fanSensor = gpu.Sensors.First(x => x.Identifier.ToString() == GPU_FAN_ID);
            }
            else
            {
                var mb = Program.Computer.Hardware.First(x => x.HardwareType == HardwareType.Mainboard);
                mb.Update();
                fanSensor = mb.SubHardware[0].Sensors.First(x => x.Identifier.ToString() == Fans[fan]);
            }
            
            var value = fanSensor.Value.GetValueOrDefault(50);
            string outputValue;

            switch (operation)
            {
                case FanOperation.Up:
                    outputValue = (value + FAN_STEP).ToString("##") + "%";
                    fanSensor.Control.SetSoftware(value + FAN_STEP);
                    break;
                case FanOperation.Down:
                    outputValue = (value - FAN_STEP).ToString("##") + "%";
                    fanSensor.Control.SetSoftware(value - FAN_STEP);
                    break;
                case FanOperation.Default:
                    outputValue = "Default";
                    fanSensor.Control.SetDefault();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }

            Program.Display.DisplayMessage("FAN CONTROL", $"{fanSensor.Name} {outputValue}");
            Console.WriteLine($"FAN CONTROL | {fanSensor.Name} set to {outputValue}");
        }
    }
}