using System.Windows.Controls;

namespace CastReporting.UI.WPF.Common
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
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
            var button = sender as Button;
            if (button?.ContextMenu != null)
            {
                button.ContextMenu.IsEnabled = true;
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
            }

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
