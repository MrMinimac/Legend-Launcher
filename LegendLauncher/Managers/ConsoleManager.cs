using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LegendLauncher.Managers
{
    public static class ConsoleManager
    {
        private const string Kernel32_DllName = "kernel32.dll";

        [DllImport(Kernel32_DllName)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32_DllName)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32_DllName)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport(Kernel32_DllName)]
        private static extern int GetConsoleOutputCP();

        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }

        public static void Show()
        {
            if (!HasConsole)
            {
                AllocConsole();
                InvalidateOutAndError();
                SetConsoleUtf8Encoding();
            }
        }

        public static void Hide()
        {
            if (HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
                InitializeOutAndError();
            }
        }

        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private static void SetConsoleUtf8Encoding()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }

        static void InvalidateOutAndError()
        {
            Type type = typeof(Console);

            System.Reflection.FieldInfo _out = type.GetField("_out", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            System.Reflection.FieldInfo _error = type.GetField("_error", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.MethodInfo _InitializeStdOutError = type.GetMethod("InitializeStdOutError", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            Debug.Assert(_out != null);
            Debug.Assert(_error != null);
            Debug.Assert(_InitializeStdOutError != null);

            _out.SetValue(null, null);
            _error.SetValue(null, null);

            _InitializeStdOutError.Invoke(null, new object[] { true });
        }

        static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }

        static void InitializeOutAndError()
        {
            Type type = typeof(Console);

            System.Reflection.FieldInfo _out = type.GetField("_out", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            System.Reflection.FieldInfo _error = type.GetField("_error", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.MethodInfo _InitializeStdOutError = type.GetMethod("InitializeStdOutError", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            Debug.Assert(_out != null);
            Debug.Assert(_error != null);
            Debug.Assert(_InitializeStdOutError != null);

            _out.SetValue(null, Console.Out);
            _error.SetValue(null, Console.Error);

            _InitializeStdOutError.Invoke(null, new object[] { true });
        }

        #region === Форматирование для вывода логов ===

        public static async Task<string> MinecraftOutput(string message)
        {
            if (message != null)
            {
                string parsedText = Parse(message);
                if (parsedText != string.Empty)
                {
                    await Task.Delay(250);
                    return parsedText;
                }
            }

            return null;
        }

        private static string Parse(string message)
        {

            string Logger, Timestamp, Level, Thread, Info, Throwable;

            if (message.Contains("log4j:Event"))
            {
                // Используем регулярные выражения для извлечения значений из строки
                Match match = Regex.Match(message, @"logger=""([^""]*)"" timestamp=""([^""]*)"" level=""([^""]*)"" thread=""([^""]*)""");

                if (match.Success)
                {
                    Logger = match.Groups[1].Value;
                    Timestamp = GetUserTime(match.Groups[2].Value);
                    Level = match.Groups[3].Value;
                    Thread = match.Groups[4].Value;

                    string FormatedString = $"\n[{Timestamp}] [{Thread}/{Level}] {Logger}";
                    return FormatedString;
                }
            }
            if (message.Contains("log4j:Message"))
            {
                // Используем регулярные выражения для извлечения значений из строки
                Match match = Regex.Match(message, @"<!\[CDATA\[(.*?)\]\]>");

                if (match.Success)
                {
                    Info = match.Groups[1].Value;

                    string FormatedString = $"{Info}";
                    return FormatedString;
                }
            }
            if (message.Contains("log4j:Throwable"))
            {
                // Используем регулярные выражения для извлечения значений из строки
                Match match = Regex.Match(message, @"<!\[CDATA\[(.*?)$");

                if (match.Success)
                {
                    Throwable = match.Groups[1].Value;

                    string FormatedString = $"{Throwable}";
                    return FormatedString;
                }
            }
            if (message.Contains("</log4j:"))
            {
                return string.Empty;
            }

            return message;
        }

        private static string GetUserTime(string message)
        {
            // Получаем информацию о часовом поясе пользователя
            TimeZoneInfo userTimeZone = TimeZoneInfo.Local;

            // Получаем метку времени из строки
            long timestamp = long.Parse(message);
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);

            // Преобразуем время в часовой пояс пользователя
            DateTime userTime = TimeZoneInfo.ConvertTime(dateTimeOffset.DateTime, TimeZoneInfo.Utc, userTimeZone);

            // Выводим отформатированное время с учетом часового пояса
            string formattedTime = userTime.ToString("HH:mm");
            return formattedTime;
        }

        #endregion
    }

}
