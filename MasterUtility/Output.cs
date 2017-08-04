using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterUtility
{
    public class Output
    {
        public static void Print(string text)
        {
            if (text.Contains("Start"))
            {
                Console.WriteLine("--------------------------------------------");
            }
            Console.WriteLine(text);
            if (text.Contains("End"))
            {
                Console.WriteLine("--------------------------------------------");
            }
        }
    }
}
