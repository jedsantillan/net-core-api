namespace FarmHub.Data.Models
{
    public class Inventory : ModelBase
    {
        public int ProductId { get; set; }
        public int HarvestPeriodId { get; set; }
        public int FarmerAssociationId { get; set; }

        public Product Product { get; set; }
        public HarvestPeriod HarvestPeriod { get; set; }
        public FarmerAssociation FarmerAssociation { get; set; }

        public float Amount { get; set; }
    }
}