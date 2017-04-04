using System;

namespace MCServerWrapper.Classes
{
    public class ExceptionPrinter
    {
        public static void PrintException(Exception ex)
        {
            ConsoleWriter.WriteLine($"[Source]: {ex.Source}", ConsoleColor.Red);
            ConsoleWriter.WriteLine($"[Target Site]: {ex.TargetSite}", ConsoleColor.Red);
            ConsoleWriter.WriteLine($"[Error Message]: {ex.Message}", ConsoleColor.Red);
            ConsoleWriter.WriteLine($"[Inner Exception]: {ex.InnerException}", ConsoleColor.Red);
        }

        public static void PrintException(Exception ex, string message)
        {
            ConsoleWriter.WriteLine($"[Message]: {message}", ConsoleColor.Red);
            PrintException(ex);
        }
    }
}
