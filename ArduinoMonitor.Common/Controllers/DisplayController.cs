using System;
using System.Linq;
using System.Threading.Tasks;
using ArduinoMonitor.Common.Constants;
using ArduinoMonitor.Common.Enums;

namespace ArduinoMonitor.Common.Controllers
{
    public static class DisplayController
    {
        private const int DISPLAY_DELAY = 2000;

        private static bool _isContentBusy;
        private static bool _forceUpdate;

        private static Screen PreviousScreen { get; set; }
        public static Screen CurrentScreen { get; set; }

        public static bool IsDisplayOn { get; set; }

        private static readonly Screen[] ImmutableScreens =
        {
            Screen.Weather
        };

        static DisplayController()
        {
            CurrentScreen = Screen.Base;

            IrController.ScreenChangeReceived += ChangeScreen;
            SerialController.DisplayStatusChanged += ChangeDisplayStatus;
        }

        public static void Init()
        {
            SerialController.WriteToSerial(DeviceSymbols.LCD_POWER_CHECK);

            while (true)
            {
                Display();
            }
        }

        public static void Display(string firstLine, string secondLine, bool isLongMessage = false)
        {
            if (!IsDisplayOn) return;

            if (isLongMessage)
            {
                _isContentBusy = true;

                Task.Delay(DISPLAY_DELAY).ContinueWith(x => _isContentBusy = false);

                _forceUpdate = true;
            }

            ClearScreen();
            SerialController.WriteToSerial(firstLine.ToUpper() + DeviceSymbols.LINE_BREAK + secondLine.ToUpper());
        }

        private static void ChangeDisplayStatus(bool visible) => IsDisplayOn = visible;

        private static void ChangeScreen(Screen screen)
        {
            if (!IsDisplayOn) return;
            if (screen == CurrentScreen) return;

            PreviousScreen = CurrentScreen;
            CurrentScreen = screen;
            _forceUpdate = true;
        }

        private static void Display()
        {
            if (_isContentBusy || !IsDisplayOn) return;

            if (!_forceUpdate && ImmutableScreens.All(s => s != CurrentScreen))
                ClearScreen();

            try
            {
                if (CurrentScreen.IsFanScreen())
                {
                    if (CurrentScreen == Screen.FrontFans) DisplayFrontFansScreen();
                    else DisplayFanScreen(CurrentScreen.GetFanType());
                }
                else
                {
                    switch (CurrentScreen)
                    {
                        case Screen.Base:
                            DisplayBaseScreen();
                            break;
                        case Screen.GPU:
                            DisplayGpuScreen();
                            break;
                        case Screen.CPU:
                            DisplayCpuScreen();
                            break;
                        case Screen.RAM:
                            DisplayRamScreen();
                            break;
                        case Screen.Weather:
                            DisplayWeatherScreen();
                            _forceUpdate = false;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(CurrentScreen), CurrentScreen, null);
                    }
                }
            }
            catch (Exception e)
            {
                Display("Error", e.Message, true);
                ChangeScreen(PreviousScreen);
            }

            Task.Delay(DISPLAY_DELAY).Wait();
        }

        private static void DisplayBaseScreen()
        {
            var info = SensorController.GetBaseInfo();

            Display(
                $"GPU:{GetString(info.GPU.Temperature, "0", "C")} | {GetString(info.GPU.Load, "0.##", "%")}",
                $"CPU:{GetString(info.CPU.Temperature, "0", "C")} | {GetString(info.CPU.Load, "0.##", "%")}"
            );
        }

        private static void DisplayWeatherScreen()
        {
            if (!_forceUpdate) return;

            var weather = WeatherController.GetWeather();

            Display(
                $"T:{weather.Temperature}|H:{weather.Humidity}%|P:{weather.PrecipitationProbability}%",
                $"P:{weather.Pressure}mm|W:{weather.Wind}m/s"
            );
        }

        private static void DisplayGpuScreen()
        {
            var info = SensorController.GetGpuInfo();

            Display(
                $"GPU|{GetString(info.UsedPercentage, "0.0", "%")}|{GetString(info.Temperature, "0.0", "C")}",
                $"P:{GetString(info.Power, "0", "W")}  M:{GetString(info.Memory, "0", "MB")}"
            );
        }

        private static void DisplayCpuScreen()
        {
            var info = SensorController.GetCpuInfo();

            Display(
                $"CPU|{GetString(info.UsedPercentage, "0.0", "%")}|{GetString(info.Temperature, "0.0", "C")}",
                $"P:{GetString(info.Power, "0", "W")}  C:{GetString(info.Clock, "0", "MHZ")}"
            );
        }

        private static void DisplayRamScreen()
        {
            var info = SensorController.GetRamInfo();

            Display(
                $"RAM | {GetString(info.UsedPercentage, "0.0", "%")}",
                $"A:{GetString(info.Available, "0.0", "G")}  U:{GetString(info.Used, "0.0", "G")}"
            );
        }

        private static void DisplayFanScreen(FanType fan)
        {
            var fanInfo = SensorController.GetFanInfo(fan);
            string fanName;

            switch (fan)
            {
                case FanType.Front1:
                    fanName = "First front";
                    break;
                case FanType.Front2:
                    fanName = "Second front";
                    break;
                case FanType.Front3:
                    fanName = "Third front";
                    break;
                default:
                    fanName = fan.ToString();
                    break;
            }

            fanName += " fan";

            Display(
                fanName,
                $"{GetString(fanInfo.RPM, "0", "RPM")} | P:{GetString(fanInfo.Percentage, "0", "%")}"
            );
        }

        private static void DisplayFrontFansScreen()
        {
            var fansInfo = SensorController.GetFrontFansInfo();

            Display(
                "   FRONT FANS  ",
                $"{GetString(fansInfo[0].Percentage, "0", "%")} | " +
                $"{GetString(fansInfo[1].Percentage, "0", "%")} | " +
                $"{GetString(fansInfo[2].Percentage, "0", "%")}"
            );
        }

        private static string GetString(float? value, string format, string symbol = "") =>
            (value?.ToString(format) ?? "~") + symbol;

        private static void ClearScreen() => SerialController.WriteToSerial(DeviceSymbols.LCD_CLEAR);
    }
}