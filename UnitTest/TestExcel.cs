using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MasterUtility;
using System.Data;
using Infragistics.Excel;

namespace UnitTest
{
    [TestClass]
    public class TestExcel
    {
        [TestMethod]
        public void Test_IsValid()
        {
            string file = @"C:\Users\mike.nguyen\Desktop\Current Work\AU ASCII Validation\AU_Validation_Rules.xlsx";

            string wsName = "Sheet1";

            Assert.IsTrue(Excel.IsValid(file, wsName));
        }

        [TestMethod]
        public void Test_Read()
        {
            string file = @"C:\Users\mike.nguyen\Desktop\Current Work\AU ASCII Validation\AU_Validation_Rules.xlsx";

            string wsName = "Sheet1";

            DataTable dt = Excel.Read(file, wsName);

        }

        [TestMethod]
        public void Test_Write()
        {
            string file = @"C:\Users\mike.nguyen\Desktop\Current Work\AU ASCII Validation\AU_Validation_Rules.xlsx";

            string wsName = "Sheet1";

            DataTable dt = Excel.Read(file, wsName);

            var wb = Excel.Write(wsName, dt);
        }
    }
}
