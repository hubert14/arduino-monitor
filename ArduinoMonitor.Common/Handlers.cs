using ArduinoMonitor.Common.Controllers;
using ArduinoMonitor.Common.Enums;

namespace ArduinoMonitor.Common
{
    public delegate void MediaHandler(MediaOperation operation);

    public delegate void FanHandler(FanOperation operation);

    public delegate void DisplayHandler(Screen screen);
}