using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CiscoAsaNetAclParser
{
    public abstract class AclObjectBase
    {
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
        public const string AccessListTag = "access-list";

        //Aliases are names to other Object Network objects which contain the ip address needed to be referenced.

        public string Name { get; set; }
        public string OriginalName { get; set; }
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
    }
}
