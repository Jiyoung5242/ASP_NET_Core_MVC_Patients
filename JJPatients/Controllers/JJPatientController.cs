using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JJPatients.Models;
using JJClassLibrary;

namespace JJPatients.Controllers
{
    public class JJPatientController : Controller
    {
        private readonly PatientsContext _context;

        public JJPatientController(PatientsContext context)
        {
            _context = context;
        }

        // GET: JJPatient
        public async Task<IActionResult> Index()
        {
            var patientsContext = _context.Patient.Include(p => p.ProvinceCodeNavigation).OrderBy(p=>p.LastName).ThenBy(p=>p.FirstName);

            return View(await patientsContext.ToListAsync());
        }

        // GET: JJPatient/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient
                .Include(p => p.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: JJPatient/Create
        public IActionResult Create()
        {
            //ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode");
            return View();
        }

        // POST: JJPatient/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,FirstName,LastName,Address,City,ProvinceCode,PostalCode,Ohip,DateOfBirth,Deceased,DateOfDeath,HomePhone,Gender")] Patient patient)
        {

            //patient.FirstName = JJValidation.JJCapitalize(patient.FirstName);
            //patient.LastName = JJValidation.JJCapitalize(patient.LastName);
            //patient.Address = JJValidation.JJCapitalize(patient.Address);
            //patient.City = JJValidation.JJCapitalize(patient.City);
            //patient.ProvinceCode = patient.ProvinceCode.ToUpper();
            //string countryCode = "";
            //if(patient.ProvinceCode != null)
            //{
            //    var province = _context.Province.Where(p => p.ProvinceCode == patient.ProvinceCode);

            //    if (!province.Any())
            //    {
            //        ModelState.AddModelError("ProvinceCode", "Province Code is not on file");
            //    }
            //    else
            //    {
            //        countryCode = province.FirstOrDefault().CountryCode;
            //    }
            //}
            

            //if (patient.PostalCode != null && (patient.ProvinceCode == null || countryCode == ""))
            //{
            //    ModelState.AddModelError("ProvinceCode", "Province Code is required to validate Postal Code");
            //}else if (patient.PostalCode != null)
            //{
            //    if (countryCode == "CA")
            //    {
            //        if (JJValidation.JJPostalCodeValidation(patient.PostalCode))
            //        {

            //            if (JJValidation.JJPostalCodeFirstChar(patient.PostalCode, patient.ProvinceCode))
            //            {
            //                patient.PostalCode = JJValidation.JJPostalCodeFormat(patient.PostalCode);

            //            }
            //            else
            //            {
            //                ModelState.AddModelError("PostalCode", "First letter of Postal Code is not valid for given province");

            //            }
            //        }
            //        else
            //        {
            //            ModelState.AddModelError("ProvinceCode", "Province Code is required to validate Canada Postal Code");
            //        }
            //    }
            //    else
            //    {
            //        string postcode = patient.PostalCode;
            //        if(JJValidation.JJZipCodeValidation(ref postcode))
            //        {
            //            patient.PostalCode = postcode;
            //        }
            //        else
            //        {
            //            ModelState.AddModelError("ProvinceCode", "Province Code is required to validate US Postal Code");
            //        }
            //    }
            //}

            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                TempData["message"] = "The data is successfuly created!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", patient.ProvinceCode);
            return View(patient);
        }

        // GET: JJPatient/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province.OrderBy(p=>p.Name), "ProvinceCode", "ProvinceCode", patient.ProvinceCode);
            return View(patient);
        }

        // POST: JJPatient/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,FirstName,LastName,Address,City,ProvinceCode,PostalCode,Ohip,DateOfBirth,Deceased,DateOfDeath,HomePhone,Gender")] Patient patient)
        {

            if (id != patient.PatientId)
            {
                return NotFound();
            }

            //string countryCode = "";
            //if (patient.ProvinceCode != null)
            //{
            //    var province = _context.Province.Where(p => p.ProvinceCode == patient.ProvinceCode);

            //    if (!province.Any())
            //    {
            //        ModelState.AddModelError("ProvinceCode", "Province Code is not on file");
            //    }
            //    else
            //    {
            //        countryCode = province.FirstOrDefault().CountryCode;
            //    }
            //}


            //if (patient.PostalCode != null && (patient.ProvinceCode == null || countryCode == ""))
            //{
            //    ModelState.AddModelError("ProvinceCode", "Province Code is required to validate Postal Code");
            //}
            //else if (patient.PostalCode != null)
            //{
            //    if (countryCode == "CA")
            //    {
            //        if (JJValidation.JJPostalCodeValidation(patient.PostalCode))
            //        {

            //            if (JJValidation.JJPostalCodeFirstChar(patient.PostalCode, patient.ProvinceCode))
            //            {
            //                patient.PostalCode = JJValidation.JJPostalCodeFormat(patient.PostalCode);

            //            }
            //            else
            //            {
            //                ModelState.AddModelError("PostalCode", "First letter of Postal Code is not valid for given province");

            //            }
            //        }
            //        else
            //        {
            //            ModelState.AddModelError("ProvinceCode", "Province Code is required to validate Canada Postal Code");
            //        }
            //    }
            //    else
            //    {
            //        string postcode = patient.PostalCode;
            //        if (JJValidation.JJZipCodeValidation(ref postcode))
            //        {
            //            patient.PostalCode = postcode;
            //        }
            //        else
            //        {
            //            ModelState.AddModelError("ProvinceCode", "Province Code is required to validate US Postal Code");
            //        }
            //    }
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                    TempData["message"]= "The data is successfuly edited!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientId))
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
            ViewData["ProvinceCode"] = new SelectList(_context.Province.OrderBy(p=>p.Name), "ProvinceCode", "ProvinceCode", patient.ProvinceCode);
            return View(patient);
        }

        // GET: JJPatient/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient
                .Include(p => p.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: JJPatient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patient.FindAsync(id);
            _context.Patient.Remove(patient);
            await _context.SaveChangesAsync();
            TempData["message"] = "The data is successfuly deleted!";
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patient.Any(e => e.PatientId == id);
        }
    }
}
