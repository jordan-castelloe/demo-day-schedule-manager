using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoDay.Data;
using DemoDay.Models;
using DemoDay.Models.ReportItems;

namespace DemoDay.Controllers
{
    public class TimeSlotsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimeSlotsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool StudentIsAvailable(Student student, TimeSlot time)
        {

            var scheduledInterviewsForStudent = _context.Interview.Where(i => i.Ranking.StudentId == student.Id).Include(i => i.TimeSlot).ToList();

            // Don't schedule more than five time slots per student
            if (scheduledInterviewsForStudent.Count() < 5)
            {
                var occupiedTimeSlotsForStudent = scheduledInterviewsForStudent.Select(i => i.TimeSlot);
                var openTimeSlotsForStudent = _context.TimeSlot.Where(ti => !occupiedTimeSlotsForStudent.Contains(ti));

                return openTimeSlotsForStudent.Contains(time);

            }
            return false;
        }

        private bool CompanyIsAvailable(Company company, TimeSlot time)
        {
            var scheduledInterviewsForCompany = _context.Interview.Where(i => i.Ranking.CompanyId == company.Id).Include(i => i.TimeSlot).ToList();
            var occupiedTimeSlotsForCompany = scheduledInterviewsForCompany.Select(i => i.TimeSlot);
            var availableTimeSlots = _context.CompanyAvailability.Where(c => c.CompanyId == company.Id).Select(ci => ci.TimeSlot).ToList();

            // Find the time slots when a) a company is generally available and b) they don't already have an interview scheduled
            var openTimeSlotsForCompany = _context.TimeSlot.Where(ti => !occupiedTimeSlotsForCompany.Contains(ti) && availableTimeSlots.Contains(ti));

            return openTimeSlotsForCompany.Contains(time);

        }

        // Returns true if an interview is already scheduled between a student and a company
        // Returns false if no interview has been scheduled
        private bool AlreadyScheduled(Student student, Company company)
        {
            var studentsInterviews = _context.Interview
                .Include(i => i.Ranking)
                    .ThenInclude(r => r.Company)
                .Where(i => i.Ranking.StudentId == student.Id)
                .ToList();

            var studentCompanies = studentsInterviews.Select(i => i.Ranking.Company);

            return studentCompanies.Contains(company);
        }

        private bool isLocationCompatible(Student student, Company company)
        {
            if (!company.isLocal)
            {
                return student.canRelocate;
            }

            return true;
        }

        private bool meetsRequirements(Student student, Company company)
        {
            if (company.requiresBachelorsDegree)
            {
                return student.hasBachelorsDegree;
            }

            return true;
        }

        private void scheduleInterview(Ranking rank, List<TimeSlot> allTimeSlots)
        {
           
            
            if (meetsRequirements(rank.Student, rank.Company) && !AlreadyScheduled(rank.Student, rank.Company))
            {
                var openSlot = allTimeSlots.FirstOrDefault(time => CompanyIsAvailable(rank.Company, time) && StudentIsAvailable(rank.Student, time));

                if (openSlot != null)
                {
                    var interview = new Interview()
                    {
                        TimeSlotId = openSlot.Id,
                        TimeSlot = openSlot,
                        RankingId = rank.Id,
                        Ranking = rank
                    };
                    _context.Add(interview);
                    _context.SaveChanges();

                }

            }
        }

        public async Task<IActionResult> ScheduleIndex(string sortBy)
        {
            // Wipe out the original interviews, we're going to generate new ones
            _context.Interview.RemoveRange(_context.Interview);

            await _context.SaveChangesAsync();

            // delete this later, this is just for testing
            var allInterviews = await _context.Interview.ToListAsync();

            // Todo: change cohort dynamically
            var rankings = await _context.Ranking
                .Include(r => r.Student)
                .Include(r => r.Company)
                .OrderBy(r => r.Company.Name)
                .ThenBy(r => r.Rank)
                .ToListAsync();

            var groupedRankings = (from r in rankings
                                   group r by r.Company.Name into gr
                                   select new CompanyList()
                                   {
                                       Company = gr.ToList()[0].Company,
                                       PriorityRankings = gr.OrderBy(r => r.Rank).ToList()
                                   }).OrderByDescending(cl => cl.Company.Name).ToList();


            var allTimeSlots = await _context.TimeSlot.ToListAsync();
            groupedRankings.ForEach(gr =>
            {
                // TODO: make async? 
                // TODO: dry this up
                // Get available time blocks for company


                // Loop through students who priortized that company first and, in order, try to assign them to an interview
                gr.PriorityRankings.ForEach(r =>
                {
                    if (r.Rank == 1)
                    {
                        scheduleInterview(r, allTimeSlots);
                    }

                });
                gr.PriorityRankings.ForEach(r =>
                {
                    // Find the first time when they're both available
                    if (r.Rank == 2)
                    {
                        scheduleInterview(r, allTimeSlots);

                    }

                });


                // Loop through students who priortized that company first and, in order, try to assign them to an interview
                gr.PriorityRankings.ForEach(r =>
                {
                    // Find the first time when they're both available
                    if (r.Rank == 3)
                    {
                        scheduleInterview(r, allTimeSlots);

                    }

                });


                // Loop through students who priortized that company first and, in order, try to assign them to an interview
                gr.PriorityRankings.ForEach(r =>
                {
                    // Find the first time when they're both available
                    if (r.Rank == 4)
                    {
                        scheduleInterview(r, allTimeSlots);
                    }

                });


                // Loop through students who priortized that company first and, in order, try to assign them to an interview
                gr.PriorityRankings.ForEach(r =>
                {
                    // Find the first time when they're both available
                    if (r.Rank == 5)
                    {
                        scheduleInterview(r, allTimeSlots);
                    }

                });

            });



            var newInterviews = await _context.Interview
               .Include(i => i.TimeSlot)
               .Include(i => i.Ranking)
                   .ThenInclude(r => r.Student)
               .Include(i => i.Ranking)
                   .ThenInclude(r => r.Company)
               .ToListAsync();

            var newInterviewsGroupedByCompany = (from i in newInterviews
                                                 group i by i.Ranking.Company.Name into gi
                                                 select new CompanyList()
                                                 {
                                                     Company = gi.ToList()[0].Ranking.Company,
                                                     InterviewSchedule = gi.OrderBy(i => i.TimeSlot.StartTime).ToList()
                                                 }).OrderBy(cl => cl.InterviewSchedule.Count()).ToList();

            var newInterviewsGroupedByStudent = (from i in newInterviews
                                                 group i by i.Ranking.Student.FirstName into gi
                                                 select new CompanyList()
                                                 {
                                                     Student = gi.ToList()[0].Ranking.Student,
                                                     InterviewSchedule = gi.OrderBy(i => i.TimeSlot.StartTime).ToList()
                                                 }).OrderBy(sl => sl.InterviewSchedule.Count()).ThenBy(sl => sl.Student.FirstName).ToList();


            

            newInterviewsGroupedByCompany.ForEach(c =>
            {
                if (c.InterviewSchedule.Count() < 5)
                {

                    // Loop through all the time slots
                    _context.TimeSlot.ToList().ForEach(time =>
                    {
                        // If the company is available in that time slot
                        if (CompanyIsAvailable(c.Company, time))
                        {
                            

                            var firstAvailableStudent = newInterviewsGroupedByStudent.FirstOrDefault(s =>
                            StudentIsAvailable(s.Student, time) &&
                            !AlreadyScheduled(s.Student, c.Company) &&
                            meetsRequirements(s.Student, c.Company) &&
                            isLocationCompatible(s.Student, c.Company));
                            if (firstAvailableStudent != null)
                            {

                                // Create a new ranking with a rank of 0
                                var ranking = new Ranking()
                                {
                                    Company = c.Company,
                                    CompanyId = c.Company.Id,
                                    Student = firstAvailableStudent.Student,
                                    StudentId = firstAvailableStudent.Student.Id,
                                    Rank = 0
                                };
                                _context.Add(ranking);

                                // Create a new interview
                                var interview = new Interview()
                                {
                                    TimeSlotId = time.Id,
                                    TimeSlot = time,
                                    RankingId = ranking.Id,
                                    Ranking = ranking
                                };
                                _context.Add(interview);
                                _context.SaveChanges();

                            }

                        }

                    });
                }
            });

            newInterviews = await _context.Interview
               .Include(i => i.TimeSlot)
               .Include(i => i.Ranking)
                   .ThenInclude(r => r.Student)
               .Include(i => i.Ranking)
                   .ThenInclude(r => r.Company)
               .ToListAsync();

          
            newInterviewsGroupedByCompany = (from i in newInterviews
                                             group i by i.Ranking.Company.Name into gi
                                             select new CompanyList()
                                             {
                                                 Company = gi.ToList()[0].Ranking.Company,
                                                 InterviewSchedule = gi.OrderBy(i => i.TimeSlot.StartTime).ToList()
                                             }).ToList();

            newInterviewsGroupedByStudent = (from i in newInterviews
                                             group i by i.Ranking.Student.FirstName into gi
                                             select new CompanyList()
                                             {
                                                 Student = gi.ToList()[0].Ranking.Student,
                                                 InterviewSchedule = gi.OrderBy(i => i.TimeSlot.StartTime).ToList()
                                             }).ToList();

            var newInterviewsGroupedByTimeSlot = (from i in newInterviews
                                                  orderby i.TimeSlot.StartTime
                                                  group i by i.TimeSlot into gi
                                                  select new CompanyList()
                                                  {
                                                      TimeSlot = gi.ToList()[0].TimeSlot,
                                                      InterviewSchedule = gi.OrderBy(i => i.TimeSlot.StartTime).ToList()
                                                  }).ToList();

            var allInterviewsPrinterFriendly = new List<CompanyList>();

            allInterviewsPrinterFriendly
                .Concat(newInterviewsGroupedByCompany)
                .Concat(newInterviewsGroupedByStudent)
                .Concat(newInterviewsGroupedByTimeSlot);

            switch (sortBy)
            {
                case "student":
                    return View(newInterviewsGroupedByStudent);
                case "company":
                    return View(newInterviewsGroupedByCompany);
                case "timeSlot":
                    return View(newInterviewsGroupedByTimeSlot);
                case "printerFriendly":
                    return View(allInterviewsPrinterFriendly);
                default:
                    return View(newInterviewsGroupedByTimeSlot);
            };
        }


        // GET: TimeSlots
        public async Task<IActionResult> Index()
        {
            return View(await _context.TimeSlot.OrderBy(t => t.StartTime).ToListAsync());
        }

        // GET: TimeSlots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeSlot = await _context.TimeSlot
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeSlot == null)
            {
                return NotFound();
            }

            return View(timeSlot);
        }

        // GET: TimeSlots/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TimeSlots/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartTime,EndTime")] TimeSlot timeSlot)
        {
            if (ModelState.IsValid)
            {
                _context.Add(timeSlot);
                await _context.SaveChangesAsync();

                // Make all companies available at this time by default
                var allCompanies = await _context.Company.ToListAsync();


                allCompanies.ForEach(company =>
                {
                    var newAvailability = new CompanyAvailability()
                    {
                        CompanyId = company.Id,
                        TimeSlotId = timeSlot.Id
                    };
                    _context.Add(newAvailability);
                });

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(timeSlot);
        }

        // GET: TimeSlots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeSlot = await _context.TimeSlot.FindAsync(id);
            if (timeSlot == null)
            {
                return NotFound();
            }
            return View(timeSlot);
        }

        // POST: TimeSlots/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartTime,EndTime")] TimeSlot timeSlot)
        {
            if (id != timeSlot.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timeSlot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimeSlotExists(timeSlot.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(timeSlot);
        }

        // GET: TimeSlots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeSlot = await _context.TimeSlot
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeSlot == null)
            {
                return NotFound();
            }

            return View(timeSlot);
        }

        // POST: TimeSlots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timeSlot = await _context.TimeSlot.FindAsync(id);
            _context.TimeSlot.Remove(timeSlot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimeSlotExists(int id)
        {
            return _context.TimeSlot.Any(e => e.Id == id);
        }
    }
}
