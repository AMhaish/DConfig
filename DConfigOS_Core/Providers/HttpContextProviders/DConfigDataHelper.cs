using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Providers.DataContexts;
using DConfigOS_Core.Models;
using System.Linq.Expressions;
using System.Data.Entity;
using Nest;
using LinqKit;

namespace DConfigOS_Core.Providers.HttpContextProviders
{
    public static class DConfigDataHelper
    {
        private const int CONDITIONS_THRESHOLD = 30;

        public static ICollection<DConfigModel> Search
            (this DConfigModel model,
            string targetLanguage,
            List<int> viewTypeIds,
            List<SearchCondition> conditions,
            SortingRules? sortBy,
            int? limit = null,
            int? skip = null,
            int? root = null,
            List<int?> roots = null
            )
        {
            IQueryable<Content> query = SearchQuery(targetLanguage, viewTypeIds, conditions,root,roots);
            //Applying ordering rules
            if (sortBy.HasValue)
            {
                switch (sortBy)
                {
                    case SortingRules.ByDate_Asc:
                        query = query.OrderBy(m => m.CreatedDate);
                        break;
                    case SortingRules.ByDate_Dec:
                        query = query.OrderByDescending(m => m.CreatedDate);
                        break;
                    case SortingRules.ByName_Asc:
                        query = query.OrderBy(m => m.Name);
                        break;
                    case SortingRules.ByName_Dec:
                        query = query.OrderByDescending(m => m.Name);
                        break;
                    case SortingRules.ByPriority_Asc:
                        query = query.OrderBy(m => m.Priority);
                        break;
                    case SortingRules.ByPriority_Dec:
                        query = query.OrderByDescending(m => m.Priority);
                        break;
                    case SortingRules.Random:
                        query = query.OrderBy(m => Guid.NewGuid());
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(m => m.CreatedDate);
            }
            //Executing query and Building dConfig models
            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }
            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }
            var results = query.ToList();
            List<DConfigModel> models = new List<DConfigModel>();
            foreach (Content c in results)
            {
                models.Add(DConfigModel.BuildDConfigModel(c));
            }
            //Applying fields ordering rules
            foreach (var condition in conditions.OrderBy(m => m.SortPriority))
            {
                if (condition.SortPriority.HasValue)
                {
                    if (condition.Asc == true)
                    {
                        models = models.OrderBy(m => m[condition.FieldId]).ToList();
                    }
                    else
                    {
                        models = models.OrderByDescending(m => m[condition.FieldId]).ToList();
                    }
                }
            }
            return models;
        }

        public static int Count
            (this DConfigModel model,
            string targetLanguage,
            List<int> viewTypeIds,
            List<SearchCondition> conditions,
            int? root = null,
            List<int?> roots = null
            )
        {
            /*DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            string queryBuilder = @"SELECT COUNT(Distinct C.Id) from Contents C inner join ContentInstances CI on C.Id=CI.ContentId";
            if (conditions != null && conditions.Count > 0)
            {
                queryBuilder += " inner join ViewFieldValues VFV on VFV.ContentId = CI.Id AND VFV.FieldId in (" + string.Join(",", conditions.Select(m => m.FieldId.ToString())).TrimEnd(',') + ") ";
            }
            queryBuilder += "WHERE CI.language='" + targetLanguage + @"'";
            if (viewTypeIds != null && viewTypeIds.Count > 0)
            {
                queryBuilder += " AND C.viewTypeId in (" + string.Join(",", viewTypeIds.Select(m => m.ToString())).TrimEnd(',') + ") ";
            }
            if (conditions != null && conditions.Count > 0)
            {
                queryBuilder += " AND (";
                foreach (var condition in conditions.Where(m => m.Context == ConditionContext.Must || m.Context == ConditionContext.Should))
                {
                    queryBuilder += "VFV.Value LIKE '%" + condition.Target + "%' OR ";
                }
                foreach (var condition in conditions.Where(m => m.Context == ConditionContext.MustNot || m.Context == ConditionContext.ShouldNot))
                {
                    queryBuilder += "VFV.Value NOT LIKE '%" + condition.Target + "%' OR ";
                }
                queryBuilder.TrimEnd(' ', 'R', 'O');
                queryBuilder += ")";
            }
            return context.Database.SqlQuery<int>(queryBuilder).First() / conditions.Count();*/
            IQueryable<Content> query = SearchQuery(targetLanguage, viewTypeIds, conditions, root, roots);
            return query.Count();
        }

        private static IQueryable<Content> SearchQuery(
            string targetLanguage,
            List<int> viewTypeIds,
            List<SearchCondition> conditions,
            int? root = null,
            List<int?> roots = null)
        {
            //Elastic search query if avialable
            //############################We need to implement parent id condition in Elastic search call
            bool elasticResult = false;
            ISearchResponse<ContentInstance> search = null;
            if (conditions != null && conditions.Count >= CONDITIONS_THRESHOLD)
            {
                DConfigOS_Core_ElasticSearchContext elasticContext = new DConfigOS_Core_ElasticSearchContext();
                QueryContainer qq = new QueryContainer();
                foreach (var condition in conditions.Where(m => m.Context == ConditionContext.Must || m.Context == ConditionContext.MustNot))
                {
                    QueryContainer sqq = new TermQuery { Field = new Field("values.fieldId"), Value = condition.FieldId.ToString() };
                    switch (condition.Type)
                    {
                        case ConditionType.Match:
                            sqq = sqq || new MatchQuery() { Field = new Field("values.value"), Query = condition.Target.ToLower() };
                            break;
                        case ConditionType.Term:
                            sqq = sqq || new TermQuery() { Field = new Field("values.value"), Value = condition.Target.ToLower() };
                            break;
                    }
                    switch (condition.Context)
                    {
                        case ConditionContext.Must:
                            qq = qq || sqq;
                            break;
                        case ConditionContext.MustNot:
                            qq = qq || !sqq;
                            break;
                    }
                }
                //search = elasticContext.ContentInstancesValues.Search(limit, skip, m => m.Nested(n => n.Path(p => p.FieldsValues).Query(sq => qq)));
            }
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            //Basic query
            IQueryable<Content> query = null;
            if (conditions != null && conditions.Count >= CONDITIONS_THRESHOLD && search != null && search.IsValid && search.Documents != null && search.Documents.Count > 0)
            {
                elasticResult = true;
                var elasticItems = search.Documents.AsEnumerable<ContentInstance>().Select(m => m.ContentId).ToList();
                query = context.Contents.AsNoTracking().Include(m => m.ContentInstances.Select(mm => mm.FieldsValues))
                    .Where(m => m.Online == true && m.ContentInstances.FirstOrDefault(ci => ci.Language == targetLanguage) != null && (!root.HasValue || m.ParentId == root.Value) && elasticItems.Contains(m.Id));
            }
            else
            {
                if (viewTypeIds != null && viewTypeIds.Count > 0)
                {
                    query = context.Contents.AsNoTracking().Include(m => m.ContentInstances.Select(mm => mm.FieldsValues))
                        .Where(m => m.Online == true && m.ContentInstances.FirstOrDefault(ci => ci.Language == targetLanguage) != null && viewTypeIds.Contains(m.ViewTypeId.Value) && (!root.HasValue || m.ParentId == root.Value));
                }
                else
                {
                    query = context.Contents.AsNoTracking().Include(m => m.ContentInstances.Select(mm => mm.FieldsValues))
                        .Where(m => m.Online == true && m.ContentInstances.FirstOrDefault(ci => ci.Language == targetLanguage) != null && (!root.HasValue || m.ParentId == root.Value));
                }
                if (roots != null && roots.Count > 0)
                {
                    query = query.Where(m => roots.Contains(m.ParentId));
                }
            }
            //Adding where conditions
            var predicate = PredicateBuilder.New<Content>();
            if (!elasticResult)
            {
                foreach (var condition in conditions)
                {
                    Expression<Func<ViewFieldValue, bool>> typePredicate = null;
                    switch (condition.Type)
                    {
                        case ConditionType.Match:
                            typePredicate = m => m.Value.ToLower().Contains(condition.Target.ToLower());
                            break;
                        case ConditionType.Term:
                            typePredicate = m => m.Value == condition.Target;
                            break;
                        case ConditionType.BiggerThan:
                            typePredicate = m => double.Parse(m.Value) > double.Parse(condition.Target);
                            break;
                        case ConditionType.SmallerThan:
                            typePredicate = m => double.Parse(m.Value) < double.Parse(condition.Target);
                            break;
                    }
                    switch (condition.Context)
                    {
                        case ConditionContext.Must:
                            query = query.Where(m =>
                               m.ContentInstances.FirstOrDefault(ci => ci.Language == targetLanguage) != null &&
                               m.ContentInstances.FirstOrDefault(ci => ci.Language == targetLanguage).FieldsValues.Where(f => f.FieldId == condition.FieldId).AsQueryable().Any(typePredicate)
                            );
                            break;
                        case ConditionContext.MustNot:
                            query = query.Where(m =>
                               m.ContentInstances.FirstOrDefault(ci => ci.Language == targetLanguage) != null &&
                               !m.ContentInstances.FirstOrDefault(ci => ci.Language == targetLanguage).FieldsValues.Where(f => f.FieldId == condition.FieldId).AsQueryable().Any(typePredicate)
                            );
                            break;
                        //http://www.albahari.com/nutshell/predicatebuilder.aspx Later check for Or conditions
                        case ConditionContext.Should:
                            predicate = predicate.Or(m =>
                                m.ContentInstances.FirstOrDefault(ci => ci.Language == targetLanguage) != null &&
                                m.ContentInstances.FirstOrDefault(ci => ci.Language == targetLanguage).FieldsValues.Where(f => f.FieldId == condition.FieldId).AsQueryable().Any(typePredicate)
                            );
                            break;
                        case ConditionContext.ShouldNot:
                            predicate = predicate.Or(m =>
                                m.ContentInstances.FirstOrDefault(ci => ci.Language == targetLanguage) != null &&
                                !m.ContentInstances.FirstOrDefault(ci => ci.Language == targetLanguage).FieldsValues.Where(f => f.FieldId == condition.FieldId).AsQueryable().Any(typePredicate)
                            );
                            break;
                    }
                }
            }
            if (conditions.Any(m => m.Context == ConditionContext.Should || m.Context == ConditionContext.ShouldNot))
            {
                query = query.Where(predicate);
            }
            return query;
        }
    }

    public class SearchCondition
    {
        public int FieldId { get; set; }
        public ConditionContext Context { get; set; }
        public ConditionType Type { get; set; }
        public string Target { get; set; }
        public int? SortPriority { get; set; }
        public bool Asc { get; set; }
    }

    public enum ConditionContext { Must, Should, MustNot, ShouldNot };

    public enum ConditionType
    {
        Match, Match_Phrase, Match_Phrase_Prefix, Multi_Match, //For full text queries
        BiggerThan, SmallerThan, //Numerical comparision (The fields should be numericals)
        Term, Terms, Terms_Set, Range, Exists, Prefix, Wildcard, Regexp, Fuzzy //For terms level queries
    };

    public enum SortingRules
    {
        ByDate_Asc, ByDate_Dec, ByName_Asc, ByName_Dec, ByPriority_Asc, ByPriority_Dec, Random
    };
}
