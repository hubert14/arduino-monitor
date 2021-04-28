using System;
using System.IO.Ports;
using System.Threading.Tasks;
using ArduinoMonitor.Common.Enums;
using ArduinoMonitor.Common.Models;
using OpenHardwareMonitor.Hardware;

namespace ArduinoMonitor.Common.Controllers
{
    public static class DisplayController
    {
        private const int DISPLAY_DELAY = 2000;

        private static bool _isContentBusy;
        private static bool _forceUpdate;
        
        public static bool IsDisplayOn { get; set; }

        private static SerialPort _port;

        static DisplayController()
        {
            CurrentScreen = Screen.Base;

            IrController.Display += ChangeScreen;
        }

        public static Screen CurrentScreen { get; set; }

        public static void StartDisplay(SerialPort port)
        {
            _port = port;
            _port.Write(IrSymbols.LCD_POWER_CHECK);
            while (true) Display();
        }

        public static void ChangeScreen(Screen screen)
        {
            if (screen == Screen.ChangeVisibility)
            {
                ChangeDisplayVisibility();
                return;
            }

            if (!IsDisplayOn) return;

            if (screen == CurrentScreen) return;

            CurrentScreen = screen;
            _forceUpdate = true;
        }

        public static void Display(string header, string message)
        {
            if (!IsDisplayOn) return;

            _isContentBusy = true;

            _port.Write(IrSymbols.LCD_CLEAR);
            _port.Write(header + IrSymbols.LINE_BREAK + message);

            Task.Delay(DISPLAY_DELAY).ContinueWith(x => _isContentBusy = false);

            _forceUpdate = true;
        }

        private static void Display()
        {
            if (_isContentBusy || !IsDisplayOn) return;

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

            Task.Delay(DISPLAY_DELAY).Wait();
        }

        private static void ChangeDisplayVisibility()
        {
            IsDisplayOn = !IsDisplayOn;
            _port.Write(IrSymbols.LCD_CLEAR);
            _port.Write(IrSymbols.LCD_POWER);
        }

        private static void DisplayBaseScreen()
        {
            var info = SensorController.GetBaseInfo();

            _port.Write(IrSymbols.LCD_CLEAR);
            _port.Write(
                $"GPU:{info.GPU.Temperature}C | {info.GPU.Load}%" +
                $"{IrSymbols.LINE_BREAK}" +
                $"CPU:{info.CPU.Temperature}C | {info.CPU.Load}%");
        }

        private static void DisplayWeatherScreen()
        {
            if (!_forceUpdate) return;

            var weather = WeatherController.GetWeather();

            _port.Write(IrSymbols.LCD_CLEAR);
            _port.Write(
                $"T:{weather.Temperature}|H:{weather.Humidity}%|P:{weather.PrecipitationProbability}%" +
                $"{IrSymbols.LINE_BREAK}" +
                $"P:{weather.Pressure}mm|W:{weather.Wind}m/s");
        }


        private static void DisplayGpuScreen()
        {
            var info = SensorController.GetGpuInfo();

            _port.Write(IrSymbols.LCD_CLEAR);
            _port.Write(
                $"GPU | {info.UsedPercentage}%" +
                $"{IrSymbols.LINE_BREAK}" +
                $"M:{info.Memory}MB");
        }

        private static void DisplayCpuScreen()
        {
            var info = SensorController.GetCpuInfo();

            _port.Write(IrSymbols.LCD_CLEAR);
            _port.Write(
                $"CPU | {info.UsedPercentage}%" +
                $"{IrSymbols.LINE_BREAK}" +
                $"P:{info.Power}W  C:{info.Clock}MHz");
        }

        private static void DisplayRamScreen()
        {
            var info = SensorController.GetRamInfo();

            _port.Write(IrSymbols.LCD_CLEAR);
            _port.Write(
                $"RAM | {info.UsedPercentage}%" +
                $"{IrSymbols.LINE_BREAK}" +
                $"A:{info.Available}G  U:{info.Used}G");
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

            _port.Write(IrSymbols.LCD_CLEAR);
            _port.Write(fanName +
                        $"{IrSymbols.LINE_BREAK}" +
                        $"{fanInfo.RPM}RPM | P:{fanInfo.Percentage}%");
        }

        private static void DisplayFrontFansScreen()
        {
            var fansInfo = SensorController.GetFrontFansInfo();

            _port.Write(IrSymbols.LCD_CLEAR);
            _port.Write("   FRONT FANS  " +
                        $"{IrSymbols.LINE_BREAK}" +
                        $"{fansInfo[0].Percentage}% | {fansInfo[1].Percentage}% | {fansInfo[2].Percentage}%");
        }
    }
}