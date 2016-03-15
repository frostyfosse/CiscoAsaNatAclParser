using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiscoAsaNetAclParser
{
    public enum ObjectGroupType
    {
        [Description("network")]
        Network,
        [Description("protocol")]
        Protocol,
        [Description("service")]
        Service
    }

    public enum ObjectGroupFieldType
    {
        Protocol,
        Port,
        NetworkObject,

    }

    public class ObjectGroup : AclObjectBase
    {
        /*
        object-group possible patterns from their ObjectGroupType
        1. network includes:
            1. network-object with <subnet>, <host (Which could be a dns name>, or <object>
            2. description <comment>
        2. protocol includes protocol-object (udp, tcp, ftp, etc...)
        3. service (After the name includes the protocol)
            1. port-object with <PortMatchType> & <port> (### or alias)
            2. group-object with <alias name to object-group service>
            3. service-object: Parameters:
                1. protocol 
                    1. [destination (literal)] <PortMatchType> & <Port>
                    2. Unknown alias (example "icmp", is this a protocol?)s
                2. [object (object-group service reference)] <alias>
        */

        public ObjectGroupType Type { get; set; }
        
    }
}
