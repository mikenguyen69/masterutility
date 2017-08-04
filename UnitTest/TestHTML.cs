using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MasterUtility;

namespace UnitTest
{
    [TestClass]
    public class TestHTML
    {
        [TestMethod]
        public void Test_RemoveTag()
        {
            string input = @"Nonmycobacterial infections. Adults: 250 mg twice daily for 7-14 days; severe infections: 500 mg twice daily. ClCr < 30 mL/min: 250 mg once daily, twice daily in severe infections, max 14 days. Legionella: 500 mg twice daily for 4 wks. Haemolytic streptococcal infection: treat for greater than or equal to 10 days. Peptic ulcer <em>(H. pylori):</em> clarithromycin 500 mg + amoxycillin 1000 mg + omeprazole 20 mg twice daily for 7-10 days. Mycobacterial infection (adjunct to other antimycobacterials). Adults, children > 12 yrs: 500 mg twice daily; if no response after 3-4 wks may incr to 1000 mg twice daily. Elderly > 65 yrs (ClCr > 30 mL/min): initially 500 mg twice daily. Prophylaxis in HIV infected adults: 500 mg twice daily";

            string result = @"Monoclonal Ab test to detect hCG in urine from 1st day of missed period; sensitivity greater than or equal to 25 IU/L. Pregnancy detection (home use)";

            string actual = HTML.RemoveTag(input);

            Assert.AreEqual(actual, result);
        }
    }
}
