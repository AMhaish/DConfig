using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core.DataCore;

namespace SABFramework.Core
{
    public class SABSettings
    {
        private Dictionary<string, string> _settingsObjects;

        public SABSettings(IEnumerable<SettingsRecord> settingsRecords)
        {
            _settingsObjects = new Dictionary<string, string>();
            foreach (SettingsRecord s in settingsRecords)
            {
                _settingsObjects.Add(s.Key, s.Value);
            }
        }

        public bool ContainsKey(string key)
        {
            return _settingsObjects.ContainsKey(key);
        }

        public string this[string key]
        {
            get
            {
                if (_settingsObjects.ContainsKey(key))
                {
                    return _settingsObjects[key];
                }
                return null;
            }
        }

        public void UpdateKey(string key,string value)
        {
            if (_settingsObjects.ContainsKey(key))
            {
                _settingsObjects[key] = value;
            }
            else
            {
                _settingsObjects.Add(key, value);
            }
        }

        public const string SABSettings_CDN = "CDN";
        public const string SABSettings_Domain = "Domain";
    }
}
