using MasterUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.FileNameImport
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\mike.nguyen\Desktop\Current Work\AU Parallel Support\Validate February ASCII\AU Source\Full20170100 GIF Images\Images";

            var files = Directory.GetFiles(path);

            foreach (string f in files)
            {
                FileInfo fi = new FileInfo(f);

                string sql = string.Format("insert into ProductImage values ('{0}')", fi.Name);

                Database.ExecuteScript(sql, Database.GetLocalConnectionString("AU_ImageReview"));
            }

        }
    }
}
