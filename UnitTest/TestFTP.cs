using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MasterUtility;

namespace UnitTest
{
    [TestClass]
    public class TestFTP
    {
        [TestMethod]
        public void DownloadFile()
        {
            string ftpLocation = @"ftp://ftp.mims.com/AU_Source_Data/AREVDailyData/AREVData_20170206.zip";
            string username = "Australia-au";
            string password = "Aust0124";
            string destination = @"C:\Users\mike.nguyen\Desktop\Current Work\AU Parallel Support\AREVData_20170206_Test.zip";

            FTP.DownloadFile(ftpLocation, username, password, destination);

            Assert.IsTrue(File.Exists(destination));
        }

        [TestMethod]
        public void UploadFile()
        {
            string ftpLocation = @"ftp://ftp.mims.com/Data%20Production/NZF/AREVData_20170206_Test.zip";
            string username = "Australia-au";
            string password = "Aust0124";
            string destination = @"C:\Users\mike.nguyen\Desktop\Current Work\AU Parallel Support\AREVData_20170206_Test.zip";

            bool check = FTP.UploadFile(ftpLocation, username, password, destination);

            Assert.IsTrue(check);
        }
    }
}
