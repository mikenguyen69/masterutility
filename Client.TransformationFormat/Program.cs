using MasterUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Client.TransformationFormat
{
    class Program
    {
        public static List<string> ErrorList = new List<string>();

        static void Main(string[] args)
        {
            //HandleInddat();

            string path = @"C:\Users\mike.nguyen\Desktop\Current Work\AU ASCII Validation\2017 May\CMS\_PACKDAT";

            string file1 = Path.Combine(path, "PACKDAT.txt");
            string file2 = Path.Combine(path, "PACKDAT_new.txt");

            string outFile1 = Path.Combine(path, "PACKDAT_Out.txt");
            string outFile2 = Path.Combine(path, "PACKDAT_new_Out.txt");

            var lines1 = File.ReadAllLines(file1);
            var lines2 = File.ReadAllLines(file2);

            List<PackRow> OldList = new List<PackRow>();
            List<PackRow> NewList = new List<PackRow>();

            foreach (string line in lines1)
            {
                OldList.Add(new PackRow { Raw = line });
            }

            List<PackRow> differentList = new List<PackRow>();

            foreach (string line in lines2)
            {
                PackRow pr = new PackRow { Raw = line };

                NewList.Add(pr);

                if (OldList.Any(x => x.Order == pr.Order && x.Raw != pr.Raw))
                {
                    differentList.Add(pr);
                }
            }

            List<string> newLine1 = new List<string>();
            foreach (string line in lines1)
            {
                PackRow pr = new PackRow { Raw = line };

                if (differentList.Any(x => x.Order == pr.Order))
                {
                    newLine1.Add(line);
                }
            }

            

            List<string> newLine2 = new List<string>();
            foreach (string line in lines2)
            {
                PackRow pr = new PackRow { Raw = line };

                if (differentList.Any(x => x.Order == pr.Order))
                {
                    newLine2.Add(line);
                }
            }


            File.WriteAllLines(outFile1, newLine1.ToArray());
            File.WriteAllLines(outFile2, newLine2.ToArray());
            
        }

        private static void HandleInddat()
        {
            string dataPath = @"..\AU Test";

            string file1 = Path.Combine(dataPath, "INDDAT.txt");
            string file2 = Path.Combine(dataPath, "INDDAT1.txt");



            string[] lines = File.ReadAllLines(file1);

            List<string> combineList = new List<string>();

            string combinePattern = @"<b>Restriction: ([[0-9]+])</b><br/>";

            string currentStreamlinePattern = @"<B>([0-9]+)</B>";

            List<string> streamlineList = new List<string>();

            List<string> streamlineCodeList = new List<string>();

            foreach (string line in lines)
            {
                if (Regex.IsMatch(line, combinePattern))
                {
                    combineList.Add(line);
                }

                if (Regex.IsMatch(line, currentStreamlinePattern))
                {
                    streamlineList.Add(line);
                }
            }


            foreach (string x in streamlineList)
            {
                var matches = Regex.Matches(x, currentStreamlinePattern);

                foreach (Match m in matches)
                {
                    string code = m.Groups[0].ToString().Replace("<B>", "").Replace("</B>", "");

                    if (!streamlineCodeList.Contains(code))
                    {
                        streamlineCodeList.Add(code);
                    }
                }
            }

            //Actual working
            List<string> sb = new List<string>();

            foreach (string x in lines)//combineList)
            {
                var matches = Regex.Matches(x, combinePattern);


                List<ReplaceDTO> listReplacement = new List<ReplaceDTO>();


                int count = 0;


                foreach (Match m in matches)
                {
                    string code = m.Groups[0].ToString();

                    ReplaceDTO dto = new ReplaceDTO
                    {
                        Order = count,
                        InString = code
                    };

                    string streamlineCode = streamlineCodeList.FirstOrDefault(a => code.Contains(a));

                    if (!string.IsNullOrEmpty(streamlineCode))
                    {
                        dto.ReplaceString = "[START]" + streamlineCode + "[END]";
                    }
                    else
                    {
                        dto.ReplaceString = "";
                    }

                    listReplacement.Add(dto);

                    count++;
                }

                string newLine = x;
                //Replacement
                if (count > 0)
                {
                    foreach (ReplaceDTO dto in listReplacement)
                    {
                        newLine = newLine.Replace(dto.InString, dto.ActualReplaceString);
                    }


                    newLine = StripOff(newLine);
                }

                sb.Add(newLine);
            }


            string output1 = Path.Combine(dataPath, "INDDAT_Output.txt");

            File.WriteAllLines(output1, sb.ToArray());
        }

        private static string StripOff(string html)
        {
            if (string.IsNullOrEmpty(html)) return "";
            html = html.Replace("&#xe9;", "e");
            html = Regex.Replace(Regex.Replace(html, @"<.+?>", " "), @"\s+", " ").TrimStart();

            html = html.Replace("[START]", "<B>").Replace("[END]", "</B>").Replace("[BREAKLINE]","<BR>").Replace("| ","|").Replace(" |","|");

            return html;
        }

        private static string Replace(string input)
        {
            return input.Replace("&lsquo;", "&#x2018;")
                    .Replace("&dagger;", "&#2020;")
                    .Replace("&Dagger;", "&#2021;")
                    .Replace("&ldquo;", "&#x201c;")
                    .Replace("&rdquo;", "&#x201d;")
                    .Replace("&ndash;", "&#x2013;")
                    .Replace("&quot;","&#x22;")
                    .Replace("&amp;","&#x26;")
                    .Replace("&apos;","&#x27;")
                    .Replace("&lt;","&#x3c;")
                    .Replace("&gt;","&#x3e;")
                    .Replace("&nbsp;","&#xa0;")
                    .Replace("&iexcl;","&#xa1;")
                    .Replace("&cent;","&#xa2;")
                    .Replace("&pound;","&#xa3;")
                    .Replace("&curren;","&#xa4;")
                    .Replace("&yen;","&#xa5;")
                    .Replace("&brvbar;","&#xa6;")
                    .Replace("&sect;","&#xa7;")
                    .Replace("&uml;","&#xa8;")
                    .Replace("&copy;","&#xa9;")
                    .Replace("&ordf;","&#xaa;")
                    .Replace("&laquo;","&#xab;")
                    .Replace("&not;","&#xac;")
                    .Replace("&shy;","&#xad;")
                    .Replace("&reg;","&#xae;")
                    .Replace("&macr;","&#xaf;")
                    .Replace("&deg;","&#xb0;")
                    .Replace("&plusmn;","&#xb1;")
                    .Replace("&sup2;","&#xb2;")
                    .Replace("&sup3;","&#xb3;")
                    .Replace("&acute;","&#xb4;")
                    .Replace("&micro;","&#xb5;")
                    .Replace("&para;","&#xb6;")
                    .Replace("&middot;","&#xb7;")
                    .Replace("&cedil;","&#xb8;")
                    .Replace("&sup1;","&#xb9;")
                    .Replace("&ordm;","&#xba;")
                    .Replace("&raquo;","&#xbb;")
                    .Replace("&frac14;","&#xbc;")
                    .Replace("&frac12;","&#xbd;")
                    .Replace("&frac34;","&#xbe;")
                    .Replace("&iquest;","&#xbf;")
                    .Replace("&Agrave;","&#xc0;")
                    .Replace("&Aacute;","&#xc1;")
                    .Replace("&Acirc;","&#xc2;")
                    .Replace("&Atilde;","&#xc3;")
                    .Replace("&Auml;","&#xc4;")
                    .Replace("&Aring;","&#xc5;")
                    .Replace("&AElig;","&#xc6;")
                    .Replace("&Ccedil;","&#xc7;")
                    .Replace("&Egrave;","&#xc8;")
                    .Replace("&Eacute;","&#xc9;")
                    .Replace("&Ecirc;","&#xca;")
                    .Replace("&Euml;","&#xcb;")
                    .Replace("&Igrave;","&#xcc;")
                    .Replace("&Iacute;","&#xcd;")
                    .Replace("&Icirc;","&#xce;")
                    .Replace("&Iuml;","&#xcf;")
                    .Replace("&ETH;","&#xd0;")
                    .Replace("&Ntilde;","&#xd1;")
                    .Replace("&Ograve;","&#xd2;")
                    .Replace("&Oacute;","&#xd3;")
                    .Replace("&Ocirc;","&#xd4;")
                    .Replace("&Otilde;","&#xd5;")
                    .Replace("&Ouml;","&#xd6;")
                    .Replace("&times;","&#xd7;")
                    .Replace("&Oslash;","&#xd8;")
                    .Replace("&Ugrave;","&#xd9;")
                    .Replace("&Uacute;","&#xda;")
                    .Replace("&Ucirc;","&#xdb;")
                    .Replace("&Uuml;","&#xdc;")
                    .Replace("&Yacute;","&#xdd;")
                    .Replace("&THORN;","&#xde;")
                    .Replace("&szlig;","&#xdf;")
                    .Replace("&agrave;","&#xe0;")
                    .Replace("&aacute;","&#xe1;")
                    .Replace("&acirc;","&#xe2;")
                    .Replace("&atilde;","&#xe3;")
                    .Replace("&auml;","&#xe4;")
                    .Replace("&aring;","&#xe5;")
                    .Replace("&aelig;","&#xe6;")
                    .Replace("&ccedil;","&#xe7;")
                    .Replace("&egrave;","&#xe8;")
                    .Replace("&eacute;","&#xe9;")
                    .Replace("&ecirc;","&#xea;")
                    .Replace("&euml;","&#xeb;")
                    .Replace("&igrave;","&#xec;")
                    .Replace("&iacute;","&#xed;")
                    .Replace("&icirc;","&#xee;")
                    .Replace("&iuml;","&#xef;")
                    .Replace("&eth;","&#xf0;")
                    .Replace("&ntilde;","&#xf1;")
                    .Replace("&ograve;","&#xf2;")
                    .Replace("&oacute;","&#xf3;")
                    .Replace("&ocirc;","&#xf4;")
                    .Replace("&otilde;","&#xf5;")
                    .Replace("&ouml;","&#xf6;")
                    .Replace("&divide;","&#xf7;")
                    .Replace("&oslash;","&#xf8;")
                    .Replace("&ugrave;","&#xf9;")
                    .Replace("&uacute;","&#xfa;")
                    .Replace("&ucirc;","&#xfb;")
                    .Replace("&uuml;","&#xfc;")
                    .Replace("&yacute;","&#xfd;")
                    .Replace("&thorn;","&#xfe;")
                    .Replace("&yuml;","&#xff;")
                    .Replace("&fnof;","&#x192;")
                    .Replace("&Alpha;","&#x391;")
                    .Replace("&Beta;","&#x392;")
                    .Replace("&Gamma;","&#x393;")
                    .Replace("&Delta;","&#x394;")
                    .Replace("&Epsilon;","&#x395;")
                    .Replace("&Zeta;","&#x396;")
                    .Replace("&Eta;","&#x397;")
                    .Replace("&Theta;","&#x398;")
                    .Replace("&Iota;","&#x399;")
                    .Replace("&Kappa;","&#x39A;")
                    .Replace("&Lambda;","&#x39B;")
                    .Replace("&Mu;","&#x39C;")
                    .Replace("&Nu;","&#x39D;")
                    .Replace("&Xi;","&#x39E;")
                    .Replace("&Omicron;","&#x39F;")
                    .Replace("&Pi;","&#x3A0;")
                    .Replace("&Rho;","&#x3A1;")
                    .Replace("&Sigma;","&#x3A3;")
                    .Replace("&Tau;","&#x3A4;")
                    .Replace("&Upsilon;","&#x3A5;")
                    .Replace("&Phi;","&#x3A6;")
                    .Replace("&Chi;","&#x3A7;")
                    .Replace("&Psi;","&#x3A8;")
                    .Replace("&Omega;","&#x3A9;")
                    .Replace("&alpha;","&#x3B1;")
                    .Replace("&beta;","&#x3B2;")
                    .Replace("&gamma;","&#x3B3;")
                    .Replace("&delta;","&#x3B4;")
                    .Replace("&epsilon;","&#x3B5;")
                    .Replace("&zeta;","&#x3B6;")
                    .Replace("&eta;","&#x3B7;")
                    .Replace("&theta;","&#x3B8;")
                    .Replace("&iota;","&#x3B9;")
                    .Replace("&kappa;","&#x3BA;")
                    .Replace("&lambda;","&#x3BB;")
                    .Replace("&mu;","&#x3BC;")
                    .Replace("&nu;","&#x3BD;")
                    .Replace("&xi;","&#x3BE;")
                    .Replace("&omicron;","&#x3BF;")
                    .Replace("&pi;","&#x3C0;")
                    .Replace("&rho;","&#x3C1;")
                    .Replace("&sigmaf;","&#x3C2;")
                    .Replace("&sigma;","&#x3C3;")
                    .Replace("&tau;","&#x3C4;")
                    .Replace("&upsilon;","&#x3C5;")
                    .Replace("&phi;","&#x3C6;")
                    .Replace("&chi;","&#x3C7;")
                    .Replace("&psi;","&#x3C8;")
                    .Replace("&omega;","&#x3C9;")
                    .Replace("&thetasym;","&#x3D1;")
                    .Replace("&upsih;","&#x3D2;")
                    .Replace("&piv;","&#x3D6;")
                    .Replace("&bull;","&#x2022;")
                    .Replace("&hellip;","&#x2026;")
                    .Replace("&prime;","&#x2032;")
                    .Replace("&Prime;","&#x2033;")
                    .Replace("&oline;","&#x203E;")
                    .Replace("&frasl;","&#x2044;")
                    .Replace("&weierp;","&#x2118;")
                    .Replace("&image;","&#x2111;")
                    .Replace("&real;","&#x211C;")
                    .Replace("&trade;","&#x2122;")
                    .Replace("&alefsym;","&#x2135;")
                    .Replace("&larr;","&#x2190;")
                    .Replace("&uarr;","&#x2191;")
                    .Replace("&rarr;","&#x2192;")
                    .Replace("&darr;","&#x2193;")
                    .Replace("&harr;","&#x2194;")
                    .Replace("&crarr;","&#x21B5;")
                    .Replace("&lArr;","&#x21D0;")
                    .Replace("&uArr;","&#x21D1;")
                    .Replace("&rArr;","&#x21D2;")
                    .Replace("&dArr;","&#x21D3;")
                    .Replace("&hArr;","&#x21D4;")
                    .Replace("&forall;","&#x2200;")
                    .Replace("&part;","&#x2202;")
                    .Replace("&exist;","&#x2203;")
                    .Replace("&empty;","&#x2205;")
                    .Replace("&nabla;","&#x2207;")
                    .Replace("&isin;","&#x2208;")
                    .Replace("&notin;","&#x2209;")
                    .Replace("&ni;","&#x220B;")
                    .Replace("&prod;","&#x220F;")
                    .Replace("&sum;","&#x2211;")
                    .Replace("&minus;","&#x2212;")
                    .Replace("&lowast;","&#x2217;")
                    .Replace("&radic;","&#x221A;")
                    .Replace("&prop;","&#x221D;")
                    .Replace("&infin;","&#x221E;")
                    .Replace("&ang;","&#x2220;")
                    .Replace("&and;","&#x2227;")
                    .Replace("&or;","&#x2228;")
                    .Replace("&cap;","&#x2229;")
                    .Replace("&cup;","&#x222A;")
                    .Replace("&int;","&#x222B;")
                    .Replace("&there4;","&#x2234;")
                    .Replace("&sim;","&#x223C;")
                    .Replace("&cong;","&#x2245;")
                    .Replace("&asymp;","&#x2248;")
                    .Replace("&ne;","&#x2260;")
                    .Replace("&equiv;","&#x2261;")
                    .Replace("&le;","&#x2264;")
                    .Replace("&ge;","&#x2265;")
                    .Replace("&sub;","&#x2282;")
                    .Replace("&sup;","&#x2283;")
                    .Replace("&nsub;","&#x2284;")
                    .Replace("&sube;","&#x2286;")
                    .Replace("&supe;","&#x2287;")
                    .Replace("&oplus;","&#x2295;")
                    .Replace("&otimes;","&#x2297;")
                    .Replace("&perp;","&#x22A5;")
                    .Replace("&sdot;","&#x22C5;")
                    .Replace("&lceil;","&#x2308;")
                    .Replace("&rceil;","&#x2309;")
                    .Replace("&lfloor;","&#x230A;")
                    .Replace("&rfloor;","&#x230B;")
                    .Replace("&lang;","&#x2329;")
                    .Replace("&rang;","&#x232A;")
                    .Replace("&loz;","&#x25CA;")
                    .Replace("&spades;","&#x2660;")
                    .Replace("&clubs;","&#x2663;")
                    .Replace("&hearts;","&#x2665;")
                    .Replace("&diams;","&#x2666;")
                    .Replace("&rsquo;", "&#x2019;")

                   
                ;
        }
      
        private static string handleTheSection(XslCompiledTransform xslTransformer, string monoName,string value,  string section)
        {
            if (string.IsNullOrEmpty(value)) return "";

            value = Replace(value);

            value = string.Format("<an-Para>{0}</an-Para>", value);

            return applyXSLTToField(xslTransformer, monoName, section.Replace("an-", ""), value);
        }

        private static string applyXSLTToField(XslCompiledTransform xslTransformer, string monoName, string section, string value)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(value)) return result;
            if (string.IsNullOrEmpty(value.Trim())) return result;
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (var streamWriter = new StreamWriter(stream))
                    {
                        streamWriter.Write(value);
                        streamWriter.Flush();
                        stream.Position = 0;

                        XPathDocument xpathDocument = new XPathDocument(stream);

                        using (StringWriter stringWriter = new StringWriter())
                        {
                            using (XmlTextWriter textWriter = new XmlTextWriter(stringWriter))
                            {
                                xslTransformer.Transform(xpathDocument, null, textWriter);
                                result = stringWriter.ToString();

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorList.Add(string.Format("Failed to apply XSLT: \nFull PI: {0}]; \n Section: {1}; \nError Message: {2}, \nValue: {3}",
                    monoName, section, ex.Message, value));
            }
            return result;
        }
    }

    public class PackRow
    {
        public string Raw { get; set; }

        public string Order
        {
            get
            {
                return Raw.Substring(0, 15);
            }
        }
    }
    public class ReplaceDTO
    {
        public int Order { get; set; }
        public string InString { get; set; }
        public string ReplaceString { get; set; }

        public string ActualReplaceString
        {
            get
            {
                if (Order > 0)
                {
                    return ";[BREAKLINE]" + ReplaceString;
                }
                else
                    return ReplaceString;
            }
        }
    }
}

