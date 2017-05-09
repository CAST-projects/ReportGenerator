using System.Windows.Input;

namespace CastReporting.UI.WPF.Commands
{
    public static class WebServiceCommand
    {
        public static RoutedCommand ActivateWebService { get; set; }

        static WebServiceCommand()
        {
            ActivateWebService = new RoutedCommand();
        }
    }
}
