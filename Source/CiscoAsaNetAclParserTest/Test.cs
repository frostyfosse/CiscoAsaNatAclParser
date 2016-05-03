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
        public void TestStandardAccessList()
        {
            var value = "access-list jnjvpn_split_tunnel standard permit 10.251.27.0 255.255.255.0";

            var parser = new Parser();
            var result = parser.Parse(new[] { value });

            Assert.IsTrue(result.AccessListResults.Count() == 1);
            Assert.IsTrue(result.AccessListResults.First().AccessLists.Count == 1);

            var accessList = result.AccessListResults.First().AccessLists.First();

            Assert.IsNotNull(accessList.Name);
            Assert.IsTrue(accessList.Permission == AccessListPermission.Permit);
            Assert.IsTrue(accessList.Type == AccessListType.Standard);
            Assert.IsTrue(accessList.SourceIPGroup.IPAddress == "10.251.27.0");
            Assert.IsTrue(accessList.SourceIPGroup.SubnetAddress == "255.255.255.0");
        }

        [TestMethod]
        public void TestSimpleObjectNetwork()
        {
            var lines = GetSampleObjectNetworkDataWithAliasReferences();

            var parser = new Parser();
            var parseResults = parser.Parse(lines);
            var results = parseResults.ObjectNetworkResults;

            Assert.IsTrue(results.Count == 3);
            Assert.IsTrue(results[0].IPGroup.IPAddress == "192.168.1.5" && results[0].IPGroup.Subnet == null);
            Assert.IsTrue(results[0].NatIPAddress == "208.97.227.215");
            Assert.IsTrue(results[1].Name == "Someone2");
            Assert.IsTrue(results[2].IPGroup.IPAddress == "192.168.1.6");
            Assert.IsTrue(results[2].IPGroup.SubnetAddress == "255.255.255.0");
        }

        [TestMethod]
        public void TestObjectUnusualGroupCondition()
        {
            var line = "access-list inside_access_in extended permit object-group IP-Common_Internet_Services any any ";

            var parser = new Parser();
            var parserResults = parser.Parse(new[] { line });

            Assert.IsTrue(parserResults.AccessListResults.Count == 1);
        }

        [TestMethod]
        public void TestToughIPReference()
        {
            var line = "access-list inside_access_in extended permit object-group mgmtzone_to_client_services object mgmtzone_servers any";
            //access-list inside_access_in extended permit object-group mgmtzone_to_client_services object mgmtzone_servers any 

            var parser = new Parser();
            var parserResults = parser.Parse(new[] { line });
            var results = parserResults.AccessListResults;

            Assert.IsTrue(results.Count() == 1);
            Assert.IsTrue(results.First().AccessLists.Count == 1);

            var accessList = results.First().AccessLists.First();

            Assert.IsNotNull(accessList.DestinationIPGroup.Port1); //I'm not sure if this test is correct...
        }

        [TestMethod]
        public void TestSourceAndDestinationIPAnyWithObjectGroup()
        {
            var line = "access-list inside_access_in extended permit udp any any object-group UDP-Common_Internet_Services";

            var parser = new Parser();
            var parserResults = parser.Parse(new[] { line });
        }

        string[] GetSampleObjectNetworkData()
        {
            var value = @"object network SuperCool_Object_host
 host 192.168.1.5
 description This is my cool description";
            var lines = value.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return lines;
        }

        string[] GetSampleObjectNetworkDataWithAliasReferences()
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
