using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoDay.Models.ViewModels
{
    public class CompanyEditViewModel
    {
        public Company Company { get; set; }

        public List<SelectListItem> TimeSlots {get; set;}

        public List<int> TimeSlotIds { get; set; }
    }
}
