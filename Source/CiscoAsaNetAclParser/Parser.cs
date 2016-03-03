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
                    string aliasErrorMessage = "";

                    if (!string.IsNullOrEmpty(objectNetwork.IPAlias))
                    {
                        aliasNetwork  = GetIpFromAliases(objectNetwork.IPAlias, objectNetworks);

                        if (aliasNetwork != null)
                        {
                            objectNetwork.IP = aliasNetwork.IP;
                            objectNetwork.Subnet = aliasNetwork.Subnet;
                        }
                        else
                            aliasErrorMessage = string.Format("Unable to find alias '{0}' for {1} '{2}'. This name was an alias provided for the host or subnet ip.", objectNetwork.IPAlias,
                                                                                                                                                                      ObjectNetwork.ObjectGroupNetworkTag,
                                                                                                                                                                      objectNetwork.Name);
                    }
                    if (!string.IsNullOrEmpty(objectNetwork.NatIPAlias))
                    {
                        aliasNetwork = GetIpFromAliases(objectNetwork.NatIPAlias, objectNetworks);

                        if (aliasNetwork != null)
                            objectNetwork.NatIP = aliasNetwork.IP != null ? aliasNetwork.IP : aliasNetwork.NatIP;
                        else
                            aliasErrorMessage = string.Format("Unable to find alias '{0}' for {1} '{2}'. This name was an alias provided for the NatIP.", objectNetwork.NatIPAlias,
                                                                                                                                                          ObjectNetwork.ObjectGroupNetworkTag,
                                                                                                                                                          objectNetwork.Name);
                    }

                    if (!string.IsNullOrEmpty(aliasErrorMessage))
                        objectNetwork.Comments.AppendLine(aliasErrorMessage);
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
            //nat (inside,outside) static Someone2
            //nat (inside,outside) static 208.97.227.215
            //nat (inside,outside) dynamic HPS_Global_DataCenter

            var values = line.Replace(ObjectNetwork.NatTag, null).Trim().Split(' ');
            IPAddress natIP = null;

            foreach (var value in values)
            {
                if (string.IsNullOrEmpty(objectNetwork.NatStatement))
                    objectNetwork.NatStatement = value.Replace(",", "|");
                else if (string.IsNullOrEmpty(objectNetwork.NatType))
                    objectNetwork.NatType = value;
                else if (objectNetwork.NatIP == null && string.IsNullOrEmpty(objectNetwork.NatIPAlias))
                {
                    if (!IPAddress.TryParse(value, out natIP))
                        objectNetwork.NatIPAlias = value;
                    else
                        objectNetwork.NatIP = natIP;
                }
                else
                    objectNetwork.NatPorts.Add(value); //TODO: Figure out how we want to display this information...
            }
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
            var trimmedValue = !string.IsNullOrEmpty(value) ? value.Trim() : "";

            if (trimmedValue.StartsWith(ObjectNetwork.HostTag))
                return TagOption.Host;
            else if (trimmedValue.StartsWith(ObjectNetwork.SubnetTag))
                return TagOption.Subnet;
            else if (trimmedValue.StartsWith(ObjectNetwork.NatTag))
                return TagOption.Nat;
            else if (trimmedValue.StartsWith(ObjectNetwork.DescriptionTag))
                return TagOption.Description;
            else if (trimmedValue.StartsWith(ObjectNetwork.ObjectNetworkTag) ||
                     trimmedValue.StartsWith(ObjectNetwork.ObjectGroupNetworkTag) ||
                     trimmedValue.StartsWith(ObjectNetwork.AccessGroupTag) ||
                     trimmedValue.StartsWith(ObjectNetwork.ObjectServiceTag) ||
                     trimmedValue.StartsWith(ObjectNetwork.ObjectTag))
                return TagOption.ObjectNetwork;
            else
                return TagOption.None;
        }
    }
}
