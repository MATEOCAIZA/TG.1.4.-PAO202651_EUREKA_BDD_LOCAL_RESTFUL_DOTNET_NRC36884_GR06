using System;
using System.IO;
using System.Web;

namespace WS_EUREKABANK_RESTFUL_DOTNET_GR06.Logging
{
    public static class LogService
    {
        private static readonly object _lock = new object();

        private static string LogDirectory
        {
            get
            {
                string root = HttpContext.Current != null
                    ? HttpContext.Current.Server.MapPath("~/App_Data/logs")
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "logs");

                if (!Directory.Exists(root))
                    Directory.CreateDirectory(root);

                return root;
            }
        }

        private static string LogFile
            => Path.Combine(LogDirectory, $"eureka-{DateTime.Now:yyyy-MM-dd}.log");

        public static void Info(string message)
            => Write("INFO ", message, null);

        public static void Warning(string message)
            => Write("WARN ", message, null);

        public static void Error(string message, Exception ex = null)
            => Write("ERROR", message, ex);

        private static void Write(string level, string message, Exception ex)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string line = $"[{timestamp}] [{level}] {message}";

                if (ex != null)
                {
                    line += Environment.NewLine
                         + "  Exception : " + ex.GetType().Name + ": " + ex.Message;

                    if (ex.InnerException != null)
                        line += Environment.NewLine
                             + "  Inner     : " + ex.InnerException.GetType().Name + ": " + ex.InnerException.Message;

                    line += Environment.NewLine
                         + "  StackTrace: " + ex.StackTrace;
                }

                lock (_lock)
                    File.AppendAllText(LogFile, line + Environment.NewLine);
            }
            catch
            {
                // Logging must never crash the application
            }
        }
    }
}
