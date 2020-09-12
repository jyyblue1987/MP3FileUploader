using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MP3Uploader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            testFTPUpload();
            FileUploaderThread obj = new FileUploaderThread();
            Thread thr = new Thread(new ThreadStart(obj.run));
            thr.Start();
        }

        private void testFTPUpload()
        {


        }
    }
}
