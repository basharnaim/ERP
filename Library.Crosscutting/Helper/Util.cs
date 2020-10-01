using System;

namespace Library.Crosscutting.Helper
{
    public static class Util
    {
        public static string ConvertedDateFormat = "MM/dd/yyyy";
        public static string DbDateFormat = "yyyy-MM-dd";
        public const string DateFormat = "dd-MMM-yyyy";
        public const string TimeFormat = "hh:mm:ss tt";
        public const string EntryTimeFormat = "hh:mm tt";
        public const string DateTimeFormat = "yyyy-MM-dd hh:mm:ss.fff";

        public static int GetEnumValue(Type enumType, string value)
        {
            string str = value;
            if (!(str == "Integer"))
            {
                if (str == "Float")
                    value = "double";
            }
            else
                value = "int";
            return (int)Enum.Parse(enumType, value);
        }
    }
}