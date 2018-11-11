using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TestCommon
{
  #region Timer

  public class Timer : IDisposable
  {
    private Stopwatch Stopwatch { get; }

    private Action<TimeSpan> OnComplete { get; }

    public Timer(Action<TimeSpan> onComplete)
    {
      Stopwatch = Stopwatch.StartNew();
      OnComplete = onComplete;
    }

    public void Dispose()
    {
      Stopwatch.Stop();
      OnComplete(Stopwatch.Elapsed);
    }
  }

  #endregion

  #region Logger

  public enum LogLevel {
    Error = 0,
    Warning = 1,
    Info = 2,
    Debug = 3
  };

  public interface ILogger
  {
    void Log(string message, LogLevel? level = null);
    Timer Measure(Action<TimeSpan> onComplete);
  }

  public abstract class CallbackLogger : ILogger
  {
    private Action<string> Output { get; }
    private LogLevel DefaultLogLevel { get; }

    public CallbackLogger(Action<string> output, LogLevel? defaultLogLevel = null)
    {
      Output = output;
      DefaultLogLevel = defaultLogLevel ?? LogLevel.Error;
    }

    public void Log(string message, LogLevel? customLevel = null)
    {
      var level = customLevel ?? DefaultLogLevel;

      if (ShouldLogMessage(level))
      {
        var levelStr = level.ToString("G").ToLowerInvariant();
        var nowStr = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
        Output($"[{levelStr}][{nowStr}]: {message}");
      }
    }

    public Timer Measure(Action<TimeSpan> onComplete) => new Timer(onComplete);

    private bool ShouldLogMessage(LogLevel level) => DefaultLogLevel >= level;
  }

  #endregion

  #region Logger Implementation

  public class ConsoleLogger : CallbackLogger
  {
    public ConsoleLogger(LogLevel? defaultLogLevel = null) : base(Console.WriteLine, defaultLogLevel)
    {
    }
  }

  #endregion

}
