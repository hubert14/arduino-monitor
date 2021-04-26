using WindowsInput;
using WindowsInput.Native;

namespace ArduinoMonitor.Controllers
{
    public static class MediaController
    {
        private static readonly InputSimulator Simulator = new InputSimulator();

        public static void PlayPause() => Simulator.Keyboard.KeyPress(VirtualKeyCode.MEDIA_PLAY_PAUSE);
        public static void Next() => Simulator.Keyboard.KeyPress(VirtualKeyCode.MEDIA_NEXT_TRACK);
        public static void Previous() => Simulator.Keyboard.KeyPress(VirtualKeyCode.MEDIA_PREV_TRACK);

        public static void AddVolume() => Simulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_UP);
        public static void ReduceVolume() => Simulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_DOWN);
        public static void MuteVolume() => Simulator.Keyboard.KeyPress(VirtualKeyCode.VOLUME_MUTE);
    }
}