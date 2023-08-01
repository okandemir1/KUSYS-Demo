namespace KUSYS.Business.Filters
{
    //Filtere uygulanırken standart verilerin tutulduğu alandır
    public class FilterModelBase
    {
        public int Page { get; set; }
        public int PageLimit { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string OrderBy { get; set; }
        public string OrderByDescending { get; set; } = "";

        public FilterModelBase(DataTableParameters dataTableParameters)
        {
            if (dataTableParameters.Order.Count > 0)
            {
                OrderBy = dataTableParameters.Columns[dataTableParameters.Order[0].Column].Name + " " +
                    dataTableParameters.Order[0].Dir;
            }
            else
            {
                OrderBy = "Id DESC ";
            }

            if (dataTableParameters.Start > 1)
            {
                Page = dataTableParameters.Start / dataTableParameters.Length;
                Page += 1; // sayfalar 1 den başlıyor
            }
            if (dataTableParameters.Length > 1)
                PageLimit = dataTableParameters.Length;

        }

        public FilterModelBase()
        {
            Page = 1;
            PageLimit = 0;
            OrderBy = "";
        }
    }
}
