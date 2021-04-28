using ArduinoMonitor.Common.Enums;
using ArduinoMonitor.Common.Models;

namespace ArduinoMonitor.Common.Controllers
{
    public static class IrController
    {
        public static event DisplayHandler Display;
        public static event MediaHandler Media;
        public static event FanHandler Fan;

        public static void DecodeCommand(string command)
        {
            command = command
                .Replace("\r\n", "")
                .Replace("\r", "")
                .Replace("\n", "");

            switch (command)
            {
                // FAN CONTROL
                case IrCommands.CH_MINUS_COMMAND:
                    Fan?.Invoke(FanOperation.Down);
                    break;

                case IrCommands.CH_PLUS_COMMAND:
                    Fan?.Invoke(FanOperation.Up);
                    break;

                case IrCommands.CH_COMMAND:
                    Fan?.Invoke(FanOperation.Default);
                    break;

                // MEDIA
                case IrCommands.PLAY_COMMAND:
                    Media?.Invoke(MediaOperation.PlayPause);
                    break;
                case IrCommands.PLUS_COMMAND:
                    Media?.Invoke(MediaOperation.AddVolume);
                    break;
                case IrCommands.MINUS_COMMAND:
                    Media?.Invoke(MediaOperation.ReduceVolume);
                    break;
                case IrCommands.EQ_COMMAND:
                    Media?.Invoke(MediaOperation.MuteVolume);
                    break;
                case IrCommands.PREV_COMMAND:
                    Media?.Invoke(MediaOperation.Previous);
                    break;
                case IrCommands.NEXT_COMMAND:
                    Media?.Invoke(MediaOperation.Next);
                    break;

                // NAVIGATION
                case IrCommands.ZERO_COMMAND:
                    Display?.Invoke(Screen.Base);
                    break;
                case IrCommands.ONE_COMMAND:
                    Display?.Invoke(Screen.GPU);
                    break;
                case IrCommands.TWO_COMMAND:
                    Display?.Invoke(Screen.CPU);
                    break;
                case IrCommands.THREE_COMMAND:
                    Display?.Invoke(Screen.RAM);
                    break;
                case IrCommands.PLUS_100_COMMAND:
                    Display?.Invoke(Screen.Weather);
                    break;
                case IrCommands.FOUR_COMMAND:
                    Display?.Invoke(Screen.FanCPU);
                    break;
                case IrCommands.FIVE_COMMAND:
                    Display?.Invoke(Screen.FanGPU);
                    break;
                case IrCommands.SIX_COMMAND:
                    Display?.Invoke(Screen.FanRear);
                    break;
                case IrCommands.SEVEN_COMMAND:
                    Display?.Invoke(Screen.FanFront1);
                    break;
                case IrCommands.EIGHT_COMMAND:
                    Display?.Invoke(Screen.FanFront2);
                    break;
                case IrCommands.NINE_COMMAND:
                    Display?.Invoke(Screen.FanFront3);
                    break;
                case IrCommands.PLUS_200_COMMAND:
                    Display?.Invoke(Screen.FrontFans);
                    break;
            }
        }
    }
}