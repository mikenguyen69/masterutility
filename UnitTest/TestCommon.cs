using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MasterUtility;

namespace UnitTest
{
    [TestClass]
    public class TestCommon
    {
        [TestMethod]
        public void TestDate()
        {
            string date = Common.GetDateFormat("");
        }
    }
}
