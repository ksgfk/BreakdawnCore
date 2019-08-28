using System;

namespace Breakdawn.Core
{
    public class Logger : Singleton<Logger>
    {
        public Action<string> OnLog { get; set; }
        public Action<string> OnWarn { get; set; }
        public Action<string> OnError { get; set; }
        public Action<Exception> OnLogException { get; set; }

        private Logger()
        {
            OnLog = Console.WriteLine;
            OnWarn = Console.WriteLine;
            OnError = Console.WriteLine;
            OnLogException = e => Console.WriteLine($"{e.Message}\n{e.StackTrace}");
        }

        public void Log(string message)
        {
            OnLog(message);
        }

        public void LogWarn(string message)
        {
            OnWarn(message);
        }

        public void LogError(string message)
        {
            OnError(message);
        }

        public void LogException(Exception e)
        {
            OnLogException(e);
        }
    }
}