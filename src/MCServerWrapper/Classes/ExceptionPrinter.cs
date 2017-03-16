using System;

namespace MCServerWrapper.Classes
{
    public class ExceptionPrinter
    {
        public static void PrintException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleWriter.WriteLine($"[Source]: {ex.Source}");
            ConsoleWriter.WriteLine($"[Target Site]: {ex.TargetSite}");
            ConsoleWriter.WriteLine($"[Error Message]: {ex.Message}");
            //ConsoleWriter.WriteLine();
            Console.ResetColor();
        }

        public static void PrintException(Exception ex, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleWriter.WriteLine($"[Message]: {message}");
            PrintException(ex);
            Console.ResetColor();
        }
    }
}
