using System.Windows.Input;

namespace CastReporting.UI.WPF.Commands
{
    public static class SettingsCommand
    {
        public static RoutedCommand SaveSettings;

        static SettingsCommand()
        {
            SaveSettings = new RoutedCommand();
        }
    }
}
