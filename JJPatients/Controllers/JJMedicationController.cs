using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JJPatients.Models;
using Microsoft.AspNetCore.Http;

namespace JJPatients.Controllers
{
    public class JJMedicationController : Controller
    {
        private readonly PatientsContext _context;
        /// <summary>
        /// Constructor of Medication controller
        /// </summary>
        /// <param name="context">Patients Context</param>
        public JJMedicationController(PatientsContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Index is the default action for this controller
        /// in the Medication controller, this will list Meication on File
        /// </summary>
        /// <returns>View</returns>
        public async Task<IActionResult> Index(string MedicationTypeId)
        {

            if (!string.IsNullOrEmpty(MedicationTypeId))
            {
                Response.Cookies.Append("MedicationTypeId", MedicationTypeId);
                HttpContext.Session.SetString("MedicationTypeId", MedicationTypeId);
            }
            else if (Request.Query["MedicationTypeId"].Any())
            {
                MedicationTypeId = Request.Query["MedicationTypeId"].ToString();
                Response.Cookies.Append("MedicationTypeId", MedicationTypeId);
                HttpContext.Session.SetString("MedicationTypeId", MedicationTypeId);
            }
            else if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedicationTypeId = Request.Cookies["MedicationTypeId"].ToString();
            }
            else if (HttpContext.Session.GetString("MedicationTypeId") != null)
            {
                MedicationTypeId = HttpContext.Session.GetString("MedicationTypeId");
                Response.Cookies.Append("MedicationTypeId", MedicationTypeId);
            }
            else
            {
                TempData["message"] = "Select Medication Type to see its medication";
                return RedirectToAction("Index", "JJMedicationType");
            }
            var medicationTypeName = _context.MedicationType.Where(a => a.MedicationTypeId.ToString() == MedicationTypeId).FirstOrDefault();
            ViewData["MedicationTypeName"] = medicationTypeName.Name;
            ViewData["MedicationTypeId"] = MedicationTypeId;
            var patientsContext = _context.Medication.Include(m => m.ConcentrationCodeNavigation).Include(m => m.DispensingCodeNavigation).Include(m => m.MedicationType);
            return View(await patientsContext.Where(m => m.MedicationTypeId.ToString().Equals(MedicationTypeId)).OrderBy(m => m.Name).ThenBy(m => m.Concentration).ToListAsync());

/*
            if (Request.QueryString.ToString() != null && Request.QueryString.ToString().IndexOf("MedicationTypeId") > 0)
            {
                string medicationTypeId = Request.Cookies["MedicationTypeId"];

                Response.Cookies.Append("MedicationTypeId", Request.Query["MedicationTypeId"]);
                string typeId = Request.Cookies["MedicationTypeId"];
                return View(await patientsContext.Where(m => m.MedicationTypeId.ToString().Equals(typeId)).OrderBy(m => m.Name).ThenBy(m => m.Concentration).ToListAsync());

            }
            else
            {

                if(!Request.Cookies.Keys.Contains("MedicationTypeId"))
                {
                    TempData["message"] = "Select Medication Type to see its medication";
                    return RedirectToAction("Index", "JJMedicationType");
                }
                else
                {
                    
                    return View(await patientsContext.Where(m => m.MedicationTypeId.ToString().Contains(Request.Cookies["MedicationTypeId"])).ToListAsync());
                }


            }
            //patientsContext = _context.Medication.Include(m => m.ConcentrationCodeNavigation).Include(m => m.DispensingCodeNavigation).Include(m => m.MedicationType);
            //return View(await patientsContext.ToListAsync());
*/
        }

        /// <summary>
        /// action to display the details of a selected Medication
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>View</returns>
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);
            if (medication == null)
            {
                return NotFound();
            }
            var Medication = _context.Medication.Where(m => m.Din == id).FirstOrDefault();
            var MedicationType = _context.MedicationType.Where(m => m.MedicationTypeId == Medication.MedicationTypeId).FirstOrDefault();
            ViewData["MedicationTypeName"] = MedicationType.Name;
            ViewData["MedicationTypeId"] = id;
            return View(medication);
        }

        /// <summary>
        /// action to create new Medication
        /// </summary>
        /// <returns>View</returns>
        public IActionResult Create()
        {

            string MedicationTID = string.Empty;
            if(Request.Cookies["MedicationTypeId"] != null)
            {
                MedicationTID = Request.Cookies["MedicationTypeId"].ToString();
            }else if (HttpContext.Session.GetString("MedicationTypeId") != null)
            {
                MedicationTID = HttpContext.Session.GetString("MedicationTypeId");
            }

            var MedicationType = _context.MedicationType.Where(m => m.MedicationTypeId.ToString() == MedicationTID).FirstOrDefault();
            ViewData["MedicationTypeName"] = MedicationType.Name;
            //ViewData["MedicationTypeId"] = MedicationTID;

            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(c=>c.ConcentrationCode), "ConcentrationCode", "ConcentrationCode");
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(d=>d.DispensingCode), "DispensingCode", "DispensingCode");
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name");
            //ViewData["MedicationTypeIdHidden"] = Request.Cookies["MedicationTypeId"].ToString();
            
            return View();
        }

        /// <summary>
        /// create method binds to a new Meication
        /// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        /// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="medication">Medication</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {

            string MedicationTID = string.Empty;
            if (Request.Cookies["MedicationTypeId"] != null)
            {
                MedicationTID = Request.Cookies["MedicationTypeId"].ToString();
            }
            else if (HttpContext.Session.GetString("MedicationTypeId") != null)
            {
                MedicationTID = HttpContext.Session.GetString("MedicationTypeId");
            }

            var MedicationType = _context.MedicationType.Where(m => m.MedicationTypeId.ToString() == MedicationTID).FirstOrDefault();
            ViewData["MedicationTypeName"] = MedicationType.Name;
            //ViewData["MedicationTypeId"] = MedicationTID;

            var isZeroExist = _context.Medication.Where(a => a.Din == medication.Din);
            if (isZeroExist.Any())
            {
                ModelState.AddModelError("", "There is already a same record for this medication. DIN : " + medication.Din);
            }
            medication.MedicationTypeId = Convert.ToInt32(MedicationTID);

            if (ModelState.IsValid)
            {
                //ModelState.AddModelError("", "There is already a same record for this medication. DIN : " + medication.Din);
                _context.Add(medication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(c => c.ConcentrationCode), "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(d => d.DispensingCode), "DispensingCode", "DispensingCode", medication.DispensingCode);
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            return View(medication);
        }

        /// <summary>
        /// action to edit selected Medication information
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>View</returns>
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication.FindAsync(id);
            if (medication == null)
            {
                return NotFound();
            }

            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(c => c.ConcentrationCode), "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(d => d.DispensingCode), "DispensingCode", "DispensingCode", medication.DispensingCode);
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            var Medication = _context.Medication.Where(m => m.Din == id).FirstOrDefault();
            var MedicationType = _context.MedicationType.Where(m => m.MedicationTypeId == Medication.MedicationTypeId).FirstOrDefault();
            ViewData["MedicationTypeName"] = MedicationType.Name;
            //ViewData["MedicationTypeId"] = id;
            return View(medication);
        }

        /// <summary>
        /// action to edit selected Medication information
        /// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        /// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="medication">Medication</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Din,Name,Image,MedicationTypeId,DispensingCode,Concentration,ConcentrationCode")] Medication medication)
        {
            if (id != medication.Din)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicationExists(medication.Din))
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
            ViewData["ConcentrationCode"] = new SelectList(_context.ConcentrationUnit.OrderBy(c => c.ConcentrationCode), "ConcentrationCode", "ConcentrationCode", medication.ConcentrationCode);
            ViewData["DispensingCode"] = new SelectList(_context.DispensingUnit.OrderBy(d => d.DispensingCode), "DispensingCode", "DispensingCode", medication.DispensingCode);
            ViewData["MedicationTypeId"] = new SelectList(_context.MedicationType, "MedicationTypeId", "Name", medication.MedicationTypeId);
            return View(medication);
        }

        /// <summary>
        /// action to delete selected Medication info
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>View</returns>
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medication
                .Include(m => m.ConcentrationCodeNavigation)
                .Include(m => m.DispensingCodeNavigation)
                .Include(m => m.MedicationType)
                .FirstOrDefaultAsync(m => m.Din == id);
            if (medication == null)
            {
                return NotFound();
            }

            var Medication = _context.Medication.Where(m => m.Din == id).FirstOrDefault();
            var MedicationType = _context.MedicationType.Where(m => m.MedicationTypeId == Medication.MedicationTypeId).FirstOrDefault();
            ViewData["MedicationTypeName"] = MedicationType.Name;
            ViewData["MedicationTypeId"] = id;
            return View(medication);
        }

        
        /// <summary>
        /// action to confirm to delete Medication info
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Index View</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var medication = await _context.Medication.FindAsync(id);
            _context.Medication.Remove(medication);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicationExists(string id)
        {
            return _context.Medication.Any(e => e.Din == id);
        }
    }
}
