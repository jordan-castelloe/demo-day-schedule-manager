﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DemoDay.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        public int CohortId { get; set; }

        public Cohort Cohort { get; set; }

        [Display(Name ="Can this student relocate outside of Huntington or Charleston?")]
        public bool canRelocate { get; set; }

        [Display(Name ="Does this student have a bachelors degree?")]
        public bool hasBachelorsDegree { get; set; }
    }
}
