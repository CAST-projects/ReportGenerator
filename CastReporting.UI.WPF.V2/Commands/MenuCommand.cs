using System.Windows.Input;

namespace CastReporting.UI.WPF.Commands
{
    public static class MenuCommand
    {
        public static RoutedCommand OpenWebServiceConfiguration;
        public static RoutedCommand OpenReporting;
        public static RoutedCommand OpenSettings;
        public static RoutedCommand OpenHelp;
        public static RoutedCommand Quit;

        static MenuCommand()
        {
            OpenWebServiceConfiguration = new RoutedCommand();
            OpenReporting = new RoutedCommand();
            OpenSettings = new RoutedCommand();
            OpenHelp = new RoutedCommand();
            Quit = new RoutedCommand();
        }
    }
}
