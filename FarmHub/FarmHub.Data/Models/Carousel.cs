using System.Collections.Generic;

namespace FarmHub.Data.Models
{
    public class Carousel : ModelBase
    {
        public Carousel()
        {
            
        }
        
        public Carousel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public virtual ICollection<CarouselItem>? Items { get; set; }
        public CarouselType Type { get; set; }
    }

    public enum CarouselType
    {
        HomePage
    }
}