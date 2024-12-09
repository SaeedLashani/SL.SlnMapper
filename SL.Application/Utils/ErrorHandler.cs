using System;
using System.ComponentModel.DataAnnotations;
using log4net;
using SL.Domain.Models;

namespace SL.Application.Utils
{
    /// <summary>
    /// A centralized error handler for managing and logging exceptions.
    /// </summary>
    public static class ErrorHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ErrorHandler));

        /// <summary>
        /// Handles exceptions by logging them and optionally rethrowing or wrapping them.
        /// </summary>
        /// <param name="ex">The exception to handle.</param>
        /// <param name="customMessage">An optional custom message for the log.</param>
        public static ErrorResponseMdl Handle(Exception ex, string customMessage = null, bool rethrow = true)
        {
            if (!string.IsNullOrEmpty(customMessage))
                Log.Error(customMessage, ex);
            else
                Log.Error(ex.Message, ex);

            if (rethrow)
            {
                throw ex switch
                {
                    ArgumentNullException argumentNullException =>
                        new ArgumentNullException(argumentNullException.ParamName, customMessage ?? ex.Message),

                    ArgumentException argumentException =>
                        new ArgumentException(customMessage ?? ex.Message, argumentException.ParamName, argumentException),

                    InvalidOperationException invalidOperationException =>
                        new InvalidOperationException(customMessage ?? ex.Message, invalidOperationException),

                    ValidationException validationException =>
                        new ValidationException(customMessage ?? ex.Message, validationException),

                    ApplicationException applicationException =>
                        new ApplicationException(customMessage ?? ex.Message, applicationException),

                    _ => new ApplicationException(customMessage ?? "An unexpected error occurred.", ex)
                };
            }
            else
            {
                bool isShowMessage = ex switch
                {
                    ArgumentNullException or
                    InvalidOperationException or
                    ValidationException or
                    FileNotFoundException => true,
                    _ => false,
                };

                return new ErrorResponseMdl
                {
                    IsShowMessage= isShowMessage,
                    Message = customMessage ?? ex.Message,
                    Details = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                };
            }
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public static void LogWarning(string message)
        {
            Log.Warn(message);
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The informational message to log.</param>
        public static void LogInfo(string message)
        {
            Log.Info(message);
        }

        /// <summary>
        /// Logs a critical error that should stop the application.
        /// </summary>
        /// <param name="ex">The exception to log as critical.</param>
        public static void LogCritical(Exception ex)
        {
            Log.Fatal("Critical error occurred!", ex);
        }
    }
}