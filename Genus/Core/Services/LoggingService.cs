using Discord;
using Microsoft.Extensions.Logging;

namespace GenusBot.Core.Services
{
    public static class LoggingService
    {
        private static readonly string _messageBaseTemplate = "{0} - {1} | Source: {2} - {3}";
        private static readonly string _messageFatalErrorTemplate = _messageBaseTemplate + "| Exception: {4}";

        public static void Log(object source, LogLevel logLevel, string message)
        {
            Console.ForegroundColor = PickForegroundColor(logLevel);

            Console.WriteLine(string.Format(_messageBaseTemplate, DateTime.Now, logLevel, source, message));
        }

        public static void LogCritical(object source, string message, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(string.Format(_messageFatalErrorTemplate, DateTime.Now, LogLevel.Critical, source, message, ex.Message));
        }

        public static Task OnLavaLog(LogMessage arg)
        {
            Log(arg.Source, ParseSeverityToLogLevel(arg.Severity), arg.Message);

            return Task.CompletedTask;
        }

        static ConsoleColor PickForegroundColor(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Information => ConsoleColor.Blue,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.DarkRed,
                LogLevel.Critical => ConsoleColor.Red,
                _ => ConsoleColor.Black,
            };
        }

        static LogLevel ParseSeverityToLogLevel(LogSeverity logSeverity)
        {
            return logSeverity switch
            {
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Critical => LogLevel.Critical,
                _ => LogLevel.Debug,
            };
        }
    }
}
