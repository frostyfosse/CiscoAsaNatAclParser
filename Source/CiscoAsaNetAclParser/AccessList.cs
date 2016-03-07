using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiscoAsaNetAclParser
{
    public class AccessList : AclObjectBase
    {
        /// <summary>
        /// The order in which to display the access list rule
        /// </summary>
        public int Sequence { get; set; }
        public string SourceType { get; set; }
        public IPSubnetGroup SourceIPGroup { get; set; }
        public string DestinationType { get; set; }
        public IPSubnetGroup DestinationIPGroup { get; set; }

        public string PortMatchType { get; set; }

        List<string> _ports;
        public List<string> Ports
        {
            get
            {
                if (_ports == null)
                    _ports = new List<string>();

                return _ports;
            }
            set
            {
                _ports = value;
            }
        }

        public string HitCount { get; set; }
    }
}
