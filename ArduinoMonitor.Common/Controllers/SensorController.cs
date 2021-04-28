using System;
using System.Collections.Generic;
using System.Linq;
using ArduinoMonitor.Common.Enums;
using ArduinoMonitor.Common.Models;
using OpenHardwareMonitor.Hardware;

namespace ArduinoMonitor.Common.Controllers
{
    public static class SensorController
    {
        #region Identifiers

        // GPU
        private const string GPU_MEMORY = "/nvidiagpu/0/smalldata/2";
        const string GPU_CORE = "/nvidiagpu/0/load/0";
        const string GPU_TEMP = "/nvidiagpu/0/temperature/0";

        // CPU
        private const string CPU_POWER = "/amdcpu/0/power/0";
        private const string CPU_CLOCK = "/amdcpu/0/clock/1";
        private const string CPU_CORE = "/amdcpu/0/load/0";
        const string CPU_TEMP = "/amdcpu/0/temperature/0";

        // RAM
        private const string RAM_USED = "/ram/data/0";
        private const string RAM_AVAILABLE = "/ram/data/1";

        // FANS
        private const string GPU_FAN_RPM = "/nvidiagpu/0/fan/0";
        private const string GPU_FAN_PERCENTAGE = "/nvidiagpu/0/control/0";

        private const string CPU_FAN_RPM = "/lpc/nct6795d/fan/1";
        private const string CPU_FAN_PERCENTAGE = "/lpc/nct6795d/control/1";

        private const string FRONT1_FAN_RPM = "/lpc/nct6795d/fan/0";
        private const string FRONT1_FAN_PERCENTAGE = "/lpc/nct6795d/control/0";

        private const string FRONT2_FAN_RPM = "/lpc/nct6795d/fan/4";
        private const string FRONT2_FAN_PERCENTAGE = "/lpc/nct6795d/control/4";

        private const string FRONT3_FAN_RPM = "/lpc/nct6795d/fan/5";
        private const string FRONT3_FAN_PERCENTAGE = "/lpc/nct6795d/control/5";

        private const string REAR_FAN_RPM = "/lpc/nct6795d/fan/2";
        private const string REAR_FAN_PERCENTAGE = "/lpc/nct6795d/control/2";

        #endregion

        private const string PERCENTAGE_FORMAT = "###.#";
        private const string RPM_FORMAT = "####";
        
        private static IComputer _computer;

        public static void Init(IComputer computer) => _computer = computer;


        private static readonly List<(FanType Type, string RPMId, string PercId)> MotherboardFans =
            new List<(FanType Fan, string RPMId, string PercentageId)>
            {
                (FanType.CPU, CPU_FAN_RPM, CPU_FAN_PERCENTAGE),
                (FanType.Front1, FRONT1_FAN_RPM, FRONT1_FAN_PERCENTAGE),
                (FanType.Front2, FRONT2_FAN_RPM, FRONT2_FAN_PERCENTAGE),
                (FanType.Front3, FRONT3_FAN_RPM, FRONT3_FAN_PERCENTAGE),
                (FanType.Rear, REAR_FAN_RPM, REAR_FAN_PERCENTAGE),
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
                    subMainboard.Sensors.First(x => x.Identifier.ToString() == fan.RPMId).Value?.ToString(RPM_FORMAT) ??
                    "~";
                var percentage =
                    subMainboard.Sensors.First(x => x.Identifier.ToString() == fan.PercId).Value?.ToString(PERCENTAGE_FORMAT) ??
                    "~";

                return new FanItem(fan.Type) {Percentage = percentage, RPM = rpm};
            }).ToList();

            var gpuItem = new FanItem(FanType.GPU);

            foreach (var sensor in gpu.Sensors)
                switch (sensor.Identifier.ToString())
                {
                    case GPU_FAN_RPM:
                        gpuItem.RPM = sensor.Value?.ToString(RPM_FORMAT) ?? "~";
                        break;
                    case GPU_FAN_PERCENTAGE:
                        gpuItem.Percentage = sensor.Value?.ToString(PERCENTAGE_FORMAT) ?? "~";
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
                    RPM = subMainBoard.Sensors.First(x => x.Identifier.ToString() == f.RPMId).Value?.ToString(RPM_FORMAT),
                    Percentage = subMainBoard.Sensors.First(x => x.Identifier.ToString() == f.PercId).Value
                        ?.ToString("###"),
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
                        case GPU_FAN_RPM:
                            gpuItem.RPM = sensor.Value?.ToString(RPM_FORMAT) ?? "~";
                            break;
                        case GPU_FAN_PERCENTAGE:
                            gpuItem.Percentage = sensor.Value?.ToString(PERCENTAGE_FORMAT) ?? "~";
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
                RPM = subMainBoard.Sensors.First(x => x.Identifier.ToString() == fanItem.RPMId).Value?.ToString(RPM_FORMAT),
                Percentage = subMainBoard.Sensors.First(x => x.Identifier.ToString() == fanItem.PercId).Value
                    ?.ToString(PERCENTAGE_FORMAT)
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
                    case GPU_CORE:
                        result.UsedPercentage = sensor.Value?.ToString(PERCENTAGE_FORMAT) ?? "~";
                        break;
                    case GPU_MEMORY:
                        result.Memory = sensor.Value?.ToString(RPM_FORMAT) ?? "~";
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
                    case CPU_CORE:
                        result.UsedPercentage = sensor.Value?.ToString(PERCENTAGE_FORMAT) ?? "~";
                        break;
                    case CPU_POWER:
                        result.Power = sensor.Value?.ToString(RPM_FORMAT) ?? "~";
                        break;
                    case CPU_CLOCK:
                        result.Clock = sensor.Value?.ToString(RPM_FORMAT) ?? "~";
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
                    case RAM_USED:
                        result.Used = sensor.Value?.ToString(PERCENTAGE_FORMAT) ?? "~";
                        break;
                    case RAM_AVAILABLE:
                        result.Available = sensor.Value?.ToString(PERCENTAGE_FORMAT) ?? "~";
                        break;
                }

            result.UsedPercentage = ((float.Parse(result.Used) * 100) / (float.Parse(result.Used) +
                                                                         float.Parse(result.Available)))
                .ToString("##.#");

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
                    result.Temperature = sensor.Value?.ToString("##") ?? "~";
                else if (sensor.Identifier.ToString() == identifiers.Load)
                    result.Load = sensor.Value?.ToString(PERCENTAGE_FORMAT) ?? "~";

            return result;
        }

        private static (string Load, string Temp) GetIdentifiers(HardwareType type)
        {
            switch (type)
            {
                case HardwareType.GpuNvidia:
                    return (GPU_CORE, GPU_TEMP);
                case HardwareType.CPU:
                    return (CPU_CORE, CPU_TEMP);
                default:
                    throw new Exception("Hardware type not supported");
            }
        }
    }
}