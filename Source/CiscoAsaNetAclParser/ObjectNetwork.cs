using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace CiscoAsaNetAclParser
{
    public class ObjectNetwork
    {
        public ObjectNetwork()
        {
        }

        /*
        Object Network: The below are the only options you will ever see
          fqdn     	Enter this keyword to specify an FQDN (Not really used according to Charles)
          host     	Enter this keyword to specify a single host object
          nat      	Enable NAT on a singleton object
          range    	Enter this keyword to specify a range (Not really used according to Charles)
          subnet   	Enter this keyword to specify a subnet
        */

        public const string ObjectNetworkTag = "object network";
        public const string HostTag = "host";
        public const string SubnetTag = "subnet";
        public const string NatTag = "nat";
        public const string DescriptionTag = "description";

        //Aliases are names to other Object Network objects which contain the ip address needed to be referenced.

        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string IPAlias { get; set; }
        public IPAddress Ip { get; set; } //This will include the ip configured for either host or subnet. Only one will exist per object network configuration.
        public IPAddress Subnet { get; set; }
        public IPAddress NatIp { get; set; }
        public string NatIpAlias { get; set; }
        public string NatType { get; set; } //Example: static, dynamic
        public string NatStatement { get; set; } //List of strings in a parenthesis. Example: (inside, outside)
        public string Description { get; set; }

        /// <summary>
        /// A list of the index numbers where the header name was referenced. This is used later to collect the field level data
        /// </summary>
        List<int> _indices;
        public List<int> Indices
        {
            get
            {
                if (_indices == null)
                    _indices = new List<int>();

                return _indices;
            }
            set
            {
                _indices = value;
            }
        }

        public List<KeyValuePair<string, string>> AdditionalColumnValues { get; set; }
    }
}
