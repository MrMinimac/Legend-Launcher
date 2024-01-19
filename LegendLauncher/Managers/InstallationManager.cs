using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;
using CmlLib.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LegendLauncher.Model;
using LegendLauncher.ViewModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;

namespace LegendLauncher.Managers
{
    public class InstallationManager
    {
        private static string minecraftDirectory = AppSettingsManager.MinecraftDirectory;
        private CMLauncher _launcher;
        private ForgeDownloader _forge;
        LoaderBorderViewModel lbViewModel;

        private readonly string serversDatDirectory = Path.Combine(minecraftDirectory, "servers.dat");
        private readonly string MCSettingsDirectory = Path.Combine(minecraftDirectory, "options.txt");
        private readonly string ServersDatUrl = "https://github.com/MrMinimac/LegendLauncher/releases/download/FilesForLauncher/servers.dat";
        private readonly string MCSettingsUrl = "https://github.com/MrMinimac/LegendLauncher/releases/download/FilesForLauncher/options.txt";
        private readonly string ModUrlsJson = "https://github.com/MrMinimac/LegendLauncher/releases/download/FilesForLauncher/Mods.json";

        #region Initialize

        private void InitializeCMLauncer()
        {
            _launcher = new CMLauncher(new MinecraftPath(minecraftDirectory));
            _launcher.FileChanged += Launcher_FileChanged;
            _launcher.ProgressChanged += Launcher_ProgressChanged;
        }

        private void InitializeMForge()
        {
            _forge = new ForgeDownloader(_launcher);
            _forge.FileChanged += Forge_FileChanged;
            _forge.ProgressChanged += Forge_ProgressChanged;
        }

        private void InirializeViewModel()
        {
            lbViewModel = LoaderBorderViewModel.Instance;
            lbViewModel.Visibility = true;
            lbViewModel.IsPlayButtonEnabled = false;
            lbViewModel.Status = "Загрузка...";
        }

        #endregion

        #region Download And Run Minecraft

        public async Task InstallAndRunMinecraft(string versionName, string userName, double ramValue)
        {
            try
            {
                InirializeViewModel();
                InitializeCMLauncer();
                InitializeMForge();

                string forgeVersion = await Task.Run(() => InstallForge(versionName));

                if (forgeVersion != null)
                {
                    await DownloadMods();
                    await UpdateServersDat();
                    await DownloadMCSettings();
                    await Task.Run(() => RunMinecraft(forgeVersion, userName, ramValue));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        #region Install Forge
        private async Task<string> InstallForge(string versionName)
        {
            try
            {
                string forgeVerison = await Task.Run(() => _forge.Install(versionName));
                return forgeVerison;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка установки Forge: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
        #endregion

        #region Launcher & Forge Downloading Progress Changed
        private void Forge_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            lbViewModel.Procentage = $"{e.ProgressPercentage}%";
        }

        private void Launcher_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            lbViewModel.Procentage = $"{e.ProgressPercentage}%";
        }
        #endregion

        #region Launcher & Forge Files Changed
        private void Forge_FileChanged(DownloadFileChangedEventArgs e)
        {
            Console.WriteLine($"Загрузка Forge: {e.FileName} {e.ProgressedFileCount}/{e.TotalFileCount}");
            lbViewModel.Status = "Загрузка...";
            lbViewModel.FileName = $"{e.FileKind}";
        }

        private void Launcher_FileChanged(DownloadFileChangedEventArgs e)
        {
            Console.WriteLine($"Загрузка Minecraft: {e.FileName} {e.ProgressedFileCount}/{e.TotalFileCount}");
            lbViewModel.Status = "Загрузка...";
            lbViewModel.FileName = $"{e.FileKind}";
        }
        #endregion

        #region Run Minecraft
        private async Task RunMinecraft(string forgeVerison, string userName, double ramValue)
        {
            try
            {
                var launchOption = new MLaunchOption
                {
                    MaximumRamMb = (int)(ramValue * 1024),
                    Session = MSession.CreateOfflineSession(userName),
                    ServerIp = "legend.myftp.org",
                    ServerPort = 25565,
                    JVMArguments = new string[] {
                        $"-Xmx{(int)(ramValue * 1024)}M",
                        $"-Xms{(int)(ramValue * 1024)}M",
                    },
                };

                var process = await _launcher.CreateProcessAsync(forgeVerison, launchOption);
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.EnableRaisingEvents = true;
                process.ErrorDataReceived += Process_ErrorDataReceived;
                process.OutputDataReceived += Process_OutputDataReceived;

                process.Start();

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    Console.WriteLine($"------------------------------------");
                    Console.WriteLine($"Запуск Minecraft с параметрами");
                    Console.WriteLine($"Имя пользователя: {userName}");
                    Console.WriteLine($"Выделенная оперативная память: {(int)(ramValue * 1024)}GB");
                    Console.WriteLine($"------------------------------------");
                    lbViewModel.Status = "Запуск...";
                    lbViewModel.FileName = "Minecraft";
                    await Task.Delay(3000);
                    lbViewModel.Visibility = false;
                    MainWindow.Instance.MinWindow();
                    NotificationManager.Notify("Майнкрафт запускается.", "Лаунчер свернут.");
                });

                process.WaitForExit();

                Application.Current.Dispatcher.Invoke( () =>
                {
                    MainWindow.Instance.NormalizeWindow();
                    lbViewModel.IsPlayButtonEnabled = true;
                });
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка запуска Minecraft {e.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #endregion

        #region Download Mods
        private async Task DownloadMods()
        {
            try
            {
                string jsonContent = await InstallationManager.ReadJsonFromServer(ModUrlsJson);

                if (!string.IsNullOrEmpty(jsonContent))
                {
                    List<ModsInfo> modDataList = JsonConvert.DeserializeObject<List<ModsInfo>>(jsonContent);

                    using (WebClient client = new WebClient())
                    {
                        for (int i = 0; i < modDataList.Count; i++)
                        {
                            ModsInfo modInfo = modDataList[i];
                            Uri uri = new Uri(modInfo.Url);

                            string fileName = System.IO.Path.GetFileName(uri.LocalPath);
                            string localFilePath = System.IO.Path.Combine(minecraftDirectory, "mods", fileName);
                            string folderPath = System.IO.Path.Combine(minecraftDirectory, "mods");

                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }

                            if (!System.IO.File.Exists(localFilePath))
                            {
                                // Скачиваем файл, если его нет
                                client.DownloadFile(uri, localFilePath);
                                await Task.Delay(100);
                                lbViewModel.Status = "Скачиваем...";
                                lbViewModel.FileName = modInfo.Name;
                            }
                            else
                            {
                                lbViewModel.Status = "Проверяем...";
                                lbViewModel.FileName = modInfo.Name;
                                await Task.Delay(50);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DownloadMods: " + ex.Message);
                MessageBox.Show($"Произошла ошибка при скачивании файлов.");
            }
        }
        #endregion

        #region Update Server.dat
        private async Task UpdateServersDat()
        {
            try
            {
                if (File.Exists(serversDatDirectory))
                {
                    File.Delete(serversDatDirectory);
                }
                using WebClient client = new WebClient();
                await client.DownloadFileTaskAsync(new Uri(ServersDatUrl), serversDatDirectory);
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateServersDat: " + ex.Message);
                MessageBox.Show($"Произошла обновлении файло.");
            }
        }
        #endregion

        #region Read Json File
        public static async Task<string> ReadJsonFromServer(string url)
        {
            try
            {
                using HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine($"ReadJsonFromServer (response): {response.StatusCode}");
                    MessageBox.Show("Произошла ошибка при соединении с свервером.");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ReadJsonFromServer (Exception): {ex.Message}");
                MessageBox.Show("Произошла ошибка при соединении с свервером.");

                return null;
            }
        }
        #endregion

        #region Download Default Minecraft Settings
        private async Task DownloadMCSettings()
        {
            try
            {
                if (!File.Exists(MCSettingsDirectory))
                {
                    using WebClient client = new WebClient();
                    await client.DownloadFileTaskAsync(new Uri(MCSettingsUrl), MCSettingsDirectory);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузки настроек." + ex.Message);
            }

        }
        #endregion

        #region Console Output Data Received
        private async void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                string ParsedMessage = await ConsoleManager.MinecraftOutput($"{e.Data}");
                if (ParsedMessage != null && ParsedMessage != string.Empty)
                {
                    Console.WriteLine(ParsedMessage);
                }
            }
        }

        private async void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                string ParsedMessage = await ConsoleManager.MinecraftOutput($"{e.Data}");
                if (ParsedMessage != null && ParsedMessage != string.Empty) 
                {
                    Console.WriteLine(ParsedMessage);
                }
            }
        }
        #endregion

    }
}
