using System;
using System.Linq;
using OpenHardwareMonitor.Hardware;

namespace ArduinoMonitor.Controllers
{
    public class SystemBaseInfo
    {
        public class HardwareBaseInfo
        {
            public string Load { get; set; }
            public string Temperature { get; set; }
        }

        public HardwareBaseInfo GPU { get; set; }
        public HardwareBaseInfo CPU { get; set; }
    }

    public class GpuInfo
    {
        public string FanRpm { get; set; }
        public string FanPercentage { get; set; }
        public string Memory { get; set; }
    }

    public class CpuInfo
    {
        public string FanRpm { get; set; }
        public string Power { get; set; }
        public string Clock { get; set; }
    }

    public class RamInfo
    {
        public string UsedPercentage { get; set; }
        public string Used { get; set; }
        public string Available { get; set; }
    }

    public static class SensorController
    {
        // GPU
        private const string GPU_FAN_RPM = "/nvidiagpu/0/fan/0";
        private const string GPU_FAN_PERCENTAGE = "/nvidiagpu/0/control/0";
        private const string GPU_MEMORY = "/nvidiagpu/0/smalldata/2";

        // CPU
        private const string CPU_FAN_RPM = "/lpc/nct6795d/fan/0";
        private const string CPU_POWER = "/amdcpu/0/power/0";
        private const string CPU_CLOCK = "/amdcpu/0/clock/1";

        // RAM
        private const string RAM_USED = "/ram/data/0";
        private const string RAM_AVAILABLE = "/ram/data/1";

        public static GpuInfo GetGpuInfo(IComputer computer)
        {
            var gpu = computer.Hardware.First(x => x.HardwareType == HardwareType.GpuNvidia);
            gpu.Update();

            var result = new GpuInfo();

            foreach (var sensor in gpu.Sensors)
                if (sensor.Identifier.ToString() == GPU_FAN_RPM)
                    result.FanRpm = sensor.Value?.ToString("####") ?? "~";
                else if (sensor.Identifier.ToString() == GPU_FAN_PERCENTAGE)
                    result.FanPercentage = sensor.Value?.ToString("####") ?? "~";
                else if (sensor.Identifier.ToString() == GPU_MEMORY)
                    result.Memory = sensor.Value?.ToString("####") ?? "~";

            return result;
        }

        public static CpuInfo GetCpuInfo(IComputer computer)
        {
            var mainBoard = computer.Hardware.First(x => x.HardwareType == HardwareType.Mainboard);
            var cpu = computer.Hardware.First(x => x.HardwareType == HardwareType.CPU);
            
            cpu.Update();
            mainBoard.Update();

            var subMainBoard = mainBoard.SubHardware[0];
            subMainBoard.Update();
            
            var result = new CpuInfo();

            result.FanRpm = subMainBoard.Sensors.First(x => x.Identifier.ToString() == CPU_FAN_RPM).Value?.ToString("####") ?? "~";

            foreach (var sensor in cpu.Sensors)
                if (sensor.Identifier.ToString() == CPU_POWER)
                    result.Power = sensor.Value?.ToString("##.#") ?? "~";
                else if (sensor.Identifier.ToString() == CPU_CLOCK)
                    result.Clock = sensor.Value?.ToString("####") ?? "~";

            return result;
        }

        public static RamInfo GetRamInfo(IComputer computer)
        {
            var ram = computer.Hardware.First(x => x.HardwareType == HardwareType.RAM);
            ram.Update();

            var result = new RamInfo();

            foreach (var sensor in ram.Sensors)
                if (sensor.Identifier.ToString() == RAM_USED)
                    result.Used = sensor.Value?.ToString("##.#") ?? "~";
                else if (sensor.Identifier.ToString() == RAM_AVAILABLE)
                    result.Available = sensor.Value?.ToString("##.#") ?? "~";

            result.UsedPercentage = ((float.Parse(result.Used) * 100) / (float.Parse(result.Used) +
                                                                     float.Parse(result.Available)))
                .ToString("##.#");
            
            return result;
        }

        public static SystemBaseInfo GetBaseInfo(IComputer computer)
        {
            var cpu = computer.Hardware.First(x => x.HardwareType == HardwareType.CPU);
            var gpu = computer.Hardware.First(x => x.HardwareType == HardwareType.GpuNvidia);

            cpu.Update();
            gpu.Update();

            var info = new SystemBaseInfo
            {
                GPU = GetBaseUnitInfo(gpu, GetIdentifiers(HardwareType.GpuNvidia)),
                CPU = GetBaseUnitInfo(cpu, GetIdentifiers(HardwareType.CPU))
            };

            return info;
        }

        private static SystemBaseInfo.HardwareBaseInfo GetBaseUnitInfo(IHardware hardware, (string Load, string Temp) identifiers)
        {
            var result = new SystemBaseInfo.HardwareBaseInfo();

            foreach (var sensor in hardware.Sensors)
                if (sensor.Identifier.ToString() == identifiers.Temp)
                    result.Temperature = sensor.Value?.ToString("##.##") ?? "~";
                else if (sensor.Identifier.ToString() == identifiers.Load)
                    result.Load = sensor.Value?.ToString("##.##") ?? "~";

            return result;
        }

        private static (string Load, string Temp) GetIdentifiers(HardwareType type)
        {
            // GPU
            const string GPU_CORE = "/nvidiagpu/0/load/0";
            const string GPU_TEMP = "/nvidiagpu/0/temperature/0";

            // CPU
            const string CPU_CORE = "/amdcpu/0/load/0";
            const string CPU_TEMP = "/amdcpu/0/temperature/0";

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