using StrikeforceInfinity.Server.Items;
using StrikeforceInfinity.Shared;
using StrikeforceInfinity.Shared.Exceptions;
using System.Text;
using static StrikeforceInfinity.Shared.SiConstants;

namespace StrikeforceInfinity.Server.Engine
{
    /// <summary>
    /// Public core class methods for locking, reading, writing and managing tasks related to logging.
    /// </summary>
    public class LogManager
    {
        private readonly ServerCore _core;
        private StreamWriter? _fileHandle = null;
        private DateTime _recycledTime = DateTime.MinValue;

        public LogManager(ServerCore core)
        {
            _core = core;
            CycleLog();
        }

        public void Write(string message) => Write(new LogEntry(message) { Severity = NsLogSeverity.Verbose });
        public void Trace(string message) => Write(new LogEntry(message) { Severity = NsLogSeverity.Trace });
        public void Write(string message, Exception ex) => Write(new LogEntry(message) { Exception = ex, Severity = NsLogSeverity.Exception });
        public void Write(string message, NsLogSeverity severity) => Write(new LogEntry(message) { Severity = severity });

        public void Start()
        {
            CycleLog();
        }

        public void Stop()
        {
            Close();
        }

        public void Checkpoint()
        {
            lock (this)
            {
                if (_fileHandle != null)
                {
                    try
                    {
                        _fileHandle.Flush();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Critical log exception. Failed to checkpoint log: {ex.Message}.");
                        throw;
                    }
                }
            }
        }

        public void Write(LogEntry entry)
        {
            try
            {
                if (entry.Severity == NsLogSeverity.Trace && _core.Settings.WriteTraceData == false)
                {
                    return;
                }

                if (entry.Exception != null)
                {
                    if (typeof(SiExceptionBase).IsAssignableFrom(entry.Exception.GetType()))
                    {
                        entry.Severity = ((SiExceptionBase)entry.Exception).Severity;
                    }
                }

                lock (this)
                {
                    CycleLog();

                    StringBuilder message = new StringBuilder();

                    message.AppendFormat("{0}|{1}|{2}", entry.DateTime.ToShortDateString(), entry.DateTime.ToShortTimeString(), entry.Severity);

                    if (entry.Message != null && entry.Message != string.Empty)
                    {
                        message.Append("|");
                        message.Append(entry.Message);
                    }

                    if (entry.Exception != null)
                    {
                        if (typeof(SiExceptionBase).IsAssignableFrom(entry.Exception.GetType()))
                        {
                            if (entry.Exception.Message != null && entry.Exception.Message != string.Empty)
                            {
                                message.AppendFormat("|Exception: {0}: ", entry.Exception.GetType().Name);
                                message.Append(entry.Exception.Message);
                            }
                        }
                        else
                        {
                            if (entry.Exception.Message != null && entry.Exception.Message != string.Empty)
                            {
                                message.Append("|Exception: ");
                                message.Append(GetExceptionText(entry.Exception));
                            }

                            if (entry.Exception.StackTrace != null && entry.Exception.StackTrace != string.Empty)
                            {
                                message.Append("|Stack: ");
                                message.Append(entry.Exception.StackTrace);
                            }
                        }
                    }

                    if (entry.Severity == NsLogSeverity.Warning)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    else if (entry.Severity == NsLogSeverity.Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (entry.Severity == NsLogSeverity.Verbose)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }

                    Console.WriteLine(message.ToString());

                    Console.ForegroundColor = ConsoleColor.Gray;

                    SiUtility.EnsureNotNull(_fileHandle);

                    _fileHandle.WriteLine(message.ToString());

                    if (_core.Settings.FlushLog)
                    {
                        _fileHandle.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical log exception. Failed write log entry: {ex.Message}.");
                throw;
            }
        }

        private string GetExceptionText(Exception excpetion)
        {
            try
            {
                var message = new StringBuilder();
                return GetExceptionText(excpetion, 0, ref message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical log exception. Failed to get exception text: {ex.Message}.");
                throw;
            }
        }

        private string GetExceptionText(Exception exception, int level, ref StringBuilder message)
        {
            try
            {
                if (exception.Message != null && exception.Message != string.Empty)
                {
                    message.AppendFormat("{0} {1}", level, exception.Message);
                }

                if (exception.InnerException != null && level < 100)
                {
                    return GetExceptionText(exception.InnerException, level + 1, ref message);
                }

                return message.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical log exception. Failed to get exception text: {ex.Message}.");
                throw;
            }
        }

        private void CycleLog()
        {
            try
            {
                lock (this)
                {
                    if (_recycledTime.Date != DateTime.Now)
                    {
                        Close();

                        _recycledTime = DateTime.Now;
                        string fileName = _core.Settings.LogDirectory + "\\" + $"{_recycledTime.Year}_{_recycledTime.Month:00}_{_recycledTime.Day:00}.txt";
                        Directory.CreateDirectory(_core.Settings.LogDirectory);
                        _fileHandle = new StreamWriter(fileName, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical log exception. Failed to cycle log file: {ex.Message}.");
                throw;
            }
        }

        public void Close()
        {
            try
            {
                if (_fileHandle != null)
                {
                    _fileHandle.Close();
                    _fileHandle.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical log exception. Failed to close log file: {ex.Message}.");
                throw;
            }
        }
    }
}
