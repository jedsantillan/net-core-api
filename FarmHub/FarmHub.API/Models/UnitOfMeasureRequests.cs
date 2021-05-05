namespace FarmHub.API.Models
{
    public class UnitOfMeasureCreateRequest
    {
        public string Name { get; set; }
        public string? ShortName { get; set; }
        public bool IsDecimal { get; set; } = true;

        public UnitOfMeasureCreateRequest(string name)
        {
            Name = name;
        }

        public UnitOfMeasureCreateRequest()
        {
            
        }
    }
    
    public class UnitOfMeasureUpdateRequest
    {
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public bool? IsDecimal { get; set; }
    }
}