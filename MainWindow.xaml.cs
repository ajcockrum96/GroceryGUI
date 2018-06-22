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
    public static class InputTypeClass
    {
        public static readonly DependencyProperty InputTypeProperty = 
                               DependencyProperty.RegisterAttached( "InputType",
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
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            QuantityBox myQuantityBox = new QuantityBox();
            myQuantityBox.Text = "10";

            for( int i = 1; i < WindowGrid.ColumnDefinitions.Count; ++i )
            {
                DateBox tmpDateBox = new DateBox();
                WindowGrid.Children.Add( tmpDateBox );
                Grid.SetColumn( tmpDateBox, i );
                Grid.SetRow( tmpDateBox, 0 );
                for( int j = 1; j < WindowGrid.RowDefinitions.Count; ++j )
                {
                    QuantityBox tmpQuantityBox = new QuantityBox();
                    WindowGrid.Children.Add( tmpQuantityBox );
                    Grid.SetColumn( tmpQuantityBox, i );
                    Grid.SetRow( tmpQuantityBox, j );
                }
            }
        }
    }
}
