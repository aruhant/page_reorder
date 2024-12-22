using System;
using System.Diagnostics;
using System.Text;

namespace Scanned_Page_Sorter
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;

    public class Debouncer : IDisposable
    {
        private Thread _thread;
        private volatile Action _action;
        private volatile int _delay = 0;

        public void Debounce(Action action, int delay = 1250)
        {
            _action = action;
            _delay = delay;

            if (_thread == null)
            {
                _thread = new Thread(() => RunThread());
                _thread.IsBackground = true;
                _thread.Start();
            }
        }

        private void RunThread()
        {
            while (true)
            {
                int d = _delay;
                _delay = 0;
                Thread.Sleep(d);
                if (_delay == 0 && _action != null)
                {
                    _action();
                    _action = null;
                }
            }
        }

        public void Dispose()
        {
            if (_thread != null)
            {
                _thread.Abort();
                _thread = null;
            }
        }
    };


    }

public class Utils { 
    
public static string RunExternalExe(string filename, string arguments = "")
        {
            var process = new Process();

            process.StartInfo.FileName = filename;
            if (!string.IsNullOrEmpty(arguments))
            {
                process.StartInfo.Arguments = arguments;
            }

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            var stdOutput = new StringBuilder();
            process.OutputDataReceived += (sender, args) => stdOutput.AppendLine(args.Data); // Use AppendLine rather than Append since args.Data is one line of output, not including the newline character.

            string stdError = null;
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                stdError = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine ("OS error while executing " + filename + " " + arguments + ": " + e.Message, e);
            return "";
            }

            if (process.ExitCode == 0)
            {
                return stdOutput.ToString();
            }
            else
            {
                var message = new StringBuilder();

                if (!string.IsNullOrEmpty(stdError))
                {
                    message.AppendLine(stdError);
                }

                if (stdOutput.Length != 0)
                {
                    message.AppendLine("Std output:");
                    message.AppendLine(stdOutput.ToString());
                }

                Console.WriteLine (filename + " " + arguments + " finished with exit code = " + process.ExitCode + ": " + message);
            return "";
        }
        }
    }


