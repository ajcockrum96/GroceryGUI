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
        private void onInputLostFocus( object sender, RoutedEventArgs args )
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
                    quantity.Text = currentValue.ToString();
                    break;
                case References.DATE_INPUT:
                    break;
                default:
                    break;
            }
        }
    }
}
