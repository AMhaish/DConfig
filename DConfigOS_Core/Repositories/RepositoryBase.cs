using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.Providers;

namespace DConfigOS_Core.Repositories
{
    public class RepositoryBase<Context> : IDisposable where Context : IDBContext, new()
    {
        private Context _DataContext;

        public virtual Context context
        {
            get
            {
                if (_DataContext == null)
                {
                    _DataContext = new Context();
                }
                return _DataContext;
            }
        }

        public void Dispose()
        {
            _DataContext?.Dispose();
        }
    }
}
