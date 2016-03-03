using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using CiscoAsaNetAclParser;
using System.Collections.Generic;
using System.Linq;

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
            var lines = GetSampleDataWithAliasReferences();

            var parser = new Parser();
            var parseResults = parser.Parse(lines);
            var results = parseResults.Results;
        }

        [TestMethod]
        public void TestJunk()
        {
            var list = new List<string>();

            for (int i = 0; i < 10; i++)
                list.Add(string.Format("My string"));

            int index = 0;
            var myIndex = 0;

            while (true)
            {
                myIndex = list.IndexOf("My string", index == 0 ? index : myIndex + 1);

                index++;
            }

        }

        string[] GetSampleData()
        {
            var value = @"object network SuperCool_Object_host
 host 192.168.1.5
 description This is my cool description";
            var lines = value.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return lines;
        }

        string[] GetSampleDataWithAliasReferences()
        {
            var value = @"! ####### This is an example of the host option
object network SuperCool_Object_host
 host 192.168.1.5
 description This is my cool description
 nat (inside,outside) static 208.97.227.215
object network Someone2
 host 208.97.227.216
object network SuperCool_Object_withsubnet
 subnet 192.168.1.6 255.255.255.0
 description This one is for subnet style
 nat (inside,outside) static Someone2";
            var lines = value.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return lines;
        }
    }
}
