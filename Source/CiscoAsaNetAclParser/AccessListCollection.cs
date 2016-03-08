using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiscoAsaNetAclParser
{
    public class AccessListCollection : AclObjectBase
    {
        List<AccessList> _accessLists = new List<AccessList>();
        public List<AccessList> AccessLists { get { return _accessLists; } set { _accessLists = value; } }
    }
}
