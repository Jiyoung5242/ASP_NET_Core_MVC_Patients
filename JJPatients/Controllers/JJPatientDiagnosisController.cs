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
    public class JJPatientDiagnosisController : Controller
    {
        private readonly PatientsContext _context;

        public JJPatientDiagnosisController(PatientsContext context)
        {
            _context = context;
        }

        // GET: JJPatientDiagnosis
        public async Task<IActionResult> Index(string PatientId)
        {

            //if (!string.IsNullOrEmpty(PatientId))
            //{
            //    Response.Cookies.Append("PatientId", PatientId);
            //    HttpContext.Session.SetString("PatientId", PatientId);
            //}
            //else if (Request.Query["PatientId"].Any())
            //{
            //    PatientId = Request.Query["PatientId"].ToString();
            //    Response.Cookies.Append("PatientId", PatientId);
            //    HttpContext.Session.SetString("PatientId", PatientId);
            //}
            //else if (Request.Cookies["PatientId"] != null)
            //{
            //    PatientId = Request.Cookies["PatientId"].ToString();
            //}
            //else if (HttpContext.Session.GetString("PatientId") != null)
            //{
            //    PatientId = HttpContext.Session.GetString("PatientId");
            //    Response.Cookies.Append("PatientId", PatientId);
            //}
            //else
            //{
            //    //TempData["message"] = "Select Patient Id to see its diagnosis";
            //    //return RedirectToAction("Index", "JJPatient");
            //    PatientId = "*";
            //}

            IQueryable<PatientDiagnosis> patientsContext;
            if (PatientId != null)
            {
                patientsContext = _context.PatientDiagnosis.Include(p => p.Diagnosis).Include(p => p.Patient).Include(p => p.PatientTreatment).Where(p => p.PatientId.ToString() == PatientId).OrderBy(p => p.Patient.LastName).ThenBy(p => p.Patient.FirstName).ThenByDescending(p => p.PatientDiagnosisId); ;
            }
            else
            {
                patientsContext = _context.PatientDiagnosis.Include(p => p.Diagnosis).Include(p => p.Patient).Include(p => p.PatientTreatment).OrderBy(p => p.Patient.LastName).ThenBy(p => p.Patient.FirstName).ThenByDescending(p => p.PatientDiagnosisId);
            }

            //patientsContext.OrderBy(p => p.Patient.LastName).ThenBy(p => p.Patient.FirstName).ThenByDescending (p=>p.PatientDiagnosisId);
            //patientsContext.OrderByDescending(p => p.PatientDiagnosisId);

            //var result = from patient in patientsContext
            //             join treatment in _context.PatientTreatment on patient.PatientDiagnosisId equals treatment.PatientDiagnosisId
            //             select new PatientDiagnosis
            //             {
            //                 PatientDiagnosisId = patient.PatientDiagnosisId,
            //                 PatientId = patient.PatientId,
            //                 DiagnosisId = patient.DiagnosisId,
            //                 Comments = patient.Comments,
            //                 PatientTreatment = (ICollection<PatientTreatment>)treatment
            //             };
            //var patentTreatement = _context.PatientDiagnosis
            //patientsContext = patientsContext.OrderBy(p => p.Patient.FirstName).ThenBy(p => p.Patient.LastName);
            //patientsContext = patientsContext.OrderByDescending(p => p.PatientTreatment.Any(item => item.PatientDiagnosisId == p.PatientDiagnosisId));
            //patientsContext.OrderByDescending(p => p.PatientTreatment.OrderByDescending(p => p.DatePrescribed));
            return View(await patientsContext.ToListAsync());
        }

        // GET: JJPatientDiagnosis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientDiagnosis = await _context.PatientDiagnosis
                .Include(p => p.Diagnosis)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientDiagnosisId == id);
            if (patientDiagnosis == null)
            {
                return NotFound();
            }

            return View(patientDiagnosis);
        }

        // GET: JJPatientDiagnosis/Create
        public IActionResult Create()
        {
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnosis, "DiagnosisId", "Name");
            ViewData["PatientId"] = new SelectList(_context.Patient, "PatientId", "FirstName");
            return View();
        }

        // POST: JJPatientDiagnosis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientDiagnosisId,PatientId,DiagnosisId,Comments")] PatientDiagnosis patientDiagnosis)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patientDiagnosis);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnosis, "DiagnosisId", "Name", patientDiagnosis.DiagnosisId);
            ViewData["PatientId"] = new SelectList(_context.Patient, "PatientId", "FirstName", patientDiagnosis.PatientId);
            return View(patientDiagnosis);
        }

        // GET: JJPatientDiagnosis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientDiagnosis = await _context.PatientDiagnosis.FindAsync(id);
            if (patientDiagnosis == null)
            {
                return NotFound();
            }
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnosis, "DiagnosisId", "Name", patientDiagnosis.DiagnosisId);
            ViewData["PatientId"] = new SelectList(_context.Patient, "PatientId", "FirstName", patientDiagnosis.PatientId);
            return View(patientDiagnosis);
        }

        // POST: JJPatientDiagnosis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientDiagnosisId,PatientId,DiagnosisId,Comments")] PatientDiagnosis patientDiagnosis)
        {
            if (id != patientDiagnosis.PatientDiagnosisId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientDiagnosis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientDiagnosisExists(patientDiagnosis.PatientDiagnosisId))
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
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnosis, "DiagnosisId", "Name", patientDiagnosis.DiagnosisId);
            ViewData["PatientId"] = new SelectList(_context.Patient, "PatientId", "FirstName", patientDiagnosis.PatientId);
            return View(patientDiagnosis);
        }

        // GET: JJPatientDiagnosis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientDiagnosis = await _context.PatientDiagnosis
                .Include(p => p.Diagnosis)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PatientDiagnosisId == id);
            if (patientDiagnosis == null)
            {
                return NotFound();
            }

            return View(patientDiagnosis);
        }

        // POST: JJPatientDiagnosis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientDiagnosis = await _context.PatientDiagnosis.FindAsync(id);
            _context.PatientDiagnosis.Remove(patientDiagnosis);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientDiagnosisExists(int id)
        {
            return _context.PatientDiagnosis.Any(e => e.PatientDiagnosisId == id);
        }
    }
}
