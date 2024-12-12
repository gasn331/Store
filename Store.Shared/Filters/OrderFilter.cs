namespace Store.Shared.Filters
{
    public class OrderFilter
    {
        public bool? IsClosed { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
