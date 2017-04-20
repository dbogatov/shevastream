using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shevastream.Services;
using Shevastream.ViewModels;

namespace Shevastream.Extensions
{
	public static class LoggerExtensions
	{
		/// <summary>
		/// Helper method to add ShevastreamLogger as a logging provider.
		/// </summary>
		/// <param name="factory">The factory to which to add a provider</param>
		/// <param name="context">Data context which will be used by ShevastreamLogger</param>
		/// <param name="filter">Filter function that decides which messages to log</param>
		/// <returns>Factory object (chaining)</returns>
		public static ILoggerFactory AddShevastreamLogger(
			this ILoggerFactory factory,
			IPushService pushService,
			Func<string, LogLevel, bool> filter = null)
		{
			factory.AddProvider(new ShevastreamLoggerProvider(filter, pushService));
			return factory;
		}

		/// <summary>
		/// Helper method to add ShevastreamLogger as a logging provider.
		/// </summary>
		/// <param name="factory">The factory to which to add a provider</param>
		/// <param name="context">Data context which will be used by ShevastreamLogger</param>
		/// <param name="minLevel">Minimal log level to print (eq. INFO)</param>
		/// <param name="exclusions">An array of strings for which if log category starts with it, 
		/// it should not be logged.</param>
		/// <returns>Factory object (chaining)</returns>
		public static ILoggerFactory AddShevastreamLogger(
			this ILoggerFactory factory,
			IPushService pushService,
			LogLevel minLevel,
			string[] exclusions = null)
		{
			Func<string, LogLevel, bool> func;

			if (exclusions != null)
			{
				func = (category, logLevel) =>
					logLevel >= minLevel &&
					!exclusions.Any((str) => category.StartsWith(str));
			}
			else
			{
				func = (category, logLevel) => logLevel >= minLevel;
			}

			return AddShevastreamLogger(factory, pushService, func);
		}
	}

	/// <summary>
	/// My custom logger provider.
	/// Mostly scaffolded code to fit the framework.
	/// </summary>
	public class ShevastreamLoggerProvider : ILoggerProvider
	{
		private readonly Func<string, LogLevel, bool> _filter;
		private readonly IPushService _pushService;

		public ShevastreamLoggerProvider(Func<string, LogLevel, bool> filter, IPushService pushService)
		{
			_pushService = pushService;
			_filter = filter;
		}

		public ILogger CreateLogger(string categoryName)
		{
			return new ShevastreamLogger(categoryName, _filter, _pushService);
		}

		public void Dispose() { }
	}

	/// <summary>
	/// Custom implementation of ILooger.
	/// </summary>
	public class ShevastreamLogger : ILogger
	{
		private static Object _lockKey = new Object();

		private string _categoryName;
		private Func<string, LogLevel, bool> _filter;
		private IPushService _pushService;

		public ShevastreamLogger(string categoryName, Func<string, LogLevel, bool> filter, IPushService pushService)
		{
			_categoryName = categoryName;
			_filter = filter;
			_pushService = pushService;
		}

		/// <summary>
		/// Returns a status of the logger depending on the log level (severity).
		/// </summary>
		public bool IsEnabled(LogLevel logLevel)
		{
			return (_filter == null || _filter(_categoryName, logLevel));
		}

		/// <summary>
		/// Main method in the ILogger interface that defines how to print a message.
		/// </summary>
		/// <param name="logLevel">Log severity</param>
		/// <param name="eventId">Event category (no used in this project)</param>
		/// <param name="state">Actual message wrapped</param>
		/// <param name="exception">Associated exception</param>
		/// <param name="formatter">Function that defines how to format a message.</param>
		public void Log<TState>(
			LogLevel logLevel,
			EventId eventId,
			TState state,
			Exception exception,
			Func<TState, Exception, string> formatter
		)
		{
			/// <summary>
			/// Return immediately is logging is not enabled for this log level
			/// </summary>
			if (!IsEnabled(logLevel))
			{
				return;
			}

			// Exception has to be set (by framework)
			if (formatter == null)
			{
				throw new ArgumentNullException(nameof(formatter));
			}

			// Generate actual message
			var message = formatter(state, exception);

			// Return for empty message
			if (string.IsNullOrEmpty(message))
			{
				return;
			}

			if (logLevel >= LogLevel.Error)
			{
				_pushService.SendLogAsync(new LogMessageViewModel
				{
					Severity = logLevel,
					Source = "status-site",
					Message = message,
					AuxiliaryData = JsonConvert.SerializeObject(new
					{
						Exception = exception.ToString()
					}),
					Category = eventId.Id
				});
			}

			// Define colors for thread identifiers
			var threadColors = new ConsoleColor[] {
				ConsoleColor.Cyan,
				ConsoleColor.DarkCyan,
				ConsoleColor.Blue,
				ConsoleColor.DarkBlue,
				ConsoleColor.DarkGreen,
				ConsoleColor.DarkMagenta,
				ConsoleColor.DarkYellow,
				ConsoleColor.DarkRed
			};

			// Generate thread label text
			var thread = Thread.CurrentThread;
			var threadLabel =
				string.IsNullOrEmpty(thread.Name) ?
				$"ThreadID: {thread.ManagedThreadId.ToString().PadLeft(3)}" :
				thread.Name.Substring(0, 12);

			// Generate a thread label color
			var threadColor = threadColors[Math.Abs(threadLabel.GetHashCode()) % threadColors.Length];

			var label = logLevel.GetLabel();

			// Do not let multiple threads enter the actual print section
			lock (ShevastreamLogger._lockKey)
			{
				// Print everything (watch for pads and new lines)
				ColoredConsole.Write($"{threadLabel}", threadColor);

				Console.Write("".PadLeft(6 - label.Text.Length));

				ColoredConsole.Write($"{label.Text}", label.ForegroundColor, label.BackgroundColor);

				Console.Write(" | ");

				Console.Write($"[{eventId.ToString().PadRight(2)}] {_categoryName}");

				Console.WriteLine();

				Console.Write($"[{DateTime.Now.ToString("MM/dd/yy HH:mm:ss")}]");

				Console.Write(" | ");

				if (exception != null)
				{
					message += Environment.NewLine + Environment.NewLine + exception.ToString();
				}

				Console.WriteLine(message);

				Console.WriteLine();
			}
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return null;
		}
	}

	/// <summary>
	/// Type of events that the system logs
	/// </summary>
	public enum LoggingEvents
	{
		Startup, Order, Feedback, Blog, ActionFilters
	}
}
