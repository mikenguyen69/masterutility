using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterUtility
{
    public class Access
    {
        public static DataTable GetDataTable(string path, string table)
        {            
            string sql = @"SELECT * FROM [" + table + "]";

            using (OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Persist Security Info=false"))
            using (OleDbCommand command = new OleDbCommand(sql, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                DataTable dataTable = new DataTable();
                dataTable.Locale = CultureInfo.CurrentCulture;
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public static void BulkCopy(string connection, string accessFile, string table)
        {
            SqlBulkCopy bulkcopy = new SqlBulkCopy(connection);
            bulkcopy.DestinationTableName = table;

            try
            {
                //Clean up                
                bulkcopy.WriteToServer(Access.GetDataTable(accessFile, table));
            }
            catch (Exception e)
            {

            }
        }
    }
}
