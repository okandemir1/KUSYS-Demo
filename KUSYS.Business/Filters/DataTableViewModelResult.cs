namespace KUSYS.Business.Filters
{
    public class DataTableViewModelResult<T>
    {
        public T Data { get; set; }
        public int TotalCount { get; set; }
        public int RecordsFiltered { get; set; }
        public bool IsSucceeded { get; set; }
    }
}
