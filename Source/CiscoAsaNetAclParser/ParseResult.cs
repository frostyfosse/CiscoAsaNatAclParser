using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiscoAsaNetAclParser
{
    public class ParseResult
    {
        List<string> _messages;
        public List<string> Messages
        {
            get
            {
                if (_messages == null)
                    _messages = new List<string>();

                return _messages;
            }
            set
            {
                _messages = value;
            }
        }
        public bool Failed { get; set; }
        public string Title { get; set; }

        List<ObjectNetwork> _results;
        public List<ObjectNetwork> Results
        {
            get
            {
                if (_results == null)
                    _results = new List<ObjectNetwork>();

                return _results;
            }
            set
            {
                _results = value;
            }
        }


        #region Helper methods and properties
        public List<string> GetCommaDelimitedResults()
        {
            var lines = new List<string>();

            lines.Add(GetHeaders());

            foreach(var result in Results)
            {
                var line = string.Join(",", new[]
                {
                    ObjectNetwork.ObjectNetworkTag,
                    result.Name,
                    result.IP != null ? result.IP.ToString() : null,
                    result.Subnet != null ? result.Subnet.ToString() : null,
                    result.IPAlias,
                    result.NatStatement,
                    result.NatType,
                    result.NatIP != null ? result.NatIP.ToString() : null,
                    result.NatIPAlias,
                    string.Join(" ", result.NatPorts),
                    result.Description,
                    result.Comments.ToString().Replace("\r\n", null)
                });

                lines.Add(line);
            }

            return lines;
        }

        string GetHeaders()
        {
            return string.Join(",", _headers);
        }

        string[] _headers = new[]
        {
            "Header Prefix (Command Type)",
            "Object Name",
            "Host IP (Internal)",
            "Subnet (Optional)",
            "Host or Subnet Alias",
            "Nat () Statement",
            "Nat Type",
            "Nat IP",
            "Nat Alias",
            "Nat Ports",
            "Description",
            "Comments"
        };
        #endregion
    }
}
