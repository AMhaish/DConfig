using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Models;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DConfigOS_Core.Providers.HttpContextProviders
{
    public class DConfigFormModel
    {
        public DConfigFormModel() { ModelState = new ModelStateDictionary(); }
        [JsonIgnore]
        public Form PageForm { get; set; }
        public string UniqueFormName { get; set; }
        public string ErrorSummary { get; set; }
        public Dictionary<int, string> ModelFieldsValues { get; set; }
        [JsonIgnore]
        public Dictionary<int, string> PageFormFieldsValues { get; set; }
        [JsonIgnore]
        public Dictionary<int, string> PageFormFieldsErrors { get; set; }
        public string PageUrl { get; set; }
        public List<DConfigFormModel> ChildrenModels { get; set; }
        [JsonIgnore]
        public ModelStateDictionary ModelState { get; set; }
        public bool IsValid { get; set; }
    }
}
