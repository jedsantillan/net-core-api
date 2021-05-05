using System.Collections.Generic;

namespace FarmHub.Data.Models
{
    public class Tag: ModelBase
    {
        public Tag(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public ICollection<Product> Products { get; set; }
    }
}