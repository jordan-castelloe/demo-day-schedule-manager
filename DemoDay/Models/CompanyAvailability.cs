using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoDay.Models
{
    public class CompanyAvailability
    {
        public int Id { get; set; }

        public Company Company { get; set; }

        public int CompanyId { get; set; }

        public TimeSlot TimeSlot {get; set;}

        public int TimeSlotId { get; set; }



    }
}
