﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace CiscoAsaNetAclParser
{
    public enum TagOption
    {
        ObjectNetwork,
        Host,
        Subnet,
        Nat,
        Description,
        None
    }

    [XmlRoot("ObjectNetworks")]
    [Serializable]
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

        //Values that can be identified at the end of the Object Network configuration collection
        public const string ObjectGroupNetworkTag = "object-group network";
        public const string ObjectServiceTag = "object service";
        public const string ObjectTag = "object";
        public const string AccessGroupTag = "access-group";

        //Aliases are names to other Object Network objects which contain the ip address needed to be referenced.

        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string IPAlias { get; set; }

        [XmlIgnore]
        public IPAddress IP { get; set; } //This will include the ip configured for either host or subnet. Only one will exist per object network configuration.

        public string IPAddress
        {
            get
            {
                if (IP == null)
                    return null;
                else
                    return IP.ToString();
            }
            set { }
        }

        [XmlIgnore]
        public IPAddress Subnet { get; set; }

        public string SubnetAddress
        {
            get
            {
                if (Subnet == null)
                    return null;
                else
                    return Subnet.ToString();
            }
            set { }
        }

        [XmlIgnore]
        public IPAddress NatIP { get; set; }

        public string NatIPAddress
        {
            get
            {
                if (NatIP == null)
                    return null;
                else
                    return NatIP.ToString();
            }
            set { }
        }

        public string NatIPAlias { get; set; }
        public string NatType { get; set; } //Example: static, dynamic
        public string NatStatement { get; set; } //List of strings in a parenthesis. Example: (inside, outside)

        List<string> _natPorts;
        public List<string> NatPorts //Anything listed after the NatIP or NatAlias (Whichever was provided)
        {
            get
            {
                if (_natPorts == null)
                    _natPorts = new List<string>();

                return _natPorts;
            }
            set
            {
                _natPorts = value;
            }
        }
        public string Description { get; set; }

        StringBuilder _comments;

        [XmlIgnore]
        public StringBuilder Comments
        {
            get
            {
                if (_comments == null)
                    _comments = new StringBuilder();

                return _comments;
            }
            set
            {
                _comments = value;
            }
        }

        /// <summary>
        /// A list of the index numbers where the header name was referenced. This is used later to collect the field level data
        /// </summary>
        List<int> _indices;

        [XmlIgnore]
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

        List<KeyValuePair<string, string>> _additionalColumnValues;
        public List<KeyValuePair<string, string>> AdditionalColumnValues
        {
            get
            {
                if (_additionalColumnValues == null)
                    _additionalColumnValues = new List<KeyValuePair<string, string>>();

                return _additionalColumnValues;
            }
            set
            {
                _additionalColumnValues = value;
            }
        }
    }
}
