using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ManageDataPatch
{
    public class ConfigDTO
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string RelatedTableName { get; set; }
        public string RelatedColumnName { get; set; }

        public ConfigDTO(DataRow dr)
        {
            TableName = dr.Field<string>("TableName");
            ColumnName = dr.Field<string>("ColumnName");
            RelatedTableName = dr.Field<string>("RelatedTableName");
            RelatedColumnName = dr.Field<string>("RelatedColumnName");
        }
    }
}
