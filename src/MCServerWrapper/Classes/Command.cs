using System;

namespace MCServerWrapper.Classes
{
    public class Command
    {
        private string[] _commands;
        private Action _action;

        public Command(string command, Action action)
        {
            _commands = new string[] { command };
            _action = action;
        }

        public Command(string[] commands, Action action)
        {
            _commands = commands;
            _action = action;
        }

        public string[] GetCommands()
        {
            return _commands;
        }

        public void Run()
        {
            _action?.Invoke();
        }
    }
}
