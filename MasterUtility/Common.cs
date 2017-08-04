using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterUtility
{
    public class Common
    {
        public static string GetDateFormat(string format)
        {
            return string.Format("{0}_{1}_{2}", DateTime.Today.Year, GetValueString(DateTime.Today.Month), GetValueString(DateTime.Today.Day));
        }

        private static string GetValueString(int value)
        {
            if (value > 9) return "" + value;

            else return string.Format("0{0}", value);
        }
    }
}
