using LegendLauncher.Managers;
using LegendLauncher.ViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LegendLauncher.Pages
{
    public partial class LoginPage : Page
    {
        Frame _frame;
        ValidationManager validationManager = new ValidationManager();
        AppSettingsManager appSettings = new AppSettingsManager();
        LoginViewModel loginViewModel = new LoginViewModel();

        public LoginPage(Frame frame)
        {
            InitializeComponent();
            _frame = frame;
            LoadSettings();
            LoadPage();
        }

        #region Load Page
        private async void LoadPage()
        {
            PageBorder.Opacity = 0;
            for (double i = 0; i < 1.0; i += 0.05)
            {
                PageBorder.Opacity = i;
                await Task.Delay(10);
            }
        }
        #endregion

        #region Login Click
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string _username = NameBox.Text;
            ValidationResult validationResult = validationManager.ValidateUsername(_username);
            if (validationResult.IsValid)
            {
                for (double i = 1.0; i > 0.0; i -= 0.1)
                {
                    PageBorder.Opacity = i;
                    await Task.Delay(20);
                }
                LoginButton.IsEnabled = false;
                appSettings = AppSettingsManager.Load();
                appSettings.UserName = _username;
                if (DataBaseManager.CheckDatabaseConnectionAsync())
                {
                    appSettings.Uuid = DataBaseManager.GetUUID(_username);
                }
                appSettings.Save();
                HomePage homePage = new HomePage(_frame);
                _frame.Navigate(homePage);
            }
        }
        #endregion

        #region Load Settings
        public void LoadSettings()
        {
            try
            {
                appSettings = AppSettingsManager.Load();
                NameBox.Text = appSettings.UserName;
                EnableButton(NameBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке настроек: {ex.Message}");
            }
        }
        #endregion

        #region TextBox Text Changed
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string _username = NameBox.Text;
            EnableButton(_username);
        }
        #endregion

        #region Enable Button
        private void EnableButton(string username)
        {
            ValidationResult validationResult = validationManager.ValidateUsername(username);
            if (validationResult.IsValid)
            {
                LoginButton.IsEnabled = true;
            }
            else
            {
                LoginButton.IsEnabled = false;
            }
        }
        #endregion
    }
}
