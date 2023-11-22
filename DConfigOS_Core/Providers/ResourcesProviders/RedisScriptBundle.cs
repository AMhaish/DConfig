using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Optimization;
using System.Configuration;
using StackExchange.Redis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Hosting;
using System.IO;
using System.Web.Compilation;
using System.Web.Util;
using System.Reflection;


namespace DConfigOS_Core.Providers.ResourcesProviders
{
    public class RedisScriptBundle : ScriptBundle
    {
        
        public RedisScriptBundle(string virtualPath)
        : base(virtualPath)
        {
        }

        public RedisScriptBundle(string virtualPath, string cdnPath)
            : base(virtualPath, cdnPath)
        { }

        public override BundleResponse CacheLookup(BundleContext context)
        {
            BundleResponse response = null;
            try
            {
                //IDatabase cache = Connection.GetDatabase();
                //string serializedTeams = cache.StringGet(context.BundleVirtualPath);
                //if (!String.IsNullOrEmpty(serializedTeams))
                //{
                //    JsonSerializerSettings settings = new JsonSerializerSettings();
                //    settings.Converters.Add(new RedisBundleResponseConverter());
                //    response = JsonConvert.DeserializeObject<RedisBundleResponse>(serializedTeams, settings);
                //}

              
            }
            catch (Exception ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Problem with Cache connection", ex);
            }
            return response;
        }

        public override void UpdateCache(BundleContext context, BundleResponse response)
        {
            try
            {
                IDatabase cache = Connection.GetDatabase();
                cache.StringSet(context.BundleVirtualPath, JsonConvert.SerializeObject(response));
            }
            catch(Exception ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Problem with Cache connection", ex);
            }
        }

        public static void ReinitializeCache(Models.ScriptsBundle bundle)
        {
            var key = "~/bundles/" + bundle.Name + bundle.ContextCompanyId.ToString();
            var oldBundle = BundleTable.Bundles.GetBundleFor(key);
            BundleTable.Bundles.Remove(oldBundle);
            var sb = new RedisScriptBundle(key);
            string[] scriptspath = bundle.Scripts.OrderBy(m => m.Priority).Select(m => m.Path).ToArray();
            sb.Include(scriptspath);
            BundleTable.Bundles.Add(sb);
        }

        public static void DeleteCache(Models.ScriptsBundle bundle)
        {
            IDatabase cache = Connection.GetDatabase();
            cache.KeyDelete("~/bundles/" + bundle.Name + bundle.ContextCompanyId.ToString());
            var key = "~/bundles/" + bundle.Name + bundle.ContextCompanyId.ToString();
            var oldBundle = BundleTable.Bundles.GetBundleFor(key);
            BundleTable.Bundles.Remove(oldBundle);
        }

        // Redis Connection string info
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = ConfigurationManager.AppSettings["CacheConnection"].ToString();
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }

    internal class RedisBundleResponse: BundleResponse
    {
        [JsonIgnore]
        public new IEnumerable<RedisBundleFile> Files { get; set; }
    }

    internal class RedisBundleFile : BundleFile
    {
        [JsonConstructor]
        public RedisBundleFile(string includedVirtualPath,VirtualFile virtualFile):base(includedVirtualPath, virtualFile)
        {
        }
    }

    internal class RedisBundleResponseConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(RedisBundleResponse));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load the JSON for the Result into a JObject
            JObject jo = JObject.Load(reader);

            // Read the properties which will be used as constructor parameters
            string content = (string)jo["Content"];
            string contentType= (string)jo["ContentType"];
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new RedisBundleFileConverter());
            RedisBundleFile[] bundleFiles = JsonConvert.DeserializeObject<RedisBundleFile[]>(jo["Files"].ToString(), settings);
            
            // Construct the Result object using the non-default constructor
            RedisBundleResponse result = new RedisBundleResponse();
            result.Content = content;
            result.ContentType = contentType;
            result.Files = bundleFiles;
            // (If anything else needs to be populated on the result object, do that here)

            // Return the result
            return result;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    internal class RedisBundleFileConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(RedisBundleFile));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load the JSON for the Result into a JObject
            JObject jo = JObject.Load(reader);

            // Read the properties which will be used as constructor parameters
            string includedVirtualPath = (string)jo["IncludedVirtualPath"];
            
            // Construct the Result object using the non-default constructor
            RedisBundleFile result = new RedisBundleFile(includedVirtualPath, null);

            // (If anything else needs to be populated on the result object, do that here)

            // Return the result
            return result;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

