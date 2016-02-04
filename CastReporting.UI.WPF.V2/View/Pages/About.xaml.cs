using System;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using CastReporting.UI.WPF.Resources.Languages;

namespace CastReporting.UI.WPF.View.Pages
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Page
    {
        public About()
        {
            InitializeComponent();
        }
        
        public static string lblAboutVersion {
        	get {
				return string.Format(Messages.lblAboutVersion, Assembly.GetExecutingAssembly().GetName().Version.ToString());
        	}
        }
    }
}
