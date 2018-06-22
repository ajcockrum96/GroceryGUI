using System;
using System.Collections.Generic;

namespace GroceryGUI
{
    public static class References
    {
        // Input Type References
        public const string QUANTITY_INPUT = "Quantity";
        public const string DATE_INPUT     = "Date";

        // Date References
        public const string MONTH = "Month";
        public const string DAY   = "Day";
        public const string YEAR  = "Year";
        public const string JAN   = "January";
        public const string FEB   = "February";
        public const string MAR   = "March";
        public const string APR   = "April";
        public const string MAY   = "May";
        public const string JUN   = "June";
        public const string JUL   = "July";
        public const string AUG   = "August";
        public const string SEP   = "September";
        public const string OCT   = "October";
        public const string NOV   = "November";
        public const string DEC   = "December";
        public const int MONTH_MAX   = 12;
        public const int DATE_RADIUS = 100;
        public static int YEAR_MIN   = DateTime.Now.Year - DATE_RADIUS;
        public static int YEAR_MAX   = YEAR_MIN + 2 * DATE_RADIUS;
        public static Dictionary<string, int> DAYS_IN_MONTH = new Dictionary<string, int>()
        {
            { JAN, 31 },
            { FEB, 28 },
            { MAR, 31 },
            { APR, 30 },
            { MAY, 31 },
            { JUN, 30 },
            { JUL, 31 },
            { AUG, 31 },
            { SEP, 30 },
            { OCT, 31 },
            { NOV, 30 },
            { DEC, 31 }
        };
        public static Dictionary<string, int> DATE_SELECTION_START = new Dictionary<string, int>()
        {
            { MONTH, 0 },
            { DAY,   3 },
            { YEAR,  6 }
        };
        public static Dictionary<string, int> DATE_SELECTION_LENGTH = new Dictionary<string, int>()
        {
            { MONTH, 2 },
            { DAY,   2 },
            { YEAR,  4 }
        };

        public static bool isLeapMonth( int month )
        {
            if( month == 2 )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isLeapYear( int year )
        {
            if( year % 4 == 0 && (year % 100 != 0 || year % 400 == 0) )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the entire string contains only digit characters.
        /// </summary>
        /// <param name="pString">string to be searched for non-digit characters</param>
        /// <returns>boolean of value true if string only contains digits, false otherwise</returns>                   
        public static bool isNumeric( string pString )
        {
            bool containsAlpha = false;
            for( int i = 0; i < pString.Length; ++i )
            {
                if( !Char.IsDigit( pString, i ) )
                {
                    containsAlpha = true;
                    break;
                }
            }
            return !containsAlpha;
        }
    }
}