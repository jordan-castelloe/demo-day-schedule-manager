using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DemoDay.Models.ViewModels
{
    public class CreateRankingViewModel
    {

        public List<SelectListItem> Companies { get; set; }

        public Student Student { get; set; }

        [Display(Name ="First Place")]
        public Ranking Place1 { get; set; }

        [Display(Name = "Second Place")]
        public Ranking Place2 { get; set; }

        [Display(Name = "Third Place")]
        public Ranking Place3 { get; set; }

        [Display(Name = "Fourth Place")]
        public Ranking Place4 { get; set; }

        [Display(Name = "Fifth Place")]
        public Ranking Place5 { get; set; }



    }
}
