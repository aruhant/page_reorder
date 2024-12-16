using System.Windows.Forms;

namespace Scanned_Page_Sorter
{
    using System;
    using System.Drawing;
    using System.Threading;

    public class Debouncer : IDisposable
    {
        private Thread _thread;
        private volatile Action _action;
        private volatile int _delay = 0;

        public void Debounce(Action action, int delay = 1250)
        {
            this._action = action;
            this._delay = delay;

            if (this._thread == null)
            {
                this._thread = new Thread(() => this.RunThread());
                this._thread.IsBackground = true;
                this._thread.Start();
            }
        }

        private void RunThread()
        {
            while (true)
            {
                int d = this._delay;
                this._delay = 0;
                Thread.Sleep(d);
                if (this._delay == 0 && this._action != null)
                {
                    this._action();
                    this._action = null;
                }
            }
        }

        public void Dispose()
        {
            if (this._thread != null)
            {
                this._thread.Abort();
                this._thread = null;
            }
        }
    }
    
}
