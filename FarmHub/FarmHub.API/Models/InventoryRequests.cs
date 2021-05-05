namespace FarmHub.API.Models
{
    public class InventoryCreateRequest
    {
        public int ProductId { get; set; }
        public int HarvestPeriodId { get; set; }
        public int FarmerAssociationId { get; set; }

        public float Amount { get; set; }

        public InventoryCreateRequest()
        {

        }
    }

    public class InventoryUpdateRequest
    {
        public int? ProductId { get; set; }
        public int? HarvestPeriodId { get; set; }
        public int? FarmerAssociationId { get; set; }

        public float? Amount { get; set; }

        public InventoryUpdateRequest()
        {

        }
    }

}
