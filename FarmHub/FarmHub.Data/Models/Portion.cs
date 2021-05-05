using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FarmHub.Data.Models
{
   
    public class Portion : ModelBase
    {
        [MaxLength(30)]
        public string DisplayName { get; set; }
        public double RealDecimalValue { get; set; }
        public virtual ICollection<ProductPortion> ProductPortions { get; set; }

        public Portion(string displayName, double realDecimalValue)
        {
            DisplayName = displayName;
            RealDecimalValue = realDecimalValue;
        }
    }
}