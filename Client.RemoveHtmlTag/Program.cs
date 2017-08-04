using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Client.RemoveHtmlTag
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string input = @"C:\Users\mike.nguyen\Desktop\Current Work\AU ASCII Validation\201706 - Jun\98. Patches\INDDATEXTENSION.txt";
            string output = @"C:\Users\mike.nguyen\Desktop\Current Work\AU ASCII Validation\201706 - Jun\98. Patches\INDDAT1EXTENSION.txt";

            var lines = File.ReadAllLines(input);
            var fullContent = File.ReadAllText(input);

            foreach (string line in lines)
            {
                Restriction rest = new Restriction(line);

                if (rest.IsStreamline)
                {
                    fullContent = fullContent.Replace("<B>" + rest.RestCode + "</B>", "[" + rest.RestCode + "]");
                }
            }

            fullContent = FormatText(fullContent);

            Encoding ExtendedASCII = Encoding.GetEncoding("iso-8859-1");

            File.WriteAllText(output, fullContent, ExtendedASCII);

        }

        public static string FormatText(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }
    }
    public class Restriction
    {
        public string Code { get; set; }
        public string RestCode { get; set; }
        public string Text { get; set; }

        public Restriction(string input)
        {
            string[] breakdowns = input.Split('|');

            Code = breakdowns[0];
            RestCode = breakdowns[1];
            Text = breakdowns[2];

        }

        public bool IsStreamline 
        {
            get
            {
                return Text.Contains(RestCode);
            }
        }        
    }
}
