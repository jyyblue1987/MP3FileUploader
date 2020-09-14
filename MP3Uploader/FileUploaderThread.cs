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
            String record_path = "";
            String ip = "", username = "", password = "";
            try
            {
                StreamReader sr = new StreamReader("C:\\config.txt");

                String val = sr.ReadLine();
                
                String[] value_list = val.Split('|');
                if (value_list.Length > 0)
                    ip = value_list[0];

                if (value_list.Length > 1)
                    username = value_list[1];

                if (value_list.Length > 2)
                    password = value_list[2];

                record_path = sr.ReadLine();

                //close the file
                sr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ftpClient = new ftp(@"ftp://" + ip + "/", username, password);

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

                        File.Delete(file);
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
