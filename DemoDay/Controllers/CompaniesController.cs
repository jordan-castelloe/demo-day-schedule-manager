using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoDay.Data;
using DemoDay.Models;
using DemoDay.Models.ViewModels;

namespace DemoDay.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            
            var allCompanies = await _context.Company.ToListAsync();

         
            return View(allCompanies);
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Company company)
        {
            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
                // Make companies available all the time by default
                var allTimeSlots = await _context.TimeSlot.ToListAsync();
               
                
                allTimeSlots.ForEach(time =>
                {
                    var newAvailability = new CompanyAvailability()
                    {
                        CompanyId = company.Id,
                        TimeSlotId = time.Id
                    };
                    _context.Add(newAvailability);
                });

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var availableTimes = _context.CompanyAvailability.Where(c => c.CompanyId == id).Select(c => c.TimeSlot).ToList();

            var vm = new CompanyEditViewModel()
            {
                Company = await _context.Company.FindAsync(id),
                TimeSlots = await _context.TimeSlot.OrderBy(t => t.StartTime).Select(time => new SelectListItem()
                {
                    Text = $"{time.StartTime.ToString("hh:mm tt")} - {time.EndTime.ToString("hh:mm tt")}",
                    Value = time.Id.ToString(),
                    Selected = availableTimes.Contains(time)

                }).ToListAsync()

            };

            return View(vm);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompanyEditViewModel vm)
        {
            _context.Update(vm.Company);
            // Wipe out their previous availabilities 
            _context.RemoveRange(_context.CompanyAvailability.Where(c => c.CompanyId == id));

            // Build new availabilities based on what they selected
            var newAvailabilities = vm.TimeSlotIds.Select(t => new CompanyAvailability()
            {
                CompanyId = id,
                TimeSlotId = t
            }).ToList();

            _context.CompanyAvailability.AddRange(newAvailabilities);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index");

        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Company.FindAsync(id);
            _context.Company.Remove(company);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
            return _context.Company.Any(e => e.Id == id);
        }
    }
}
