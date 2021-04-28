using System;

namespace ArduinoMonitor.Common.Enums
{
    public enum Screen
    {
        ChangeVisibility,
        Base,
        GPU,
        CPU,
        RAM,
        Weather,
        FrontFans,
        FanCPU,
        FanGPU,
        FanFront1,
        FanFront2,
        FanFront3,
        FanRear
    }

    public static class ScreenExtensions
    {
        public static FanType GetFanType(this Screen screen)
        {
            switch (screen)
            {
                case Screen.FanCPU:
                    return FanType.CPU;
                case Screen.FanGPU:
                    return FanType.GPU;
                case Screen.FanFront1:
                    return FanType.Front1;
                case Screen.FanFront2:
                    return FanType.Front2;
                case Screen.FanFront3:
                    return FanType.Front3;
                case Screen.FanRear:
                    return FanType.Rear;
                default:
                    throw new Exception("Not a Fan screen");
            }
        }

        public static bool IsFanScreen(this Screen screen)
        {
            return
                screen == Screen.FrontFans ||
                screen == Screen.FanCPU ||
                screen == Screen.FanGPU ||
                screen == Screen.FanFront1 ||
                screen == Screen.FanFront2 ||
                screen == Screen.FanFront3 ||
                screen == Screen.FanRear;
        }
    }
}