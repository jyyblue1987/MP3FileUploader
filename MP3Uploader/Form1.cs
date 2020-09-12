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
using Microsoft.Win32;

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

            
        }

        private void getFTPSetting()
        {
            try
            {
                RegistryKey read = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
                object currentValue = read.GetValue("FTPSeting");

                string val = "";
                if (currentValue != null)
                    val = currentValue.ToString();

                String[] value_list = val.Split('|');
                if (value_list.Length > 0)
                    txtIP.Text = value_list[0];

                if (value_list.Length > 1)
                    txtUsername.Text = value_list[2];

                if (value_list.Length > 2)
                    txtPassword.Text = value_list[3];

                //if (currentValue == null || String.Compare(val, Application.ExecutablePath, true) != 0)
                //{
                //    RegistryKey add = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                //    add.SetValue("ScreenLog", Application.ExecutablePath);
                //}
                //else
                //    MessageBox.Show("You are welcome");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please run as administrator");
                //return;
            }

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
