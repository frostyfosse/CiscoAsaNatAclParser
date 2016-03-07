using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiscoAsaNetAclParser
{
    public class AccessListGroup : AclObjectBase
    {
         public List<AccessList> AccessLists { get; set; }
    }
}
