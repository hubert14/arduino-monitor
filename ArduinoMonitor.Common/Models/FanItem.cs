using ArduinoMonitor.Common.Enums;

namespace ArduinoMonitor.Common.Models
{
    public class FanItem
    {
        public FanType Fan { get; }

        public float? RPM { get; set; }
        public float? Percentage { get; set; }

        public FanItem(FanType fan) => Fan = fan;
    }
}