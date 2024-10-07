using System.Windows.Forms;

namespace Scanned_Page_Sorter
{
    using System;
    using System.Threading;

    public class Debouncer : IDisposable
    {
        private Thread thread;
        private volatile Action action;
        private volatile int delay = 0;

        public void Debounce(Action action, int delay = 1250)
        {
            this.action = action;
            this.delay = delay;

            if (this.thread == null)
            {
                this.thread = new Thread(() => this.RunThread());
                this.thread.IsBackground = true;
                this.thread.Start();
            }
        }

        private void RunThread()
        {
            while (true)
            {
                int d = this.delay;
                this.delay = 0;
                Thread.Sleep(d);
                if (this.delay == 0 && this.action != null)
                {
                    this.action();
                    this.action = null;
                }
            }
        }

        public void Dispose()
        {
            if (this.thread != null)
            {
                this.thread.Abort();
                this.thread = null;
            }
        }
    }

    public partial class pageSorterForm : Form
    {
    }
}
