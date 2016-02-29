using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CiscoAsaNetAclParser
{
    public class Parser
    {
        public Parser()
        {

        }

        /*
            ! ####### This is an example of the host option
            object network SuperCool_Object_host
             host 192.168.1.5
             description This is my cool description
             nat (inside,outside) static 208.97.227.215
            
            ! ####### This is an example of the subnet option
            object network Someone2
             host 208.97.227.216
             
            object network SuperCool_Object_withsubnet
             subnet 192.168.1.6 255.255.255.0
             description This one is for subnet style
             nat (inside,outside) static Someone2
        */

        public ParseResult Parse(string[] lines)
        {
            var result = new ParseResult();

            result.Results = CollectObjectNetworkChecklist(lines);
            string currentObjectNetwork = "";

            //Processing the actual object network detail
            try
            {
                foreach (var objectNetwork in result.Results)
                {
                    currentObjectNetwork = objectNetwork.Name;
                    ParseInternal(objectNetwork, lines);
                }

                result.Failed = false;
                result.Title = "Completed task";
            }
            catch(Exception e)
            {
                result.Failed = true;
                result.Title = string.Format("An error occurred while collecting data for the object network '{0}'.", currentObjectNetwork);
                result.Messages.Add(e.ToString());
            }

            return result;
        }

        void ParseInternal(ObjectNetwork objectNetwork, string[] lines)
        {
            foreach (var index in objectNetwork.Indices)
            {
                var skippedGroup = lines.Skip(index);
                bool firstLine = true;

                foreach (var line in skippedGroup)
                {
                    bool groupComplete = AddConfiguration(objectNetwork, line);



                    if (groupComplete)
                        break;
                }
            }
        }

        bool AddConfiguration(ObjectNetwork objectNetwork, string line)
        {
            bool groupComplete = false;

            switch (FindTagType(line))
            {
                case TagOption.ObjectNetwork:
                    if (line == objectNetwork.OriginalName)
                        groupComplete = false;
                    else //Found the end of the configuration
                        groupComplete = true;
                    break;
                case TagOption.Host:
                    GetHost(objectNetwork, line);
                    break;
                case TagOption.Subnet:
                    GetSubnet(objectNetwork, line);
                    break;
                case TagOption.Nat:
                    GetNat(objectNetwork, line);
                    break;
                case TagOption.Description:
                    objectNetwork.Description = line.Replace(ObjectNetwork.DescriptionTag, null).Trim();
                    break;
                case TagOption.None:
                    GetOthers(objectNetwork, line);
                    break;
                default:
                    break;
            }

            return groupComplete;
        }

        void GetHost(ObjectNetwork objectNetwork, string line)
        {
            string lineParsed = line.Replace(ObjectNetwork.HostTag, null).Trim();
            IPAddress hostIp = null;
            if (IPAddress.TryParse(lineParsed, out hostIp))
                objectNetwork.IP = hostIp;
            else
                objectNetwork.IPAlias = lineParsed;
        }

        void GetSubnet(ObjectNetwork objectNetwork, string line)
        {
            string lineParsed = line.Replace(ObjectNetwork.SubnetTag, null).Trim();
            string[] ips = lineParsed.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            bool foundIp = false;

            foreach (var stringIp in ips)
            {
                IPAddress subnetIp = null;

                if (IPAddress.TryParse(lineParsed, out subnetIp))
                {
                    if (!foundIp)
                    {
                        objectNetwork.IP = subnetIp;
                        foundIp = true;
                    }
                    else
                        objectNetwork.Subnet = subnetIp;
                }
                else
                {
                    objectNetwork.IPAlias = lineParsed;
                    break;
                }
            }
        }

        void GetNat(ObjectNetwork objectNetwork, string line)
        {

        }

        void GetOthers(ObjectNetwork objectNetwork, string line)
        {
            var firstSpace = line.IndexOf(" ", 0);
            var pair = new KeyValuePair<string, string>(line.Substring(0, firstSpace), line.Substring(firstSpace));

            objectNetwork.AdditionalColumnValues.Add(pair);
        }

        List<ObjectNetwork> CollectObjectNetworkChecklist(IEnumerable<string> lines)
        {
            var objectNetworkReferences = lines.Where(x => x.Contains(ObjectNetwork.ObjectNetworkTag));
            var objectNetworks = new List<ObjectNetwork>();

            int index = 0;

            //Collecting all the starting points for the object networks
            foreach (var header in objectNetworkReferences)
            {
                if (objectNetworks.Any(x => x.OriginalName == header))
                {
                    objectNetworks.Where(x => x.OriginalName == header).First().Indices.Add(index);
                    continue;
                }
                else
                {
                    var objectNetwork = new ObjectNetwork()
                    {
                        Name = header.Replace(ObjectNetwork.ObjectNetworkTag, null).Trim(),
                        OriginalName = header
                    };
                    objectNetwork.Indices.Add(index);

                    objectNetworks.Add(objectNetwork);
                }

                index++;
            }

            return objectNetworks;
        }

        TagOption FindTagType(string value)
        {
            if (value.Contains(ObjectNetwork.HostTag))
                return TagOption.Host;
            else if (value.Contains(ObjectNetwork.SubnetTag))
                return TagOption.Subnet;
            else if (value.Contains(ObjectNetwork.NatTag))
                return TagOption.Nat;
            else if (value.Contains(ObjectNetwork.DescriptionTag))
                return TagOption.Description;
            else if (value.Contains(ObjectNetwork.ObjectNetworkTag))
                return TagOption.ObjectNetwork;
            else
                return TagOption.None;
        }
    }
}
