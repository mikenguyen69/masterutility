using MasterUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ManageDataPatch
{
    class Program
    {
        static void Main(string[] args)
        {
            string query = @"
SELECT pt.name [TableName], pc.Name [ColumnName], rt.name [RelatedTableName], rc.name [RelatedColumnName]  
FROM sys.foreign_key_columns f 
INNER JOIN sys.tables pt ON pt.object_id = f.parent_object_id
INNER JOIN sys.columns pc ON pc.object_id = pt.object_id AND pc.column_id = f.constraint_column_id
INNER JOIN sys.tables rt ON rt.object_id = f.referenced_object_id AND rt.Name <> pt.name
INNER JOIN sys.columns rc ON rc.object_id = rt.object_id AND rc.column_id = f.referenced_column_id
WHERE  rt.Name NOT IN (
	'NhiRule','NhiCode','GDDB_HL7_Form_L','GDDB_FORM_L','PIL','MedsafeDocument','CodeTableType','DsmRoute','Drtc','Formulary'
)
";

            var result = Database.GetDataSet(query, Database.GetServerConnectionString("InterimDB_AU_20170512_195004_TestDB_Release"));

            List<ConfigDTO> configList = new List<ConfigDTO>();

            foreach (DataRow dr in result.Tables[0].Rows)
            {
                ConfigDTO cf = new ConfigDTO(dr);

                configList.Add(cf);
            }

            //Look for dependency

            List<string> level1 = configList.Where(x => !configList.Any(a => a.TableName == x.RelatedTableName)).Select(x => "'" + x.RelatedTableName + "'").Distinct().ToList();

            string output = string.Join(",", level1);

            List<string> level2 = configList.Where(x => level1.Any(a => a == x.RelatedTableName)).Select(x => x.TableName).Distinct().ToList();

            List<string> level3 = configList.Where(x => level2.Any(a => a == x.RelatedTableName)).Select(x => x.TableName).Distinct().ToList();

            List<string> level4 = configList.Where(x => level3.Any(a => a == x.RelatedTableName)).Select(x => x.TableName).Distinct().ToList();

            List<string> level5 = configList.Where(x => level4.Any(a => a == x.RelatedTableName)).Select(x => x.TableName).Distinct().ToList();

            List<string> level6 = configList.Where(x => level5.Any(a => a == x.RelatedTableName)).Select(x => x.TableName).Distinct().ToList();
        }
    }
}
