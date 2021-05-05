using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmHub.Data.Models
{
    public abstract class ModelBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}