using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.PackingReduction
{
    public class Step1_GenerateViolationReport
    {
        public static string idbScript = @"
   SELECT 
	a.CounterpartId ProductGroupingId, REPLACE(b.LegacyIdString, '|' + CAST(b.LegacyId AS NVARCHAR(5)),'') ProductCode, b.LegacyId FormCode
	, MAX(c.LegacyId) MaxPackCode, b.LegacyId + 1 NewFormCode, REPLACE(b.LegacyIdString, '|' + CAST(b.LegacyId AS NVARCHAR(5)),'') + '|' + CAST(b.LegacyId + 1 AS NVARCHAR(5)) NewFormCodeString
    FROM CmsLive_Primary.dbo.LegacyIdMapping a 
    INNER JOIN CmsLive_Primary.dbo.LegacyIdMappingPartnership b ON b.LegacyIdMappingId = a.Id
    INNER JOIN CmsLive_Primary.dbo.LegacyIdMappingPartnership c ON c.RelatedLegacyIdMappingId = b.LegacyIdMappingId
    INNER JOIN CmsLive_Primary.dbo.LegacyIdMapping d ON d.Id = c.LegacyIdMappingId
    WHERE a.LegacyConcept = 'ProductGrouping_AU' 
    AND d.LegacyConcept <> 'Product_AU' AND d.IsDeleted=0 AND c.IsDeleted=0
    GROUP BY a.CounterpartId, a.LegacyConcept, b.LegacyIdString, b.LegacyId
    HAVING MAX(c.LegacyId) > 80
    ORDER BY MAX(c.LegacyId)
";

        private static void Step2(string outputFile, DataTable data)
        {
            //Output.Print(" Step 2 - Start....");

            //var memoryStream = new MemoryStream();
            //Workbook wb = Excel.Write("Sheet1", data);

            //wb.Save(memoryStream);

            //if (File.Exists(outputFile))
            //{
            //    File.Delete(outputFile);
            //}

            //using (FileStream fileStream = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //{
            //    memoryStream.WriteTo(fileStream);
            //}

            //Console.WriteLine(".... New excel file has been created at " + outputFile + ".");

            //Output.Print(" Step 2 - End!");
        }

    }
}
