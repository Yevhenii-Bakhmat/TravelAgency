﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using TravelAgency.Data;
using TravelAgency.Models;

namespace TravelAgency.Controllers {
    public class VocationsController : Controller {
        private readonly ApplicationDbContext _context;

        public VocationsController(ApplicationDbContext context) {
            _context = context;
        }

        // GET: Vocations
        public async Task<IActionResult> Index(string searchString, int order = 0) {
            var applicationDbContext = await _context.Vocations.Include(v => v.Customer).Include(v => v.Operator)
                .Include(v => v.Tour).ToListAsync();
            if (!string.IsNullOrEmpty(searchString)) {
                var sS = searchString.Split(' ');
                foreach (var parameter in sS) {
                    applicationDbContext = applicationDbContext.Where(v =>
                        v.DaysCount.ToString().Contains(parameter) || v.Customer.LastName.Contains(parameter) ||
                        v.Customer.FirstName.Contains(parameter) || v.Customer.FatherName.Contains(parameter) ||
                        v.Customer.LastName.Contains(parameter) || v.Operator.LastName.Contains(parameter) ||
                        v.BeginDateTime.ToString("dd-MMMM-yyyy").Contains(parameter) ||
                        v.Tour.Country.Contains(parameter)).ToList();
                }
            }

            switch (order) {
                case 1: {
                    applicationDbContext = applicationDbContext.OrderBy(v => v.BeginDateTime).ToList();
                    break;
                }
                case 2: {
                    applicationDbContext = applicationDbContext.OrderBy(v => v.DaysCount).ToList();
                    break;
                }
                case 3: {
                    applicationDbContext = applicationDbContext.OrderBy(v => v.Tour).ToList();
                    break;
                }
                case 4: {
                    applicationDbContext = applicationDbContext.OrderBy(v => v.OperatorId).ToList();
                    break;
                }
                case 5: {
                    applicationDbContext = applicationDbContext.OrderBy(v => v.CustomerId).ToList();
                    break;
                }
                default: {
                    applicationDbContext = applicationDbContext.OrderBy(v => v.VocationId).ToList();
                    break;
                }
            }

            return View(applicationDbContext);
        }

        // GET: Vocations/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var vocation = await _context.Vocations.Include(v => v.Customer).Include(v => v.Operator)
                .Include(v => v.Tour).FirstOrDefaultAsync(m => m.VocationId == id);
            if (vocation == null) {
                return NotFound();
            }

            return View(vocation);
        }

        // GET: Vocations/Create
        public IActionResult Create() {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "UserId", "LastName");
            ViewData["OperatorId"] = new SelectList(_context.Operators, "OperatorId", "LastName");
            ViewData["TourId"] = new SelectList(_context.Tours, "TourId", "Country");
            return View();
        }

        // POST: Vocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("VocationId,BeginDateTime,DaysCount,TourId,OperatorId,CustomerId")]
            Vocation vocation) {
            if (ModelState.IsValid) {
                var o = await _context.Operators.FindAsync(vocation.OperatorId);
                var c = await _context.Customers.FindAsync(vocation.CustomerId);
                var t = await _context.Tours.FindAsync(vocation.TourId);
                vocation.Customer = c;
                vocation.Operator = o;
                vocation.Tour = t;
                _context.Add(vocation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "UserId", "LastName", vocation.CustomerId);
            ViewData["OperatorId"] = new SelectList(_context.Operators, "OperatorId", "LastName", vocation.OperatorId);
            ViewData["TourId"] = new SelectList(_context.Tours, "TourId", "Country", vocation.TourId);
            return View(vocation);
        }

        // GET: Vocations/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var vocation = await _context.Vocations.FirstOrDefaultAsync(v => v.VocationId == id);
            if (vocation == null) {
                return NotFound();
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "UserId", "LastName", vocation.CustomerId);
            ViewData["OperatorId"] = new SelectList(_context.Operators, "OperatorId", "LastName", vocation.OperatorId);
            ViewData["TourId"] = new SelectList(_context.Tours, "TourId", "Country", vocation.TourId);
            return View(vocation);
        }

        // POST: Vocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("VocationId,BeginDateTime,DaysCount,TourId,OperatorId,CustomerId")]
            Vocation vocation) {
            if (id != vocation.VocationId) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(vocation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!VocationExists(vocation.OperatorId)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "UserId", "LastName", vocation.CustomerId);
            ViewData["OperatorId"] = new SelectList(_context.Operators, "OperatorId", "LastName", vocation.OperatorId);
            ViewData["TourId"] = new SelectList(_context.Tours, "TourId", "Country", vocation.TourId);
            return View(vocation);
        }

        // GET: Vocations/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var vocation = await _context.Vocations.Include(v => v.Customer).Include(v => v.Operator)
                .Include(v => v.Tour).FirstOrDefaultAsync(m => m.OperatorId == id);
            if (vocation == null) {
                return NotFound();
            }

            return View(vocation);
        }

        // POST: Vocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var vocation = await _context.Vocations.FindAsync(id);
            _context.Vocations.Remove(vocation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VocationExists(int id) {
            return _context.Vocations.Any(e => e.OperatorId == id);
        }

        public async Task<FileContentResult> Statistics() {
            //TODO: OrderBy
            var applicationDbContext = await _context.Vocations.Include(v => v.Customer)
                .Include(v => v.Operator)
                .Include(v => v.Tour)
                .ToListAsync();
            var s = applicationDbContext.Aggregate("ID,Customer,Begin Date,DaysCount,Tour,Operator",
                (current,
                        vocation) =>
                    current +
                    $"\n{vocation.VocationId},{vocation.Customer.LastName}," +
                    $"{vocation.BeginDateTime},{vocation.DaysCount}," +
                    $"{vocation.Tour.Country},{vocation.Operator.LastName}");
            return File(Encoding.UTF8.GetBytes(s),
                "text/csv",
                "Vocations.csv");
        }

        public async Task<FileContentResult> TourRating() {
            var applicationDbContext = _context.Vocations.Include(v => v.Customer).Include(v => v.Operator)
                .Include(v => v.Tour);
            var content = from item in applicationDbContext
                group item by new {item.Tour.Country, item.Tour.TourId, item.Tour.VisaGoal}
                into grp
                select new {ID = grp.Key.TourId, Tour = grp.Key.Country, Goal = grp.Key.VisaGoal, Rating = grp.Count()};
            var s = "ID,Tour,Goal,Rating";
            foreach (var pair in content) {
                s += $"\n{pair.ID},{pair.Tour},{pair.Goal},{pair.Rating}";
            }

            return File(Encoding.UTF8.GetBytes(s), "text/csv", "TourFashion.csv");
        }

        public async Task<FileContentResult> PriseList() => File(
            Encoding.UTF8.GetBytes((await _context.Tours.Include(s => s.Hotel).ToListAsync()).Aggregate(
                "ID, Страна, Отель, Последняя дата, Цена трансцера, Цель визы, Цена визы, Цена за день, Общая стоимость дня",
                (current, item) =>
                    current +
                    $"\n{item.TourId},{item.Country},{item.Hotel.Name},{item.EndDate},{item.TransportCost},{item.VisaGoal},{item.VisaCost},{item.OneDayCost},{item.TransportCost + item.VisaCost + item.OneDayCost}")),
            "text/csv", $"TourList {DateTime.Now:yyyy MMMM dd}.csv");

        public async Task<FileContentResult> CountryRating() => File(
            Encoding.UTF8.GetBytes(
                (await _context.Vocations.GroupBy(item => new {item.Tour.Country})
                    .Select(grp => new {Country = grp.Key, Rating = grp.Count()}).OrderByDescending(c => c.Rating)
                    .ToListAsync()).Aggregate("Страна, Рейтинг",
                    (current, item) => current + $"\n{item.Country.Country}, {item.Rating}")), "text/csv",
            $"Country_Rating {DateTime.Now}.csv");

        public async Task<FileContentResult> OperatorRating() => File(
            Encoding.UTF8.GetBytes(
                (await _context.Vocations.Include(v => v.Customer).Include(v => v.Operator)
                    .GroupBy(item => new {item.Operator.LastName})
                    .Select(grp => new {Operator = grp.Key, Rating = grp.Count()}).ToListAsync())
                .Aggregate("Оператор, Рейтинг",
                    (current, item) => current + $"\n{item.Operator.LastName}, {item.Rating}")), "text/csv",
            $"Operator_Rating {DateTime.Now}.csv");

        public async Task<FileContentResult> CustomerRating() => File(
            Encoding.UTF8.GetBytes(
                (await _context.Vocations.Include(v => v.Customer).Include(v => v.Operator)
                    .GroupBy(item => new {item.Customer.LastName})
                    .Select(grp => new {Customer = grp.Key, Rating = grp.Count()}).OrderByDescending(i => i.Rating)
                    .ToListAsync()).Aggregate("Клиент, Рейтинг",
                    (current, item) => current + $"\n{item.Customer.LastName}, {item.Rating}")), "text/csv",
            $"Customer_Rating {DateTime.Now}.csv");

        public async Task<FileContentResult> GoalStats() {
            var content = from v in _context.Vocations
                join t in _context.Tours on v.TourId equals t.TourId into gj
                from grp in gj
                select new {
                    Goal = grp.VisaGoal.ToString(),
                    TourCount = 1,
                    MoneySpent = grp.TransportCost + grp.VisaCost + v.DaysCount * grp.OneDayCost,
                    TimeSpent = v.DaysCount
                };
            var grouped = content.AsEnumerable().GroupBy(item => new {item.Goal}).Select(grp =>
                new {
                    Goal = grp.Key,
                    TourCount = grp.Sum(i => i.TourCount),
                    MoneySpent = grp.Sum(i => i.MoneySpent),
                    TimeSpent = grp.Sum(i => i.TimeSpent)
                }).ToList();
            var buffer = grouped.Aggregate("Цель,Всего туров,Проведено дней,Потрачено денег",
                (current, item) =>
                    current + $"\n{item.Goal.Goal},{item.TourCount},{item.TimeSpent},{item.MoneySpent}$");
            return File(Encoding.UTF8.GetBytes(buffer), "text/csv", "GoalStats.csv");
        }
    }
}