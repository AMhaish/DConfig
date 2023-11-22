using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using DConfigOS_Core.Models;
using System.Web.Configuration;
using System.Threading;

namespace DConfigOS_Core.Providers.DataContexts
{
    public class DConfigOS_Core_ElasticSearchContext
    {
        protected readonly IElasticClient Client;
        public bool ContextObjectsFree { get; set; }
        public int? ContextCompanyId { get; set; }
        private const string ContentInstancesIndex = "instances";
        public ElasticSearchDbSet<ContentInstance> ContentInstancesValues { get; set; }

        public DConfigOS_Core_ElasticSearchContext(int contextCompanyId) : this(false, contextCompanyId) { }

        public DConfigOS_Core_ElasticSearchContext(bool contextFree) : this(contextFree, null) { }

        public DConfigOS_Core_ElasticSearchContext() : this(false, null) { }

        public DConfigOS_Core_ElasticSearchContext(bool contextObjectsFree, int? contextCompanyId) : this(WebConfigurationManager.AppSettings["ElasticHost"], WebConfigurationManager.AppSettings["ElasticPort"], contextObjectsFree, contextCompanyId) { }

        public DConfigOS_Core_ElasticSearchContext(string serverPath, string port, bool contextObjectsFree, int? contextCompanyId)
        {
            var isFiddlerRunning = Process.GetProcessesByName("fiddler").Any();
            var host = isFiddlerRunning ? "ipv4.fiddler" : serverPath;
            var node = new Uri("http://" + host + ":" + port);
            ConnectionSettings settings = new ConnectionSettings(node);
            settings.DefaultIndex(ContentInstancesIndex)
                .DefaultMappingFor<ContentInstance>(m => m.IndexName(ContentInstancesIndex).TypeName(ContentInstancesIndex))
                .DefaultMappingFor<ViewFieldValue>(m => m.IndexName(ContentInstancesIndex).TypeName(ContentInstancesIndex)).RequestTimeout(TimeSpan.FromMilliseconds(300));
            if (isFiddlerRunning)
            {
                settings.PrettyJson();
            }
            Client = new ElasticClient(settings);
            ContextCompanyId = contextCompanyId;
            ContextObjectsFree = contextObjectsFree;
            ContentInstancesValues = new ElasticSearchDbSet<ContentInstance>(Client, ContentInstancesIndex, ContextObjectsFree, ContextCompanyId,
                m => m.Term(f => f.Field(ff => ff.ContextCompanyId).Value(ContextCompanyId))
                //m.Nested(mm => mm.Path(f => f.Field).Query(f => f.Term(ff => ff.Field(s => s.Field.ViewTypeId).Value(ContextCompanyId))))
                );
            ContentInstancesValues.ErrorHandler = HandleElasticSearchError;
            CreateIndexes();
        }

        protected void HandleElasticSearchError(string message)
        {
            new Thread(() =>
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.SendEmailNotification("Exception in elastic search call", message);
            }).Start();
        }

        public virtual void CreateIndexes()
        {
            ContentInstancesValues.CreateIndex();
        }

        public static void InitializeIndexes()
        {
            new Thread(() =>
            {
                DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext(true);
                DConfigOS_Core_ElasticSearchContext elasticContext = new DConfigOS_Core_ElasticSearchContext(true);
                int count = context.ViewFieldValue.Count();
                int pageSize = 1000;
                for (int i = 0; i < count / pageSize; i++)
                {
                    var result = elasticContext.ContentInstancesValues.BulkInsert(context.ContentInstances.OrderBy(m => m.Id).Skip(i * pageSize).Take(pageSize).ToList());
                    if (!result.IsValid)
                    {
                        if (SABFramework.Core.SABCoreEngine.Instance.ErrorHandler != null)
                        {
                            SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError(result.ServerError.Error.Reason);
                        }
                        break;
                    }
                }
            }).Start();
        }
    }

    public class ElasticSearchDbSet<TEntity> where TEntity : class
    {
        public readonly string IndexName;
        public bool ContextObjectsFree { get; set; }
        public int? ContextCompanyId { get; set; }
        Func<QueryContainerDescriptor<TEntity>, QueryContainer> ContextCompanyFilter { get; set; }
        protected readonly IElasticClient Client;
        public Action<string> ErrorHandler { get; set; }

        public ElasticSearchDbSet(IElasticClient client, string indexName, bool contextObjectsFree, int? contextCompanyId, Func<QueryContainerDescriptor<TEntity>, QueryContainer> contextCompanyFilter)
        {
            IndexName = indexName;
            Client = client;
            ContextCompanyFilter = contextCompanyFilter;
            ContextCompanyId = contextCompanyId;
            ContextObjectsFree = contextObjectsFree;
        }

        public ICreateIndexResponse CreateIndex()
        {
            var response = Client.IndexExists(IndexName);
            if (!response.IsValid)
            {
                ErrorHandler(response.DebugInformation);
                return null;
            }
            if (response.Exists)
            {
                return null;
            }
            var response2 = Client.CreateIndex(IndexName, index =>
                index.Mappings(ms =>
                    ms.Map<TEntity>(x => x.AutoMap())));
            if (!response2.IsValid)
            {
                ErrorHandler(response.DebugInformation);
            }
            return response2;
        }

        public void Index(TEntity documentObj)
        {
            new Thread(() =>
            {
                var addDocResponse = Client.IndexDocument<TEntity>(documentObj);
                if (!addDocResponse.IsValid)
                {
                    ErrorHandler(addDocResponse.DebugInformation);
                }
            }).Start();
        }

        public void UnIndex(TEntity documentObj)
        {
            new Thread(() =>
            {
                var removeDocResponse = Client.Delete<TEntity>(documentObj);
                if (!removeDocResponse.IsValid)
                {
                    ErrorHandler(removeDocResponse.DebugInformation);
                }
            }).Start();
        }

        public IBulkResponse BulkInsert(IEnumerable<TEntity> entities)
        {
            var request = new BulkDescriptor();
            foreach (var entity in entities)
            {
                request
                    .Index<TEntity>(op => op
                        .Id(Guid.NewGuid().ToString())
                        .Index(IndexName)
                        .Document(entity))
                        .RequestConfiguration(r => r.RequestTimeout(TimeSpan.FromMinutes(7)));
            }
            var response = Client.Bulk(request);
            if (!response.IsValid)
            {
                ErrorHandler(response.DebugInformation);
            }
            return response;
        }

        public ISearchResponse<TEntity> Search(int? limit, int? skip, Func<QueryContainerDescriptor<TEntity>, QueryContainer> query, string[] filterPath = null)
        {
            if (ContextObjectsFree)
                return Client.Search<TEntity>(m => m.Index(IndexName).From(skip).Size(limit).Query(query));
            else if (ContextCompanyId.HasValue)
                return Client.Search<TEntity>(m => m.Index(IndexName).From(skip).Size(limit).Query(ContextCompanyFilter).Query(query));
            else if (!SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.CurrentUserIsAdministrator && SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.ContextCompanyId.HasValue)
                return Client.Search<TEntity>(m => m.Index(IndexName).From(skip).Size(limit).Query(ContextCompanyFilter).Query(query));
            else
                return Client.Search<TEntity>(m => m.Index(IndexName).From(skip).Size(limit).Query(query));
        }
    }
}
