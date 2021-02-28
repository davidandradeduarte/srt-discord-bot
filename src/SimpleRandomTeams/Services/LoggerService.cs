using System;
using DSharpPlus;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SimpleRandomTeams.Services
{
    public static class LoggerService
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

        public static void LogDebug(DiscordClient client, string message)
        {
            client.Logger.LogDebug(message);
            Log.Debug(message);
        }

        public static void LogInformation(DiscordClient client, string message)
        {
            client.Logger.LogInformation(message);
            Log.Information(message);
        }
        
        public static void LogInformation(DiscordClient client, EventId eventId, string message)
        {
            client.Logger.LogInformation(eventId, message);
            Log.Information(message);
        }

        public static void LogWarning(DiscordClient client, string message)
        {
            client.Logger.LogWarning(message);
            Log.Warning(message);
        }
        
        public static void LogError(DiscordClient client, Exception exception, string message)
        {
            client.Logger.LogError(exception, message);
            Log.Error(exception, message);
        }
        
        public static void LogError(DiscordClient client, EventId eventId, Exception exception, string message)
        {
            client.Logger.LogError(eventId, exception, message);
            Log.Error(exception, message);
        }
        
        public static void LogError(DiscordClient client, EventId eventId, string message, DateTime time)
        {
            client.Logger.LogError(eventId, message, time);
            Log.Error(message, time);
        }
        
        public static void LogError(DiscordClient client, Exception exception)
        {
            client.Logger.LogError(exception, exception.Message);
            Log.Error(exception, exception.Message);
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception) e.ExceptionObject;
            Log.Error(exception.StackTrace, exception.Message, exception);
        }
    }
}