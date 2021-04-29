using System;
using System.Collections.Generic;
using System.Linq;
using ArduinoMonitor.Common.Constants;
using ArduinoMonitor.Common.Enums;
using OpenHardwareMonitor.Hardware;

namespace ArduinoMonitor.Common.Controllers
{
    internal static class FanController
    {
        private static IComputer _computer;

        static FanController() => IrController.FanChangeReceived += ChangeFan;

        public static void Init(IComputer computer) => _computer = computer;

        private const float FAN_STEP = 10;

        private static readonly Dictionary<FanType, string> Fans = new Dictionary<FanType, string>
        {
            {FanType.CPU, HardwareIdentifiers.CPU_FAN_PERCENTAGE},
            {FanType.GPU, HardwareIdentifiers.GPU_FAN_PERCENTAGE},
            {FanType.Front1, HardwareIdentifiers.FRONT1_FAN_PERCENTAGE},
            {FanType.Front2, HardwareIdentifiers.FRONT2_FAN_PERCENTAGE},
            {FanType.Front3, HardwareIdentifiers.FRONT3_FAN_PERCENTAGE},
            {FanType.Rear, HardwareIdentifiers.REAR_FAN_PERCENTAGE}
        };

        public static void ChangeFan(FanOperation operation)
        {
            ISensor fanSensor;
            var outputValue = string.Empty;

            if (DisplayController.CurrentScreen == Screen.FrontFans)
            {
                var mb = _computer.Hardware.First(x => x.HardwareType == HardwareType.Mainboard);
                mb.Update();
                var fans = mb.SubHardware[0].Sensors
                    .Where(x =>
                        x.Identifier.ToString() == HardwareIdentifiers.FRONT1_FAN_PERCENTAGE ||
                        x.Identifier.ToString() == HardwareIdentifiers.FRONT2_FAN_PERCENTAGE ||
                        x.Identifier.ToString() == HardwareIdentifiers.FRONT3_FAN_PERCENTAGE)
                    .ToList();

                var minValue = fans.Min(x => x.Value) ?? 50;

                fans.ForEach(x => outputValue = ChangeFan(x, minValue, operation));

                DisplayController.Display("Fan control", $"{outputValue}", true);
                return;
            }

            FanType fan;
            try
            {
                fan = DisplayController.CurrentScreen.GetFanType();
            }
            catch (Exception e)
            {
                DisplayController.Display("Error", e.Message, true);
                return;
            }

            if (fan == FanType.GPU)
            {
                var gpu = _computer.Hardware.First(x => x.HardwareType == HardwareType.GpuNvidia);
                gpu.Update();

                fanSensor = gpu.Sensors.First(x => x.Identifier.ToString() == HardwareIdentifiers.GPU_FAN_PERCENTAGE);
            }
            else
            {
                var mb = _computer.Hardware.First(x => x.HardwareType == HardwareType.Mainboard);
                mb.Update();
                fanSensor = mb.SubHardware[0].Sensors.First(x => x.Identifier.ToString() == Fans[fan]);
            }

            var value = fanSensor.Value.GetValueOrDefault(50);
            outputValue = ChangeFan(fanSensor, value, operation);

            DisplayController.Display("Fan control", $"{outputValue}", true);
        }

        private static string ChangeFan(ISensor fan, float value, FanOperation operation)
        {
            var resultValue = value;

            string outputValue;
            switch (operation)
            {
                case FanOperation.Up:
                    resultValue += FAN_STEP;
                    if (resultValue > 100) resultValue = 100;
                    outputValue = resultValue.ToString("##0") + "%";
                    fan.Control.SetSoftware(resultValue);
                    break;
                case FanOperation.Down:
                    resultValue -= FAN_STEP;
                    if (resultValue < 0) resultValue = 0;
                    outputValue = resultValue.ToString("##0") + "%";
                    fan.Control.SetSoftware(resultValue);
                    break;
                case FanOperation.Default:
                    outputValue = "Default";
                    fan.Control.SetDefault();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }

            return outputValue;
        }
    }
}