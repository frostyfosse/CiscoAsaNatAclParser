using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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



        #region Helper methods and properties
        public enum OutputResultType
        {
            AccessList,
            ObjectNetwork
        }

        public List<string> GetCommaDelimitedResults(OutputResultType type)
        {
            var lines = new List<string>();

            switch (type)
            {
                case OutputResultType.AccessList:
                    GetAccessListResults(lines);
                    break;
                case OutputResultType.ObjectNetwork:
                    GetObjectNetworkResults(lines);
                    break;
                default:
                    throw new Exception(string.Format("'{0}' is an unexpected output result type. Please contact developer to handle this.", type));
            }

            return lines;
        }

        #region ObjectNetwork items
        List<ObjectNetwork> _objectNetworkResults;

        public List<ObjectNetwork> ObjectNetworkResults
        {
            get
            {
                if (_objectNetworkResults == null)
                    _objectNetworkResults = new List<ObjectNetwork>();

                return _objectNetworkResults;
            }
            set
            {
                _objectNetworkResults = value;
            }
        }

        List<string> GetObjectNetworkResults(List<string> lines)
        {
            lines.Add(GetObjectNetworkHeaders());

            foreach (var result in ObjectNetworkResults)
            {
                var line = string.Join(",", new[]
                {
                    ObjectNetwork.ObjectNetworkTag,
                    result.Name,
                    result.IPGroup.IP != null ? result.IPGroup.IP.ToString() : null,
                    result.IPGroup.Subnet != null ? result.IPGroup.Subnet.ToString() : null,
                    result.IPGroup.IPAlias,
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

        string GetObjectNetworkHeaders()
        {
            return string.Join(",", _objectNetworkHeaders);
        }

        string[] _objectNetworkHeaders = new[]
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

        #region AccessList items
        List<string> GetAccessListResults(List<string> lines)
        {
            //throw new NotImplementedException();

            return lines;
        }

        List<AccessList> _accessListResults;

        public List<AccessList> AccessListResults
        {
            get
            {
                if (_accessListResults == null)
                    _accessListResults = new List<AccessList>();

                return _accessListResults;
            }
            set
            {
                _accessListResults = value;
            }
        }

        string GetAccessListHeaders()
        {
            return string.Join(",", _accessListHeaders);
        }

        string[] _accessListHeaders = new[]
        {
            ""
        };
        #endregion
        #endregion
    }
}
