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
    class QuantityBox : TextBox
    {
        /// <summary>
        /// 
        /// </summary>
        public QuantityBox() : base()
        {
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            TextAlignment = TextAlignment.Center;

            Style = (Style) FindResource( "TextBox" );

            Text = "0";
        }

        /// <summary>
        /// Limits the text input to numeric characters, and restricts the
        /// TextBox to a maximum of 2 digits total.
        /// </summary>
        /// <param name="pEventArgs">Event information passed by TextInput Event</param>
        protected override void OnTextInput( TextCompositionEventArgs pEventArgs )
        {
            if( Text.Length >= 2 || !References.isNumeric( Text ) )
            {
                pEventArgs.Handled = true;
            }
            base.OnTextInput( pEventArgs );
        }

        /// <summary>
        /// Adds the extra functionality of incrementing and decrementing the
        /// current numeric value of the contents of the TextBox, using the UP
        /// and DOWN arrow keys respectively. This functionality is limited to
        /// a lower numerical bound of 0 and an upper numerical bound of 99.
        /// </summary>
        /// <param name="pEventArgs">Event information passed by KeyDown Event</param>
        protected override void OnPreviewKeyDown( KeyEventArgs pEventArgs )
        {
            int currentValue = 0;
            if( Text.Length > 0 )
            {
                currentValue = int.Parse( Text );
            }

            switch( pEventArgs.Key )
            {
                case Key.Up:
                    if( currentValue < 99 )
                    {
                        ++currentValue;

                    }
                    Text = currentValue.ToString();
                    break;
                case Key.Down:
                    if( currentValue > 0 )
                    {
                        --currentValue;
                    }
                    Text = currentValue.ToString();
                    break;
                case Key.Space:
                    pEventArgs.Handled = true;
                    break;
                default:
                    break;
            }
            base.OnPreviewKeyDown( pEventArgs );
        }

        /// <summary>
        /// Modifies the contents of the QuantityBox so that the number is
        /// always represented by at least 1 number (empty box is changed
        /// to 0).
        /// </summary>
        /// <param name="pEventArgs">Event information passed by LostFocus Event</param>
        protected override void OnLostFocus( RoutedEventArgs pEventArgs )
        {
            int currentValue = 0;
            if( Text.Length > 0 )
            {
                currentValue = int.Parse( Text );
            }
            Text = currentValue.ToString();
            base.OnLostFocus( pEventArgs );
        }
    }
}
