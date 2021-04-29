using ArduinoMonitor.Common.Constants;
using ArduinoMonitor.Common.Enums;

namespace ArduinoMonitor.Common.Controllers
{
    public static class IrController
    {
        public static event ScreenHandler ScreenChangeReceived;
        public static event MediaHandler MediaChangeReceived;
        public static event FanHandler FanChangeReceived;

        public static void Init() => SerialController.IrCommandReceived += DecodeCommand;

        public static void DecodeCommand(string command)
        {
            switch (command)
            {
                // FAN CONTROL
                case IrCommands.CH_MINUS:
                    FanChangeReceived?.Invoke(FanOperation.Down);
                    break;
                case IrCommands.CH_PLUS:
                    FanChangeReceived?.Invoke(FanOperation.Up);
                    break;
                case IrCommands.CH:
                    FanChangeReceived?.Invoke(FanOperation.Default);
                    break;

                // MEDIA
                case IrCommands.PLAY:
                    MediaChangeReceived?.Invoke(MediaOperation.PlayPause);
                    break;
                case IrCommands.PLUS:
                    MediaChangeReceived?.Invoke(MediaOperation.AddVolume);
                    break;
                case IrCommands.MINUS:
                    MediaChangeReceived?.Invoke(MediaOperation.ReduceVolume);
                    break;
                case IrCommands.PREV:
                    MediaChangeReceived?.Invoke(MediaOperation.Previous);
                    break;
                case IrCommands.NEXT:
                    MediaChangeReceived?.Invoke(MediaOperation.Next);
                    break;

                // NAVIGATION
                case IrCommands.ZERO:
                    ScreenChangeReceived?.Invoke(Screen.Base);
                    break;
                case IrCommands.ONE:
                    ScreenChangeReceived?.Invoke(Screen.GPU);
                    break;
                case IrCommands.TWO:
                    ScreenChangeReceived?.Invoke(Screen.CPU);
                    break;
                case IrCommands.THREE:
                    ScreenChangeReceived?.Invoke(Screen.RAM);
                    break;
                case IrCommands.PLUS_100:
                    ScreenChangeReceived?.Invoke(Screen.Weather);
                    break;
                case IrCommands.FOUR:
                    ScreenChangeReceived?.Invoke(Screen.FanCPU);
                    break;
                case IrCommands.FIVE:
                    ScreenChangeReceived?.Invoke(Screen.FanGPU);
                    break;
                case IrCommands.SIX:
                    ScreenChangeReceived?.Invoke(Screen.FanRear);
                    break;
                case IrCommands.SEVEN:
                    ScreenChangeReceived?.Invoke(Screen.FanFront1);
                    break;
                case IrCommands.EIGHT:
                    ScreenChangeReceived?.Invoke(Screen.FanFront2);
                    break;
                case IrCommands.NINE:
                    ScreenChangeReceived?.Invoke(Screen.FanFront3);
                    break;
                case IrCommands.PLUS_200:
                    ScreenChangeReceived?.Invoke(Screen.FrontFans);
                    break;
            }
        }
    }
}