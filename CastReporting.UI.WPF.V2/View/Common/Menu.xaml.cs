using CastReporting.UI.WPF.Resources.Languages;
using System.Reflection;
using System.Windows.Controls;

namespace CastReporting.UI.WPF.Common
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        /// <summary>
     

        /// <summary>
        /// 
        /// </summary>
        public Menu()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;

            DataContext = this;          
        }


        /// <summary>
        /// Refresh all the bindings on controls
        /// </summary>
        public void Refersh()
        {
            DataContext = null;      
            DataContext = this;      
        }
    }
}
