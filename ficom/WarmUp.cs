using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ficom
{
    public partial class WarmUp : Form
    {
        public WarmUp()
        {
            InitializeComponent();
            this.TopMost = true;     
        }

        public void RunWarmUp()
        {
            if (warmupStatusTxt.InvokeRequired)
            {
                GlobalFunc.warmupStatus = 1;
                warmupStatusTxt.Invoke(new MethodInvoker
                    (delegate
                        {
                            warmupStatusTxt.Text = GlobalFunc.rm.GetString("runWarmUp");
                        }
                    )
                );
            }

            #region ACTUAL DO
            for (int i = 0; i < 100; i++)
            {
                if (pb_Process.InvokeRequired)
                {
                    pb_Process.Invoke(new MethodInvoker
                        (delegate
                        {
                            pb_Process.Value += 1;
                            warmupStatusTxt.Text = GlobalFunc.rm.GetString("runWarmUp");
                        }
                    ));
                }
            }
            #endregion 

            if (pb_Process.Value == 100)
            {
                Thread.Sleep(500);
                warmupStatusTxt.Invoke(new MethodInvoker
                    (delegate
                    {
                        warmupStatusTxt.Text = GlobalFunc.rm.GetString("complete");
                    }
                    )
                );
                Thread.Sleep(500);
                GlobalFunc.warmupStatus = 0;
            }
        }

        private void WarmUp_Load(object sender, EventArgs e)
        {
            ThreadStart ts = new ThreadStart(RunWarmUp);
            Thread worker_thread = new Thread(ts);
            worker_thread.Start();
            RunWarmUp();

            while (!worker_thread.IsAlive)
            {
                Thread.Sleep(100);
            }
        }
    }
}
