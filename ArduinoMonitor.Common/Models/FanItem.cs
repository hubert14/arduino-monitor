using ArduinoMonitor.Common.Enums;

namespace ArduinoMonitor.Common.Models
{
    public class FanItem
    {
        public FanType Fan { get; }

        public string RPM { get; set; }
        public string Percentage { get; set; }

        public FanItem(FanType fan) => Fan = fan;
    }
}