using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoDay.Models.ReportItems
{
    public class CompanyList
    {
        public Company Company { get; set; }

        public List<Ranking> PriorityRankings { get; set; } = new List<Ranking>();
    }
}
