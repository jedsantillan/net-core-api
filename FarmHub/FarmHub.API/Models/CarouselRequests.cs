using FarmHub.Data.Models;

namespace FarmHub.API.Models
{
    public class CarouselRequest
    {
        public string Name { get; set; }
        public CarouselType Type { get; set; }
        public CarouselItemRequest[] Items { get; set; }
    }
    public class CarouselItemRequest
    {
        public string ImageRedirectUrl { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ButtonCaption { get; set; }
        public string ButtonLinkUrl { get; set; }
        public string ImageUrl { get; set; }
    }
}