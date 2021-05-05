#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FarmHub.Data.Models
{
    public class HarvestPeriod : ModelBase
    {
        [MaxLength(100)]
        public string Name { get; set; }
        
        [MaxLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = false;
        
        
        public DateTime StartOrderDate { get; set; }
        public DateTime LastOrderDate { get; set; }

        public DateTime StartCommitmentDate { get; set; }
        public DateTime LastCommitmentDate { get; set; }

        public DateTime DispatchDate { get; set; }
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

        public HarvestPeriod(string name)
        {
            Name = name;
        }
    }
}