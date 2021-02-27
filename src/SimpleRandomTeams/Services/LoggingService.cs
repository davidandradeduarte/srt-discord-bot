using System;
using Serilog;

namespace SimpleRandomTeams.Services
{
    public static class LoggingService
    {
        public static void AddLogging()
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File(@"logs/log.log", rollingInterval: RollingInterval.Day)
                    .WriteTo.Console()
                    .CreateLogger();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error when creating the logger");
                Console.WriteLine(e.StackTrace ?? string.Empty, e.Message, e);
                throw;
            }
            
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception) e.ExceptionObject;
            Log.Error(exception.StackTrace, exception.Message, exception);
        }
    }
}