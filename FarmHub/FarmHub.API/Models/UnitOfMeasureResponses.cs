namespace FarmHub.API.Models
{
    public class UnitOfMeasureViewResponse
    {
        public string Name { get; set; }
        public string? ShortName { get; set; }
        public bool IsDecimal { get; set; }
        public int Id { get; set; }
    }
}
