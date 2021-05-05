using FarmHub.Data.Models;
using Microsoft.AspNetCore.Http;

namespace FarmHub.API.Models
{
    public class ImageRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile UploadFile { get; set; }
        public ImageTypeEnum ImageType { get; set; }
        public int? ProductId { get; set; }


        public ImageRequest(IFormFile uploadFile, ImageTypeEnum imageType)
        {
            UploadFile = uploadFile;
            ImageType = imageType;
        }

        public ImageRequest()
        {
        }
    }

}
