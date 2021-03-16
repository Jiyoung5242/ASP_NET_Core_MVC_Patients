using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JJPatients.Models;

namespace JJPatients.Controllers
{
    public class JJConcentrationUnitController : Controller
    {
        private readonly PatientsContext _context;
        /// <summary>
        /// Constructor of concentration Unit for user
        /// </summary>
        /// <param name="context">Patients Context</param>
        public JJConcentrationUnitController(PatientsContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Index is the default action for this controller
        /// in the Concentration Unit controller, this will list Concentrate Unit on File
        /// </summary>
        /// <returns>View</returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.ConcentrationUnit.ToListAsync());
        }

        /// <summary>
        /// action to display the details of a selected Concentration Unit
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>View</returns>
        // 
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concentrationUnit = await _context.ConcentrationUnit
                .FirstOrDefaultAsync(m => m.ConcentrationCode == id);
            if (concentrationUnit == null)
            {
                return NotFound();
            }

            return View(concentrationUnit);
        }

        /// <summary>
        /// action to create new Concentration Unit
        /// </summary>
        /// <returns>View</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// create method binds to a new Concetration Unit
        /// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        /// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="concentrationUnit">Concentration Unit</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConcentrationCode")] ConcentrationUnit concentrationUnit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(concentrationUnit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(concentrationUnit);
        }

        /// <summary>
        /// action to edit selected Concentration Unit information
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>View</returns>
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concentrationUnit = await _context.ConcentrationUnit.FindAsync(id);
            if (concentrationUnit == null)
            {
                return NotFound();
            }
            return View(concentrationUnit);
        }

        /// <summary>
        /// action to edit selected Concentration Unit information
        /// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        /// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="concentrationUnit">Concentration Unit</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ConcentrationCode")] ConcentrationUnit concentrationUnit)
        {
            if (id != concentrationUnit.ConcentrationCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(concentrationUnit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConcentrationUnitExists(concentrationUnit.ConcentrationCode))
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
            return View(concentrationUnit);
        }

        /// <summary>
        /// action to delete selected Concentration Unit info
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>View</returns>
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concentrationUnit = await _context.ConcentrationUnit
                .FirstOrDefaultAsync(m => m.ConcentrationCode == id);
            if (concentrationUnit == null)
            {
                return NotFound();
            }

            return View(concentrationUnit);
        }

        /// <summary>
        /// action to confirm to delete Concentration Unit info
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Index View</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var concentrationUnit = await _context.ConcentrationUnit.FindAsync(id);
            _context.ConcentrationUnit.Remove(concentrationUnit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// check whether Concentration Unit exists or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool ConcentrationUnitExists(string id)
        {
            return _context.ConcentrationUnit.Any(e => e.ConcentrationCode == id);
        }
    }
}
