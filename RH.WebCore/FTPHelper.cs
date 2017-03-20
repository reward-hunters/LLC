using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace RH.WebCore
{
    public class FTPHelper
    {

        private readonly HashSet<string> AddedAddreses = new HashSet<string>();
        public FTPHelper(string address, string login, string password)
        {
            Address = address;
            Login = login;
            Password = password;
        }

        public void Upload(string filePath, string newfileName)
        {
            using (MemoryStream ms = new MemoryStream())
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
                ms.Write(bytes, 0, (int)file.Length);
                ms.Flush();

                Upload(ms, newfileName);
            }
        }
        public void Upload(MemoryStream stream, string fileName)
        {
            try
            {
                try
                {
                    if (!AddedAddreses.Contains(Address))
                    {
                        WebRequest folderRequest = WebRequest.Create(Address);
                        folderRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                        folderRequest.Credentials = new NetworkCredential(Login, Password);
                        using (var resp = (FtpWebResponse) folderRequest.GetResponse())
                            AddedAddreses.Add(Address);
                    }
                }
                catch           // папка может существовать. а адекватой проверки нет.
                {

                }

                var request = (FtpWebRequest)WebRequest.Create(Address + @"/" + fileName);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(Login, Password);

                request.UseBinary = true;

                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Close();

                var requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();

                var response = (FtpWebResponse)request.GetResponse();
                Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
                response.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool IsFileExists(string path)
        {
            var request = (FtpWebRequest)WebRequest.Create
                (path);
            request.Credentials = new NetworkCredential("i2q1d8b1", "B45B2nnFv$!j6V");
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                return false;
            }
            return true;
        }

        public string Address { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
