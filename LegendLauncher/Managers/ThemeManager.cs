using System;
using System.IO;
using System.Windows;

namespace LegendLauncher.Managers
{
    class ThemeManager
    {

        public static void ChangeTheme(string ThemeName, bool isDark)
        {
            string path = Path.Combine("Styles", ThemeName + ".xaml");
            string DarkLightPath;

            if (isDark)
            {
                DarkLightPath = Path.Combine("Styles", "DarkTheme.xaml");
            }
            else
            {
                DarkLightPath = Path.Combine("Styles", "LightTheme.xaml");
            }

            Uri uri = new Uri(path, UriKind.Relative);
            Uri DarkLightUri = new Uri(DarkLightPath, UriKind.Relative);

            ResourceDictionary theme = new ResourceDictionary()
            {
                Source = uri,
            };

            ResourceDictionary DarkLightTheme = new ResourceDictionary()
            {
                Source = DarkLightUri,
            };

            App.Current.Resources.MergedDictionaries.Clear();
            App.Current.Resources.MergedDictionaries.Add(theme);
            App.Current.Resources.MergedDictionaries.Add(DarkLightTheme);

            if (App.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.UpdateLayout(); // Обновите макет, чтобы изменения темы вступили в силу
            }
        }

    }
}
