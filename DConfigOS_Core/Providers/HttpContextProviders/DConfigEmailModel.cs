using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Providers.HttpContextProviders
{
    public class DConfigEmailModel: DConfigModel
    {
        public List<DConfigEmailModel> SubModels { get; set; }
        public DConfigEmailModel():base()
        {

        }
    }
}
