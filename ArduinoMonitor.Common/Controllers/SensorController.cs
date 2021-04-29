using System;
using System.Collections.Generic;
using System.Linq;
using ArduinoMonitor.Common.Constants;
using ArduinoMonitor.Common.Enums;
using ArduinoMonitor.Common.Models;
using OpenHardwareMonitor.Hardware;

namespace ArduinoMonitor.Common.Controllers
{
    public static class SensorController
    {
        private static IComputer _computer;

        public static void Init(IComputer computer) => _computer = computer;

        private static readonly List<(FanType Type, string RPMId, string PercId)> MotherboardFans =
            new List<(FanType Fan, string RPMId, string PercentageId)>
            {
                (FanType.CPU, HardwareIdentifiers.CPU_FAN_RPM, HardwareIdentifiers.CPU_FAN_PERCENTAGE),
                (FanType.Front1, HardwareIdentifiers.FRONT1_FAN_RPM, HardwareIdentifiers.FRONT1_FAN_PERCENTAGE),
                (FanType.Front2, HardwareIdentifiers.FRONT2_FAN_RPM, HardwareIdentifiers.FRONT2_FAN_PERCENTAGE),
                (FanType.Front3, HardwareIdentifiers.FRONT3_FAN_RPM, HardwareIdentifiers.FRONT3_FAN_PERCENTAGE),
                (FanType.Rear, HardwareIdentifiers.REAR_FAN_RPM, HardwareIdentifiers.REAR_FAN_PERCENTAGE),
            };

        public static List<FanItem> GetFansInfo()
        {
            var gpu = _computer.Hardware.First(x => x.HardwareType == HardwareType.GpuNvidia);
            gpu.Update();
            var mainboard = _computer.Hardware.First(x => x.HardwareType == HardwareType.Mainboard);
            mainboard.Update();
            var subMainboard = mainboard.SubHardware[0];
            subMainboard.Update();

            var resultFansList = MotherboardFans.Select(fan =>
            {
                var rpm =
                    subMainboard.Sensors.First(x => x.Identifier.ToString() == fan.RPMId).Value;
                var percentage =
                    subMainboard.Sensors.First(x => x.Identifier.ToString() == fan.PercId).Value;

                return new FanItem(fan.Type) {Percentage = percentage, RPM = rpm};
            }).ToList();

            var gpuItem = new FanItem(FanType.GPU);

            foreach (var sensor in gpu.Sensors)
                switch (sensor.Identifier.ToString())
                {
                    case HardwareIdentifiers.GPU_FAN_RPM:
                        gpuItem.RPM = sensor.Value;
                        break;
                    case HardwareIdentifiers.GPU_FAN_PERCENTAGE:
                        gpuItem.Percentage = sensor.Value;
                        break;
                }

            resultFansList.Add(gpuItem);
            return resultFansList;
        }

        public static List<FanItem> GetFrontFansInfo()
        {
            var mainboard = _computer.Hardware.First(x => x.HardwareType == HardwareType.Mainboard);
            mainboard.Update();
            var subMainBoard = mainboard.SubHardware[0];
            subMainBoard.Update();

            return MotherboardFans.Where(x =>
                    x.Type == FanType.Front1 || x.Type == FanType.Front2 || x.Type == FanType.Front3)
                .Select(f => new FanItem(f.Type)
                {
                    RPM = subMainBoard.Sensors.First(x => x.Identifier.ToString() == f.RPMId).Value,
                    Percentage = subMainBoard.Sensors.First(x => x.Identifier.ToString() == f.PercId).Value
                }).ToList();
        }

        public static FanItem GetFanInfo(FanType type)
        {
            if (type == FanType.GPU)
            {
                var gpu = _computer.Hardware.First(x => x.HardwareType == HardwareType.GpuNvidia);
                gpu.Update();

                var gpuItem = new FanItem(FanType.GPU);

                foreach (var sensor in gpu.Sensors)
                    switch (sensor.Identifier.ToString())
                    {
                        case HardwareIdentifiers.GPU_FAN_RPM:
                            gpuItem.RPM = sensor.Value;
                            break;
                        case HardwareIdentifiers.GPU_FAN_PERCENTAGE:
                            gpuItem.Percentage = sensor.Value;
                            break;
                    }

                return gpuItem;
            }

            var mainboard = _computer.Hardware.First(x => x.HardwareType == HardwareType.Mainboard);
            mainboard.Update();
            var subMainBoard = mainboard.SubHardware[0];
            subMainBoard.Update();

            var fanItem = MotherboardFans.First(x => x.Type == type);

            return new FanItem(type)
            {
                RPM = subMainBoard.Sensors.First(x => x.Identifier.ToString() == fanItem.RPMId).Value,
                Percentage = subMainBoard.Sensors.First(x => x.Identifier.ToString() == fanItem.PercId).Value
            };
        }

        public static GpuInfo GetGpuInfo()
        {
            var gpu = _computer.Hardware.First(x => x.HardwareType == HardwareType.GpuNvidia);
            gpu.Update();

            var result = new GpuInfo();

            foreach (var sensor in gpu.Sensors)
                switch (sensor.Identifier.ToString())
                {
                    case HardwareIdentifiers.GPU_TEMP:
                        result.Temperature = sensor.Value;
                        break;
                    case HardwareIdentifiers.GPU_POWER:
                        result.Power = sensor.Value;
                        break;
                    case HardwareIdentifiers.GPU_CORE:
                        result.UsedPercentage = sensor.Value;
                        break;
                    case HardwareIdentifiers.GPU_MEMORY:
                        result.Memory = sensor.Value;
                        break;
                }

            return result;
        }

        public static CpuInfo GetCpuInfo()
        {
            var mainBoard = _computer.Hardware.First(x => x.HardwareType == HardwareType.Mainboard);
            var cpu = _computer.Hardware.First(x => x.HardwareType == HardwareType.CPU);

            cpu.Update();
            mainBoard.Update();

            var subMainBoard = mainBoard.SubHardware[0];
            subMainBoard.Update();

            var result = new CpuInfo();

            foreach (var sensor in cpu.Sensors)
                switch (sensor.Identifier.ToString())
                {
                    case HardwareIdentifiers.CPU_TEMP:
                        result.Temperature = sensor.Value;
                        break;
                    case HardwareIdentifiers.CPU_CORE:
                        result.UsedPercentage = sensor.Value;
                        break;
                    case HardwareIdentifiers.CPU_POWER:
                        result.Power = sensor.Value;
                        break;
                    case HardwareIdentifiers.CPU_CLOCK:
                        result.Clock = sensor.Value;
                        break;
                }

            return result;
        }

        public static RamInfo GetRamInfo()
        {
            var ram = _computer.Hardware.First(x => x.HardwareType == HardwareType.RAM);
            ram.Update();

            var result = new RamInfo();

            foreach (var sensor in ram.Sensors)
                switch (sensor.Identifier.ToString())
                {
                    case HardwareIdentifiers.RAM_USED:
                        result.Used = sensor.Value;
                        break;
                    case HardwareIdentifiers.RAM_AVAILABLE:
                        result.Available = sensor.Value;
                        break;
                }

            result.UsedPercentage = (result.Used * 100) / (result.Used + result.Available);
            return result;
        }

        public static SystemBaseInfo GetBaseInfo()
        {
            var cpu = _computer.Hardware.First(x => x.HardwareType == HardwareType.CPU);
            var gpu = _computer.Hardware.First(x => x.HardwareType == HardwareType.GpuNvidia);

            cpu.Update();
            gpu.Update();

            var info = new SystemBaseInfo
            {
                GPU = GetBaseUnitInfo(gpu, GetIdentifiers(HardwareType.GpuNvidia)),
                CPU = GetBaseUnitInfo(cpu, GetIdentifiers(HardwareType.CPU))
            };

            return info;
        }

        private static SystemBaseInfo.HardwareBaseInfo GetBaseUnitInfo(IHardware hardware,
            (string Load, string Temp) identifiers)
        {
            var result = new SystemBaseInfo.HardwareBaseInfo();

            foreach (var sensor in hardware.Sensors)
                if (sensor.Identifier.ToString() == identifiers.Temp)
                    result.Temperature = sensor.Value;
                else if (sensor.Identifier.ToString() == identifiers.Load)
                    result.Load = sensor.Value;

            return result;
        }

        private static (string Load, string Temp) GetIdentifiers(HardwareType type)
        {
            switch (type)
            {
                case HardwareType.GpuNvidia:
                    return (HardwareIdentifiers.GPU_CORE, HardwareIdentifiers.GPU_TEMP);
                case HardwareType.CPU:
                    return (HardwareIdentifiers.CPU_CORE, HardwareIdentifiers.CPU_TEMP);
                default:
                    throw new Exception("Hardware type not supported");
            }
        }
    }
}