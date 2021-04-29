namespace ArduinoMonitor.Common.Constants
{
    public static class HardwareIdentifiers
    {
        // GPU
        public const string GPU_MEMORY = "/nvidiagpu/0/smalldata/2";
        public const string GPU_CORE = "/nvidiagpu/0/load/0";
        public const string GPU_TEMP = "/nvidiagpu/0/temperature/0";
        public const string GPU_POWER = "/nvidiagpu/0/power/0";

        // CPU
        public const string CPU_POWER = "/amdcpu/0/power/0";
        public const string CPU_CLOCK = "/amdcpu/0/clock/1";
        public const string CPU_CORE = "/amdcpu/0/load/0";
        public const string CPU_TEMP = "/amdcpu/0/temperature/0";

        // RAM
        public const string RAM_USED = "/ram/data/0";
        public const string RAM_AVAILABLE = "/ram/data/1";

        // FANS
        public const string GPU_FAN_RPM = "/nvidiagpu/0/fan/0";
        public const string GPU_FAN_PERCENTAGE = "/nvidiagpu/0/control/0";

        public const string CPU_FAN_RPM = "/lpc/nct6795d/fan/1";
        public const string CPU_FAN_PERCENTAGE = "/lpc/nct6795d/control/1";

        public const string FRONT1_FAN_RPM = "/lpc/nct6795d/fan/0";
        public const string FRONT1_FAN_PERCENTAGE = "/lpc/nct6795d/control/0";

        public const string FRONT2_FAN_RPM = "/lpc/nct6795d/fan/4";
        public const string FRONT2_FAN_PERCENTAGE = "/lpc/nct6795d/control/4";

        public const string FRONT3_FAN_RPM = "/lpc/nct6795d/fan/5";
        public const string FRONT3_FAN_PERCENTAGE = "/lpc/nct6795d/control/5";

        public const string REAR_FAN_RPM = "/lpc/nct6795d/fan/2";
        public const string REAR_FAN_PERCENTAGE = "/lpc/nct6795d/control/2";
    }
}