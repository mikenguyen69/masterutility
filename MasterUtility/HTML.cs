using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterUtility
{
    public class HTML
    {
        public static string RemoveTag(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            HtmlDocument doc = new HtmlDocument();
            
            doc.LoadHtml(input);

            string text = doc.DocumentNode.InnerText.Replace("&nbsp;", " ");

            text = text
                .Replace("û", " ").Replace("�","")
                .Replace("á", "beta").Replace("à", "alpha")                
                .Replace("ñ", "+/-")
                .Replace("  <R>", " ").Replace(" <R>", ". ")
                .Replace("ò", "greater than or equal to").Replace("ó", "less than or equal to")
                .Replace(">>", ">").Replace("<<", "<")
                //.Replace(";", ",")
                .Replace("<I>","<em>").Replace("<D>","<em>").Replace("<D>","</em>").Replace("<sub>", "<em>").Replace("</sub>", "</em>")
                .Replace("`", "'")
                ;

            return text;
        }
    }
}
