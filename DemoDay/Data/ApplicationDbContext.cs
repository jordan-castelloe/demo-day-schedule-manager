using System;
using System.Collections.Generic;
using System.Text;
using DemoDay.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemoDay.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cohort> Cohort { get; set; }
        public DbSet<Company> Company { get; set; }

        public DbSet<Interview> Interview { get; set; }

        public DbSet<Ranking> Ranking { get; set; }

        public DbSet<Student> Student { get; set; }

        public DbSet<TimeSlot> TimeSlot { get; set; }
    }
}
