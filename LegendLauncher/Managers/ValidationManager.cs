using LegendLauncher.ViewModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace LegendLauncher.Managers
{
    public class ValidationManager : ValidationRule
    {
        LoginViewModel loginViewModel = new LoginViewModel();

        public ValidationType Type { get; set; }
        
        public enum ValidationType
        {
            Username,
            Password,
            Email
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string input = value?.ToString();

            switch (Type)
            {
                case ValidationType.Username:
                    return ValidateUsername(input);
                case ValidationType.Password:
                    return ValidatePassword(input);
                case ValidationType.Email:
                    return ValidateEmail(input);
                default:
                    return ValidationResult.ValidResult;
            }
        }

        public ValidationResult ValidatePassword(string password)
        {
            if (password.Length > 16)
            {
                return new ValidationResult(false, "Пароль слишком длинный");
            }
            if (password.Length < 8)
            {
                return new ValidationResult(false, "Пароль слишком короткий");
            }
            if (!Regex.IsMatch(password, "^[a-zA-Z0-9_]+$"))
            {
                return new ValidationResult(false, "Логин содержит запрещенные символы");
            }
            return ValidationResult.ValidResult;
        }

        public ValidationResult ValidateUsername(string username)
        {
            if (username == null)
            {
                return ValidationResult.ValidResult;
            }
            if (username.Length > 16)
            {
                return new ValidationResult(false, "Логин слишком длинный");
            }
            if (username.Length < 3)
            {
                return new ValidationResult(false, "Логин слишком короткий");
            }
            if (!Regex.IsMatch(username, "^[a-zA-Z0-9_]+$"))
            {
                return new ValidationResult(false, "Логин содержит запрещенные символы");
            }
            return ValidationResult.ValidResult;
        }

        private ValidationResult ValidateEmail(string email)
        {
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return new ValidationResult(false, "Пожалуйста, введите корректный адрес электронной почты.");
            }
            return ValidationResult.ValidResult;
        }
    }
}
