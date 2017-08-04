using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MasterUtility
{
    public class FTP
    {
        public static void DownloadFile(string ftpLocation, string username, string password, string outputFile)
        {
            FtpWebRequest request = Authenticate(ftpLocation, WebRequestMethods.Ftp.DownloadFile,  username, password);

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())           
            {
                Console.WriteLine("Download Complete, status {0}", response.StatusDescription);

                if (File.Exists(outputFile))
                    File.Delete(outputFile);

                File.WriteAllBytes(outputFile, Read(responseStream));
            }           
        }

        public static bool UploadFile(string ftpLocation, string username, string password, string fileLocation)
        {
            if (!File.Exists(fileLocation)) {
                throw new Exception("File not exists");
            }

            byte[] fileContents = File.ReadAllBytes(fileLocation);

            FtpWebRequest request = Authenticate(ftpLocation, WebRequestMethods.Ftp.UploadFile, username, password);

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();
            
            using(FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {                 
                Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
            }

            return true;
        }

        #region Helper
        private static FtpWebRequest Authenticate(string ftpLocation, string method,  string username, string password)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpLocation);
            request.Method = method;
            request.Credentials = new NetworkCredential(username, password);
            return request;
        }

        private static byte[] Read(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
        #endregion
    }
}
