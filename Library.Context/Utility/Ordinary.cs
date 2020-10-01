using System;
using System.Linq;

namespace Library.Context.Utility
{
    public class Ordinary
    {
        public static string GetRandomNumber(int count)
        {
            if (count < 4)
            {
                count = 4;
            }
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, count)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        public static string AgeCalc( DateTime pfromDate, DateTime ptoDate)
        {
            string age = "0y";
            int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            DateTime fromDate;
            DateTime toDate;
            int year;
            int month;
            int day; 

            DateTime dtDob = new DateTime();

            dtDob = Convert.ToDateTime(pfromDate);

            int year1 = dtDob.Year;
            int month1 = dtDob.Month;
            int date1 = dtDob.Day;

            int year2 = DateTime.Now.Year;
            int month2 = DateTime.Now.Month;
            int date2 = DateTime.Now.Day;

            //int Month = 0;
            //int Year = 0;
            //int Date = 0;




            fromDate = Convert.ToDateTime(pfromDate); 
            toDate = Convert.ToDateTime(ptoDate);


            //Day Calculation

            int increment = 0;
            if (fromDate.Day > toDate.Day)
            {
                increment = monthDay[fromDate.Month - 1];
            }



            if (increment == -1)
            {
                if (DateTime.IsLeapYear( fromDate.Year))
                {
                    increment = 29;
                }
                else
                {
                    increment = 28;
                }
            }



            if (increment != 0)
            {
                day = toDate.Day + increment -  fromDate.Day;
                increment = 1;
            }
            else
            {
                day =  toDate.Day -  fromDate.Day;
            }



            //Month Calculation

            if (fromDate.Month + increment >  toDate.Month)
            {
                 month = toDate.Month + 12 - ( fromDate.Month + increment);
                increment = 1;
            }
            else
            {
                 month = toDate.Month - ( fromDate.Month + increment);
                increment = 0;
            }

            //Year Calculation

             year =  toDate.Year - ( fromDate.Year + increment);



            age = year + " Y " + month + " M " + day + " D";
            return age;
        }
    }
}
