namespace FarmHub.API.Models
{
    public class FarmerAssociationCreateRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public string ContactNo { get; set; }
        public string? Email { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public FarmerAssociationCreateRequest()
        {

        }
    }

    public class FarmerAssociationUpdateRequest
    {
        public string? Name { get; set; }
        public string? Address { get; set; }

        public string? ContactNo { get; set; }
        public string? Email { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public FarmerAssociationUpdateRequest()
        {

        }
    }
}
