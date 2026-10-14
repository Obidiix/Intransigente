using System;
using System.Globalization;

namespace Dazed.Vitals
{
    public static class Scriptorium
    {
        public static event Action<string> OnEntryRecord;
        public static void Message(object? obj) => Message(message: obj?.ToString());
        public static void Message(string? message)
        {
            string result = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) + " - ";
            result += string.IsNullOrEmpty(message) ? "Null message." : message.EndWithDot();
            Console.WriteLine(result);
            OnEntryRecord?.Invoke("Message: " + result);
        }
        public static void Warn(object? obj) => Warn(message: obj?.ToString());
        public static void Warn(string? message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            string result = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) + " - ";
            result += string.IsNullOrEmpty(message) ? "Null message." : message.EndWithDot();
            Console.WriteLine(result);
            Console.ResetColor();
            OnEntryRecord?.Invoke("Warning: " + result);
        }
        public static void Warn(int entry) => Warn(entry.ToString());
        public static void Error(object obj) => Error(obj.ToString()!);
        public static void Error(string? message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            string result = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) + " - ";
            result += string.IsNullOrEmpty(message) ? "Null message." : message.EndWithDot();
            Console.WriteLine(result);
            Console.ResetColor();
            OnEntryRecord?.Invoke("Error: " + result);
        }

        public static void LogException(Exception ex)
        {
            Error("An exception occurred.");
            Error($"Error message: {ex.Message}");
            Error($"Stack trace: {ex.StackTrace}");
            Error($"Source: {ex.Source}");
        }
    }
}
