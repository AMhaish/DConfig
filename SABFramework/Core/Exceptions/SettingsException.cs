using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SABFramework.Core.Exceptions
{
    [Serializable]
    public class SettingsException:Exception
    {
        public SettingsException(string settingRecordName) : base("Missing record in settings section of SAB config file, record name: " + settingRecordName) { }
    }
}
