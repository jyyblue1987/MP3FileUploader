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
            getFTPSetting();
            
        }

        private void getFTPSetting()
        {
            try
            {
                RegistryKey read = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
                object currentValue = read.GetValue("FTPSetting");

                string val = "";
                if (currentValue != null)
                    val = currentValue.ToString();

                String[] value_list = val.Split('|');
                if (value_list.Length > 0)
                    txtIP.Text = value_list[0];

                if (value_list.Length > 1)
                    txtUsername.Text = value_list[1];

                if (value_list.Length > 2)
                    txtPassword.Text = value_list[2];
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                String value = txtIP.Text + "|" + txtUsername.Text + "|" + txtPassword.Text;
                RegistryKey add = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                add.SetValue("FTPSetting", value);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please run as administrator");
                //return;
            }
        }

        FileUploaderThread obj;
        private void btnTest_Click(object sender, EventArgs e)
        {
            //ftp ftpClient = new ftp(@"ftp://" + txtIP.Text + "/", txtUsername.Text, txtPassword.Text);
            //try
            //{
            //    Boolean flag = ftpClient.isConnected();
            //    if( flag == true )
            //        MessageBox.Show("Connection is succesed");
            //    else
            //        MessageBox.Show("Connection is failed");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //ftpClient = null;
            obj = new FileUploaderThread();
            Thread thr = new Thread(new ThreadStart(obj.run));
            thr.Start();

        }

    }
}
