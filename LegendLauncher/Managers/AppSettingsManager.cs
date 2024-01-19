using System;
using System.IO;
using System.Xml.Serialization;

namespace LegendLauncher.Managers
{
    public class AppSettingsManager
    {
        public static readonly string MinecraftDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".Legend");
        private static readonly string filePath = System.IO.Path.Combine(MinecraftDirectory, "Settings.config");

        public string UserName { get; set; }
        public string Uuid { get; set; }
        public string ProfileImage { get; set; }
        public double RamValue { get; set; }
        public string IsTheme { get; set; }
        public bool IsDarkTheme { get; set; }

        #region Load Settings
        public static AppSettingsManager Load()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(AppSettingsManager));
                    using var reader = new StreamReader(filePath);
                    return (AppSettingsManager)serializer.Deserialize(reader);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при загрузке настроек: {ex.Message}");
                }
            }

            // Вернуть настройки по умолчанию, если файл не существует или произошла ошибка при загрузке
            return new AppSettingsManager { RamValue = 4, IsTheme = "GreenTheme", IsDarkTheme = true };
        }
        #endregion

        #region Save Settings
        public void Save()
        {
            try
            {
                if (!Directory.Exists(MinecraftDirectory))
                {
                    Directory.CreateDirectory(MinecraftDirectory);
                }
                var serializer = new XmlSerializer(typeof(AppSettingsManager));
                using var writer = new StreamWriter(filePath);
                serializer.Serialize(writer, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }
        #endregion
    }
}
