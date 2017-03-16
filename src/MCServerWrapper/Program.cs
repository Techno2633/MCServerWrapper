using MCServerWrapper.Classes;
using System;
using System.Threading;

namespace MCServerWrapper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "Main";
            ServerProgram server = new ServerProgram();
            server.Start();
        }
    }
}
