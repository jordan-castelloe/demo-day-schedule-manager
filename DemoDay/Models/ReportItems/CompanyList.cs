using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoDay.Models.ReportItems
{
    public class CompanyList
    {
        public Company Company { get; set; }

        public Student Student { get; set; }

        public TimeSlot TimeSlot { get; set; }

        public List<Ranking> PriorityRankings { get; set; } = new List<Ranking>();

        public List<Interview> InterviewSchedule { get; set; } = new List<Interview>();
    }
}
