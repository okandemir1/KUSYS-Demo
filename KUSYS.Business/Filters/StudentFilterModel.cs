using KUSYS.Dto;

namespace KUSYS.Business.Filters
{
    public class StudentFilterModel : FilterModelBase
    {
        public string Term { get; set; }

        public StudentFilterModel(DataTableParameters dataTableParameters)
            : base(dataTableParameters)
        {
            if (dataTableParameters.Search?.Value?.Length > 0)
                Term = dataTableParameters.Search.Value;
        }
    }
    public static partial class FilterExtensions
    {
        public static IQueryable<StudentSimpleDto> AddSearchFilters(this IQueryable<StudentSimpleDto> input, StudentFilterModel filter)
        {
            if (filter != null)
            {
                if (filter.Term?.Length > 0)
                {
                    input = input.Where(x => x.Firstname.Contains(filter.Term) || x.Lastname.Contains(filter.Term));
                }
            }

            return input;
        }
    }
}
