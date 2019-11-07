using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace CloudSite.Models.Log
{
    public static class LogManager
    {
        public static void createOrCheckIfExistFileLog()
        {
            if (!File.Exists(Variables.LOG_FILE_PATH))
            {
                File.Create(Variables.LOG_FILE_PATH);
            }
            
            return;
        }

        private static string putDefaultStringForLog(string message)
        {
            string defaultString = string.Format("Utc - {0}/{1}/{2} {3}:{4}:{5}.{6} | ", 
                DateTime.UtcNow.Day.ToString("d2"), DateTime.UtcNow.Month.ToString("d2"), DateTime.UtcNow.Year, DateTime.UtcNow.Hour.ToString("d2"), 
                DateTime.Now.Minute.ToString("d2"), DateTime.Now.Second.ToString("d2"), DateTime.Now.Millisecond.ToString("d3"));

            return defaultString + message;
        }

        public static void writeOnLog(string message)
        {
            string finalMassage = putDefaultStringForLog(message);

            try
            {
                using (TextWriter logFile = new StreamWriter(Variables.LOG_FILE_PATH, true))
                {
                    logFile.WriteLine(finalMassage);
                }
            }
            catch (Exception)
            {
                Task ptl = new Task(() => putInLogLater(finalMassage));
                ptl.Start();
            }
        }

        private static void putInLogLater(string message)
        {
            int attempts = 3;

            while (--attempts > 0)
            {
                Thread.Sleep(5000);

                try
                {
                    using (TextWriter logFile = new StreamWriter(Variables.LOG_FILE_PATH, true))
                    {
                        logFile.WriteLine(message);
                    }
                }
                catch (Exception){ }
            }
        }
    }
}