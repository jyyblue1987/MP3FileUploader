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
        private const String MY_SERVICE = "window99";
        public Form1()
        {
            InitializeComponent();       
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getFTPSetting();
            showServiceState();            
        }

        private void getFTPSetting()
        {
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader("config.txt");
                //Read the first line of text
                String val = sr.ReadLine();

                String[] value_list = val.Split('|');
                if (value_list.Length > 0)
                    txtIP.Text = value_list[0];

                if (value_list.Length > 1)
                    txtUsername.Text = value_list[1];

                if (value_list.Length > 2)
                    txtPassword.Text = value_list[2];
               
                //close the file
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }


        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            try
            {
                File.Copy("config.txt", "C:\\config.txt", true);
            }
            catch (Exception ex)
            {

            }

            if (ServiceInstaller.ServiceIsInstalled(MY_SERVICE))
            {
                ServiceInstaller.StopService(MY_SERVICE);
                ServiceInstaller.Uninstall(MY_SERVICE);               
            }
            else
            {
                string exe_path = Application.ExecutablePath;            
                String dir_path = Path.GetDirectoryName(exe_path);
                String service_path = dir_path + "/" + "window99.exe";
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
            String value = txtIP.Text + "|" + txtUsername.Text + "|" + txtPassword.Text;
            String app_data_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);                
                
            try
            {
                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter("config.txt");
                sw.WriteLine(txtIP.Text + "|" + txtUsername.Text + "|" + txtPassword.Text);
                sw.WriteLine(app_data_path + "\\R");
                //Close the file
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }

            MessageBox.Show(app_data_path);         
        }

        FileUploaderThread obj;
        private void btnTest_Click(object sender, EventArgs e)
        {
            ftp ftpClient = new ftp(@"ftp://" + txtIP.Text + "/", txtUsername.Text, txtPassword.Text);
            try
            {
                Boolean flag = ftpClient.isConnected();
                if (flag == true)
                    MessageBox.Show("Connection is succesed");
                else
                    MessageBox.Show("Connection is failed");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            ftpClient = null;

            //obj = new FileUploaderThread();
            //Thread thr = new Thread(new ThreadStart(obj.run));
            //thr.Start();

        }

    }
}
