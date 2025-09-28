using System;
using System.IO;
using System.Windows.Forms;

namespace Scanned_Page_Sorter.Lib.Utils
{
    /// <summary>
    /// Provides centralized logging and error handling functionality
    /// </summary>
    public static class Logger
    {
        private static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "ScannedPageSorter", 
            "application.log");

        static Logger()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
            }
            catch
            {
                // Fallback to console logging only if directory creation fails
            }
        }

        /// <summary>
        /// Logs an informational message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="source">Optional source of the message</param>
        public static void LogInfo(string message, string source = null)
        {
            LogMessage("INFO", message, source);
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="source">Optional source of the message</param>
        public static void LogWarning(string message, string source = null)
        {
            LogMessage("WARN", message, source);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="source">Optional source of the message</param>
        public static void LogError(string message, string source = null)
        {
            LogMessage("ERROR", message, source);
        }

        /// <summary>
        /// Logs an exception with full details
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="source">Optional source where the exception occurred</param>
        public static void LogException(Exception ex, string source = null)
        {
            var message = $"{ex.Message}\nStack Trace: {ex.StackTrace}";
            if (ex.InnerException != null)
            {
                message += $"\nInner Exception: {ex.InnerException.Message}";
            }
            LogMessage("ERROR", message, source);
        }

        /// <summary>
        /// Shows an error message to the user and logs it
        /// </summary>
        /// <param name="userMessage">User-friendly message to display</param>
        /// <param name="technicalDetails">Technical details to log</param>
        /// <param name="source">Optional source of the error</param>
        public static void ShowErrorToUser(string userMessage, string technicalDetails = null, string source = null)
        {
            // Log the technical details
            if (!string.IsNullOrEmpty(technicalDetails))
            {
                LogError(technicalDetails, source);
            }
            else
            {
                LogError(userMessage, source);
            }

            // Show user-friendly message
            MessageBox.Show(userMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows a warning message to the user and logs it
        /// </summary>
        /// <param name="message">The warning message</param>
        /// <param name="source">Optional source of the warning</param>
        public static void ShowWarningToUser(string message, string source = null)
        {
            LogWarning(message, source);
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Core logging method
        /// </summary>
        /// <param name="level">Log level (INFO, WARN, ERROR)</param>
        /// <param name="message">The message to log</param>
        /// <param name="source">Optional source of the message</param>
        private static void LogMessage(string level, string message, string source)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var sourceInfo = string.IsNullOrEmpty(source) ? "" : $" [{source}]";
            var logEntry = $"{timestamp} [{level}]{sourceInfo}: {message}";

            // Always log to console
            Console.WriteLine(logEntry);

            // Try to log to file
            try
            {
                using (var writer = new StreamWriter(LogFilePath, true))
                {
                    writer.WriteLine(logEntry);
                }
            }
            catch
            {
                // Silently fail if file logging doesn't work
                // Console logging will still occur
            }
        }

        /// <summary>
        /// Clears the log file
        /// </summary>
        public static void ClearLog()
        {
            try
            {
                if (File.Exists(LogFilePath))
                {
                    File.Delete(LogFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to clear log file: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the path to the current log file
        /// </summary>
        /// <returns>Path to log file</returns>
        public static string GetLogFilePath()
        {
            return LogFilePath;
        }

        /// <summary>
        /// Safely executes an action and logs any exceptions
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="source">Source identifier for logging</param>
        /// <param name="userErrorMessage">User-friendly error message if action fails</param>
        /// <returns>True if action succeeded, false if it threw an exception</returns>
        public static bool SafeExecute(Action action, string source = null, string userErrorMessage = null)
        {
            try
            {
                action?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, source);
                
                if (!string.IsNullOrEmpty(userErrorMessage))
                {
                    ShowErrorToUser(userErrorMessage, ex.Message, source);
                }
                
                return false;
            }
        }

        /// <summary>
        /// Safely executes a function and logs any exceptions
        /// </summary>
        /// <typeparam name="T">Return type of the function</typeparam>
        /// <param name="func">The function to execute</param>
        /// <param name="defaultValue">Default value to return on exception</param>
        /// <param name="source">Source identifier for logging</param>
        /// <param name="userErrorMessage">User-friendly error message if function fails</param>
        /// <returns>Function result or default value on exception</returns>
        public static T SafeExecute<T>(Func<T> func, T defaultValue = default(T), string source = null, string userErrorMessage = null)
        {
            try
            {
                return func != null ? func() : defaultValue;
            }
            catch (Exception ex)
            {
                LogException(ex, source);
                
                if (!string.IsNullOrEmpty(userErrorMessage))
                {
                    ShowErrorToUser(userErrorMessage, ex.Message, source);
                }
                
                return defaultValue;
            }
        }
    }
}