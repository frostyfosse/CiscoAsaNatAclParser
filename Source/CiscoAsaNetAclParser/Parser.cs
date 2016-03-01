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
            string currentEvent = "";

            //Processing the actual object network detail
            try
            {
                //First pass
                currentEvent = "Processing first pass of object networks to collect any available configurations.";
                ParseCollection(result.Results, lines, true, out currentObjectNetwork);

                //Second pass for aliases
                currentEvent = "Processing second pass of object networks to translate any aliases.";
                ParseCollection(result.Results, lines, false, out currentObjectNetwork);

                result.Failed = false;
                result.Title = "Completed task";
            }
            catch(Exception e)
            {
                result.Failed = true;
                result.Title = string.Format("An error occurred while {0}. This was for the object network '{1}'.", currentEvent, currentObjectNetwork);
                result.Messages.Add(e.ToString());
            }

            return result;
        }

        void ParseCollection(List<ObjectNetwork> objectNetworks, string[] lines, bool firstPass, out string currentObjectNetwork)
        {
            currentObjectNetwork = "";

            foreach (var objectNetwork in objectNetworks)
            {
                currentObjectNetwork = objectNetwork.Name;

                if (firstPass)
                    ParseConfigurationsForObjectNetwork(objectNetwork, lines);
                else
                {
                    ObjectNetwork aliasNetwork = null;

                    if (!string.IsNullOrEmpty(objectNetwork.IPAlias))
                    {
                        aliasNetwork  = GetIpFromAliases(objectNetwork.IPAlias, objectNetworks);
                        objectNetwork.IP = aliasNetwork.IP;
                        objectNetwork.Subnet = aliasNetwork.Subnet;
                    }
                    if (!string.IsNullOrEmpty(objectNetwork.NatIPAlias))
                    {
                        aliasNetwork = GetIpFromAliases(objectNetwork.NatIPAlias, objectNetworks);
                        objectNetwork.NatIP = aliasNetwork.NatIP;
                    }
                }
            }
        }

        void ParseConfigurationsForObjectNetwork(ObjectNetwork objectNetwork, string[] lines)
        {
            //First pass in collecting all configurations for a specific object network.
            foreach (var index in objectNetwork.Indices)
            {
                var skippedGroup = lines.Skip(index);

                foreach (var line in skippedGroup)
                {
                    bool groupComplete = AddConfiguration(objectNetwork, line);
                    
                    if (groupComplete)
                        break;
                }
            }
        }

        ObjectNetwork GetIpFromAliases(string objectNetworkAlias, List<ObjectNetwork> objectNetworks)
        {
            return objectNetworks.Where(x => x.Name == objectNetworkAlias).FirstOrDefault();
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

                if (IPAddress.TryParse(stringIp, out subnetIp))
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
            //TODO: Complete this
        }

        void GetOthers(ObjectNetwork objectNetwork, string line)
        {
            var firstSpace = line.IndexOf(" ", 0);

            var pair = new KeyValuePair<string, string>(line.Substring(0, firstSpace), 
                                                        line.Substring(firstSpace < 0 ? 0 : firstSpace));

            objectNetwork.AdditionalColumnValues.Add(pair);
        }

        List<ObjectNetwork> CollectObjectNetworkChecklist(IEnumerable<string> lines)
        {
            var objectNetworkReferences = lines.Where(x => x.Contains(ObjectNetwork.ObjectNetworkTag));
            var objectNetworks = new List<ObjectNetwork>();

            //Collecting all the starting points for the object networks
            foreach (var header in objectNetworkReferences.Distinct())
            {
                var objectNetwork = new ObjectNetwork()
                {
                    Name = header.Replace(ObjectNetwork.ObjectNetworkTag, null).Trim(),
                    OriginalName = header
                };

                bool firstRound = true;
                int lastIndex = 0;

                while (true)
                {
                    lastIndex = lines.ToList().IndexOf(objectNetwork.OriginalName, firstRound ? 0 : lastIndex + 1);

                    if (lastIndex == -1)
                        break;

                    objectNetwork.Indices.Add(lastIndex);
                    firstRound = false;
                }

                objectNetworks.Add(objectNetwork);
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
            else if (value.Contains(ObjectNetwork.ObjectNetworkTag) ||
                     value.Contains(ObjectNetwork.ObjectGroupNetworkTag) ||
                     value.Contains(ObjectNetwork.AccessGroupTag) ||
                     value.Contains(ObjectNetwork.ObjectServiceTag) ||
                     value.Contains(ObjectNetwork.ObjectTag))
                return TagOption.ObjectNetwork;
            else
                return TagOption.None;
        }
    }
}
