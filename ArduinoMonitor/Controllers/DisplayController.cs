using System;
using System.IO.Ports;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;

namespace ArduinoMonitor.Controllers
{
    public enum Screen
    {
        Base,
        GPU,
        CPU,
        RAM,
        Weather,
        FanCPU,
        FanGPU,
        FanFront1,
        FanFront2,
        FanFront3,
        FanRear
    }

    public class DisplayController
    {
        private const string LCD_CLEAR_SYMBOL = "@";
        private const string LINE_BREAK_SYMBOL = "$";

        private const int INFO_DELAY = 2000;
        private const int MESSAGE_DELAY = INFO_DELAY * 2;

        private static bool IsContentBusy;

        private readonly SerialPort _port;
        private readonly IComputer _computer;

        private Screen CurrentScreen { get; set; }

        private bool _forceUpdate = false;

        public DisplayController(SerialPort port, IComputer computer)
        {
            _port = port;
            _computer = computer;
            CurrentScreen = Screen.Base;
        }

        public void StartDisplay()
        {
            while (true)
            {
                Display();
            }
        }

        public void ChangeScreen(Screen screen)
        {
            if (screen == CurrentScreen) return;
            CurrentScreen = screen;
            _forceUpdate = true;
        }

        public void DisplayMessage(string header, string message)
        {
            IsContentBusy = true;

            _port.Write(LCD_CLEAR_SYMBOL);
            _port.Write(header + LINE_BREAK_SYMBOL + message);

            Task.Delay(MESSAGE_DELAY).ContinueWith(x => IsContentBusy = false);

            _forceUpdate = true;
        }

        private void DisplayBaseScreen()
        {
            var info = SensorController.GetBaseInfo(_computer);

            _port.Write(LCD_CLEAR_SYMBOL);
            _port.Write(
                $"GPU:{info.GPU.Temperature}C|{info.GPU.Load}%" +
                $"{LINE_BREAK_SYMBOL}" +
                $"CPU:{info.CPU.Temperature}C|{info.CPU.Load}%");
        }

        private void DisplayWeatherScreen()
        {
            if (!_forceUpdate) return;

            var weather = WeatherController.GetWeather();

            _port.Write(LCD_CLEAR_SYMBOL);
            _port.Write(
                $"T:{weather.Temperature}|H:{weather.Humidity}%|P:{weather.PrecipitationProbability}%" +
                $"{LINE_BREAK_SYMBOL}" +
                $"P:{weather.Pressure}mm|W:{weather.Wind}m/s");
        }

        private void Display()
        {
            if (IsContentBusy) return;

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
                case Screen.FanCPU:
                    DisplayFanScreen(FanType.CPU);
                    break;
                case Screen.FanGPU:
                    DisplayFanScreen(FanType.GPU);
                    break;
                case Screen.FanFront1:
                    DisplayFanScreen(FanType.Front1);
                    break;
                case Screen.FanFront2:
                    DisplayFanScreen(FanType.Front2);
                    break;
                case Screen.FanFront3:
                    DisplayFanScreen(FanType.Front3);
                    break;
                case Screen.FanRear:
                    DisplayFanScreen(FanType.Rear);
                    break;
                case Screen.Weather:
                    DisplayWeatherScreen();
                    _forceUpdate = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(CurrentScreen), CurrentScreen, null);
            }

            Task.Delay(INFO_DELAY).Wait();
        }

        private void DisplayGpuScreen()
        {
            var info = SensorController.GetGpuInfo(_computer);

            _port.Write(LCD_CLEAR_SYMBOL);
            _port.Write(
                $"GPU|F:{info.FanRpm}RPM" +
                $"{LINE_BREAK_SYMBOL}" +
                $"F:{info.FanPercentage}%|M:{info.Memory}MB");
        }

        private void DisplayCpuScreen()
        {
            var info = SensorController.GetCpuInfo(_computer);

            _port.Write(LCD_CLEAR_SYMBOL);
            _port.Write(
                $"CPU|F:{info.FanRpm}RPM" +
                $"{LINE_BREAK_SYMBOL}" +
                $"P:{info.Power}W|C:{info.Clock}MHz");
        }

        private void DisplayRamScreen()
        {
            var info = SensorController.GetRamInfo(_computer);

            _port.Write(LCD_CLEAR_SYMBOL);
            _port.Write(
                $"RAM|{info.UsedPercentage}%" +
                $"{LINE_BREAK_SYMBOL}" +
                $"A:{info.Available}G|U:{info.Used}G");
        }

        private void DisplayFanScreen(FanType fan)
        {
            var fanInfo = SensorController.GetFanInfo(_computer, fan);
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
    }
}