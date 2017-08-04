using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterUtility
{
    public class Database
    {
        public static string GetLocalConnectionString(string databaseName)
        {
            return string.Format("Server=(local);Database={0};Trusted_Connection=True", databaseName);
        }

        public static string GetServerConnectionString(string databaseName)
        {
            return string.Format("Server=10.10.10.33\\Interimdb;Database={0};User Id=sa;Password=p@ssw0rd1;", databaseName);
        }

        public static string GetServer31ConnectionString(string databaseName)
        {
            return string.Format("Server=10.10.10.31\\Interimdb;Database={0};User Id=sa;Password=p@ssw0rd123;", databaseName);
        }

        public static string GetLiveServerConnectionString(string databaseName)
        {
            return string.Format("Server=10.10.10.33;Database={0};User Id=sa;Password=MimsCms@Live;", databaseName);
        }

        public static string Get37ConnectionString(string databaseName)
        {
            return string.Format("Server=10.10.10.37\\SQLEXPRESS2008R2;Database={0};User Id=sa;Password=p@ssw0rdqazplm;", databaseName);
        }


        public static void ExecuteScript(string script, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();

                command.CommandText = script;
                command.ExecuteNonQuery();
            }
        }


        public static DataSet GetDataSet(string script, string connectionString)
        {
            DataSet dataSet = new DataSet();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(script, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                command.CommandTimeout = 0;
                connection.Open();
                adapter.Fill(dataSet);
            }

            return dataSet;
        }

        public static int GetDataCount(string script, string connectionString)
        {
            DataSet ds = GetDataSet(script, connectionString);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows.Count;

            return 0;
        }

        public static DataRow GetDataRow(string script, string connectionString)
        {
            DataSet ds = GetDataSet(script, connectionString);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];

            return null;
        }
    }
}
