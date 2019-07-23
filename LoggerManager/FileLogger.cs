using System;
using System.IO;
using System.Threading.Tasks;
using IPA.LoggerManager.Helpers;

namespace IPA.LoggerManager
{
    public class FileLogger : LogBase
     {
        public string filePath = "";

        public FileLogger(string fName)
        {
            filePath = fName;
        }

        private System.Threading.Tasks.Task LoggerWriter(string type, string message)
        {
            if (type == null) throw new ArgumentNullException("LOGGER: type");
            if (type == string.Empty) throw new ArgumentException("empty", "LOGGER: type");
            if (message == null) throw new ArgumentNullException("LOGGER: message");
            if (message == string.Empty) throw new ArgumentException("empty", "LOGGER: message");
            lock (lockObj)
            {
                using (StreamWriter streamWriter = new StreamWriter(filePath, append: true))
                {
                    string logMessage = Utils.GetTimeStamp() + type + message;
                    streamWriter.WriteLine(logMessage);
                    streamWriter.Close();
                }
                return Task.FromResult(0);
            }
        }

        // DEBUG LOGGING
        public override System.Threading.Tasks.Task debug(string message)
        {
            return LoggerWriter(" [d] ", message);
        }
        // INFO LOGGING
        public override System.Threading.Tasks.Task info(string message)
        {
            return LoggerWriter(" [i] ", message);
        }
        // WARNING LOGGING
        public override System.Threading.Tasks.Task warning(string message)
        {
            return LoggerWriter(" [w] ", message);
        }
        // ERROR LOGGING
        public override System.Threading.Tasks.Task error(string message)
        {
            return LoggerWriter(" [e] ", message);
        }
        // FATAL LOGGING
        public override System.Threading.Tasks.Task fatal(string message)
        {
            return LoggerWriter(" [f] ", message);
        }
    }
}
