using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SABFramework.PreDefinedModules.MembershipModule.Models
{
    public class FilteredDbSet<TEntity> : DbSet<TEntity>, IDbSet<TEntity>, IOrderedQueryable<TEntity>, IOrderedQueryable, IQueryable<TEntity>, IQueryable, IEnumerable<TEntity>, IEnumerable, IListSource 
       where TEntity : SABCoreEntity
    {
        protected readonly DbSet<TEntity> Set;
        protected readonly IQueryable<TEntity> FilteredSet;
        protected readonly Action<TEntity> InitializeEntity;
        protected DbContext currentContext;
        protected int? ContextCompanyId;
        protected bool ContextFree;

        public FilteredDbSet(DbContext context,int? contextCompanyId,bool contextFree) : this(context, contextCompanyId, contextFree, context.Set<TEntity>(), n => true, null) { }

        public FilteredDbSet(DbContext context, int? contextCompanyId, bool contextFree, Expression<Func<TEntity, bool>> filter) : this(context, contextCompanyId, contextFree, context.Set<TEntity>(), filter, null) { }

        public FilteredDbSet(DbContext context,  int? contextCompanyId, bool contextFree, Expression<Func<TEntity, bool>> filter, Action<TEntity> initializeEntity): this(context, contextCompanyId, contextFree, context.Set<TEntity>(), filter, initializeEntity){}

        protected FilteredDbSet(DbContext context, int? contextCompanyId, bool contextFree, DbSet<TEntity> set, Expression<Func<TEntity, bool>> filter, Action<TEntity> initializeEntity)
        {
            this.currentContext = context;
            this.ContextCompanyId = contextCompanyId;
            this.ContextFree = contextFree;
            Set = set;
            if(ContextFree)
                FilteredSet = set.Where(filter);
            else if (ContextCompanyId.HasValue)
                FilteredSet = set.Where(m =>  m.ContextCompanyId == ContextCompanyId.Value).Where(filter);
            else if(MembershipProvider.Instance!=null && MembershipProvider.Instance.ContextCompanyId.HasValue)
                FilteredSet = set.Where(m =>  m.ContextCompanyId == MembershipProvider.Instance.ContextCompanyId).Where(filter);
            else
                FilteredSet = set.Where(filter);
            MatchesFilter = filter.Compile();
            InitializeEntity = initializeEntity;
        }

        public Func<TEntity, bool> MatchesFilter { get; private set; }

        public void ThrowIfEntityDoesNotMatchFilter(TEntity entity)
        {
            if (!MatchesFilter(entity))
                throw new ArgumentOutOfRangeException();
        }

        public override TEntity Add(TEntity entity)
        {
            DoInitializeEntity(entity);
            ThrowIfEntityDoesNotMatchFilter(entity);
            return Set.Add(entity);
        }

        public override TEntity Attach(TEntity entity)
        {
            ThrowIfEntityDoesNotMatchFilter(entity);
            return Set.Attach(entity);
        }

        public override TDerivedEntity Create<TDerivedEntity>()
        {
            var entity = Set.Create<TDerivedEntity>();
            DoInitializeEntity(entity);
            return (TDerivedEntity) entity;
        }

        public override TEntity Create()
        {
            var entity = Set.Create();
            DoInitializeEntity(entity);
            return entity;
        }

        public override TEntity Find(params object[] keyValues)
        {
            var entity = Set.Find(keyValues);
            if (entity == null)
                return null;

            // If the user queried an item outside the filter, then we throw an error.
            // If IDbSet had a Detach method we would use it...sadly, we have to be ok with the item being in the Set.
            ThrowIfEntityDoesNotMatchFilter(entity);
            return entity;
        }

        public override TEntity Remove(TEntity entity)
        {
            ThrowIfEntityDoesNotMatchFilter(entity);
            return Set.Remove(entity);
        }

        /// <summary>
        /// Returns the items in the local cache
        /// </summary>
        /// <remarks>
        /// It is possible to add/remove entities via this property that do NOT match the filter.
        /// Use the <see cref="ThrowIfEntityDoesNotMatchFilter"/> method before adding/removing an item from this collection.
        /// </remarks>
        public override ObservableCollection<TEntity> Local { get { return Set.Local; } }

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator() { return FilteredSet.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return FilteredSet.GetEnumerator(); }

        Type IQueryable.ElementType { get { return typeof(TEntity); } }

        Expression IQueryable.Expression { get { return FilteredSet.Expression; } }

        IQueryProvider IQueryable.Provider { get { return FilteredSet.Provider; } }

        bool IListSource.ContainsListCollection { get { return false; } }

        ObservableCollection<TEntity> IDbSet<TEntity>.Local
        {
            get
            {
                return this.Set.Local;
            }
        }

        IList IListSource.GetList() { throw new InvalidOperationException(); }

        void DoInitializeEntity(TEntity entity)
        {
            InitializeEntity?.Invoke(entity);
        }

        
    }

}
