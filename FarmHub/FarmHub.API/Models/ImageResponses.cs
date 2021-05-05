namespace FarmHub.API.Models
{
    public class ImageViewResponse
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string ImageType { get; set; }
        public int? ProductId { get; set; }
    }
}
