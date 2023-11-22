using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Repositories.AppsServices;
using DConfigOS_Core.Repositories.IOServices;
using DConfigOS_Core.Repositories.WebsiteContentServices;

namespace DConfigOS_Core
{
    public class DIInitializer : NinjectModule
    {
        public override void Load()
        {
            Bind<IAppsExtentionsAPI>().To<AppsExtentionsAPI>();
            Bind<IAppAPIRepository>().To<AppsAPI>();
            Bind<IPackagesAPI>().To<PackagesAPI>();
            Bind<IWidgetsAPIRepository>().To<WidgetsAPIRepository>();
            //Bind<IFoldersAPI>().To<FoldersAPI>();
            Bind<IContentsAPI>().To<ContentsAPI>();
            Bind<IFormsAPI>().To<FormsAPI>();
            Bind<IFormFieldsAPI>().To<FormFieldsAPI>();
            Bind<IFormsFieldsEnumsAPI>().To<FormsFieldsEnumsAPI>();
            Bind<IFormsInstancesAPI>().To<FormsInstancesAPI>();
            Bind<IFormSubmitEventsAPI>().To<FormSubmitEventsAPI>();
            Bind<IScriptsAPI>().To<ScriptsAPI>();
            Bind<IScriptsBundlesAPI>().To<ScriptsBundlesAPI>();
            Bind<IStylesAPI>().To<StylesAPI>();
            Bind<IStylesBundlesAPI>().To<StylesBundlesAPI>();
            Bind<IStagesAPI>().To<StagesAPI>();
            Bind<ITemplatesAPI>().To<TemplatesAPI>();
            Bind<IViewFieldsAPI>().To<ViewFieldsAPI>();
            Bind<IViewFieldsEnumsAPI>().To<ViewFieldsEnumsAPI>();
            Bind<IViewTypesAPI>().To<ViewTypesAPI>();
            Bind<IFoldersAPI>().To<AzureBlobsAPI>();
        }
    }
}
