namespace FarmHub.API.Models
{
    public class CartItemModel
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public string ProductId { get; set; }
        public string Text { get; set; }
        public double Price { get; set; }
        public double DiscountedPrice { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Portion { get; set; }
        public double RealDecimalValue { get; set; }
    }
}