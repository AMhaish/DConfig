using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SABFramework.Core
{
    internal class SABModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(IAction) && ((SABActionControllerBase)controllerContext.Controller).ActionInfo  !=null && ((SABActionControllerBase)controllerContext.Controller).ActionInfo.ActionType!=null)
            {
                var extensionedBindingContext = new ModelBindingContext
                {
                    ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, ((SABActionControllerBase)controllerContext.Controller).ActionInfo.ActionType),
                    ModelState = bindingContext.ModelState,
                    PropertyFilter = bindingContext.PropertyFilter,
                    ValueProvider = bindingContext.ValueProvider
                };
                return base.BindModel(controllerContext, extensionedBindingContext);
            }
            else
            {
                return base.BindModel(controllerContext, bindingContext);
            }
        }
    }
}
