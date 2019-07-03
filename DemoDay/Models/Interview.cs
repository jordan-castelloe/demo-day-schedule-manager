using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoDay.Models
{
    public class Interview
    {
        public int Id { get; set; }

        public int TimeSlotId {get; set;}

        public TimeSlot TimeSlot { get; set; }

        public Ranking Ranking { get; set; }

        public int RankingId { get; set; }
    }
}
