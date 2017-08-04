using Infragistics.Excel;
using MasterUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.PackingLegacyReport
{
    class Program
    {
        static string const_local_path = @"..\Data";

        static void Main(string[] args)
        {
            //1. Import list of code

            string file = Directory.GetFiles(const_local_path, "*.txt").FirstOrDefault();

            FileInfo fi = new FileInfo(file);

            if (!File.Exists(file))
            {
                Console.WriteLine("MISSING DATA FILE in Data Folder OR WRONG FILE FORMAT !!!");
               
                Console.ReadKey();

                return;
            }

            string fileName = fi.Name.Replace(fi.Extension, "");

            string codeList = File.ReadAllText(file);

            string codeListString = string.Join(",",
                codeList.Replace("\r", "").Split('\n')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x =>  x ).ToArray()).ToString();

            //2. Step 1
            DataTable dt = Step1(codeListString, fileName, Database.GetLiveServerConnectionString("CmsLive_Primary"));

            //3. To Excel
            string excelFile = Path.Combine(const_local_path, "PackingLegacyReport_" + Common.GetDateFormat("") + ".xlsx");
            Step2(excelFile, dt);

            Console.ReadKey();
        }


        private static DataTable Step1(string codeListString, string interimDBName, string connectionString)
        {
            Output.Print(" Step 1 - Start....");
            Output.Print(" Executing long query which may take up to 2 minutes.... Be patient !");

            #region Script
            string sql = string.Format(@"
WITH TempTable AS (
	SELECT 
		a.LegacyId ProductCode, p.LegacyId FormCode, pa.LegacyId PackCode, ISNULL(pm.LegacyIdReferenceStem, pm.CounterpartId) PackingId, 
		REPLACE(pm.LegacyIdReference, pm.LegacyIdReferenceStem + '|', '') PbsSchedule_RestrictionCode, pm.CounterpartId, CASE WHEN a.DateCreate > p.DateCreate THEN a.DateCreate ELSE p.DateCreate END DateCreate
	FROM dbo.LegacyIdMapping a 
	INNER JOIN dbo.LegacyIdMappingPartnership p ON p.RelatedLegacyIdMappingId = a.Id
	INNER JOIN LegacyIdMappingPartnership pa ON pa.RelatedLegacyIdMappingId = p.LegacyIdMappingId AND pa.CountryCode='AU'
	INNER JOIN dbo.LegacyIdMapping pm ON pm.Id = pa.LegacyIdMappingId
	WHERE a.LegacyConcept='Brand_AU' AND pa.IsDeleted = 0 AND p.IsDeleted = 0 
	AND a.LegacyId IN (
		{0}
	)
)
, temp1 AS (
	SELECT DISTINCT a.*, b.Code PbsScheduleCode, c.Title PbsScheduleCategory, r.Code PbsRestrictionCode, r.Indication PbsRestrictionIndication , pa1.IsDeleted
	FROM TempTable a 
	inner JOIN Packing pa1 ON pa1.PackingId = a.PackingId AND pa1.IsDeleted = 0
	LEFT JOIN dbo.PbsSchedule b ON a.PbsSchedule_RestrictionCode like b.Code + '|%' AND b.IsDeleted=0
	LEFT JOIN dbo.PbsCategory c ON c.PbsCategoryId = b.PbsCategoryId AND c.IsDeleted = 0
	LEFT JOIN dbo.PbsRestriction r ON r.PbsScheduleId = b.PbsScheduleId AND r.IsActive = 1 AND r.IsDeleted=0 AND a.PbsSchedule_RestrictionCode LIKE '%|' + r.Code
	WHERE a.PbsSchedule_RestrictionCode IS NULL OR LEN(a.PbsSchedule_RestrictionCode) <=LEN('4375G|1') OR (LEN(a.PbsSchedule_RestrictionCode) > LEN('4375G|1') AND r.Code IS NOT NULL)
)

SELECT 
	distinct a.ProductCode, a.FormCode, a.PackCode , c.PackingDescription, a.PbsScheduleCode, a.PbsScheduleCategory, a.PbsRestrictionCode, a.PbsRestrictionIndication
	,a.DateCreate--, a.PackingId, a.CounterpartId, 
	, b.pack_description AU_PackingDescription
	, ISNULL(d.[Description], e.[Description]) CMS_PackingDescription
FROM temp1 a 
LEFT JOIN [10.10.10.33\INTERIMDB].AU_ParallelSupport.dbo.v_auPacking b ON b.prod_code = a.ProductCode AND b.form_code = a.FormCode AND b.pack_code = a.PackCode
LEFT JOIN [10.10.10.33\INTERIMDB].AU_ParallelSupport.dbo.CmsPacking c ON c.PackingID = a.PackingId
LEFT JOIN [10.10.10.33\INTERIMDB].[{1}].dbo.Packing d ON d.Id = a.CounterpartId
LEFT JOIN [10.10.10.33\INTERIMDB].[{1}].dbo.PackingSubsidySchemeMapping e ON e.Id = a.CounterpartId
ORDER BY a.ProductCode, a.FormCode, a.PackCode
", codeListString, interimDBName)
;
            #endregion
            DataTable dt = Database.GetDataSet(sql, connectionString).Tables[0];
            
            Console.WriteLine(".... Query has been executed correctly. New DataTable has been created.");

            Output.Print(" Step 1 - End!");

            return dt;
        }

        private static void Step2(string outputFile, DataTable data)
        {
            Output.Print(" Step 2 - Start....");

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

            Output.Print(" Step 2 - End!");
        }
    }
}
