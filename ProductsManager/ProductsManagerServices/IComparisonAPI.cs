using CompetitiveAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompetitiveAnalysis.ProductsManagerServices
{
    public interface IComparisonAPI : IDisposable
    {
        List<Comparison> GetComparisons();
        Comparison GetComparison(int Id);
        int CreateComparison(Comparison t, List<ComparisonFilter> Filters);
        int UpdateComparison(Comparison t, List<ComparisonFilter> Filters);
        int DeleteComparison(int Id);
        int UpdateComparisonProducts(int comparisonId, List<int> productsIds);

    }
}
