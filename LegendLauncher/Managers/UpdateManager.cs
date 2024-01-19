using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace LegendLauncher.Managers
{
    public class UpdateManager
    {
        private readonly MainWindow mainWindow;
        private string updateCheckUrl = "https://github.com/MrMinimac/LegendLauncher/releases/download/FilesForLauncher/updateinfo.txt";
        private string updateUrl = "https://github.com/MrMinimac/LegendLauncher/releases/download/FilesForLauncher/Setup.exe";
        private string localLauncherFolder = Path.Combine(AppSettingsManager.MinecraftDirectory, "Launcher");

        public event EventHandler UpdateInstallationCompleted;

        public UpdateManager(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            Task.Run(() => CheckForUpdates());
        }

        #region Check Updates
        private void CheckForUpdates()
        {
            try
            {
                WebClient client = new WebClient();
                string updateInfo = client.DownloadString(updateCheckUrl);

                if (updateInfo.Length < 5 || updateInfo.Length > 20)
                {
                    ConsoleManager.Show();
                    Console.WriteLine("Недопустимый формат строки версии:   " + updateInfo);
                    return;
                }

                Version serverVersion = new Version(updateInfo);
                Version appVersion = Assembly.GetEntryAssembly().GetName().Version;

                string serverVersionString = serverVersion.ToString();
                string versionString = appVersion.ToString();

                if (serverVersion > appVersion)
                {
                    DownloadAndInstallUpdate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверке обновлений: " + ex.Message);
            }
        }
        #endregion

        #region Download and Install Updates
        private void DownloadAndInstallUpdate()
        {
            try
            {
                WebClient client = new WebClient();
                string tempPath = Path.Combine(Path.GetTempPath(), "Setup.exe");

                // Скачивание файла обновления
                client.DownloadFile(updateUrl, tempPath);
                CMD($"start {tempPath}");
                CMD($"taskkill /f /im LegendLauncher.exe && timeout /t 1 && rmdir /s /q {localLauncherFolder}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("[Launcher] Ошибка при установке обновления: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region CMD
        public void CMD(string line)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/c {line}",
                WindowStyle = ProcessWindowStyle.Hidden,
            });
        }
        #endregion
    }
}
