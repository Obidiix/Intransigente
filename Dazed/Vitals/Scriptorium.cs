using System;
using System.Globalization;

namespace Dazed.Vitals
{
    public static class Scriptorium
    {
        public static event Action<string> OnEntryRecord;

        public static void Message(object obj) => Scriptorium.Message(obj.ToString());
        public static void Message(string message)
        {
            if (string.IsNullOrEmpty(message))
                Console.WriteLine("Null.");
            else
                Console.WriteLine(message.EndWithPoint());
            OnEntryRecord?.Invoke("Message: " + message);
        }
        public static void Warn(object obj) => Warn(obj.ToString()!);
        public static void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(DateTime.Now.ToString(CultureInfo.InvariantCulture) + " - ");
            if (string.IsNullOrEmpty(message))
                Console.WriteLine("Null.");
            else
                Console.WriteLine(message.EndWithPoint());
            Console.ResetColor();
            OnEntryRecord?.Invoke("Warning: " + message);
        }
        public static void Warn(int entry) => Warn(entry.ToString());
        public static void Error(object obj) => Error(obj.ToString()!);
        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (string.IsNullOrEmpty(message))
                Console.WriteLine("Null.");
            else
                Console.WriteLine(message.EndWithPoint());
            Console.ResetColor();
            OnEntryRecord?.Invoke("Error: " + message);
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
