using Infragistics.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MasterUtility
{
    public class Excel
    {
        public static Workbook Write(string wsName, DataTable dataTable)
        {
            Workbook workbook = new Workbook(WorkbookFormat.Excel2007);

            Worksheet worksheet = workbook.Worksheets.Add(wsName);

            //Construct Excel Sheet header
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                //set column name
                worksheet.Rows[0].Cells[i].Value = dataTable.Columns[i].ToString();
                //set Font for column name
                worksheet.Rows[0].Cells[i].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;

                worksheet.Columns[i].Width = (dataTable.Columns[i].ColumnName.Length + 2) * 256;
            }

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                #region Handling Value of different type e.g. string, Guid, bool, Datetime etc.
                //if (dataTable.[i] is string)
                //{
                //    string value = rdr.GetString(i);

                //    if (!string.IsNullOrEmpty(value))
                //    {
                //        if (value.Length < 80)
                //        {
                //            if (ws.Columns[i].Width < (value.Length + 2) * 256)
                //                ws.Columns[i].Width = (value.Length + 2) * 256;
                //        }
                //        else
                //        {
                //            ws.Columns[i].Width = (83) * 256;
                //            ws.Columns[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                //        }
                //    }

                //    ws.Rows[irow].Cells[i].Value = rdr[i];
                //}
                //else if (rdr[i] is Guid)
                //{
                //    ws.Rows[irow].Cells[i].Value = ((Guid)rdr[i]).ToString();
                //}
                //else if (rdr[i] is bool)
                //{
                //    ws.Rows[irow].Cells[i].Value = (bool)rdr[i] ? 1 : 0;
                //}
                //else if (rdr[i] is DateTime)
                //{
                //    ws.Rows[irow].Cells[i].Value = (DateTime)rdr[i];
                //}
                //else if (rdr[i] is int)
                //{
                //    ws.Rows[irow].Cells[i].Value = (int)rdr[i];
                //}
                //else
                //{
                //    ws.Rows[irow].Cells[i].Value = rdr[i];
                //}
                #endregion

                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    worksheet.Rows[i+1].Cells[j].Value = dataTable.Rows[i].Field<object>(dataTable.Columns[j]);
                }
            }
            d
            return workbook;
        }

        public static DataTable Read(string file, string wsName)
        {
            var workbook = Workbook.Load(file, false);

            var worksheet = workbook.Worksheets.FirstOrDefault(x => x.Name == wsName);

            DataTable dataTable = new DataTable(wsName);

            //First row is to setup the column
            for (int j = 0; j < worksheet.Rows[0].Cells.Count(); j++)
            {
                dataTable.Columns.Add(worksheet.Rows[0].Cells[j].Value.ToString(), typeof(string));
            }

            for (int i = 1; i < worksheet.Rows.Count(); i++)
            {
                List<string> objectList = new List<string>();
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    string value = "";

                    if (worksheet.Rows[i].Cells[j].Value != null) 
                        value = worksheet.Rows[i].Cells[j].Value.ToString();

                    objectList.Add(value);
                }

                dataTable.Rows.Add(objectList.ToArray());                
            }

            return dataTable;
            
        }

        public static bool IsValid(string file, string wsName)
        {            
            if (!File.Exists(file)) return false;


            if (!Regex.IsMatch((new FileInfo(file)).Extension, "xlxs|xls", RegexOptions.IgnoreCase)) return false;

            var workbook = Workbook.Load(file, false) ;
            if (!workbook.Worksheets.Any(x => x.Name == wsName)) return false;

            return true;
        }
    }
}
