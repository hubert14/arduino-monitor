using System;
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

            var splittedCommand = command.Split(':');

            var header = splittedCommand[0];
            var body = splittedCommand[1];

            switch (header)
            {
                case IrHeaders.SCREEN_ON_STATUS:
                    DisplayController.IsDisplayOn = body == "1";
                    return;
                case IrHeaders.IR_COMMAND:
                    ProcessIrCommand(body);
                    return;
            }
        }

        private static void ProcessIrCommand(string command)
        {
            switch (command)
            {
                // FAN CONTROL
                case IrCommands.CH_MINUS:
                    Fan?.Invoke(FanOperation.Down);
                    break;
                case IrCommands.CH_PLUS:
                    Fan?.Invoke(FanOperation.Up);
                    break;
                case IrCommands.CH:
                    Fan?.Invoke(FanOperation.Default);
                    break;

                // MEDIA
                case IrCommands.PLAY:
                    Media?.Invoke(MediaOperation.PlayPause);
                    break;
                case IrCommands.PLUS:
                    Media?.Invoke(MediaOperation.AddVolume);
                    break;
                case IrCommands.MINUS:
                    Media?.Invoke(MediaOperation.ReduceVolume);
                    break;
                case IrCommands.PREV:
                    Media?.Invoke(MediaOperation.Previous);
                    break;
                case IrCommands.NEXT:
                    Media?.Invoke(MediaOperation.Next);
                    break;

                // NAVIGATION
                case IrCommands.EQ:
                    Display?.Invoke(Screen.ChangeVisibility);
                    break;
                case IrCommands.ZERO:
                    Display?.Invoke(Screen.Base);
                    break;
                case IrCommands.ONE:
                    Display?.Invoke(Screen.GPU);
                    break;
                case IrCommands.TWO:
                    Display?.Invoke(Screen.CPU);
                    break;
                case IrCommands.THREE:
                    Display?.Invoke(Screen.RAM);
                    break;
                case IrCommands.PLUS_100:
                    Display?.Invoke(Screen.Weather);
                    break;
                case IrCommands.FOUR:
                    Display?.Invoke(Screen.FanCPU);
                    break;
                case IrCommands.FIVE:
                    Display?.Invoke(Screen.FanGPU);
                    break;
                case IrCommands.SIX:
                    Display?.Invoke(Screen.FanRear);
                    break;
                case IrCommands.SEVEN:
                    Display?.Invoke(Screen.FanFront1);
                    break;
                case IrCommands.EIGHT:
                    Display?.Invoke(Screen.FanFront2);
                    break;
                case IrCommands.NINE:
                    Display?.Invoke(Screen.FanFront3);
                    break;
                case IrCommands.PLUS_200:
                    Display?.Invoke(Screen.FrontFans);
                    break;
            }
        }
    }
}