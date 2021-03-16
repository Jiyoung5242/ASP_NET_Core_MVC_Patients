using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JJPatients.Models;
using Microsoft.AspNetCore.Authorization;

namespace JJPatients.Controllers
{
    [Authorize(Roles = "members")]
    public class JJCountryController : Controller
    {
        private readonly PatientsContext _context;

        /// <summary>
        /// Constructor of Country Controller
        /// </summary>
        /// <param name="context">Patients Context</param>
        public JJCountryController(PatientsContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Index is the default action for this controller
        /// in the Country controller, this will list Countries on File
        /// </summary>
        /// <returns>View</returns>
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Country.ToListAsync());
        }

        /// <summary>
        /// action to display the details of a selected Country Code
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>View</returns>
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country
                .FirstOrDefaultAsync(m => m.CountryCode == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        /// <summary>
        /// action to create new country
        /// </summary>
        /// <returns>View</returns>
        [Authorize(Roles = "administrators,medicalstaff")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// create method binds to a new Country
        /// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        /// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="country">Country</param>
        /// <returns>View</returns>
        ///
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CountryCode,Name,PostalPattern,PhonePattern,FederalSalesTax")] Country country)
        {
            if (ModelState.IsValid)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        /// <summary>
        /// action to edit selected country information except country code
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>View</returns>
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        /// <summary>
        /// action to edit selected country information except country code
        /// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        /// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="country">Country</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CountryCode,Name,PostalPattern,PhonePattern,FederalSalesTax")] Country country)
        {
            if (id != country.CountryCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.CountryCode))
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
            return View(country);
        }

        /// <summary>
        /// action to delete selected country info
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>View</returns>
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country
                .FirstOrDefaultAsync(m => m.CountryCode == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        /// <summary>
        /// action to confirm to delete country info
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Index View</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var country = await _context.Country.FindAsync(id);
            _context.Country.Remove(country);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// check whether coutry exists or not
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        private bool CountryExists(string id)
        {
            return _context.Country.Any(e => e.CountryCode == id);
        }
    }
}
