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
            var args = new ParserArgs(lines);
            var results = args.Results;

            try
            {
                //Processing object network
                ParseObjectNetworkCollection(args);
                ParseAccessListCollection(args);

                results.Title = "Completed task";
            }
            catch (Exception e)
            {
                results.CompletionSeverity = ResultSeverity.Errors;
                results.Title = string.Format("An error occurred while {0}. This was for the item '{1}'.", args.CurrentEvent, args.CurrentItem);
                results.Messages.Add(e.ToString());
            }

            return args.Results;
        }

        #region ObjectNetwork methods
        void ParseObjectNetworkCollection(ParserArgs args)
        {
            CollectObjectNetworkChecklist(args);

            for (int i = 0; i < 1; i++)
            {

                foreach (var objectNetwork in args.Results.ObjectNetworkResults)
                {
                    args.CurrentItem = objectNetwork.Name;

                    if (i == 0)
                    {
                        args.CurrentEvent = "Processing first pass of object networks to collect any available configurations.";
                        ParseLinesForObjectNetworkConfigurations(objectNetwork, args);
                    }
                    else
                    {
                        args.CurrentEvent = "Processing second pass of object networks to translate any aliases.";
                        GetNetworkDetailFromAliases(objectNetwork, args);
                    }
                }
            }
        }

        void ParseLinesForObjectNetworkConfigurations(ObjectNetwork focusedObjectNetwork, ParserArgs args)
        {
            //First pass in collecting all configurations for a specific object network.
            foreach (var index in focusedObjectNetwork.Indices)
            {
                var skippedGroup = args.Lines.Skip(index);

                foreach (var line in skippedGroup)
                {
                    bool groupComplete = AddConfiguration(focusedObjectNetwork, line);
                    
                    if (groupComplete)
                        break;
                }
            }
        }

        #region Get methods
        void GetNetworkDetailFromAliases(ObjectNetwork focusedObjectNetwork, ParserArgs args)
        {
            string aliasErrorMessage = "";
            ObjectNetwork aliasNetwork = null;
            var objectNetworks = args.Results.ObjectNetworkResults;

            if (!string.IsNullOrEmpty(focusedObjectNetwork.IPGroup.IPAlias))
            {
                aliasNetwork = GetIpFromAliases(focusedObjectNetwork.IPGroup.IPAlias, objectNetworks.ToList());

                if (aliasNetwork != null)
                {
                    focusedObjectNetwork.IPGroup.IP = aliasNetwork.IPGroup.IP;
                    focusedObjectNetwork.IPGroup.Subnet = aliasNetwork.IPGroup.Subnet;
                }
                else
                    aliasErrorMessage = string.Format("Unable to find alias '{0}' for {1} '{2}'. This name was an alias provided for the host or subnet ip.", focusedObjectNetwork.IPGroup.IPAlias,
                                                                                                                                                              ObjectNetwork.ObjectGroupNetworkTag,
                                                                                                                                                              focusedObjectNetwork.Name);
            }
            if (!string.IsNullOrEmpty(focusedObjectNetwork.NatIPAlias))
            {
                aliasNetwork = GetIpFromAliases(focusedObjectNetwork.NatIPAlias, objectNetworks.ToList());

                if (aliasNetwork != null)
                    focusedObjectNetwork.NatIP = aliasNetwork.IPGroup.IP != null ? aliasNetwork.IPGroup.IP : aliasNetwork.NatIP;
                else
                    aliasErrorMessage = string.Format("Unable to find alias '{0}' for {1} '{2}'. This name was an alias provided for the NatIP.", focusedObjectNetwork.NatIPAlias,
                                                                                                                                                  ObjectNetwork.ObjectGroupNetworkTag,
                                                                                                                                                  focusedObjectNetwork.Name);
            }

            if (!string.IsNullOrEmpty(aliasErrorMessage))
                focusedObjectNetwork.Comments.AppendLine(aliasErrorMessage);
        }

        void GetHost(ObjectNetwork objectNetwork, string line)
        {
            string lineParsed = line.Replace(ObjectNetwork.HostTag, null).Trim();
            IPAddress hostIp = null;

            if (IPAddress.TryParse(lineParsed, out hostIp))
                objectNetwork.IPGroup.IP = hostIp;
            else
                objectNetwork.IPGroup.IPAlias = lineParsed;
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
                        objectNetwork.IPGroup.IP = subnetIp;
                        foundIp = true;
                    }
                    else
                        objectNetwork.IPGroup.Subnet = subnetIp;
                }
                else
                {
                    objectNetwork.IPGroup.IPAlias = lineParsed;
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
                    objectNetwork.NatStatement = string.Format("\"{0}\"", value);
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

        ObjectNetwork GetIpFromAliases(string objectNetworkAlias, List<ObjectNetwork> objectNetworks)
        {
            return objectNetworks.Where(x => x.Name == objectNetworkAlias).FirstOrDefault();
        }
        #endregion

        bool AddConfiguration(ObjectNetwork objectNetwork, string line)
        {
            bool groupComplete = false;

            switch (FindTagType(line))
            {
                case ObjectNetworkTagOption.ObjectNetwork:
                    if (line == objectNetwork.OriginalName)
                        groupComplete = false;
                    else //Found the end of the configuration
                        groupComplete = true;
                    break;
                case ObjectNetworkTagOption.Host:
                    GetHost(objectNetwork, line);
                    break;
                case ObjectNetworkTagOption.Subnet:
                    GetSubnet(objectNetwork, line);
                    break;
                case ObjectNetworkTagOption.Nat:
                    GetNat(objectNetwork, line);
                    break;
                case ObjectNetworkTagOption.Description:
                    objectNetwork.Description = line.Replace(ObjectNetwork.DescriptionTag, null).Trim();
                    break;
                case ObjectNetworkTagOption.None:
                    GetOthers(objectNetwork, line);
                    break;
                default:
                    break;
            }

            return groupComplete;
        }

        void CollectObjectNetworkChecklist(ParserArgs args)
        {
            var objectNetworkReferences = args.Lines.Where(x => x.Contains(ObjectNetwork.ObjectNetworkTag));

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
                    lastIndex = args.Lines.ToList().IndexOf(objectNetwork.OriginalName, firstRound ? 0 : lastIndex + 1);

                    if (lastIndex == -1)
                        break;

                    objectNetwork.Indices.Add(lastIndex);
                    firstRound = false;
                }

                args.Results.ObjectNetworkResults.Add(objectNetwork);
            }
        }

        ObjectNetworkTagOption FindTagType(string value)
        {
            var trimmedValue = !string.IsNullOrEmpty(value) ? value.Trim() : "";

            if (trimmedValue.StartsWith(ObjectNetwork.HostTag))
                return ObjectNetworkTagOption.Host;
            else if (trimmedValue.StartsWith(ObjectNetwork.SubnetTag))
                return ObjectNetworkTagOption.Subnet;
            else if (trimmedValue.StartsWith(ObjectNetwork.NatTag))
                return ObjectNetworkTagOption.Nat;
            else if (trimmedValue.StartsWith(ObjectNetwork.DescriptionTag))
                return ObjectNetworkTagOption.Description;
            else if (trimmedValue.StartsWith(ObjectNetwork.ObjectNetworkTag) ||
                     trimmedValue.StartsWith(ObjectNetwork.ObjectGroupNetworkTag) ||
                     trimmedValue.StartsWith(ObjectNetwork.AccessGroupTag) ||
                     trimmedValue.StartsWith(ObjectNetwork.ObjectServiceTag) ||
                     trimmedValue.StartsWith(ObjectNetwork.ObjectTag))
                return ObjectNetworkTagOption.ObjectNetwork;
            else
                return ObjectNetworkTagOption.None;
        }
        #endregion

        #region Access-List Methods
        void ParseAccessListCollection(ParserArgs args)
        {
            //access-list outside_cryptomap_8 extended permit ip 10.251.27.0 255.255.255.0 host 10.42.8.233 
            //access-list jnjvpn_split_tunnel standard permit 10.251.27.0 255.255.255.0 
            //access-list OpenSys_access_in extended deny ip any4 object DMZ-LAN 
            //access-list mgmtzone_access_in extended permit udp object HPS_TP_PROD_LAN object-group HPS_remote_offices 

            args.Lines = args.Lines.Where(x => x.StartsWith(AccessList.AccessListTag));
            
            ParseAccessListLinesIntoGroups(args);

            foreach (var collection in args.Results.AccessListResults)
            {
                foreach(var acl in collection.AccessLists)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        if (i == 0)
                            ParseAccessListByType(args, acl);
                        else
                        {
                            //TODO: 2nd pass - Go find alias information....
                        }
                    }
                }
            }
        }

        void ParseAccessListByType(ParserArgs args, AccessList acl)
        {
            switch (acl.Type)
            {
                case AccessListType.Standard:
                    ParseStandardAccesList(acl, args);
                    break;
                case AccessListType.Extended:
                    ParseExtendedAccessList(acl, args);
                    break;
                case AccessListType.Remark:
                    ParseRemarksAccessList(acl, args);
                    break;
                default:
                    break;
            }
        }

        void ParseStandardAccesList(AccessList acl, ParserArgs parseArgs)
        {
            foreach(var line in acl.LinesToProcess.Skip(1))
            {
                IPAddress address;

                if (IPAddress.TryParse(line, out address))
                {
                    if (acl.SourceIPGroup.IPAddress == null)
                        acl.SourceIPGroup.IP = address;
                    else if (acl.SourceIPGroup.Subnet == null)
                        acl.SourceIPGroup.Subnet = address;
                }
                else
                {
                    //TOOD: Figure out if there are other conditions I should be aware of...
                    if (line == AccessList.HostTag)
                        continue;
                    else if (string.IsNullOrEmpty(acl.SourceIPGroup.IPWildCard))
                        acl.SourceIPGroup.IPWildCard = line;
                }
            }
        }

        void ParseExtendedAccessList(AccessList acl, ParserArgs parseArgs)
        {

        }

        void ParseRemarksAccessList(AccessList acl, ParserArgs parseArgs)
        {
            var value = string.Join(" ", acl.LinesToProcess);
            acl.Comments.Append(string.Format("\"{0}\"", value));
        }

        void ParseAccessListLinesIntoGroups(ParserArgs args)
        {
            var aclCollections = new List<AccessListCollection>();
            int sequence = 0;

            foreach (var line in args.Lines)
            {
                var split = line.Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var name = split[1];
                var stringType = split[2];
                var stringPermission = split[3];
                var originalName = string.Join(" ", AccessList.AccessListTag, name);
                var collection = aclCollections.Where(x => x.Name == name).FirstOrDefault();
                var accessList = new AccessList()
                {
                    Name = name,
                    OriginalName = originalName,
                    Sequence = sequence++,
                    Permission = FindAccessListPermission(stringPermission),
                    LinesToProcess = split.Skip(3)
                };

                try
                {
                    accessList.Type = FindAccessListType(stringType);
                }
                catch(Exception)
                {
                    if (args.Results.CompletionSeverity != ResultSeverity.Errors)
                        args.Results.CompletionSeverity = ResultSeverity.Warnings;
                    args.Results.Messages.Add(string.Format("Unknown ACL Type '{0}' for Access-list '{1}'. This line will be skipped.", stringType, name));

                    continue;
                }

                if (collection != null)
                    collection.AccessLists.Add(accessList);
                else
                {
                    collection = new AccessListCollection()
                    {
                        Name = name,
                        OriginalName = originalName
                    };
                    collection.AccessLists.Add(accessList);
                    aclCollections.Add(collection);
                }
            }

            args.Results.AccessListResults.AddRange(aclCollections);
        }

        AccessListType FindAccessListType(string value)
        {
            var trimmedValue = value.Trim();

            if (CompareIfMatch(trimmedValue, AccessListType.Extended))
                return AccessListType.Extended;
            else if (CompareIfMatch(trimmedValue, AccessListType.Remark))
                return AccessListType.Remark;
            else if (CompareIfMatch(trimmedValue, AccessListType.Standard))
                return AccessListType.Standard;
            else
                throw new Exception(string.Format("Unknown access type '{0}'", value));
        }

        AccessListPermission FindAccessListPermission(string value)
        {
            var trimmedValue = value.Trim();

            if (CompareIfMatch(value, AccessListPermission.Deny))
                return AccessListPermission.Deny;
            else if (CompareIfMatch(value, AccessListPermission.Permit))
                return AccessListPermission.Permit;
            else
                return AccessListPermission.None;
        }

        bool CompareIfMatch(string value, AccessListType type, bool ignoreCase = true)
        {
            return (string.Compare(value, type.ToString(), ignoreCase) == 0);
        }

        bool CompareIfMatch(string value, AccessListPermission type, bool ignoreCase = true)
        {
            return (string.Compare(value, type.ToString(), ignoreCase) == 0);
        }
        #endregion
    }

    public class ParserArgs
    {
        public ParserArgs(IEnumerable<string> lines)
        {
            Lines = lines;
        }

        public string CurrentItem { get; set; }
        public string CurrentEvent { get; set; }
        public IEnumerable<string> Lines { get; set; }

        ParseResult _results = new ParseResult();
        public ParseResult Results { get { return _results; } set { _results = value; } }
    }
}
