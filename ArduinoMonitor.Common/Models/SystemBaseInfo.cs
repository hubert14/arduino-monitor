namespace ArduinoMonitor.Common.Models
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
}