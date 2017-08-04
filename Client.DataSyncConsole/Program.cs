using Ionic.Zip;
using MasterUtility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Client.DataSyncConsole
{
    public class Program
    {
        static string const_local_path = @"..\Data";
        static string const_ftp_path = @"ftp://ftp.mims.com/AU_Source_Data/AREVDailyData/";
        static string const_ftp_username = "Australia-au";
        static string const_ftp_password = "Aust0124";
        static string const_db_name = "AU_ParallelSupport";
        
        
        static void Main(string[] args)
        {
            //Static parameters
            //// 1. FTP filename
            //// 2. Local location 
            //// 3. List of Prodcode 

            string connectionString = Database.GetServerConnectionString(const_db_name);

            #region Setup -- Stable No Change
            string file = Directory.GetFiles(const_local_path, "*.txt").FirstOrDefault();

            FileInfo fi = new FileInfo(file);

            if (!File.Exists(file) || !fi.Name.Contains("AREVData"))
            {
                Console.WriteLine("MISSING DATA FILE in Data Folder OR WRONG FILE FORMAT !!!");
                Console.WriteLine("Data file should be name as AREVDATA_YYYMMDD and contains list of Prodcodes");

                Console.ReadKey();

                return;
            }

            string fileName = fi.Name.Replace(fi.Extension,"");

            string codeList = File.ReadAllText(file);


            #endregion

            #region Preparation -- Stable No Change
            string codeListString = string.Join(",",
                 codeList.Replace("\r", "").Split('\n')
                     .Where(x => !string.IsNullOrEmpty(x))
                     .Select(x => "(" + x + ")").ToArray()).ToString();
          
            #endregion

            #region Execution Step -- Stable No Change

            string localFile = Step1_DownloadFTP(fileName);

            string accessFile = Step2_Unzip(localFile);

            Step3_ImportToServer(accessFile, connectionString);

            Step4_ExecuteScript(codeListString, connectionString);

            #endregion

            Console.ReadKey();
        }

        private static string Step1_DownloadFTP(string fileName)
        {
            Print(" Step 1 - Start.....");

            string ftpFilePath = Path.Combine(const_ftp_path, fileName + ".zip");
            string localFilePath = Path.Combine(const_local_path, fileName + ".zip");

            FTP.DownloadFile(ftpFilePath, const_ftp_username, const_ftp_password, localFilePath);

            Console.WriteLine(".... Downloading FTP file has been successful.");
            Console.WriteLine(".... File is downloaded to " + localFilePath);


            Print(" Step 1 - End!");
            return localFilePath;
        }

        private static string Step2_Unzip(string localFile)
        {
            Print(" Step 2 - Start....");

            string accessFile = "";

            using (ZipFile zip = ZipFile.Read(localFile))
            {
                foreach (ZipEntry e in zip)
                {
                    e.Extract(const_local_path, ExtractExistingFileAction.OverwriteSilently);

                    accessFile = Path.Combine(const_local_path, e.FileName);
                }
            }

            
            Console.WriteLine(".... Unzip file has been successful.");
            Console.WriteLine(".... Filed is unzipped to " + accessFile);

            Print(" Step 2 -- End!");

            return accessFile;
        }

        private static void Step3_ImportToServer(string accessFile, string connectionString)
        {
            Print(" Step 3 - Start....");

            string[] tableNameLists =  { "arevMBM", "arevFORMULATION", "arevPACK"};
            
            foreach (string tableName in tableNameLists)
            {
                Database.ExecuteScript(string.Format("delete from {0}", tableName), connectionString);

                Access.BulkCopy(connectionString, accessFile, tableName);
            }

            Console.WriteLine(".... Bulk copying access to server has been successful.");

            Print(" Step 3 - End!");
        }

        private static void Step4_ExecuteScript(string codeListString, string connectionString)
        {
            Print(" Step 4 - Start....");

            #region Script
            string sql = string.Format(@"
-- Setup IDs
declare @TEMP table (Code int)
insert into @TEMP
VALUES 
{0}

DELETE FROM AU_ParallelSupport.dbo.arevMBM_backup
DELETE FROM AU_ParallelSupport.dbo.arevFORMULATION_backup

-- 1. Backup 
INSERT INTO AU_ParallelSupport.dbo.arevMBM_backup
SELECT * FROM AU_Pharma.dbo.arevMBM WHERE Code IN ( SELECT * FROM @TEMP )

INSERT INTO AU_ParallelSupport.dbo.arevFORMULATION_backup
SELECT *  FROM AU_Pharma.dbo.arevFORMULATION WHERE PROD_CODE IN ( SELECT * FROM @TEMP )

-- 2. Delete

DELETE FROM AU_Pharma.dbo.arevMBM WHERE CODE IN ( SELECT * FROM @TEMP )


DELETE FROM AU_Pharma.dbo.arevFORMULATION WHERE PROD_CODE IN ( SELECT * FROM @TEMP )

-- 3. Update
INSERT INTO AU_Pharma.dbo.arevMBM
SELECT * FROM AU_ParallelSupport.dbo.arevMBM a WHERE a.code IN ( SELECT * FROM @TEMP )

INSERT INTO AU_Pharma.dbo.arevFORMULATION
SELECT * FROM AU_ParallelSupport.dbo.arevFORMULATION a WHERE a.PROD_CODE IN ( SELECT * FROM @TEMP )", codeListString)
;
            #endregion

            Database.ExecuteScript(sql, connectionString);

            Console.WriteLine(".... Copy data to Live AU PHarma has been successful.");

            Print(" Step 4 - End!");
        }

        private static void Print(string text)
        {
            if (text.Contains("Start"))
            {                
                Console.WriteLine("--------------------------------------------");
            }
            Console.WriteLine(text);
            if (text.Contains("End"))
            {
                Console.WriteLine("--------------------------------------------");                
            }
        }
    }
}
