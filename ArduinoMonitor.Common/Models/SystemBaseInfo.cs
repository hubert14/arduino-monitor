namespace ArduinoMonitor.Common.Models
{
    public class SystemBaseInfo
    {
        public class HardwareBaseInfo
        {
            public float? Load { get; set; }
            public float? Temperature { get; set; }
        }

        public HardwareBaseInfo GPU { get; set; }
        public HardwareBaseInfo CPU { get; set; }
    }
}