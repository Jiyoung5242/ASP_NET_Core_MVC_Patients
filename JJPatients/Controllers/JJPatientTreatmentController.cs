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
    public class JJPatientTreatmentController : Controller
    {
        private readonly PatientsContext _context;

        public JJPatientTreatmentController(PatientsContext context)
        {
            _context = context;
        }

        // GET: JJPatientTreatment
        public async Task<IActionResult> Index(string PatientDiagnosisId, string PatientName, string DiagnosisName)
        {

            if (!string.IsNullOrEmpty(PatientDiagnosisId))
            {
                Response.Cookies.Append("PatientDiagnosisId", PatientDiagnosisId);
                HttpContext.Session.SetString("PatientDiagnosisId", PatientDiagnosisId);
            }
            else if (Request.Query["PatientDiagnosisId"].Any())
            {
                PatientDiagnosisId = Request.Query["PatientDiagnosisId"].ToString();
                Response.Cookies.Append("PatientDiagnosisId", PatientDiagnosisId);
                HttpContext.Session.SetString("PatientDiagnosisId", PatientDiagnosisId);
            }
            else if (Request.Cookies["PatientDiagnosisId"] != null)
            {
                PatientDiagnosisId = Request.Cookies["PatientDiagnosisId"].ToString();
            }
            else if (HttpContext.Session.GetString("PatientDiagnosisId") != null)
            {
                PatientDiagnosisId = HttpContext.Session.GetString("PatientDiagnosisId");
                Response.Cookies.Append("PatientDiagnosisId", PatientDiagnosisId);
            }
            else
            {
                TempData["message"] = "Select Patient Id to see its diagnosis";
                return RedirectToAction("Index", "JJPatientDignosis");
                
            }

            if (PatientName != null)
            {
                Response.Cookies.Append("PatientName", PatientName);
            }else if (Request.Cookies["PatientName"] != null)
            {
                PatientName = Request.Cookies["PatientName"].ToString();
            }
            if(DiagnosisName != null)
            {
                Response.Cookies.Append("DiagnosisName", DiagnosisName);
            }else if (Request.Cookies["DiagnosisName"] != null)
            {
                DiagnosisName = Request.Cookies["DiagnosisName"].ToString();
            }

            var patientsContext = _context.PatientTreatment.Include(p => p.PatientDiagnosis).Include(p => p.Treatment).Where(p=>p.PatientDiagnosisId.ToString() == PatientDiagnosisId).OrderByDescending(p=>p.DatePrescribed);

            ViewData["DiagnosisName"] = DiagnosisName;
            ViewData["PatientName"] = PatientName;
            return View(await patientsContext.ToListAsync());
        }

        // GET: JJPatientTreatment/Details/5
        public async Task<IActionResult> Details(int? id, string PatientName, string DiagnosisName)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatment
                .Include(p => p.PatientDiagnosis)
                .Include(p => p.Treatment)
                .FirstOrDefaultAsync(m => m.PatientTreatmentId == id);
            if (patientTreatment == null)
            {
                return NotFound();
            }

            if (PatientName == null && Request.Cookies["PatientName"] != null)
            {
                PatientName = Request.Cookies["PatientName"].ToString();
            }
            if (DiagnosisName == null && Request.Cookies["DiagnosisName"] != null)
            {
                DiagnosisName = Request.Cookies["DiagnosisName"].ToString();
            }

            ViewData["DiagnosisName"] = DiagnosisName;
            ViewData["PatientName"] = PatientName;

            return View(patientTreatment);
        }

        // GET: JJPatientTreatment/Create
        public IActionResult Create(string PatientName, string DiagnosisName)
        {
            string PatientDiagnosisId = String.Empty;
            if (Request.Cookies["PatientDiagnosisId"] != null)
            {
                PatientDiagnosisId = Request.Cookies["PatientDiagnosisId"].ToString();
            }
            else if (HttpContext.Session.GetString("PatientDiagnosisId") != null)
            {
                PatientDiagnosisId = HttpContext.Session.GetString("PatientDiagnosisId");
                Response.Cookies.Append("PatientDiagnosisId", PatientDiagnosisId);
            }
            if (PatientName == null && Request.Cookies["PatientName"] != null)
            {
                PatientName = Request.Cookies["PatientName"].ToString();
            }
            if (DiagnosisName == null && Request.Cookies["DiagnosisName"] != null)
            {
                DiagnosisName = Request.Cookies["DiagnosisName"].ToString();
            }

            var PatientDiagnosisContext = _context.PatientDiagnosis.Where(p => p.PatientDiagnosisId.ToString() == PatientDiagnosisId).FirstOrDefault();
            //var DiagnosisId = PatientDiagnosisContext.DiagnosisId;

            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnosis, "PatientDiagnosisId", "PatientDiagnosisId", PatientDiagnosisContext.PatientDiagnosisId);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment.Where(p => p.DiagnosisId.ToString() == PatientDiagnosisId), "TreatmentId", "Name");
            ViewData["DiagnosisName"] = DiagnosisName;
            ViewData["PatientName"] = PatientName;

            var patientTreatment = _context.PatientTreatment.FirstOrDefault();
            patientTreatment.DatePrescribed = DateTime.Now;
            patientTreatment.PatientDiagnosisId = int.Parse(PatientDiagnosisId);
            return View(patientTreatment);
        }

        // POST: JJPatientTreatment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientTreatmentId,TreatmentId,DatePrescribed,Comments,PatientDiagnosisId")] PatientTreatment patientTreatment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patientTreatment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var PatientDiagnosisContext = _context.PatientDiagnosis.Where(p => p.PatientDiagnosisId == patientTreatment.PatientDiagnosisId).FirstOrDefault();
            //var DiagnosisId = PatientDiagnosisContext.DiagnosisId;

            string PatientName = String.Empty;
            string DiagnosisName = String.Empty;
            if (Request.Cookies["PatientName"] != null)
            {
                PatientName = Request.Cookies["PatientName"].ToString();
            }
            if (Request.Cookies["DiagnosisName"] != null)
            {
                DiagnosisName = Request.Cookies["DiagnosisName"].ToString();
            }
            ViewData["DiagnosisName"] = DiagnosisName;
            ViewData["PatientName"] = PatientName;

            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnosis, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment.Where(p=>p.DiagnosisId == patientTreatment.PatientDiagnosisId), "TreatmentId", "Name", patientTreatment.TreatmentId);
            
            return View();
        }

        // GET: JJPatientTreatment/Edit/5
        public async Task<IActionResult> Edit(int? id, string PatientName, string DiagnosisName)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatment.FindAsync(id);
            if (patientTreatment == null)
            {
                return NotFound();
            }

            var PatientDiagnosisContext = _context.PatientDiagnosis.Where(p => p.PatientDiagnosisId == id).FirstOrDefault();
            //var DiagnosisId = PatientDiagnosisContext.DiagnosisId;

            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnosis, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment.Where(p=>p.DiagnosisId == patientTreatment.PatientDiagnosisId), "TreatmentId", "Name", patientTreatment.TreatmentId);
            //ViewData["DatePrescribed"] = patientTreatment.DatePrescribed.ToString("dd MMMMM yyyy HH:mm");

            if (PatientName == null & Request.Cookies["PatientName"] != null)
            {
                PatientName = Request.Cookies["PatientName"].ToString();
            }
            if (DiagnosisName == null && Request.Cookies["DiagnosisName"] != null)
            {
                DiagnosisName = Request.Cookies["DiagnosisName"].ToString();
            }

            ViewData["DiagnosisName"] = DiagnosisName;
            ViewData["PatientName"] = PatientName;

            return View(patientTreatment);
        }

        // POST: JJPatientTreatment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientTreatmentId,TreatmentId,DatePrescribed,Comments,PatientDiagnosisId")] PatientTreatment patientTreatment)
        {
            if (id != patientTreatment.PatientTreatmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientTreatment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientTreatmentExists(patientTreatment.PatientTreatmentId))
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
            string PatientName = String.Empty;
            string DiagnosisName = String.Empty;
            if (Request.Cookies["PatientName"] != null)
            {
                PatientName = Request.Cookies["PatientName"].ToString();
            }
            if (Request.Cookies["DiagnosisName"] != null)
            {
                DiagnosisName = Request.Cookies["DiagnosisName"].ToString();
            }
            ViewData["DiagnosisName"] = DiagnosisName;
            ViewData["PatientName"] = PatientName;

            ViewData["PatientDiagnosisId"] = new SelectList(_context.PatientDiagnosis, "PatientDiagnosisId", "PatientDiagnosisId", patientTreatment.PatientDiagnosisId);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "Name", patientTreatment.TreatmentId);
            return View(patientTreatment);
        }

        // GET: JJPatientTreatment/Delete/5
        public async Task<IActionResult> Delete(int? id, string PatientName, string DiagnosisName)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientTreatment = await _context.PatientTreatment
                .Include(p => p.PatientDiagnosis)
                .Include(p => p.Treatment)
                .FirstOrDefaultAsync(m => m.PatientTreatmentId == id);
            if (patientTreatment == null)
            {
                return NotFound();
            }
            if (PatientName == null & Request.Cookies["PatientName"] != null)
            {
                PatientName = Request.Cookies["PatientName"].ToString();
            }
            if (DiagnosisName == null && Request.Cookies["DiagnosisName"] != null)
            {
                DiagnosisName = Request.Cookies["DiagnosisName"].ToString();
            }

            ViewData["DiagnosisName"] = DiagnosisName;
            ViewData["PatientName"] = PatientName;
            return View(patientTreatment);
        }

        // POST: JJPatientTreatment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientTreatment = await _context.PatientTreatment.FindAsync(id);
            _context.PatientTreatment.Remove(patientTreatment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientTreatmentExists(int id)
        {
            return _context.PatientTreatment.Any(e => e.PatientTreatmentId == id);
        }
    }
}
