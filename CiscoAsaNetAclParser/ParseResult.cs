using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiscoAsaNetAclParser
{
    public class ParseResult
    {
        public List<string> Messages { get; set; }
        public bool Failed { get; set; }
        public string Title { get; set; }
    }
}
