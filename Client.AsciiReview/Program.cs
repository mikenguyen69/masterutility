using MasterUtility;
using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.AsciiReview
{
    class Program
    {
        static void Main(string[] args)
        {
            //string folder = @"C:\Users\mike.nguyen\Desktop\Current Work\ASCII Structure\AU";

            //string outputFile = Path.Combine(folder, "output.txt");

            //var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);

            //StringBuilder sb = new StringBuilder();

            //foreach (string f in files)
            //{
            //    sb.AppendLine(f.Replace(folder + @"\", ""));
            //}

            //File.WriteAllText(outputFile, sb.ToString());


            string excelFile = @"C:\Users\mike.nguyen\Desktop\Current Work\Report\Simplified_ProductImageSize_Report.xlsx";


            DataTable dt = Excel.Read(excelFile, "Sheet1");

            string sourceImage = @"C:\Temp\PICCompare\PIC";
            string cmsImage = @"C:\Temp\PICCompare\PICCMS";
            string dest_sourceImage = @"C:\Users\mike.nguyen\Desktop\Current Work\Report\Image\AU";
            string dest_cmsImage = @"C:\Users\mike.nguyen\Desktop\Current Work\Report\Image\CMS";
            string outputFile = @"C:\Users\mike.nguyen\Desktop\Current Work\Report\Image\Output.html";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table border='1'><tr><td>Description</td><td>Source Image</td><td>CMS Image</td></tr>");

            foreach (DataRow dr in dt.Rows)
            {
                string src = dr["Source ImageName"].ToString() + ".gif";
                string cms = dr["CMS ImageName"].ToString() + ".gif";

                File.Copy(Path.Combine(sourceImage, src), Path.Combine(dest_sourceImage, src), true);
                File.Copy(Path.Combine(cmsImage, cms), Path.Combine(dest_cmsImage, cms), true);

                string description = "";
                StringBuilder sb1 = new StringBuilder();
                sb1.AppendLine(string.Format("Code = {0}|{1}", dr[0].ToString(), dr[1].ToString()));
                sb1.AppendLine(string.Format("<br />Source Description = {0}", dr["Source ImageDescription"].ToString()));
                sb1.AppendLine(string.Format("<br />CMS Description = {0}", dr["CMS ImageDescription"].ToString()));

                description = sb1.ToString();

                string imageSource = string.Format(@"<img src='AU\{0}' alt = '{1}' >", src , dr["Source ImageDescription"].ToString());
                string imageCms = string.Format(@"<img src='CMS\{0}' alt = '{1}' >", cms, dr["CMS ImageDescription"].ToString());

                sb.AppendLine(string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", description, imageSource, imageCms));
            }

            sb.AppendLine("</table>");

            File.WriteAllText(outputFile, sb.ToString());
        }
     

    }
}
