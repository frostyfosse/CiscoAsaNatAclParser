using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CiscoAsaNetAclParser
{
    public enum ResultSeverity
    {
        None,
        Errors,
        Warnings
    }

    public class ParseResult
    {
        public ParseResult()
        {
            CompletionSeverity = ResultSeverity.None;
        }

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
        public ResultSeverity CompletionSeverity { get; set; }
        public string Title { get; set; }

        #region Helper methods and properties
        public enum OutputResultType
        {
            AccessList,
            ObjectNetwork
        }

        public IEnumerable<string> GetCommaDelimitedResults(OutputResultType type)
        {
            switch (type)
            {
                case OutputResultType.AccessList:
                    return GetAccessListResults();
                case OutputResultType.ObjectNetwork:
                    return GetObjectNetworkResults();
                default:
                    throw new Exception(string.Format("'{0}' is an unexpected output result type. Please contact developer to handle this.", type));
            }
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

        IEnumerable<string> GetObjectNetworkResults()
        {
            var lines = new List<string>();

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
            "Header Prefix (Command Type)", "Object Name", "Host IP (Internal)", "Subnet (Optional)",
            "Host or Subnet Alias", "Nat () Statement", "Nat Type", "Nat IP", "Nat Alias", "Nat Ports",
            "Description", "Comments"
        };
        #endregion

        #region AccessList items
        IEnumerable<string> GetAccessListResults()
        {
            var lines = new List<string>();

            lines.Add(GetAccessListHeaders());

            foreach(var aclCollection in AccessListResults)
            {
                foreach(var acl in aclCollection.AccessLists)
                {
                    var line = string.Join(",", acl.Sequence, AccessList.AccessListTag, acl.Name, null, null, acl.Type.ToString().ToLower(),
                                                acl.Permission == AccessListPermission.None ? null : acl.Permission.ToString().ToLower(), acl.Protocol, 
                                                acl.SourceType, acl.SourceIPGroup.IPAddress, acl.SourceIPGroup.Subnet, acl.SourceIPGroup.IPAlias, acl.SourceIPGroup.Port1, acl.SourceIPGroup.Port2,
                                                acl.DestinationType, acl.DestinationIPGroup.IPAddress, acl.DestinationIPGroup.SubnetAddress, acl.DestinationIPGroup.IPAlias, acl.DestinationIPGroup.Port1, acl.DestinationIPGroup.Port2,
                                                acl.PortMatchType, acl.HitCount, string.Format("\"{0}\"",acl.Comments.ToString().Replace("\"", null)));

                    lines.Add(line);
                }
            }

            return lines;
        }

        List<AccessListCollection> _accessListResults;

        public List<AccessListCollection> AccessListResults
        {
            get
            {
                if (_accessListResults == null)
                    _accessListResults = new List<AccessListCollection>();

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
            "Sequence", "Header", "ACL Name", "Line", "Line_Number", "ACL Type", "Permission", "Protocol",
            "Source Type", "Source IP", "Source Subnet", "Source Alias", "Source Port 1", "Source Port 2",
            "Destination Type", "Destination IP", "Destination Subnet", "Destination Alias", "Destination Port 1", "Destination Port 2",
            "Port Match Type", "Hit Count", "Comments"
        };
        #endregion
        #endregion
    }
}
