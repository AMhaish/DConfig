using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DConfigOS_Core.Layer2.ActionsModels
{
    public enum ContentsTreeNodeType { Page, Partial, Redirect, Download, ContentVersion, Container, Item };
    
    public class TreeNodeModel
    {
        public string id { get; set; }
        public string text { get; set; }
        public string state { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ContentsTreeNodeType type { get; set; }
        public List<TreeNodeModel> children { get; set; }
        public Object obj { get; set; }
        public Object addObj { get; set; }
        public bool plentyChildren { get; set; }
    }

    public class TreeNodeModel_HasChildren
    {
        public string id { get; set; }
        public string text { get; set; }
        public string state { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ContentsTreeNodeType type { get; set; }
        public bool children { get; set; }
        public Object obj { get; set; }
        public Object addObj { get; set; }
        public bool plentyChildren { get; set; }
    }
}
