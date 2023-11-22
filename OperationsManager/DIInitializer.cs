using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.Core;
using OperationsManager.OperationsManagerServices;

namespace NotificationsManager
{
    public class DIInitializer : NinjectModule
    {
        public override void Load()
        {
            Bind<IOperationsInstancesAPI>().To<OperationsInstancesAPI>();
            Bind<IOperationsAPI>().To<OperationsAPI>();
        }
    }
}
