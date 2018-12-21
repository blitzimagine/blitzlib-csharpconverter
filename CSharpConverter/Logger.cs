using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpConverter
{
    public class Logger
    {
        public enum LogLevel
        {
            INFO,
            WARN,
            ERROR,
            DEBUG
        }

        private string _name;

        public Logger(string name)
        {
            _name = name;
        }

        public void Log(LogLevel level, string msg)
        {
            switch (level)
            {
                case LogLevel.INFO:
                    Console.WriteLine(_name + "/" + level + ": " + msg);
                    break;
                case LogLevel.WARN:
                    Console.WriteLine(_name + "/" + level + ": " + msg);
                    break;
                case LogLevel.ERROR:
                    Console.WriteLine(_name + "/" + level + ": " + msg);
                    break;
                case LogLevel.DEBUG:
#if DEBUG
                    Console.WriteLine(_name + "/" + level + ": " + msg);
#endif
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public void Info(string msg)
        {
            Log(LogLevel.INFO, msg);
        }

        public void Warn(string msg)
        {
            Log(LogLevel.WARN, msg);
        }

        public void Error(string msg)
        {
            Log(LogLevel.ERROR, msg);
        }

        public void Debug(string msg)
        {
            Log(LogLevel.DEBUG, msg);
        }
    }
}
