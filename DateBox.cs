using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GroceryGUI
{
    class DateBox : TextBox
    {
        private int fYear;
        private int fMonth;
        private int fDay;

        private int fCurrentDigit;

        private bool fMaxYear;
        private bool fMinYear;
        private bool fMaxMonth;
        private bool fMinMonth;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDateString"></param>
        public DateBox( string pDateString ) : base()
        {
            // Initialize Class Fields
            DateTime dateValue = DateTime.ParseExact( pDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture );
            fYear  = dateValue.Year;
            fMonth = dateValue.Month;
            fDay   = dateValue.Day;

            fCurrentDigit = 0;
            fMaxYear  = (fYear == References.YEAR_MAX);
            fMinYear  = (fYear == References.YEAR_MIN);
            fMaxMonth = fMaxYear && (fMonth == References.MONTH_MAX);
            fMinMonth = fMinYear && (fMonth == 1);

            // Format Text Box
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Bottom;
            TextAlignment = TextAlignment.Center;
            Style = (Style) FindResource( "TextBox" );

            updateDateText();
            OnLostFocus( null );
        }

        /// <summary>
        /// 
        /// </summary>
        public DateBox() : this( DateTime.Now.ToString( "MM/dd/yyyy" ) ) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pEventArgs">Event information passed by TextInput Event</param>
        protected override void OnTextInput( TextCompositionEventArgs pEventArgs )
        {
            if( References.isNumeric( pEventArgs.Text ) )
            {
                string locationString = dateLocation( SelectionStart );
                selectDateLocation( locationString );

                int start = SelectionStart;
                int index = start + fCurrentDigit;

                Text = Text.Remove( index, 1 );
                Text = Text.Insert( index, pEventArgs.Text );

                int range = 1;
                References.DATE_SELECTION_LENGTH.TryGetValue( locationString, out range );
                fCurrentDigit = (++fCurrentDigit) % range;

                if( fCurrentDigit == 0 )
                {
                    fixDate();
                }

                selectDateLocation( locationString );
            }
            pEventArgs.Handled = true;
            base.OnTextInput( pEventArgs );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pEventArgs">Event information passed by KeyDown Event</param>
        protected override void OnPreviewKeyDown( KeyEventArgs pEventArgs )
        {
            string locationString = "";

            switch( pEventArgs.Key )
            {
                case Key.Up:
                    // Get starting index of portion of the date to be edited
                    locationString = dateLocation( SelectionStart );

                    // Increment the current portion of the date
                    incrementDateLocation( locationString, 1 );
                    selectDateLocation( locationString );
                    pEventArgs.Handled = true;
                    break;
                case Key.Down:
                    // Get starting index of portion of the date to be edited
                    locationString = dateLocation( SelectionStart );

                    // Decrement the current portion of the date
                    incrementDateLocation( locationString, -1 );
                    selectDateLocation( locationString );
                    pEventArgs.Handled = true;
                    break;
                case Key.Back:
                case Key.Delete:
                    pEventArgs.Handled = true;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pEventArgs"></param>
        protected override void OnGotFocus( RoutedEventArgs pEventArgs )
        {
            DateTime date = DateTime.ParseExact( Text, "MMMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture );
            Text = date.ToString( "MM/dd/yyyy" );
            base.OnGotFocus( pEventArgs );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pEventArgs">Event information passed by LostFocus Event</param>
        protected override void OnLostFocus( RoutedEventArgs pEventArgs )
        {
            fCurrentDigit = 0;
            fixDate();
            DateTime date = DateTime.ParseExact( Text, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture );
            Text = date.ToString( "MMMM dd, yyyy" );
            if( pEventArgs != null )
            {
                base.OnLostFocus( pEventArgs );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pEventArgs"></param>
        protected override void OnPreviewMouseLeftButtonUp( MouseButtonEventArgs pEventArgs )
        {
            fCurrentDigit = 0;
            updateDateText();
            if( SelectionLength > 0 )
            {
                string locationString = dateLocation( SelectionStart, SelectionLength );
                selectDateLocation( locationString );
            }
        }

        /// <summary>
        /// Returns a location string (retrieved from References) that
        /// corresponds to the portion of the date (i.e. month, day, or year)
        /// that <paramref name="pStart"/> is in.
        /// </summary>
        /// <remarks>
        /// This function assumes the date has been maintained in the expected
        /// MM/dd/yyyy format.
        /// </remarks>
        /// <param name="pStart">int of the position of the caret or the beginning of the highlighted selection</param>
        /// <returns>string from References that corresponds to date location</returns>
        private string dateLocation( int pStart )
        {
            string locationString = "";
            switch( pStart )
            {
                // Month Edit
                case 0:
                case 1:
                case 2:
                    locationString = References.MONTH;
                    break;
                // Day Edit
                case 3:
                case 4:
                case 5:
                    locationString = References.DAY;
                    break;
                // Year Edit
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    locationString = References.YEAR;
                    break;
                default:
                    locationString = References.MONTH;
                    break;
            }
            return locationString;
        }

        private string dateLocation( int pStart, int pLength )
        {
            return dateLocation( pStart + (pLength / 2) );
        }

        /// <summary>
        /// Selects either the month, day, or year portion of a date contained
        /// in a TextBox object. 
        /// </summary>
        /// <remarks>
        /// This function assumes the date has been maintained in the expected
        /// MM/dd/yyyy format.
        /// </remarks>
        /// <param name="pLocationString">string from References that corresponds to date location to highlight</param>
        private void selectDateLocation( string pLocationString )
        {
            int start = 0;
            int length = 0;
            References.DATE_SELECTION_START.TryGetValue( pLocationString, out start );
            References.DATE_SELECTION_LENGTH.TryGetValue( pLocationString, out length );
            SelectionStart = start;
            SelectionLength = length;
        }

        /// <summary>
        /// Adds specified amount to either the month, day, or year portion of
        /// a date contained in a TextBox object.  After adding this amount,
        /// fixDate(...) is then called to solve any date formatting issues.
        /// </summary>
        /// <param name="pLocationString"></param>
        /// <param name="pInc"></param>
        private void incrementDateLocation( string pLocationString, int pInc )
        {
            // Get value of date location
            selectDateLocation( pLocationString );
            int currentValue = int.Parse( Text.Substring( SelectionStart, SelectionLength ) );

            // Increment the location
            currentValue += pInc;

            // Update date text
            string formatString = "{0}{1:D" + String.Format( "{0:D1}", SelectionLength ) + "}{2}";
            Text = String.Format( formatString,
                                  Text.Substring( 0, SelectionStart ),
                                  currentValue,
                                  Text.Substring( SelectionLength + SelectionStart ) );

            // Fix date if "rolled over"
            fixDate();
        }

        /// <summary>
        /// 
        /// </summary>
        private void fixYear()
        {
            if( fYear > References.YEAR_MAX )
            {
                fYear = References.YEAR_MAX;
            }
            else if( fYear < References.YEAR_MIN )
            {
                fYear = References.YEAR_MIN;
            }
            fMaxYear = (fYear == References.YEAR_MAX);
            fMinYear = (fYear == References.YEAR_MIN);
        }

        /// <summary>
        /// 
        /// </summary>
        private void fixMonth()
        {
            while( fMonth < 1 )
            {
                if( fMinYear )
                {
                    fMonth = 1;
                }
                else
                {
                    --fYear;
                    fMonth += References.MONTH_MAX;
                }
            }

            while( fMonth > References.MONTH_MAX )
            {
                if( fMaxYear )
                {
                    fMonth = References.MONTH_MAX;
                }
                else
                {
                    ++fYear;
                    fMonth -= References.MONTH_MAX;
                }
            }

            fMaxMonth = fMaxYear && (fMonth == References.MONTH_MAX);
            fMinMonth = fMinYear && (fMonth == 1);
        }

        private void fixDay()
        {
            int daysInMonth = 0;
            do
            {
                if( daysInMonth > 0 )
                {
                    if( fMaxMonth )
                    {
                        fDay = daysInMonth;
                        break;
                    }
                    else
                    {
                        ++fMonth;
                        fixMonth();
                        fDay -= daysInMonth;
                    }
                }
                string monthString =
                       DateTime.ParseExact( String.Format( "{0:D2}", fMonth ),
                                            "MM",
                                            System.Globalization.CultureInfo.InvariantCulture ).ToString( "MMMM" );
                References.DAYS_IN_MONTH.TryGetValue( monthString, out daysInMonth );
                if( References.isLeapMonth( fMonth ) && References.isLeapYear( fYear ) )
                {
                    daysInMonth++;
                }
            } while( fDay > daysInMonth );

            while( fDay < 1 )
            {
                if( fMinMonth )
                {
                    fDay = 1;
                }
                else
                {
                    --fMonth;
                    fixMonth();
                    string monthString =
                           DateTime.ParseExact( String.Format( "{0:D2}", fMonth ),
                                                "MM",
                                                System.Globalization.CultureInfo.InvariantCulture ).ToString( "MMMM" );
                    References.DAYS_IN_MONTH.TryGetValue( monthString, out daysInMonth );
                    if( References.isLeapMonth( fMonth ) && References.isLeapYear( fYear ) )
                    {
                        daysInMonth++;
                    }
                    fDay += daysInMonth;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void parseDateText()
        {
            parseDateText( References.YEAR );
            parseDateText( References.MONTH );
            parseDateText( References.DAY );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pLocationString"></param>
        private void parseDateText( string pLocationString )
        {
            int start = 0;
            int length = 0;
            References.DATE_SELECTION_START.TryGetValue( pLocationString, out start );
            References.DATE_SELECTION_LENGTH.TryGetValue( pLocationString, out length );

            switch( pLocationString )
            {
                case References.YEAR:
                    fYear = int.Parse( Text.Substring( start, length ) );
                    break;
                case References.MONTH:
                    fMonth = int.Parse( Text.Substring( start, length ) );
                    break;
                case References.DAY:
                    fDay = int.Parse( Text.Substring( start, length ) );
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void updateDateText()
        {
            int yearLength  = 0;
            int monthLength = 0;
            int dayLength   = 0;

            References.DATE_SELECTION_LENGTH.TryGetValue( References.YEAR, out yearLength );
            References.DATE_SELECTION_LENGTH.TryGetValue( References.MONTH, out monthLength );
            References.DATE_SELECTION_LENGTH.TryGetValue( References.DAY, out dayLength );

            string monthFormat = "{0:D" + String.Format( "{0:D1}", monthLength ) + "}";
            string dayFormat   = "{1:D" + String.Format( "{0:D1}", dayLength )   + "}";
            string yearFormat  = "{2:D" + String.Format( "{0:D1}", yearLength )  + "}";
            Text = String.Format( monthFormat + "/" + dayFormat + "/" + yearFormat, fMonth, fDay, fYear );
        }

        /// <summary>
        /// 
        /// </summary>
        private void fixDate()
        {
            parseDateText();
            fixYear();
            fixMonth();
            fixDay();
            updateDateText();
        }
    }
}
