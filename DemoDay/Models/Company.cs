using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoDay.Models
{
    public class Company
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool isLocal { get; set; }

        public bool requiresBachelorsDegree { get; set; }


    }
}
