using System;

namespace Library.Context.Utility
{

    public static class NumberToWordsConverter
    {
        // Single-digit and small number names
        private static string[] _smallNumbers = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };

        // Tens number names from twenty upwards
        private static string[] _tens = new[] { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        // Scale number names for use during recombination
        private static string[] _scaleNumbers = new[] { "", "Thousand", "Lac", "Crore" };
        private static int[] _scaleDividors = new[] { 1000, 100, 100 };


        // Converts an integer value into English words
        public static string Convert(double number)
        {
            var tk = (long)number;
            var paisa = (int)Math.Round(number * 100 - tk * 100);
            if (paisa == 0)
            {
                return Convert(tk) + " Taka Only";
            }

            return Convert(tk) + " Taka and " + Convert(paisa) + " Paisa only";
        }

        // Converts an integer value into English words
        public static string Convert(long number)
        {
            // Zero rule
            if (number == 0)
                return _smallNumbers[0];

            // Array to hold four three-digit groups
            var digitGroups = new long[4];

            // Ensure a positive number to extract from
            long positive = Math.Abs(number);

            // Extract the three-digit groups
            for (int i = 0; i < 3; i++)
            {
                digitGroups[i] = positive % _scaleDividors[i];
                positive /= _scaleDividors[i];
            }

            digitGroups[3] = positive;

            // Convert each three-digit group to words
            string[] groupText = new string[4];

            for (int i = 0; i < 3; i++)
            {
                groupText[i] = ThreeDigitGroupToWords(digitGroups[i]);
            }

            groupText[3] = Convert(digitGroups[3]);

            // Recombine the three-digit groups
            string combined = groupText[0];
            bool appendAnd;

            // Determine whether an 'and' is needed
            appendAnd = digitGroups[0] > 0 && digitGroups[0] < 100;

            // Process the remaining groups in turn, smallest to largest
            for (int i = 1; i < 4; i++)
            {
                // Only add non-zero items
                if (digitGroups[i] != 0)
                {
                    // Build the string to add as a prefix
                    string prefix = groupText[i] + " " + _scaleNumbers[i];

                    if (combined.Length != 0)
                        prefix += appendAnd ? " and " : ", ";

                    // Opportunity to add 'and' is ended
                    appendAnd = false;

                    // Add the three-digit group to the combined string
                    combined = prefix + combined;
                }
            }

            // Negative rule
            if (number < 0)
                combined = "Negative " + combined;

            return combined;
        }


        // Converts a three-digit group into English words
        private static string ThreeDigitGroupToWords(long threeDigits)
        {
            // Initialise the return text
            string groupText = "";

            // Determine the hundreds and the remainder
            var hundreds = threeDigits / 100;
            var tensUnits = threeDigits % 100;

            // Hundreds rules
            if (hundreds != 0)
            {
                groupText += _smallNumbers[hundreds] + " Hundred";

                if (tensUnits != 0)
                    groupText += " and ";
            }

            // Determine the tens and units
            var tens = tensUnits / 10;
            var units = tensUnits % 10;

            // Tens rules
            if (tens >= 2)
            {
                groupText += _tens[tens];
                if (units != 0)
                    groupText += " " + _smallNumbers[units];
            }
            else if (tensUnits != 0)
                groupText += _smallNumbers[tensUnits];

            return groupText;
        }
    }



}