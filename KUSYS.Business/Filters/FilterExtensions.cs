using System.Linq.Dynamic.Core;

namespace KUSYS.Business.Filters
{
    //Tüm filtreleme işlemleri buradan geçerek sayfalama işlemi yapılır
    public static partial class FilterExtensions
    {
        public static IQueryable<TSource> AddOrderAndPageFilters<TSource>(this IQueryable<TSource> input, FilterModelBase filter)
        {
            if (filter != null)
            {
                if (filter.OrderBy.Length > 0)
                {
                    input = input.OrderBy(filter.OrderBy);
                }
                else if (filter.OrderByDescending.Length > 0)
                {
                    input = input.OrderBy(filter.OrderByDescending + " DESC");
                }

                if (filter.Skip > 0 && filter.Take > 0)
                {
                    return input
                        .SkipIf(true, filter.Skip)
                        .TakeIf(true, filter.Take);
                }
                return input
                    .SkipIf(filter.Page > 0, (filter.Page - 1) * filter.PageLimit)
                    .TakeIf(filter.PageLimit > 0, filter.PageLimit);
            }
            return input;
        }
    }
}
