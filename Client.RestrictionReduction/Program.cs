using Infragistics.Excel;
using MasterUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.RestrictionReduction
{
    class Program
    {
        static void Main(string[] args)
        {
            //Database.ExecuteScript(sql, Database.GetLocalConnectionString("AU_ImageReview"));

            DataTable dt = Database.GetDataSet(@"SELECT DISTINCT a.Code, a.Description, a.ShortDescription, a.Note, a.LegacyConcept ,

REPLACE(REPLACE(REPLACE(
	(SELECT distinct s.Code FROM dbo.SubsidyScheme s 
	INNER JOIN dbo.rel_SubsidyScheme_Rule r ON r.SubsidySchemeId = s.Id 
	INNER JOIN dbo.[Rule] rr ON rr.Id = r.RuleId
	WHERE rr.Id = a.Id AND rr.LegacyConcept='PbsRestrictionCombination_AU'
	FOR XML PATH ('')),'</CODE><CODE>',' ;'),'<CODE>',''),'</CODE>','') PBSSchedule

FROM [dbo].[Rule] a", Database.GetLocalConnectionString("PbsRestriction_Patch")).Tables[0];


            List<DTO> listDto = new List<DTO>();

            foreach (DataRow dr in dt.Rows)
            {
                DTO d = new DTO
                {
                    Code = dr.Field<string>("Code"),
                    Indication = dr.Field<string>("Description"),
                    AbbrevIndication = dr.Field<string>("ShortDescription"),
                    Note = dr.Field<string>("Note"),
                    LegacyConcept = dr.Field<string>("LegacyConcept"),
                    PbsCode = dr.Field<string>("PBSSchedule")

                };

                listDto.Add(d);
                
            }

            var combinedList = listDto.Where(x => x.LegacyConcept == "PbsRestrictionCombination_AU");
            var normalList = listDto.Where(x => x.LegacyConcept == "PbsRestriction_AU");

            int count = 0;

            foreach (DTO d in combinedList)
            {
                List<DTO> matchedList = new List<DTO>();
              
                foreach (DTO b in normalList)
                {
                    if (d.Indication.Contains(b.ImproveCode))
                    {
                        matchedList.Add(b);
                    }
                }

                List<string> report2List = new List<string>();
                List<string> report3List = new List<string>();
                #region Handle grouping of node
                var groupingList = matchedList.GroupBy(x => x.Note).ToList();
                foreach (var group in groupingList)
                {
                    string noteText = group.Key;

                    int addCount = 0;
                    foreach (DTO dd in group.OrderBy(x => x.Code))
                    {
                        if (addCount == 0)
                        {
              
                            report2List.Add("<font color='Red'><b><i>[Restriction start]</i></b></font>");
                            addCount++;
                        }

                        //if (!report2List.Contains(dd.Indication))
                        //{
                            
                        //}
                        report2List.Add(dd.Indication);
                      
                    }
                    if (!string.IsNullOrEmpty(noteText))
                    {
                        report2List.Add("<font color='Red'><b><i>[Note start]</i></b></font>");
                      
                      
                        report2List.Add(noteText);
                    }



                }

                #endregion

                #region Handle all


                foreach (DTO m in matchedList.OrderBy(x => x.Code))
                {
                    string newText = m.CombineIndication;

                    report3List.Add(newText);
                }
                #endregion



                //var noteList = matchedList.GroupBy(x => x)

                string finalNewText =

                d.NewIndication = string.Join("<BR>", report2List); 
               
                Console.WriteLine(string.Format(" Processsing {0}/{1}", count++, combinedList.Count() ));
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table border='1'><tr><td width='10%'>PbsCode List</td><td width='30%'>Old Restriction Text</td><td width='60%'>New Restriction Text </td></tr>");

            foreach (var dto in combinedList.Where(x => x.ReduceIndication != x.NewIndication))
            {
                sb.AppendLine(string.Format("<tr><td valign=top>{0}</td><td valign=top>{1}</td><td valign=top>{2}</td>", dto.PbsCode, dto.Indication, dto.NewIndication));
            }

            sb.AppendLine("</table>");

            string fileOutput = @"D:\Work space\Mike's\6 POC Project\__CORE REUSE\MasterUtility\Client.RestrictionReduction\bin\Data\OutputFile.html";

            File.WriteAllText(fileOutput, sb.ToString());

            Console.WriteLine(".... New excel file has been created at " + fileOutput + ".");   

            #region Excel Handle
            //create database
            //DataTable dataTable = new DataTable();
            ////dataTable.Columns.Add(new DataColumn("PBS List", typeof(string)));
            //dataTable.Columns.Add(new DataColumn("Old Restriction", typeof(String)));
            //dataTable.Columns.Add(new DataColumn("New Restriction", typeof(String)));

            //foreach (DTO d in combinedList)
            //{
            //    dataTable.Rows.Add(d.Indication, d.NewIndication);
            //}

            //var memoryStream = new MemoryStream();
            //Workbook wb = Excel.Write("Sheet1", dataTable);
                
            #endregion
        }
    }

    public class DTO
    {
        public string Code { get; set; }
        public string Indication { get; set; }
        public string AbbrevIndication { get; set; }
        public string Note { get; set; }

        public string NewIndication { get; set; }
        public string LegacyConcept { get; set; }

        public string ReduceIndication { get; set; }

        public string PbsCode { get; set; }

        public string ImproveCode
        {
            get
            {
                return "<b>Restriction: [" + Code + "]</b>";
            }
        }

        public string CombineIndication
        {
            get
            {
                string result = "<font color='Red'><b><i>[Restriction start]</i></b></font><BR>" + Indication;

                if (!string.IsNullOrEmpty(Note))
                {
                    result += "<BR><font color='Red'><b><i>[Note start]</i></b></font><BR>" + Note;
                }

                return result;
            }
        }
    }
}
