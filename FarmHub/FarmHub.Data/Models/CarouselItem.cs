namespace FarmHub.Data.Models
{
    public class CarouselItem : ModelBase
    {
        public CarouselItem()
        {
            
        }
        public CarouselItem(string imageUrl)
        {
            ImageUrl = imageUrl;
        }

        public int Id { get; set; }
        public int CarouselId { get; set; }
        public string ImageUrl { get; set; }
        public string? ImageRedirectUrl { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? ButtonCaption { get; set; }
        public string? ButtonLinkUrl { get; set; }

        public virtual Carousel? Carousel { get; set; }
    }
}