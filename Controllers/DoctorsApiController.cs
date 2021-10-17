using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etap12.Controllers
{
    [Route("api/Doctors")]
    [ApiController]
    public class DoctorsApiController : ControllerBase
    {
        private readonly ISTP1Context _context;

        public DoctorsApiController(ISTP1Context context)
        {
            _context = context;
        }

        // GET: api/Doctors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Object>>> GetDoctors()
        {
            return await _context.Doctors
                .Select(d => new { d.DoctorId, d.DoctorName, d.DateStartWork, d.Education, d.DoctorPhoneNumber, d.DoctorPhoto, d.HospitalId, patientsCount = d.Patients.Count})
                .ToListAsync();
        }

        // GET: api/Doctors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Object>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            var plist = await _context.Patients
                .Where(p => p.DoctorId == id)
                .Select(p => new { p.PatientId, p.PatientName})
                .ToListAsync();

            return new { doctor.DoctorId, doctor.DoctorName, doctor.DateStartWork, doctor.Education, doctor.DoctorPhoneNumber, doctor.DoctorPhoto, doctor.HospitalId, patients = plist };
        }

        // PUT: api/Doctors/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctor(int id, Doctor doctor)
        {
            if (id != doctor.DoctorId)
            {
                return BadRequest();
            }

            _context.Entry(doctor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Doctors
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Object>> PostDoctor(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDoctor", new { id = doctor.DoctorId }, new { doctor.DoctorId, doctor.DoctorName, doctor.DateStartWork, doctor.Education, doctor.DoctorPhoneNumber, doctor.DoctorPhoto, doctor.HospitalId });
        }

        // DELETE: api/Doctors/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Doctor>> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.DoctorId == id);
        }
    }
}
