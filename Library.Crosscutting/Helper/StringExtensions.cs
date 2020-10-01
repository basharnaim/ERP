using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Library.Crosscutting.Helper
{
    public static class StringExtensions
    {
        public static int ToInt(this string input, bool throwException = false)
        {
            int result;
            if (int.TryParse(input, out result) || !throwException)
                return result;
            throw new FormatException(string.Format("'{0}' cannot be converted as int", input));
        }

        public static long ToLong(this string input, bool throwException = false)
        {
            long result;
            if (long.TryParse(input, out result) || !throwException)
                return result;
            throw new FormatException(string.Format("'{0}' cannot be converted as long", input));
        }

        public static double ToDouble(this string input, bool throwException = false)
        {
            string s = input;
            NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = ".";
            double num = 0;
            ref double local = ref num;
            if (double.TryParse(s, NumberStyles.AllowDecimalPoint, numberFormatInfo, out local) || !throwException)
                return num;
            throw new FormatException(string.Format("'{0}' cannot be converted as Double", input));
        }

        public static decimal ToDecimal(this string input, bool throwException = false)
        {
            string s = input;
            NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = ".";
            decimal num = 0;
            ref decimal local = ref num;
            if (decimal.TryParse(s, NumberStyles.AllowDecimalPoint, numberFormatInfo, out local) || !throwException)
                return num;
            throw new FormatException(string.Format("'{0}' cannot be converted as Decimal", input));
        }

        public static bool ToBoolean(this string input, bool throwException = false)
        {
            bool result;
            if (bool.TryParse(input, out result) || !throwException)
                return result;
            throw new FormatException(string.Format("'{0}' cannot be converted as boolean", input));
        }

        public static string Reverse(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        public static int SafeGetLength(this string valueOrNull)
        {
            return (valueOrNull ?? string.Empty).Length;
        }

        public static string ToSentence(this string input)
        {
            if (string.IsNullOrWhiteSpace(input) || Regex.Match(input, "[0-9A-Z]+$").Success)
                return input;
            return Regex.Replace(input, "(\\B[A-Z])", " $1");
        }

        public static string GetLast(this string input, int howMany)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            string str = input.Trim();
            return howMany >= str.Length ? str : str.Substring(str.Length - howMany);
        }

        public static string GetFirst(this string input, int howMany)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            string str = input.Trim();
            return howMany >= str.Length ? str : input.Substring(0, howMany);
        }

        public static bool IsEmail(this string input)
        {
            return Regex.Match(input, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", RegexOptions.IgnoreCase).Success;
        }

        public static bool IsPhone(this string input)
        {
            return Regex.Match(input, "^\\+?(\\d[\\d-. ]+)?(\\([\\d-. ]+\\))?[\\d-. ]+\\d$", RegexOptions.IgnoreCase).Success;
        }

        public static bool IsNumber(this string input)
        {
            return Regex.Match(input, "^[0-9]+$", RegexOptions.IgnoreCase).Success;
        }

        public static int ExtractNumber(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;
            Match match = Regex.Match(input, "[0-9]+", RegexOptions.IgnoreCase);
            return match.Success ? match.Value.ToInt(false) : 0;
        }

        public static string ExtractEmail(this string input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input))
                return string.Empty;
            Match match = Regex.Match(input, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*", RegexOptions.IgnoreCase);
            return match.Success ? match.Value : string.Empty;
        }

        public static string ExtractQueryStringParamValue(this string queryString, string paramName)
        {
            if (string.IsNullOrWhiteSpace(queryString) || string.IsNullOrWhiteSpace(paramName))
                return string.Empty;
            string str1 = queryString.Replace("?", "");
            if (!str1.Contains("="))
                return string.Empty;
            string str2;
            return ((IEnumerable<string>)str1.Split('&')).Select(piQ => piQ.Split('=')).ToDictionary(piKey => piKey[0].ToLower().Trim(), piValue => piValue[1]).TryGetValue(paramName.ToLower().Trim(), out str2) ? str2 : string.Empty;
        }

        public static string PascelCase(this string input)
        {
            return input.Substring(0, 1).ToUpper() + input.Substring(1);
        }

        public static string CamelCase(this string input)
        {
            return input.Substring(0, 1).ToLower() + input.Substring(1);
        }

        public static string SeparateWords(this string input)
        {
            string empty = string.Empty;
            int num = -1;
            foreach (char c in input)
            {
                ++num;
                if (num != 0 && char.ToUpper(c) == c)
                    empty += " ";
                empty += c.ToString();
            }
            return empty;
        }
    }
}