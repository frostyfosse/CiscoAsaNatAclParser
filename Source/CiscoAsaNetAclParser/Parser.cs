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

        List<ObjectNetwork> _objectNetworks;
        public List<ObjectNetwork> ObjectNetworks
        {
            get
            {
                if (_objectNetworks == null)
                    _objectNetworks = new List<ObjectNetwork>();

                return _objectNetworks;
            }
            set
            {
                _objectNetworks = value;
            }
        }

        public ParseResult Parse(string[] lines)
        {
            var result = new ParseResult();

            CollectObjectNetworkChecklist(lines);

            //Processing the actual object network detail
            foreach (var objectNetwork in ObjectNetworks)
            {
                foreach(var index in objectNetwork.Indices)
                {
                    var skippedGroup = lines.Skip(index);
                    bool firstLine = true;

                    foreach (var line in skippedGroup)
                    { 
                        if (line == objectNetwork.OriginalName)
                        {
                            firstLine = false;
                            continue;
                        }
                        else if (line != objectNetwork.OriginalName && firstLine)
                            throw new Exception(string.Format("Collection out of sync for parsing. Attempted to collect field data for '{0}', but received data for '{1}'.", objectNetwork.OriginalName, line));
                        else if (line.Contains(ObjectNetwork.ObjectNetworkTag) && !firstLine) //Found the end of the configuration
                            break;

                        if (line.Contains(ObjectNetwork.HostTag))
                            GetHost(objectNetwork, line);
                        else if (line.Contains(ObjectNetwork.SubnetTag))
                            GetSubnet(objectNetwork, line);
                        else if (line.Contains(ObjectNetwork.NatTag))
                        {

                        }
                    }
                }
            }

            result.Failed = false;
            result.Title = "Completed task";
            result.Results = ObjectNetworks;

            return result;
        }

        private static void GetHost(ObjectNetwork objectNetwork, string line)
        {
            string lineParsed = line.Replace(ObjectNetwork.HostTag, null).Trim();
            IPAddress hostIp = null;
            if (IPAddress.TryParse(lineParsed, out hostIp))
                objectNetwork.Ip = hostIp;
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
                        objectNetwork.Ip = subnetIp;
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

        void CollectObjectNetworkChecklist(IEnumerable<string> lines)
        {
            var objectNetworkReferences = lines.Where(x => x.Contains(ObjectNetwork.ObjectNetworkTag));

            int index = 0;

            //Collecting all the starting points for the object networks
            foreach (var header in objectNetworkReferences)
            {
                if (ObjectNetworks.Any(x => x.OriginalName == header))
                {
                    ObjectNetworks.Where(x => x.OriginalName == header).First().Indices.Add(index);
                    continue;
                }

                ObjectNetworks.Add(new ObjectNetwork()
                {
                    Name = header.Replace(ObjectNetwork.ObjectNetworkTag, null).Trim(),
                    OriginalName = header
                });

                index++;
            }
        }
    }
}
