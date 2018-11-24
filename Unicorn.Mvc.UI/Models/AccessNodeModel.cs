using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn.Mvc.UI.Models
{
    public class AccessNodeModel
    {
        public string Action { get; set; }
        public string Title { get; set; }
        public NodeAccessType Access { get; set; }
        public bool HasChildren { get; set; }
    }
    public enum NodeAccessType
    {
        HasAccess = 0,
        NoAccess = 1,
        SubNodes = 2
    }

    public class AccessNodeSaveModel : AccessNodeModel
    {
        public List<AccessNodeSaveModel> SubActions { get; set; }
    }
}
