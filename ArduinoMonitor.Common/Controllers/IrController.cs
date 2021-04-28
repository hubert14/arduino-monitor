namespace ArduinoMonitor.Common.Controllers
{
    public static class IrCommands
    {
        public const string CH_MINUS_COMMAND = "BA45FF00";
        public const string CH_COMMAND = "B946FF00";
        public const string CH_PLUS_COMMAND = "B847FF00";

        public const string MINUS_COMMAND = "F807FF00";
        public const string PLUS_COMMAND = "EA15FF00";
        public const string EQ_COMMAND = "F609FF00";

        public const string ZERO_COMMAND = "E916FF00";
        public const string ONE_COMMAND = "F30CFF00";
        public const string TWO_COMMAND = "E718FF00";
        public const string THREE_COMMAND = "A15EFF00";
        public const string FOUR_COMMAND = "F708FF00";
        public const string FIVE_COMMAND = "E31CFF00";
        public const string SIX_COMMAND = "A55AFF00";
        public const string SEVEN_COMMAND = "BD42FF00";
        public const string EIGHT_COMMAND = "AD52FF00";
        public const string NINE_COMMAND = "B54AFF00";

        public const string PLUS_100_COMMAND = "E619FF00";
        public const string PLUS_200_COMMAND = "F20DFF00";

        public const string PLAY_COMMAND = "BC43FF00";
        public const string PREV_COMMAND = "BB44FF00";
        public const string NEXT_COMMAND = "BF40FF00";
    }

    public class IrController
    {
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
                    FanController.ChangeFan(FanOperation.Down);
                    break;

                case IrCommands.CH_PLUS_COMMAND:
                    FanController.ChangeFan(FanOperation.Up);
                    break;

                case IrCommands.CH_COMMAND:
                    FanController.ChangeFan(FanOperation.Default);
                    break;

                // MEDIA
                case IrCommands.PLAY_COMMAND:
                    MediaController.PlayPause();
                    break;
                case IrCommands.PLUS_COMMAND:
                    MediaController.AddVolume();
                    break;
                case IrCommands.MINUS_COMMAND:
                    MediaController.ReduceVolume();
                    break;
                case IrCommands.EQ_COMMAND:
                    MediaController.MuteVolume();
                    break;
                case IrCommands.PREV_COMMAND:
                    MediaController.Previous();
                    break;
                case IrCommands.NEXT_COMMAND:
                    MediaController.Next();
                    break;

                // NAVIGATION
                case IrCommands.ZERO_COMMAND:
                    MainController.Display.ChangeScreen(Screen.Base);
                    break;
                case IrCommands.ONE_COMMAND:
                    MainController.Display.ChangeScreen(Screen.GPU);
                    break;
                case IrCommands.TWO_COMMAND:
                    MainController.Display.ChangeScreen(Screen.CPU);
                    break;
                case IrCommands.THREE_COMMAND:
                    MainController.Display.ChangeScreen(Screen.RAM);
                    break;
                case IrCommands.PLUS_100_COMMAND:
                    MainController.Display.ChangeScreen(Screen.Weather);
                    break;
                case IrCommands.FOUR_COMMAND:
                    MainController.Display.ChangeScreen(Screen.FanCPU);
                    break;
                case IrCommands.FIVE_COMMAND:
                    MainController.Display.ChangeScreen(Screen.FanGPU);
                    break;
                case IrCommands.SIX_COMMAND:
                    MainController.Display.ChangeScreen(Screen.FanRear);
                    break;
                case IrCommands.SEVEN_COMMAND:
                    MainController.Display.ChangeScreen(Screen.FanFront1);
                    break;
                case IrCommands.EIGHT_COMMAND:
                    MainController.Display.ChangeScreen(Screen.FanFront2);
                    break;
                case IrCommands.NINE_COMMAND:
                    MainController.Display.ChangeScreen(Screen.FanFront3);
                    break;
                case IrCommands.PLUS_200_COMMAND:
                    MainController.Display.ChangeScreen(Screen.FrontFans);
                    break;
            }
        }
    }
}