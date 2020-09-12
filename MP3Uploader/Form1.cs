using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace MP3Uploader
{
    public partial class Form1 : Form
    {
        private const String MY_SERVICE = "MP3UploaderService";
        public Form1()
        {
            InitializeComponent();       
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            showServiceState();
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            if (ServiceInstaller.ServiceIsInstalled(MY_SERVICE))
            {
                ServiceInstaller.StopService(MY_SERVICE);
                ServiceInstaller.Uninstall(MY_SERVICE);               
            }
            else
            {
                string exe_path = Application.ExecutablePath;            
                String dir_path = Path.GetDirectoryName(exe_path);
                String service_path = dir_path + "/" + "MP3UploaderService.exe";
                ServiceInstaller.InstallAndStart(MY_SERVICE, MY_SERVICE, service_path);                
            }
            showServiceState();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            ServiceState state = ServiceInstaller.GetServiceStatus(MY_SERVICE);
            if (state == ServiceState.Running)
            {
                ServiceInstaller.StopService(MY_SERVICE);                
            }
            else
            {
                ServiceInstaller.StartService(MY_SERVICE);                
            }

            showServiceState();
        }

        private void showServiceState()
        {
            ServiceState state = ServiceInstaller.GetServiceStatus(MY_SERVICE);
            txtState.Text = state.ToString();

            if (ServiceInstaller.ServiceIsInstalled(MY_SERVICE))
                btnInstall.Text = "Uninstall";
            else
                btnInstall.Text = "Install";

            if (state == ServiceState.Running)
                btnStart.Text = "Stop";
            else
                btnStart.Text = "Start";
        }

    }
}
