using ArduinoMonitor.Common.Enums;

namespace ArduinoMonitor.Common
{
    public delegate void MediaHandler(MediaOperation operation);

    public delegate void FanHandler(FanOperation operation);

    public delegate void ScreenHandler(Screen screen);

    public delegate void IrCommandHandler(string command);

    public delegate void DisplayStatusHandler(bool visible);
}