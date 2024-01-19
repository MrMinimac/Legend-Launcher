using CmlLib.Core.Downloader;
using CmlLib.Core.Installer.Forge.Versions;
using CmlLib.Core.Installer.Forge;
using CmlLib.Core.Version;
using CmlLib.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;

namespace LegendLauncher.Managers
{
    class ForgeDownloader
    {
        public static readonly string ForgeAdUrl = "https://adfoc.us/serve/sitelinks/?id=271228&url=https://maven.minecraftforge.net/";

        private readonly CMLauncher _launcher;

        private readonly IForgeInstallerVersionMapper _installerMapper;

        private readonly ForgeVersionLoader _versionLoader;

        public event DownloadFileChangedHandler? FileChanged;

        public event EventHandler<ProgressChangedEventArgs>? ProgressChanged;

        public event EventHandler<string>? InstallerOutput;

        public ForgeDownloader(CMLauncher launcher)
        {
            _launcher = launcher;
            _installerMapper = new ForgeInstallerVersionMapper();
            _versionLoader = new ForgeVersionLoader(new HttpClient());
        }

        private ForgeInstallOptions createDefaultOptions()
        {
            return new ForgeInstallOptions(_launcher.MinecraftPath)
            {
                Downloader = new SequenceDownloader()
            };
        }

        public Task<string> Install(string mcVersion, bool forceUpdate = false)
        {
            return Install(mcVersion, createDefaultOptions(), forceUpdate);
        }

        public async Task<string> Install(string mcVersion, ForgeInstallOptions options, bool forceUpdate = false)
        {
            IEnumerable<ForgeVersion> source = await _versionLoader.GetForgeVersions(mcVersion).ConfigureAwait(continueOnCapturedContext: false);
            ForgeVersion forgeVersion = source.FirstOrDefault((ForgeVersion v) => v.IsRecommendedVersion) ?? source.FirstOrDefault((ForgeVersion v) => v.IsLatestVersion) ?? source.FirstOrDefault() ?? throw new InvalidOperationException("Cannot find any version");
            return await Install(forgeVersion, options, forceUpdate).ConfigureAwait(continueOnCapturedContext: false);
        }

        public Task<string> Install(string mcVersion, string forgeVersion, bool forceUpdate = false)
        {
            return Install(mcVersion, forgeVersion, createDefaultOptions(), forceUpdate);
        }

        public async Task<string> Install(string mcVersion, string forgeVersion, ForgeInstallOptions options, bool forceUpdate = false)
        {
            string forgeVersion2 = forgeVersion;
            ForgeVersion forgeVersion3 = (await _versionLoader.GetForgeVersions(mcVersion).ConfigureAwait(continueOnCapturedContext: false)).FirstOrDefault((ForgeVersion v) => v.ForgeVersionName == forgeVersion2) ?? throw new InvalidOperationException("Cannot find version name " + forgeVersion2);
            return await Install(forgeVersion3, options, forceUpdate).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<string> Install(ForgeVersion forgeVersion, ForgeInstallOptions options, bool forceUpdate)
        {
            IForgeInstaller installer = _installerMapper.CreateInstaller(forgeVersion);
            if (await checkVersionInstalled(installer.VersionName).ConfigureAwait(continueOnCapturedContext: false) && !forceUpdate)
            {
                return installer.VersionName;
            }

            MVersion version = await checkAndDownloadVanillaVersion(forgeVersion.MinecraftVersionName).ConfigureAwait(continueOnCapturedContext: false);
            if (string.IsNullOrEmpty(options.JavaPath))
            {
                options.JavaPath = getJavaPath(version);
            }

            installer.FileChanged += delegate (DownloadFileChangedEventArgs e)
            {
                this.FileChanged?.Invoke(e);
            };
            installer.ProgressChanged += delegate (object? s, ProgressChangedEventArgs e)
            {
                this.ProgressChanged?.Invoke(this, e);
            };
            installer.InstallerOutput += delegate (object? s, string e)
            {
                this.InstallerOutput?.Invoke(this, e);
            };
            await installer.Install(options).ConfigureAwait(continueOnCapturedContext: false);
            showAd();
            await _launcher.GetAllVersionsAsync().ConfigureAwait(continueOnCapturedContext: false);
            return installer.VersionName;
        }

        private async Task<MVersion> checkAndDownloadVanillaVersion(string mcVersion)
        {
            MVersion version = await _launcher.GetVersionAsync(mcVersion).ConfigureAwait(continueOnCapturedContext: false);
            await _launcher.CheckAndDownloadAsync(version).ConfigureAwait(continueOnCapturedContext: false);
            return version;
        }

        private async Task<bool> checkVersionInstalled(string versionName)
        {
            try
            {
                await _launcher.GetVersionAsync(versionName).ConfigureAwait(continueOnCapturedContext: false);
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        private string getJavaPath(MVersion version)
        {
            string text = _launcher.GetJavaPath(version);
            if (string.IsNullOrEmpty(text) || !File.Exists(text))
            {
                text = _launcher.GetDefaultJavaPath();
            }

            if (string.IsNullOrEmpty(text) || !File.Exists(text))
            {
                throw new InvalidOperationException("Cannot find any java binary. Set java binary path");
            }

            return text;
        }

        private void showAd()
        {

        }
    }
}
