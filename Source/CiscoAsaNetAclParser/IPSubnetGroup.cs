using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CiscoAsaNetAclParser
{
    public class IPSubnetGroup
    {
        public string IPAlias { get; set; }

        [XmlIgnore]
        public IPAddress IP { get; set; } //This will include the ip configured for either host or subnet. Only one will exist per object network configuration.

        public string IPAddress
        {
            get
            {
                if (!string.IsNullOrEmpty(IPWildCard))
                    return IPWildCard;
                else if (IP != null)
                    return IP.ToString();
                else
                    return null;
            }
            set { }
        }

        public string IPWildCard { get; set; }

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

        public string Port1 { get; set; }
        public string Port2 { get; set; }

        public bool HasMinimumIPCriteria
        {
            get
            {
                if (IP == null &&
                    string.IsNullOrEmpty(IPWildCard) &&
                    string.IsNullOrEmpty(IPAlias))
                    return false;
                else
                    return true;
            }
        }

        public bool HasAlias
        {
            get
            {
                if (string.IsNullOrEmpty(IPAlias))
                    return false;
                else
                    return true;
            }
        }
    }
}
