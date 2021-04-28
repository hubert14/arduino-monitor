using System;
using System.IO.Ports;
using System.Threading.Tasks;
using ArduinoMonitor.Common.Enums;
using OpenHardwareMonitor.Hardware;

namespace ArduinoMonitor.Common.Controllers
{
    public static class DisplayController
    {
        private const string LCD_CLEAR_SYMBOL = "@";
        private const string LINE_BREAK_SYMBOL = "$";

        private const int DISPLAY_DELAY = 2000;

        private static bool _isContentBusy;
        private static bool _forceUpdate;

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

            while (true) Display();
        }

        public static void ChangeScreen(Screen screen)
        {
            if (screen == CurrentScreen) return;
            CurrentScreen = screen;
            _forceUpdate = true;
        }

        public static void Display(string header, string message)
        {
            _isContentBusy = true;

            _port.Write(LCD_CLEAR_SYMBOL);
            _port.Write(header + LINE_BREAK_SYMBOL + message);

            Task.Delay(DISPLAY_DELAY).ContinueWith(x => _isContentBusy = false);

            _forceUpdate = true;
        }

        private static void Display()
        {
            if (_isContentBusy) return;

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

        private static void DisplayBaseScreen()
        {
            var info = SensorController.GetBaseInfo();

            _port.Write(LCD_CLEAR_SYMBOL);
            _port.Write(
                $"GPU:{info.GPU.Temperature}C | {info.GPU.Load}%" +
                $"{LINE_BREAK_SYMBOL}" +
                $"CPU:{info.CPU.Temperature}C | {info.CPU.Load}%");
        }

        private static void DisplayWeatherScreen()
        {
            if (!_forceUpdate) return;

            var weather = WeatherController.GetWeather();

            _port.Write(LCD_CLEAR_SYMBOL);
            _port.Write(
                $"T:{weather.Temperature}|H:{weather.Humidity}%|P:{weather.PrecipitationProbability}%" +
                $"{LINE_BREAK_SYMBOL}" +
                $"P:{weather.Pressure}mm|W:{weather.Wind}m/s");
        }


        private static void DisplayGpuScreen()
        {
            var info = SensorController.GetGpuInfo();

            _port.Write(LCD_CLEAR_SYMBOL);
            _port.Write(
                $"GPU | {info.UsedPercentage}%" +
                $"{LINE_BREAK_SYMBOL}" +
                $"M:{info.Memory}MB");
        }

        private static void DisplayCpuScreen()
        {
            var info = SensorController.GetCpuInfo();

            _port.Write(LCD_CLEAR_SYMBOL);
            _port.Write(
                $"CPU | {info.UsedPercentage}%" +
                $"{LINE_BREAK_SYMBOL}" +
                $"P:{info.Power}W  C:{info.Clock}MHz");
        }

        private static void DisplayRamScreen()
        {
            var info = SensorController.GetRamInfo();

            _port.Write(LCD_CLEAR_SYMBOL);
            _port.Write(
                $"RAM | {info.UsedPercentage}%" +
                $"{LINE_BREAK_SYMBOL}" +
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

            _port.Write(LCD_CLEAR_SYMBOL);
            _port.Write(fanName +
                        $"{LINE_BREAK_SYMBOL}" +
                        $"{fanInfo.RPM}RPM | P:{fanInfo.Percentage}%");
        }

        private static void DisplayFrontFansScreen()
        {
            var fansInfo = SensorController.GetFrontFansInfo();

            _port.Write(LCD_CLEAR_SYMBOL);
            _port.Write("   FRONT FANS  " +
                        $"{LINE_BREAK_SYMBOL}" +
                        $"{fansInfo[0].Percentage}% | {fansInfo[1].Percentage}% | {fansInfo[2].Percentage}%");
        }
    }
}