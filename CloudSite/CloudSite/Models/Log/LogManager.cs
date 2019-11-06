using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace CloudSite.Models.Log
{
    public static class LogManager
    {
        public static void createFileLog()
        {
            if (!File.Exists(Variables.LOG_DIRECTORY_PATH))
            {
                File.Create(Variables.LOG_DIRECTORY_PATH + @"/Log.log");
            }
            
            return;
        }

        private static string putDefaultStringForLog(string message)
        {
            string defaultString = string.Format("{0}/{1}/{2} {3}:{4}:{5}.{6} | ", 
                DateTime.UtcNow.Day.ToString("d2"), DateTime.UtcNow.Month.ToString("d2"), DateTime.UtcNow.Year, DateTime.UtcNow.Hour.ToString("d2"), 
                DateTime.Now.Minute.ToString("d2"), DateTime.Now.Second.ToString("d2"), DateTime.Now.Millisecond.ToString("d3"));

            return defaultString + message;
        }

        public static void writeOnLog(string message)
        {
            string finalMassage = putDefaultStringForLog(message);
            using(TextWriter logFile = new StreamWriter(Variables.LOG_DIRECTORY_PATH + @"/Log.log", true))
            {
                logFile.WriteLine(finalMassage);
            }
        }
    }
}