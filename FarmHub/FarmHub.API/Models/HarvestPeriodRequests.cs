using System;

namespace FarmHub.API.Models
{
    public class HarvestPeriodCreateRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime StartOrderDate { get; set; }
        public DateTime LastOrderDate { get; set; }
        public DateTime StartCommitmentDate { get; set; }
        public DateTime LastCommitmentDate { get; set; }
        public DateTime DispatchDate { get; set; }

        public HarvestPeriodCreateRequest(string name)
        {
            Name = name;
        }

        public HarvestPeriodCreateRequest()
        {

        }
    }


    public class HarvestPeriodUpdateRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; } = false;

        public DateTime? StartOrderDate { get; set; }
        public DateTime? LastOrderDate { get; set; }

        public DateTime? StartCommitmentDate { get; set; }
        public DateTime? LastCommitmentDate { get; set; }

        public DateTime? DispatchDate { get; set; }

        public HarvestPeriodUpdateRequest()
        {
            
        }
    }
}