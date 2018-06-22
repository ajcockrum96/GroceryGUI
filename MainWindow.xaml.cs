using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            if( year % 4 == 0 && ( year % 100 != 0 || year % 400 == 0 ) )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public static class InputTypeClass
    {
        public static readonly DependencyProperty InputTypeProperty = DependencyProperty.RegisterAttached( "InputType",
                                                                                                    typeof( string ),
                                                                                                    typeof( InputType ),
                                                                                                    new FrameworkPropertyMetadata( "", FrameworkPropertyMetadataOptions.None ) );

        public static string GetInputType( object obj )
        {
            UIElement element = (UIElement) obj;
            if( element == null )
                throw new ArgumentNullException( "element" );
            return (string) element.GetValue( InputTypeProperty );
        }
        public static void SetInputType( object obj, string value )
        {
            UIElement element = (UIElement) obj;
            if( element == null )
                throw new ArgumentNullException( "element" );
            element.SetValue( InputTypeProperty, value );
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int fCurrentDigit = 0;


        public MainWindow()
        {
            InitializeComponent();
        }

        /* Event Handlers */
        /// <summary>
        /// <para>Filter for text input into TextBox objects that register this
        /// function as their PreviewTextInput event handler.</para>
        /// <para>TextBox objects of type <see cref="References.QUANTITY_INPUT"/>
        /// will be restricted to digit-only input and a character limit of 2.</para>
        /// <para>TextBox objects of type <see cref="References.DATE_INPUT"/>
        /// will be restricted to...</para>
        /// </summary>
        /// <param name="pSender">object that is undergoing the TextInput Event</param>
        /// <param name="pArgs">Event information passed by TextInput Event</param>
        private void onTextInput( object pSender, TextCompositionEventArgs pArgs )
        {
            switch( InputTypeClass.GetInputType( pSender ) )
            {
                case References.QUANTITY_INPUT:
                    TextBox quantityBox = (TextBox) pSender;
                    if( quantityBox.Text.Length >= 2 || !isNumeric( pArgs.Text ) )
                    {
                        pArgs.Handled = true;
                    }
                    break;
                case References.DATE_INPUT:
                    if( isNumeric( pArgs.Text ) )
                    {
                        TextBox dateBox = (TextBox) pSender;
                        // Determine the current portion of date to be edited
                        string locationString = dateLocation( dateBox.SelectionStart );

                        // Highlight the current portion of the date to be edited
                        selectDateLocation( dateBox, locationString );

                        // Get starting index of portion of the date to be edited
                        int start = dateBox.SelectionStart;

                        // Determine what the current digit of the field being edited
                        int index = start + fCurrentDigit;

                        // Change the current digit of the field being edited
                        dateBox.Text = dateBox.Text.Remove( index, 1 );
                        dateBox.Text = dateBox.Text.Insert( index, pArgs.Text );

                        // Increment the current digit of the field being edited (wrapping around if end of field is reached)
                        int range = 1;
                        References.DATE_SELECTION_LENGTH.TryGetValue( locationString, out range );
                        fCurrentDigit = ++fCurrentDigit % range;

                        // Fix the date input when the full input is edited
                        if( fCurrentDigit == 0 )
                        {
                            fixDate( dateBox );
                        }

                        // Re-highlight the current portion of the date being edited
                        selectDateLocation( dateBox, locationString );
                    }
                    pArgs.Handled = true;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// <para>Filter for text input into TextBox objects that register this
        /// function as their PreviewKeyDown event handler.</para>
        /// <para>TextBox objects of type <see cref="References.QUANTITY_INPUT"/>
        /// will gain the extra functionality of incrementing and decrementing
        /// the current numeric value of their contents, using the UP and DOWN
        /// arrow keys respectively. This functionality is limited to a
        /// lower numerical bound of 0 and an upper numerical bound of 99.</para>
        /// <para>TextBox objects of type <see cref="References.DATE_INPUT"/>
        /// will...</para>
        /// </summary>
        /// <param name="pSender">object that is undergoing the KeyDown Event</param>
        /// <param name="pArgs">Event information passed by TextInput Event</param>
        private void onKeyInput( object pSender, KeyEventArgs pArgs )
        {
            switch( InputTypeClass.GetInputType( pSender ) )
            {
                case References.QUANTITY_INPUT:
                    TextBox quantityBox = (TextBox) pSender;
                    int currentValue = 0;
                    if( quantityBox.Text.Length > 0 )
                    {
                        currentValue = int.Parse( quantityBox.Text );
                    }

                    switch( pArgs.Key )
                    {
                        case Key.Up:
                            if( currentValue < 99 )
                            {
                                ++currentValue;
                                
                            }
                            quantityBox.Text = currentValue.ToString();
                            break;
                        case Key.Down:
                            if( currentValue > 0 )
                            {
                                --currentValue;
                            }
                            quantityBox.Text = currentValue.ToString();
                            break;
                        case Key.Space:
                            pArgs.Handled = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case References.DATE_INPUT:
                    TextBox dateBox = (TextBox) pSender;
                    string locationString = "";

                    switch( pArgs.Key )
                    {
                        case Key.Up:
                            // Get starting index of portion of the date to be edited
                            locationString = dateLocation( dateBox.SelectionStart );

                            // Increment the current portion of the date
                            incrementDateLocation( dateBox, locationString, 1 );
                            selectDateLocation( dateBox, locationString );
                            pArgs.Handled = true;
                            break;
                        case Key.Down:
                            // Get starting index of portion of the date to be edited
                            locationString = dateLocation( dateBox.SelectionStart );

                            // Decrement the current portion of the date
                            incrementDateLocation( dateBox, locationString, -1 );
                            selectDateLocation( dateBox, locationString );
                            pArgs.Handled = true;
                            break;
                        case Key.Back:
                        case Key.Delete:
                            pArgs.Handled = true;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private void onInputGotFocus( object pSender, RoutedEventArgs pArgs )
        {
            switch( InputTypeClass.GetInputType( pSender ) )
            {
                case References.DATE_INPUT:
                    TextBox dateBox = (TextBox) pSender;
                    DateTime date = DateTime.ParseExact( dateBox.Text, "MMMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture );
                    dateBox.Text = date.ToString( "MM/dd/yyyy" );
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// <para>Responds to the loss of focus of a certain TextBox element
        /// and udpates its contents as needed.</para>
        /// <para>The contents of TextBox objects of type <see cref="References.QUANTITY_INPUT"/>
        /// will be corrected to store at least 1 digit, but contain no padding
        /// zeros, unless the value is 0.</para>
        /// <para>The contents of TextBox objects of type <see cref="References.DATE_INPUT"/>
        /// will be...</para>
        /// </summary>
        /// <param name="pSender">object that has lost focus</param>
        /// <param name="pArgs">Event information passed by LostFocus Event</param>
        private void onInputLostFocus( object pSender, RoutedEventArgs pArgs )
        {
            fCurrentDigit = 0;
            switch( InputTypeClass.GetInputType( pSender ) )
            {
                case References.QUANTITY_INPUT:
                    TextBox quantityBox = (TextBox) pSender;
                    int currentValue = 0;
                    if( quantityBox.Text.Length > 0 )
                    {
                        currentValue = int.Parse( quantityBox.Text );
                    }
                    quantityBox.Text = currentValue.ToString();
                    break;
                case References.DATE_INPUT:
                    fixDate( (TextBox) pSender );
                    TextBox dateBox = (TextBox) pSender;
                    DateTime date = DateTime.ParseExact( dateBox.Text, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture );
                    dateBox.Text = date.ToString( "MMMM dd, yyyy" );
                    break;
                default:
                    break;
            }
        }

        private void onSelectFinished( object pSender, MouseButtonEventArgs pArgs )
        {
            fCurrentDigit = 0;
            fixDate( (TextBox) pSender );
            switch( InputTypeClass.GetInputType( pSender ) )
            {
                case References.DATE_INPUT:
                    TextBox dateBox = (TextBox) pSender;
                    if( dateBox.SelectionLength > 0 )
                    {
                        string locationString = dateLocation( dateBox.SelectionStart );
                        selectDateLocation( dateBox, locationString );
                    }
                    break;
                default:
                    break;
            }
        }

        /* Helper Functions */
        /// <summary>
        /// Returns true if the entire string contains only digit characters.
        /// </summary>
        /// <param name="pString">string to be searched for non-digit characters</param>
        /// <returns>true if string only contains digits; false otherwise</returns>
        private bool isNumeric( string pString )
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

        private void selectDateLocation( TextBox pDateBox, string pLocationString )
        {
            int start = 0;
            int length = 0;
            References.DATE_SELECTION_START.TryGetValue( pLocationString, out start );
            References.DATE_SELECTION_LENGTH.TryGetValue( pLocationString, out length );
            pDateBox.SelectionStart = start;
            pDateBox.SelectionLength = length;
        }

        private void incrementDateLocation( TextBox pDateBox, string pLocationString, int pInc )
        {
            // Get value of date location
            selectDateLocation( pDateBox, pLocationString );
            int currentValue = int.Parse( pDateBox.Text.Substring( pDateBox.SelectionStart, pDateBox.SelectionLength ) );

            // Increment the location
            currentValue += pInc;

            // Update date text
            string formatString = "{0}{1:D" + String.Format( "{0:D1}", pDateBox.SelectionLength ) + "}{2}";
            pDateBox.Text = String.Format( formatString, pDateBox.Text.Substring( 0, pDateBox.SelectionStart ), currentValue, pDateBox.Text.Substring( pDateBox.SelectionLength + pDateBox.SelectionStart ) );

            // Fix date if "rolled over"
            fixDate( pDateBox );
        }

        private void fixYear( int pYear, out bool oMax, out bool oMin, out int oYear )
        {
            oMax = false;
            oMin = false;
            oYear = pYear;
            if( pYear > References.YEAR_MAX )
            {
                oYear = References.YEAR_MAX;
                oMax = true;
            }
            else if( pYear < References.YEAR_MIN )
            {
                oYear = References.YEAR_MIN;
                oMin = true;
            }
        }

        private void fixMonth( int pMonth, int pYear, out bool oMax, out bool oMin, out int oMonth, out int oYear )
        {
            oMax = false;
            oMin = false;
            oMonth = pMonth;
            oYear = pYear;

            while( oMonth < 1 )
            {
                --oYear;
                fixYear( oYear, out oMax, out oMin, out oYear );
                if( oMin )
                {
                    oMonth = 1;
                }
                else
                {
                    oMonth += References.MONTH_MAX;
                }
            }

            while( oMonth > References.MONTH_MAX )
            {
                ++oYear;
                fixYear( oYear, out oMax, out oMin, out oYear );
                if( oMax )
                {
                    oMonth = References.MONTH_MAX;
                }
                else
                {
                    oMonth -= References.MONTH_MAX;
                }
            }
        }

        private void fixDate( TextBox pDateBox )
        {
            // Get Month
            int month = 1;
            int monthStart = 0;
            int monthLength = 0;
            References.DATE_SELECTION_START.TryGetValue( References.MONTH, out monthStart );
            References.DATE_SELECTION_LENGTH.TryGetValue( References.MONTH, out monthLength );
            month = int.Parse( pDateBox.Text.Substring( monthStart, monthLength ) );

            // Get Day
            int day = 1;
            int dayStart = 0;
            int dayLength = 0;
            References.DATE_SELECTION_START.TryGetValue( References.DAY, out dayStart );
            References.DATE_SELECTION_LENGTH.TryGetValue( References.DAY, out dayLength );
            day = int.Parse( pDateBox.Text.Substring( dayStart, dayLength ) );

            // Get Year
            int year = References.YEAR_MIN;
            int yearStart = 0;
            int yearLength = 0;
            References.DATE_SELECTION_START.TryGetValue( References.YEAR, out yearStart );
            References.DATE_SELECTION_LENGTH.TryGetValue( References.YEAR, out yearLength );
            year = int.Parse( pDateBox.Text.Substring( yearStart, yearLength ) );

            // Fix Year
            bool tmpMax = false;
            bool tmpMin = false;
            fixYear( year, out tmpMax, out tmpMin, out year );

            // Fix Month
            tmpMax = false;
            tmpMin = false;
            fixMonth( month, year, out tmpMax, out tmpMin, out month, out year );

            // Fix Day
            int daysInMonth = 0;
            do
            {
                if( daysInMonth > 0 )
                {
                    ++month;
                    tmpMax = false;
                    tmpMin = false;
                    fixMonth( month, year, out tmpMax, out tmpMin, out month, out year );
                    if( tmpMax )
                    {
                        day = daysInMonth;
                        break;
                    }
                    else
                    {
                        day -= daysInMonth;
                    }
                }
                string monthString = DateTime.ParseExact( String.Format( "{0:D2}", month ), "MM", System.Globalization.CultureInfo.InvariantCulture ).ToString( "MMMM" );
                References.DAYS_IN_MONTH.TryGetValue( monthString, out daysInMonth );
                if( References.isLeapMonth( month ) && References.isLeapYear( year ) )
                {
                    daysInMonth++;
                }
            } while( day > daysInMonth );

            while( day < 1 )
            {
                --month;
                tmpMax = false;
                tmpMin = false;
                fixMonth( month, year, out tmpMax, out tmpMin, out month, out year );
                if( tmpMin )
                {
                    day = 1;
                }
                else
                {
                    string monthString = DateTime.ParseExact( String.Format( "{0:D2}", month ), "MM", System.Globalization.CultureInfo.InvariantCulture ).ToString( "MMMM" );
                    References.DAYS_IN_MONTH.TryGetValue( monthString, out daysInMonth );
                    if( References.isLeapMonth( month ) && References.isLeapYear( year ) )
                    {
                        daysInMonth++;
                    }
                    day += daysInMonth;
                }
            }

            string monthFormat = "{0:D" + String.Format( "{0:D1}", monthLength ) + "}/";
            string dayFormat   = "{1:D" + String.Format( "{0:D1}", dayLength ) + "}/";
            string yearFormat  = "{2:D" + String.Format( "{0:D1}", yearLength ) + "}";
            pDateBox.Text = String.Format( monthFormat + dayFormat + yearFormat, month, day, year );
        }
    }
}
