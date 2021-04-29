using System;
using ArduinoMonitor.Common.Enums;
using InputSimulatorStandard;
using InputSimulatorStandard.Native;

namespace ArduinoMonitor.Common.Controllers
{
    public static class MediaController
    {
        static MediaController() => IrController.MediaChangeReceived += HandleOperation;

        /// <summary>
        /// Need for reference to media controller
        /// </summary>
        public static void Init()
        {
        }

        private static readonly InputSimulator Simulator = new InputSimulator();

        private static void HandleOperation(MediaOperation operation)
        {
            switch (operation)
            {
                case MediaOperation.PlayPause:
                    Simulator.Keyboard.KeyPress(VirtualKeyCode.MEDIA_PLAY_PAUSE);
                    return;
                case MediaOperation.Next:
                    Simulator.Keyboard.KeyPress(VirtualKeyCode.MEDIA_NEXT_TRACK);
                    break;
                case MediaOperation.Previous:
                    Simulator.Keyboard.KeyPress(VirtualKeyCode.MEDIA_PREV_TRACK);
                    break;
                case MediaOperation.AddVolume:
                    Simulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_UP);
                    break;
                case MediaOperation.ReduceVolume:
                    Simulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_DOWN);
                    break;
                case MediaOperation.MuteVolume:
                    Simulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_MUTE);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }
        }
    }
}