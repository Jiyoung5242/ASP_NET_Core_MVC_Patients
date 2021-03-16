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
    public class JJMedicationTypeController : Controller
    {
        private readonly PatientsContext _context;
        /// <summary>
        /// Constructor of Medication Type controller
        /// </summary>
        /// <param name="context">Patients Context</param>
        public JJMedicationTypeController(PatientsContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Index is the default action for this controller
        /// in the Medication Type controller, this will list Meication Type on File
        /// </summary>
        /// <returns>View</returns>
        public async Task<IActionResult> Index()
        {
            var medicationType = from m in _context.MedicationType select m;

            return View(await _context.MedicationType.OrderBy(m => m.Name).ToListAsync());
        }

        /// <summary>
        /// action to display the details of a selected Medication Type
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>View</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicationType = await _context.MedicationType
                .FirstOrDefaultAsync(m => m.MedicationTypeId == id);
            if (medicationType == null)
            {
                return NotFound();
            }

            return View(medicationType);
        }

        /// <summary>
        /// action to create new Medication Type
        /// </summary>
        /// <returns>View</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// create method binds to a new Meication Type
        /// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        /// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="medicationType">Medication Type</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicationTypeId,Name")] MedicationType medicationType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicationType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medicationType);
        }

        /// <summary>
        /// action to edit selected Medication Type information
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>View</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicationType = await _context.MedicationType.FindAsync(id);
            if (medicationType == null)
            {
                return NotFound();
            }
            return View(medicationType);
        }

        /// <summary>
        /// action to edit selected Medication Type information
        /// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        /// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="medicationType">Medication Type</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MedicationTypeId,Name")] MedicationType medicationType)
        {
            if (id != medicationType.MedicationTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicationType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicationTypeExists(medicationType.MedicationTypeId))
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
            return View(medicationType);
        }

        /// <summary>
        /// action to delete selected Medication Type info
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>View</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicationType = await _context.MedicationType
                .FirstOrDefaultAsync(m => m.MedicationTypeId == id);
            if (medicationType == null)
            {
                return NotFound();
            }

            return View(medicationType);
        }

        /// <summary>
        /// action to confirm to delete Medication Type info
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Index View</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicationType = await _context.MedicationType.FindAsync(id);
            _context.MedicationType.Remove(medicationType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// check whether Medication Type exists or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool MedicationTypeExists(int id)
        {
            return _context.MedicationType.Any(e => e.MedicationTypeId == id);
        }
    }
}
