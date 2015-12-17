using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillFramework
{
    public class Logger
    {
        public delegate void LogDelegate(string format, params object[] ps);
        public static LogDelegate LogInfoHandler = null;
        public static LogDelegate LogErrorHandler = null;

        public static void Error(string format, params object[] ps)
        {
            if (LogErrorHandler != null) LogErrorHandler(format, ps);
        }
        public static void Info(string format, params object[] ps)
        {
            if (LogInfoHandler != null) LogInfoHandler(format, ps);
        }
    }
}
