using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.Entity.Core.Objects;

namespace SABFramework.ModulesUtilities
{
    public static class DbContextExtensions
    {
        public static void Filter<TContext, TParentEntity, TCollectionEntity>(this TContext context, Expression<Func<TContext, IDbSet<TParentEntity>>> path, Expression<Func<TParentEntity, ICollection<TCollectionEntity>>> collection, Expression<Func<TCollectionEntity, Boolean>> filter)
            where TContext : DbContext
            where TParentEntity : class, new()
            where TCollectionEntity : class
        {
            (context as IObjectContextAdapter).ObjectContext.ObjectMaterialized += delegate(Object sender, ObjectMaterializedEventArgs e)
            {
                if (e.Entity is TParentEntity)
                {
                    String navigationProperty = collection.ToString().Split('.')[1];
                    DbCollectionEntry col = context.Entry(e.Entity).Collection(navigationProperty);
                    col.CurrentValue = new FilteredCollection<TCollectionEntity>(null, col, filter);
                }
            };
        }
        //See more at: http://weblogs.asp.net/ricardoperes/filter-collections-automatically-with-entity-framework-code-first#sthash.TfY2wARd.dpuf
    }
}
