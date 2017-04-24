using System;
using System.Threading;

namespace MCServerWrapper.Classes
{
    public static class ConsoleWriter
    {
        public static void WriteLine(string value)
        {
            string prefix = $"[{DateTime.Now.ToString("HH:mm:ss")}] [WrapperThread/{Thread.CurrentThread.Name}]: ";
            Console.WriteLine(prefix + value);
        }

        public static void WriteLine(string value, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            WriteLine(value);
            Console.ResetColor();
        }
    }
}
