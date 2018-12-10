using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace ExpressInstaller
{
    class Logger
    {
        public static string LogDir { get; set; }

        public static void Log(string message)
        {
                using (StreamWriter w = File.AppendText(LogDir))
                {
                    WriteToLog(message, w);
                }
        }


        private static void WriteToLog(string logMessage, TextWriter txtWriter)
        {
            txtWriter.WriteLine("{0}: {1}\r\n", DateTime.Now.ToString("HH:mm:ss"),
                logMessage);
            
        }
    }
}
