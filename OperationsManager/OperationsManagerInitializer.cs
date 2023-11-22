using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.Core;
using System.Data.Entity;
using System.Timers;
using OperationsManager.Models;
using OperationsManager.OperationsManagerServices;
using DConfigOS_Core.Repositories.Utilities;

namespace OperationsManager
{
    public class OperationsManagerInitializer : IInitializer
    {
        const double interval30Minutes = 60 * 30  * 1000;

        public IOperationsInstancesAPI OperationsInstancesAPI { get; set; }
        public void Initialize()
        {
            //Database.SetInitializer(new SABFramework.ModulesUtilities.CreateTablesOnlyIfTheyDontExist<OperationsManager_DBContext>());
            Timer invoicesTimer = new Timer(interval30Minutes);
            invoicesTimer.Elapsed += new ElapsedEventHandler(Invoices_Timer);
            invoicesTimer.Enabled = true;
        }

        protected void Invoices_Timer(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now.Hour == 0)
            {
                CreateOperationsInstances();
            }
        }

        protected void CreateOperationsInstances()
        {
            OperationsInstancesAPI.CreateOperationsInstances();

        }
    }
}
