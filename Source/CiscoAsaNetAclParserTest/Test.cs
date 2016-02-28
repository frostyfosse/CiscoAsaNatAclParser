using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace CiscoAsaNetAclParserTest
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void TestMethod1()
        {
            IPAddress address = null;

            IPAddress.TryParse("192.168.1.1", out address);
        }
    }
}
