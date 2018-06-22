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
        private int inputCount = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// <para>Filter for text input into TextBox objects that register this
        /// function as their PreviewTextInput event handler.</para>
        /// <para>TextBox objects of type <see cref="References.QUANTITY_INPUT"/>
        /// will be restricted to digit-only input and a character limit of 2.</para>
        /// <para>TextBox objects of type <see cref="References.DATE_INPUT"/>
        /// will be restricted to...</para>
        /// </summary>
        /// <param name="sender">object that is undergoing the TextInput Event</param>
        /// <param name="args">Event information passed by TextInput Event</param>
        private void onTextInput( object sender, TextCompositionEventArgs args )
        {
            switch( InputTypeClass.GetInputType( sender ) )
            {
                case References.QUANTITY_INPUT:
                    if( ((TextBox) sender).Text.Length < 2 )
                    {
                        if( !isNumeric( args.Text ) )
                        {
                            args.Handled = true;
                        }
                    }
                    else
                    {
                        args.Handled = true;
                    }
                    break;
                case References.DATE_INPUT:
                    // TODO: Input Control
                    if( isNumeric( args.Text ) )
                    {
                        TextBox date = (TextBox) sender;
                        string locationString = dateLocation( date.SelectionStart );
                        fixDateSelection( date, locationString );
                        int start = date.SelectionStart;
                        int index = start + inputCount;
                        date.Text = date.Text.Remove( index, 1 );
                        date.Text = date.Text.Insert( index, args.Text );
                        int range = 1;
                        References.DATE_SELECTION_LENGTH.TryGetValue( locationString, out range );
                        inputCount = ++inputCount % range;
                        if( inputCount == 0 )
                        {
                            fixDateInput( date, locationString );
                        }
                        fixDateSelection( date, locationString );
                    }
                    args.Handled = true;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Returns true if the entire string contains only digit characters.
        /// </summary>
        /// <param name="s">string to be searched for non-digit characters</param>
        /// <returns>true if string only contains digits; false otherwise</returns>
        private bool isNumeric( string s )
        {
            bool containsAlpha = false;
            for( int i = 0; i < s.Length; ++i )
            {
                if( !Char.IsDigit( s, i ) )
                {
                    containsAlpha = true;                    
                    break;
                }
            }
            return !containsAlpha;
        }

        private string dateLocation( int start )
        {
            string locationString = "";
            switch( start )
            {
                // Month Edit
                case 0:
                case 1:
                    locationString = References.MONTH;
                    break;
                // Day Edit
                case 2:
                case 3:
                case 4:
                    locationString = References.DAY;
                    break;
                // Year Edit
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    locationString = References.YEAR;
                    break;
                default:
                    locationString = References.MONTH;
                    break;
            }
            return locationString;
        }

        private void fixDateSelection( TextBox date, string locationString )
        {
            int start = 0;
            int length = 0;
            References.DATE_SELECTION_START.TryGetValue( locationString, out start );
            References.DATE_SELECTION_LENGTH.TryGetValue( locationString, out length );
            date.SelectionStart = start;
            date.SelectionLength = length;
        }

        private void fixDateInput( TextBox date, string locationString )
        {
            int start = 0;
            int length = 0;
            References.DATE_SELECTION_START.TryGetValue( locationString, out start );
            References.DATE_SELECTION_LENGTH.TryGetValue( locationString, out length );
            int value = int.Parse( date.Text.Substring( start, length ) );
            switch( locationString )
            {
                case References.MONTH:
                    if( value < 1 )
                    {
                        value = 1;
                    }
                    else if( value > References.MONTH_MAX )
                    {
                        value = References.MONTH_MAX;
                    }
                    break;
                case References.DAY:
                    int monthStart  = 0;
                    int monthLength = 0;
                    References.DATE_SELECTION_START.TryGetValue( References.MONTH, out monthStart );
                    References.DATE_SELECTION_LENGTH.TryGetValue( References.MONTH, out monthLength );
                    string month = DateTime.ParseExact( date.Text.Substring( monthStart, monthLength ), "MM", System.Globalization.CultureInfo.InvariantCulture ).ToString( "MMMM" );
                    int dayMax = 0;
                    References.DAYS_IN_MONTH.TryGetValue( month, out dayMax );
                    if( value < 1 )
                    {
                        value = 1;
                    }
                    else if( value > dayMax )
                    {
                        value = dayMax;
                    }
                    break;
                case References.YEAR:
                    if( value < References.YEAR_MIN )
                    {
                        value = References.YEAR_MIN;
                    }
                    else if( value > References.YEAR_MAX )
                    {
                        value = References.YEAR_MAX;
                    }
                    break;
                default:
                    if( value < 1 )
                    {
                        value = 1;
                    }
                    else if( value > References.MONTH_MAX )
                    {
                        value = References.MONTH_MAX;
                    }
                    break;
            }
            string formatString = "{0}{1:D" + String.Format( "{0:D1}", length ) + "}{2}";
            date.Text = String.Format( formatString, date.Text.Substring( 0, start ), value, date.Text.Substring( start + length ) );
        }

        private void incrementDateSelection( TextBox date, string locationString, int increment )
        {
            fixDateSelection( date, locationString );
            int currentValue = int.Parse( date.Text.Substring( date.SelectionStart, date.SelectionLength ) );
            currentValue += increment;
            string formatString = "{0}{1:D" + String.Format( "{0:D1}", date.SelectionLength ) + "}{2}";
            date.Text = String.Format( formatString, date.Text.Substring( 0, date.SelectionStart ), currentValue, date.Text.Substring( date.SelectionLength + date.SelectionStart ) );
            fixDateInput( date, locationString );
            fixDateSelection( date, locationString );
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
        /// <param name="sender">object that is undergoing the KeyDown Event</param>
        /// <param name="args">Event information passed by TextInput Event</param>
        private void onKeyInput( object sender, KeyEventArgs args)
        {
            switch( InputTypeClass.GetInputType( sender ) )
            {
                case References.QUANTITY_INPUT:
                    TextBox quantity = (TextBox) sender;
                    int currentValue = 0;
                    if( quantity.Text.Length > 0 )
                    {
                        currentValue = int.Parse( quantity.Text );
                    }
                    
                    switch( args.Key )
                    {
                        case Key.Up:
                            if( currentValue < 99 )
                            {
                                ++currentValue;
                                quantity.Text = currentValue.ToString();
                            }
                            break;
                        case Key.Down:
                            if( currentValue > 0 )
                            {
                                --currentValue;
                                quantity.Text = currentValue.ToString();
                            }
                            break;
                        case Key.Space:
                            args.Handled = true;
                            break;
                        default:
                            break;
                    }
                    break;
                case References.DATE_INPUT:
                    TextBox date = (TextBox) sender;
                    string locationString = "";
                    switch( args.Key )
                    {
                        case Key.Up:
                            args.Handled = true;
                            locationString = dateLocation( date.SelectionStart );
                            incrementDateSelection( date, locationString, 1 );
                            break;
                        case Key.Down:
                            args.Handled = true;
                            locationString = dateLocation( date.SelectionStart );
                            incrementDateSelection( date, locationString, -1 );
                            break;
                        case Key.Back:
                        case Key.Delete:
                            args.Handled = true;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }		
		private void onInputGotFocus( object sender, RoutedEventArgs e )
        {
            switch( InputTypeClass.GetInputType( sender ) )
            {
                case References.DATE_INPUT:
                    TextBox dateBox = (TextBox) sender;
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
        /// <param name="sender">object that has lost focus</param>
        /// <param name="args">Event information passed by LostFocus Event</param>
        private void onInputLostFocus( object sender, RoutedEventArgs e )
        {
            inputCount = 0;
            fixDateInput( (TextBox) sender, References.MONTH );
            fixDateInput( (TextBox) sender, References.DAY );
            fixDateInput( (TextBox) sender, References.YEAR );
            switch( InputTypeClass.GetInputType( sender ) )
            {
				case References.QUANTITY_INPUT:
                    TextBox quantity = (TextBox) sender;
                    int currentValue = 0;
                    if( quantity.Text.Length > 0 )
                    {
                        currentValue = int.Parse( quantity.Text );
                    }
                    quantity.Text = currentValue.ToString();
                    break;
                case References.DATE_INPUT:
                    TextBox dateBox = (TextBox) sender;
                    DateTime date = DateTime.ParseExact( dateBox.Text, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture );
                    dateBox.Text = date.ToString( "MMMM dd, yyyy" );
                    break;
                default:
                    break;
            }
        }
		private void onSelectFinished( object sender, MouseButtonEventArgs args )
        {
            inputCount = 0;
            fixDateInput( (TextBox) sender, References.MONTH );
            fixDateInput( (TextBox) sender, References.DAY );
            fixDateInput( (TextBox) sender, References.YEAR );
            switch( InputTypeClass.GetInputType( sender ) )
            {
                case References.DATE_INPUT:
                    TextBox date = (TextBox) sender;
                    if( date.SelectionLength > 0 )
                    {
                        string locationString = dateLocation( date.SelectionStart );
                        fixDateSelection( date, locationString );
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
