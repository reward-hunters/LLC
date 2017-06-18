using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;

namespace RH.Core.Helpers
{
    public class FTPHelper
    {
        public static string Login = "i2q1d8b1";
        public static string Password = "qXzCmM9PJaJH@Ee";

        public static readonly HashSet<string> AddedAddreses = new HashSet<string>();
        public FTPHelper(string address)
        {
            Address = address;
        }

        public static void UpdateAddress(string address)
        {
            try
            {
                if (!AddedAddreses.Contains(address))
                {
                    WebRequest folderRequest = WebRequest.Create(address);
                    folderRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                    folderRequest.Credentials = new NetworkCredential(Login, Password);
                    using (var resp = (FtpWebResponse)folderRequest.GetResponse())
                        AddedAddreses.Add(address);
                }
            }
            catch           // папка может существовать. а адекватой проверки нет.
            {
                AddedAddreses.Add(address);
            }
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
                UpdateAddress(Address);

                var request = (FtpWebRequest)WebRequest.Create(Address + @"/" + fileName);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(Login, Password);

                request.UseBinary = true;

                stream.Seek(0, SeekOrigin.Begin);
                stream.Flush();

                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                //  stream.Close();

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

        private static string GetCaseSensitiveFileName(string filePath)
        {
            var dirPath = Path.GetDirectoryName(filePath).Replace(@"\", @"/").Replace(@"ftp:/1", @"ftp://1");
            var fileName = Path.GetFileName(filePath);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(dirPath);
            request.Credentials = new NetworkCredential(Login, Password);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            using (var response = (FtpWebResponse)request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                while (!reader.EndOfStream)
                {
                    var name = reader.ReadLine();
                    if (name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                        return dirPath + "/" + name;
                }
            }
            return string.Empty;
        }


        public static Bitmap DownloadImage(string filePath)
        {
            if (filePath.StartsWith(@"ftp:/1"))
                filePath = filePath.Replace(@"ftp:/1", @"ftp://1");

            var dirPath = GetCaseSensitiveFileName(filePath);
            if (string.IsNullOrEmpty(dirPath))
                return null;

            var request = (FtpWebRequest)FtpWebRequest.Create(dirPath);
            request.Credentials = new NetworkCredential(Login, Password);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            var ftpResponse = (FtpWebResponse)request.GetResponse();

            byte[] buffer = new byte[16 * 1024];
            Bitmap result;
            using (var ftpStream = ftpResponse.GetResponseStream())
            {
                var ms = new MemoryStream();

                int read;
                while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                ms.Seek(0, SeekOrigin.Begin);
                result = (Bitmap)Image.FromStream(ms);
            }
            return result;
        }

        public static void CopyFromFtpToFtp(string oldFilePath, string newFilePathDir, string newFileName, ZipOutputStream zipStream, string zipFileName)
        {
            UpdateAddress(newFilePathDir);
            if (oldFilePath.StartsWith(@"ftp:/1"))
                oldFilePath = oldFilePath.Replace(@"ftp:/1", @"ftp://1");
            oldFilePath = GetCaseSensitiveFileName(oldFilePath);

            var request = (FtpWebRequest)FtpWebRequest.Create(oldFilePath);
            request.Credentials = new NetworkCredential(Login, Password);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            var ftpResponse = (FtpWebResponse)request.GetResponse();

            byte[] buffer = new byte[16 * 1024];
            using (var ftpStream = ftpResponse.GetResponseStream())
            {
                var ms = new MemoryStream();

                int read;
                while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                ms.Seek(0, SeekOrigin.Begin);

                var ftpHelper = new FTPHelper(newFilePathDir);
                ftpHelper.Upload(ms, newFileName);

                if (zipStream != null)          // если нужно запаковать тоже
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var newEntry = new ZipEntry(zipFileName);
                    newEntry.DateTime = DateTime.Now;
                    zipStream.PutNextEntry(newEntry);
                    ms.CopyTo(zipStream);
                    zipStream.CloseEntry();
                }
            }
        }


        public static bool IsFileExists(string path)
        {
            var request = (FtpWebRequest)WebRequest.Create(path);
            request.Credentials = new NetworkCredential(Login, Password);
            request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
            request.UseBinary = true;

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
    }
}
