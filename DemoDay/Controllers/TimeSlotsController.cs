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
            var occupiedTimeSlotsForStudent = scheduledInterviewsForStudent.Select(i => i.TimeSlot);
            var openTimeSlotsForStudent = _context.TimeSlot.Where(ti => !occupiedTimeSlotsForStudent.Contains(ti));

            return openTimeSlotsForStudent.Contains(time);

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
                                   }).ToList();

            var allTimeSlots = await _context.TimeSlot.ToListAsync();

            groupedRankings.ForEach(gr =>
            {
                // TODO: make async? 
                // TODO: dry this up
                // Get available time blocks for company


                // Loop through students who priortized that company first and, in order, try to assign them to an interview
                gr.PriorityRankings.ForEach(r =>
                {
                    // Find the first time when they're both available
                    if (r.Rank == 1)
                    {
                        var openSlot = allTimeSlots.FirstOrDefault(time => CompanyIsAvailable(r.Company, time) && StudentIsAvailable(r.Student, time));

                        if (openSlot != null)
                        {
                            var interview = new Interview()
                            {
                                TimeSlotId = openSlot.Id,
                                TimeSlot = openSlot,
                                RankingId = r.Id,
                                Ranking = r
                            };
                            _context.Add(interview);
                            _context.SaveChanges();

                        }
                    }

                });


                // Loop through students who priortized that company first and, in order, try to assign them to an interview
                gr.PriorityRankings.ForEach(r =>
                {
                    // Find the first time when they're both available
                    if (r.Rank == 2)
                    {
                        var openSlot = allTimeSlots.FirstOrDefault(time => CompanyIsAvailable(r.Company, time) && StudentIsAvailable(r.Student, time));

                        if (openSlot != null)
                        {
                            var interview = new Interview()
                            {
                                TimeSlotId = openSlot.Id,
                                TimeSlot = openSlot,
                                RankingId = r.Id,
                                Ranking = r
                            };
                            _context.Add(interview);
                            _context.SaveChanges();

                        }

                    }

                });


                // Loop through students who priortized that company first and, in order, try to assign them to an interview
                gr.PriorityRankings.ForEach(r =>
                {
                    // Find the first time when they're both available
                    if (r.Rank == 3)
                    {
                        var openSlot = allTimeSlots.FirstOrDefault(time => CompanyIsAvailable(r.Company, time) && StudentIsAvailable(r.Student, time));


                        if (openSlot != null)
                        {
                            var interview = new Interview()
                            {
                                TimeSlotId = openSlot.Id,
                                TimeSlot = openSlot,
                                RankingId = r.Id,
                                Ranking = r
                            };
                            _context.Add(interview);
                            _context.SaveChanges();

                        }

                    }

                });


                // Loop through students who priortized that company first and, in order, try to assign them to an interview
                gr.PriorityRankings.ForEach(r =>
                {
                    // Find the first time when they're both available
                    if (r.Rank == 4)
                    {
                        var openSlot = allTimeSlots.FirstOrDefault(time => CompanyIsAvailable(r.Company, time) && StudentIsAvailable(r.Student, time));

                        if (openSlot != null)
                        {
                            var interview = new Interview()
                            {
                                TimeSlotId = openSlot.Id,
                                TimeSlot = openSlot,
                                RankingId = r.Id,
                                Ranking = r
                            };
                            _context.Add(interview);
                            _context.SaveChanges();

                        }
                    }

                });


                // Loop through students who priortized that company first and, in order, try to assign them to an interview
                gr.PriorityRankings.ForEach(r =>
                {
                    // Find the first time when they're both available
                    if (r.Rank == 5)
                    {
                        var openSlot = allTimeSlots.FirstOrDefault(time => CompanyIsAvailable(r.Company, time) && StudentIsAvailable(r.Student, time));

                        if (openSlot != null)
                        {
                            var interview = new Interview()
                            {
                                TimeSlotId = openSlot.Id,
                                TimeSlot = openSlot,
                                RankingId = r.Id,
                                Ranking = r
                            };
                            _context.Add(interview);
                            _context.SaveChanges();

                        }
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
                                                 }).ToList();

            var newInterviewsGroupedByStudent = (from i in newInterviews
                                                 group i by i.Ranking.Student.FirstName into gi
                                                 select new CompanyList()
                                                 {
                                                     Student = gi.ToList()[0].Ranking.Student,
                                                     InterviewSchedule = gi.OrderBy(i => i.TimeSlot.StartTime).ToList()
                                                 }).ToList();

            // Check and make sure that students have five interviews

            // Check and make sure that companies have four interviews

            var allStudents = _context.Student.ToList();

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
                            // Get the first available student
                            // Add a check to make sure the student doesn't already have an interview with that company
                            var firstAvailableStudent = allStudents.FirstOrDefault(s => StudentIsAvailable(s, time) && !AlreadyScheduled(s, c.Company));

                            // Create a new ranking with a rank of 0
                            var ranking = new Ranking()
                            {
                                Company = c.Company,
                                CompanyId = c.Company.Id,
                                Student = firstAvailableStudent,
                                StudentId = firstAvailableStudent.Id,
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



            switch (sortBy)
            {
                case "student":
                    return View(newInterviewsGroupedByStudent);
                case "company":
                    return View(newInterviewsGroupedByCompany);
                case "timeSlot":
                    return View(newInterviewsGroupedByTimeSlot);
                default:
                    return View(newInterviewsGroupedByTimeSlot);
            };

           


            // Take the top six students and try to schedule them in interview blocks
            // But ONLY if the student doesn't have any interviews currently in the interview table with that timeslot id

            // If a company has less than six interviews or a student has less than six interviews, match them up

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
