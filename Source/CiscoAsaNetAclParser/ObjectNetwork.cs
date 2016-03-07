using System;
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
    public enum ObjectNetworkTagOption
    {
        ObjectNetwork,
        Host,
        Subnet,
        Nat,
        Description,
        None
    }

    [Serializable]
    public class ObjectNetwork : AclObjectBase
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

        IPSubnetGroup _ipGroup;
        public IPSubnetGroup IPGroup
        {
            get
            {
                if (_ipGroup == null)
                    _ipGroup = new IPSubnetGroup();

                return _ipGroup;
            }
            set
            {
                _ipGroup = value;
            }
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
