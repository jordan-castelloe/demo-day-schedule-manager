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

            if (openTimeSlotsForStudent.Contains(time))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private bool CompanyIsAvailable(Company company, TimeSlot time)
        {
            var scheduledInterviewsForCompany = _context.Interview.Where(i => i.Ranking.CompanyId == company.Id).Include(i => i.TimeSlot).ToList();
            var occupiedTimeSlotsForCompany = scheduledInterviewsForCompany.Select(i => i.TimeSlot);
            var openTimeSlotsForCompany = _context.TimeSlot.Where(ti => !occupiedTimeSlotsForCompany.Contains(ti));

            if (openTimeSlotsForCompany.Contains(time))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private async Task<List<Interview>> BuildSchedule()
        {

            List<Interview> InterviewList = new List<Interview>();
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
                        var avaialbleSlot = allTimeSlots.FirstOrDefault(time => CompanyIsAvailable(r.Company, time) && StudentIsAvailable(r.Student, time));

                        var interview = new Interview()
                        {
                            TimeSlotId = avaialbleSlot.Id,
                            TimeSlot = avaialbleSlot,
                            RankingId = r.Id,
                            Ranking = r
                        };

                        InterviewList.Add(interview);
                    }

                });


                // Loop through students who priortized that company first and, in order, try to assign them to an interview
                gr.PriorityRankings.ForEach(r =>
                {
                    // Find the first time when they're both available
                    if (r.Rank == 2)
                    {
                        var avaialbleSlot = allTimeSlots.FirstOrDefault(time => CompanyIsAvailable(r.Company, time) && StudentIsAvailable(r.Student, time));

                        var interview = new Interview()
                        {
                            TimeSlotId = avaialbleSlot.Id,
                            TimeSlot = avaialbleSlot,
                            RankingId = r.Id,
                            Ranking = r
                        };

                        InterviewList.Add(interview);
                    }

                });


                // Loop through students who priortized that company first and, in order, try to assign them to an interview
                gr.PriorityRankings.ForEach(r =>
                {
                    // Find the first time when they're both available
                    if (r.Rank == 3)
                    {
                        var avaialbleSlot = allTimeSlots.FirstOrDefault(time => CompanyIsAvailable(r.Company, time) && StudentIsAvailable(r.Student, time));

                        var interview = new Interview()
                        {
                            TimeSlotId = avaialbleSlot.Id,
                            TimeSlot = avaialbleSlot,
                            RankingId = r.Id,
                            Ranking = r
                        };

                        InterviewList.Add(interview);
                    }

                });


                // Loop through students who priortized that company first and, in order, try to assign them to an interview
                gr.PriorityRankings.ForEach(r =>
                {
                    // Find the first time when they're both available
                    if (r.Rank == 4)
                    {
                        var avaialbleSlot = allTimeSlots.FirstOrDefault(time => CompanyIsAvailable(r.Company, time) && StudentIsAvailable(r.Student, time));

                        var interview = new Interview()
                        {
                            TimeSlotId = avaialbleSlot.Id,
                            TimeSlot = avaialbleSlot,
                            RankingId = r.Id,
                            Ranking = r
                        };

                        InterviewList.Add(interview);
                    }

                });


                // Loop through students who priortized that company first and, in order, try to assign them to an interview
                gr.PriorityRankings.ForEach(r =>
                {
                    // Find the first time when they're both available
                    if (r.Rank == 5)
                    {
                        var avaialbleSlot = allTimeSlots.FirstOrDefault(time => CompanyIsAvailable(r.Company, time) && StudentIsAvailable(r.Student, time));

                        var interview = new Interview()
                        {
                            TimeSlotId = avaialbleSlot.Id,
                            TimeSlot = avaialbleSlot,
                            RankingId = r.Id,
                            Ranking = r
                        };

                        InterviewList.Add(interview);
                    }

                });

            });

           ;

            // Take the top six students and try to schedule them in interview blocks
            // But ONLY if the student doesn't have any interviews currently in the interview table with that timeslot id

            // If a company has less than six interviews or a student has less than six interviews, match them up
            return InterviewList;
        }

        public async Task<IActionResult> ScheduleIndex()
        {
            var schedule = await BuildSchedule();
            return View(schedule);
        }
        // GET: TimeSlots
        public async Task<IActionResult> Index()
        {
            return View(await _context.TimeSlot.ToListAsync());
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
