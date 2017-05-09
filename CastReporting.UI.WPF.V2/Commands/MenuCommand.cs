using System.Windows.Input;

namespace CastReporting.UI.WPF.Commands
{
    public static class MenuCommand
    {
        public static RoutedCommand OpenWebServiceConfiguration { get; set; }
        public static RoutedCommand OpenReporting { get; set; }
        public static RoutedCommand OpenSettings { get; set; }
        public static RoutedCommand OpenHelp { get; set; }
        public static RoutedCommand Quit { get; set; }

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
