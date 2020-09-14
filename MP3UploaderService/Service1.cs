using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using MP3Uploader;
using System.Threading;

namespace MP3UploaderService
{
    public partial class Service1 : ServiceBase
    {
        FileUploaderThread obj;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();

            obj = new FileUploaderThread();
            Thread thr = new Thread(new ThreadStart(obj.run));
            thr.Start();
        }

        protected override void OnStop()
        {
            obj.setThreadFlag(false);
        }
    }
}
