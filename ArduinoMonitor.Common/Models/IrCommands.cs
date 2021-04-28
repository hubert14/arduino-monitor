﻿namespace ArduinoMonitor.Common.Models
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
}