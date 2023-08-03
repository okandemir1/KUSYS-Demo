using KUSYS.Dto;
using KUSYS.Model;

namespace KUSYS.Business.Filters
{
    public class StudentCourseFilterModel : FilterModelBase
    {
        public string Term { get; set; }

        public StudentCourseFilterModel(DataTableParameters dataTableParameters)
            : base(dataTableParameters)
        {
            if (dataTableParameters.Search?.Value?.Length > 0)
                Term = dataTableParameters.Search.Value;
        }
    }
    public static partial class FilterExtensions
    {
        public static IQueryable<Student> AddSearchFilters(this IQueryable<Student> input, StudentCourseFilterModel filter)
        {
            if (filter != null)
            {
                if (filter.Term?.Length > 0)
                {
                    input = input.Where(x => x.FirstName.Contains(filter.Term) || x.LastName.Contains(filter.Term));
                }
            }

            return input;
        }
    }
}
