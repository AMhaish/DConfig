using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Models;
using System.Collections;
using System.Reflection;

namespace DConfigOS_Core.Providers.HttpContextProviders
{
    public abstract class DConfigView<T> : System.Web.Mvc.WebViewPage<T> where T : DConfigModel
    {
        public abstract override void Execute();

        protected override void InitializePage()
        {
            if (Model != null)
            {
                var model = (DConfigModel)Model;
                if (model.ActiveContentInstance != null)
                {
                    var template = model.ActiveContentInstance.ViewTemplate;
                    while (template.LayoutTemplate != null && TemplateName != template.Name)
                    {
                        template = template.LayoutTemplate;
                    }
                    while (template.LayoutTemplate != null && template.LayoutTemplate.IsContainer)
                    {
                        template = template.LayoutTemplate;
                    }
                    if (template.LayoutTemplate != null)
                        Layout = template.LayoutTemplate.Path;
                }
            }
            base.InitializePage();
        }

        public string TemplateName
        {
            get
            {
                var start = VirtualPath.LastIndexOf('/') + 1;
                var end = VirtualPath.IndexOf(".cshtml");
                return VirtualPath.Substring(start, end - start);
            }
        }
    }


}
