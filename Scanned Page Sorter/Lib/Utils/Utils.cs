namespace Scanned_Page_Sorter
{
    using System;
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
    }

}
