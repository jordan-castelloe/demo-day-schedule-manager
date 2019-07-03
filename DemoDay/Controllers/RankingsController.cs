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
    public class RankingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RankingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rankings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ranking.Include(r => r.Company).Include(r => r.Student);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Rankings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ranking = await _context.Ranking
                .Include(r => r.Company)
                .Include(r => r.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ranking == null)
            {
                return NotFound();
            }

            return View(ranking);
        }

        // GET: Rankings/Create
        public async Task<IActionResult> Create(int id)
        {
            CreateRankingViewModel vm = new CreateRankingViewModel();

            // Add the student's id to the view model
            vm.Student = await _context.Student.Where(s => s.Id == id).FirstOrDefaultAsync();
            vm.Companies = await _context.Company.Select(c => new SelectListItem()
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToListAsync();
            return View(vm);
        }

        // POST: Rankings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRankingViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Wipe out previous rankings and start fresh so that each student has only one set of rankings
                    var previousRankings = _context.Ranking.Where(r => r.StudentId == vm.Student.Id);
                    await previousRankings.ForEachAsync(pr => _context.Remove(pr));
                    await _context.SaveChangesAsync();
                   
                    List<Ranking> Rankings = new List<Ranking>()
                    {
                        vm.Place1, vm.Place2, vm.Place3, vm.Place4, vm.Place5
                    };

                    for(var i = 0; i < Rankings.Count; i++){
                        // TODO: Refactor, this is fragile
                        var r = Rankings[i];
                        r.Rank = i + 1; 
                        r.StudentId = vm.Student.Id;
                        _context.Ranking.Add(r);
                    }
                   
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Students");

                }
                catch(Exception e)
                {
                    return View(vm);

                };

            } else
            {
                return View(vm);
            }
            
            
            
        }

        // GET: Rankings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ranking = await _context.Ranking.FindAsync(id);
            if (ranking == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Id", ranking.CompanyId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Id", ranking.StudentId);
            return View(ranking);
        }

        // POST: Rankings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Rank,StudentId,CompanyId")] Ranking ranking)
        {
            if (id != ranking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ranking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RankingExists(ranking.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Id", ranking.CompanyId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Id", ranking.StudentId);
            return View(ranking);
        }

        // GET: Rankings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ranking = await _context.Ranking
                .Include(r => r.Company)
                .Include(r => r.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ranking == null)
            {
                return NotFound();
            }

            return View(ranking);
        }

        // POST: Rankings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ranking = await _context.Ranking.FindAsync(id);
            _context.Ranking.Remove(ranking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RankingExists(int id)
        {
            return _context.Ranking.Any(e => e.Id == id);
        }
    }
}
