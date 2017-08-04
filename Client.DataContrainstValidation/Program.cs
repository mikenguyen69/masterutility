using Infragistics.Excel;
using MasterUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.DataContrainstValidation
{
    class Program
    {
        static void Main(string[] args)
        {
            List<DTO> ListObjects = new List<DTO>();
            List<string> packageList = new List<string> { "AU_ASCII_ABBREV", "AU_ASCII_ABBREVSUPPLEMENT", "AU_ASCII_ALLERGY", "AU_ASCII_CMI", "AU_ASCII_DRUGALERT", "AU_ASCII_DRUGALERTBC", "AU_ASCII_FULL", "AU_ASCII_INTERACT", "AU_ASCII_VIRTUALENTITIES" };

            foreach (string package in packageList)
            {
                string sql = @"SELECT DISTINCT REPLACE(REPLACE(a.name,'_new',''),'_original','') [Table] ,  b.name [Column] FROM sys.tables a 
INNER JOIN sys.columns b ON a.object_id = b.object_id
WHERE a.Name LIKE '%_new' OR a.Name LIKE '%_original'
ORDER BY REPLACE(REPLACE(a.name,'_new',''),'_original','')";

                var result = Database.GetDataSet(sql, Database.Get37ConnectionString(package));

                foreach (DataRow dr in result.Tables[0].Rows)
                {
                    DTO dto = new DTO(package, dr);
                    
                    ListObjects.Add(dto);
                }                
            }

            string outputFile = @"C:\Users\mike.nguyen\Desktop\Current Work\AU ASCII Validation\201706 - Jun\99. Validation Constraints\Validation_Report_V2.xlsx";

            ExportToExcel(outputFile, SetupDataTable(ListObjects));
        }

        private static DataTable SetupDataTable(List<DTO> listObjects)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add(new DataColumn("Package", typeof(string)));
            dt.Columns.Add(new DataColumn("TableName", typeof(string)));
            dt.Columns.Add(new DataColumn("ColumnName", typeof(string)));
            dt.Columns.Add(new DataColumn("IsNotNull_New", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsNotNull_Old", typeof(bool)));

            foreach (DTO dto in listObjects)
            {
                dt.Rows.Add(dto.Package, dto.TableName, dto.ColumName, dto.IsNotNull_New, dto.IsNotNull_Old);
            }

            return dt;
        }

        private static void ExportToExcel(string outputFile, DataTable data)
        {
            Output.Print(" ExportToExcel  - Start....");

            var memoryStream = new MemoryStream();
            Workbook wb = Excel.Write("Sheet1", data);

            wb.Save(memoryStream);

            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            using (FileStream fileStream = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                memoryStream.WriteTo(fileStream);
            }

            Console.WriteLine(".... New excel file has been created at " + outputFile + ".");

            Output.Print(" ExportToExcel - End!");
        }
    }

    public class DTO
    {
        public string Package { get; set; }
        public string TableName { get; set; }
        public string ColumName { get; set; }
        public bool IsNotNull_New { get; set; }
        public bool IsNotNull_Old { get; set; }

        public DTO(string package, DataRow dr)
        {
            Package = package;
            TableName = dr.Field<string>("Table");
            ColumName = dr.Field<string>("Column");
            IsNotNull_New = isNOtAllEmpty("new");
            IsNotNull_Old = isNOtAllEmpty("original");
        }

        private bool isNOtAllEmpty(string type)
        {
            string sql = string.Format(@"select * from [{0}_{2}] where cast([{1}] as nvarchar(max)) is null OR cast([{1}] as nvarchar(max)) = '' OR cast([{1}] as nvarchar(max)) = '0' or LEN(cast([{1}] as nvarchar(max))) = 0", TableName, ColumName, type);

            int count = Database.GetDataCount(sql, Database.Get37ConnectionString(Package));

            return count == 0;
        }
    }
}
