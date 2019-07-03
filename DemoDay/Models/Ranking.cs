using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoDay.Models
{
    public class Ranking
    {
        public int Id { get; set; }

        public int Rank { get; set; }

        public Student Student { get; set; }

        public int StudentId { get; set; }

        public Company Company { get; set; }

        public int CompanyId { get; set; }
    }
}
