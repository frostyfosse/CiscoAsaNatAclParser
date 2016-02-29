using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using CiscoAsaNetAclParser;

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

        [TestMethod]
        public void TestSimpleObjectNetwork()
        {
            var lines = GetSampleData();

            var parser = new Parser();
            var result = parser.Parse(lines);
        }

        string[] GetSampleData()
        {
            var value = @"object network SuperCool_Object_host
 host 192.168.1.5
 description This is my cool description";
            var lines = value.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return lines;
        }
    }
}
