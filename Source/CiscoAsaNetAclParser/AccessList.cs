using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiscoAsaNetAclParser
{
    public enum AccessListType
    {
        Standard,
        Extended,
        Remark
    }

    public enum AccessListPermission
    {
        Permit,
        Deny,
        None
    }

    public class AccessList : AclObjectBase
    {
        public const string PortEqualsTag = "eq";
        public const string PortRangeTag = "range";
        public const string PortIntervalTag = "interval";
        public static string[] MatchConditionSymbol = new[] { PortEqualsTag, PortRangeTag, PortIntervalTag };
        public const string HitCountPrefix = "hitcnt=";

        public AccessList()
        {
            SourceIPGroup = new IPSubnetGroup();
            DestinationIPGroup = new IPSubnetGroup();
        }

        /// <summary>
        /// The order in which to display the access list rule
        /// </summary>
        public int Sequence { get; set; }
        public string SourceType { get; set; }
        public IPSubnetGroup SourceIPGroup { get; set; }
        public string DestinationType { get; set; }
        public IPSubnetGroup DestinationIPGroup { get; set; }
        public AccessListType Type { get; set; }
        public AccessListPermission Permission { get; set; }
        public string Protocol { get; set; }
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

        public IEnumerable<string> LinesToProcess { get; set; }

        public bool HasAliases
        {
            get
            {
                if (SourceIPGroup.HasAlias || DestinationIPGroup.HasAlias)
                    return true;
                else
                    return false;
            }
        }
    }
}
