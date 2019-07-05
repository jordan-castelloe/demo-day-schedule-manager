using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DemoDay.Models
{
    public class Company
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name="Is this company in Huntington or Charleston?")]
        public bool isLocal { get; set; }

        [Display(Name = "Does this company require a bachelors degree?")]
        public bool requiresBachelorsDegree { get; set; }


    }
}
