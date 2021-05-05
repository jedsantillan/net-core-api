using System.ComponentModel.DataAnnotations;

namespace FarmHub.Data.Models
{
    public class Image : ModelBase
    {
        [MaxLength(250)]
        public string ImageUrl { get; set; }
        public ImageTypeEnum ImageType { get; set; }

        [MaxLength(250)]
        public string? Title { get; set; }

        public int? ProductId { get; set; }
        public virtual Product? Product{ get; set; }

        public Image(string imageUrl, ImageTypeEnum imageType)
        {
            ImageUrl = imageUrl;
            ImageType = imageType;
        }

        public Image()
        {

        }
    }

    public enum ImageTypeEnum
    {
        ProductImage,
        ProfileImage,
        OtherImage
    }

}
