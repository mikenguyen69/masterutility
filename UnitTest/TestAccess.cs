using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MasterUtility;

namespace UnitTest
{
    [TestClass]
    public class TestAccess
    {
        [TestMethod]
        public void GetDataTable()
        {
            string path = @"C:\Users\mike.nguyen\Desktop\Current Work\AU Parallel Support\AREVData_20170206\AREVData.mdb";

            var test = Access.GetDataTable(path, "arevMBM");

            Assert.IsTrue(test.Rows.Count > 0);
        }
    }
}
