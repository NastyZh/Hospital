using Hospital.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public DoctorController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    [HttpPost]
    public IActionResult AddDoctor([FromBody] Doctor doctor)
    {
        if (doctor.Cabinet != null)
        {
            var cabinet = _context.Cabinets.Find(doctor.Cabinet.Id);
            if (cabinet != null)
            {
                doctor.Cabinet = cabinet;
            }
        }

        if (doctor.Specialization != null)
        {
            var specialization = _context.Specializations.Find(doctor.Specialization.Id);
            if (specialization != null)
            {
                doctor.Specialization = specialization;
            }
        }

        _context.Doctors.Add(doctor);
        _context.SaveChanges();
        return Ok(doctor);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDoctor(int id, [FromBody] Doctor updatedDoctor)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor == null)
        {
            return NotFound("Doctor not found.");
        }

       
        if (updatedDoctor.DistrictId.HasValue)
        {
            var districtExists = await _context.Districts.AnyAsync(d => d.Id == updatedDoctor.DistrictId);
            if (!districtExists)
            {
                return BadRequest($"District with ID {updatedDoctor.DistrictId} does not exist.");
            }
        }

        
        doctor.FullName = updatedDoctor.FullName;
        doctor.Cabinet.Id = updatedDoctor.Cabinet.Id;
        doctor.Specialization.Id = updatedDoctor.Specialization.Id;
        doctor.DistrictId = updatedDoctor.DistrictId;

        
        await _context.SaveChangesAsync();

        return Ok(doctor);
    }

    
    
    [HttpDelete("{id}")]
    public IActionResult DeleteDoctor(int id)
    {
        var doctor = _context.Doctors.Find(id);
        if (doctor == null)
        {
            return NotFound();
        }
        _context.Doctors.Remove(doctor);
        _context.SaveChanges();
        return NoContent();
    }
   
    [HttpGet]
    public async Task<IActionResult> GetDoctors(
        [FromQuery] string sortBy = "Name",
        [FromQuery] bool descending = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _context.Doctors.AsQueryable();

        query = sortBy.ToLower() switch
        {
            "name" => descending ? query.OrderByDescending(d => d.FullName) : query.OrderBy(d => d.FullName),
            "specialization" => descending ? query.OrderByDescending(d => d.Specialization.Name) : query.OrderBy(d => d.Specialization.Name),
            _ => descending ? query.OrderByDescending(d => d.FullName) : query.OrderBy(d => d.FullName)
        };

        var totalItems = await query.CountAsync();
        var doctors = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = new
        {
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Doctors = doctors
        };

        return Ok(result);
    }
    
    [HttpGet("{id}/edit")]
    public async Task<IActionResult> GetDoctorForEdit(int id)
    {
        var doctor = await _context.Doctors
            .Include(d => d.Specialization)
            .Include(d => d.Cabinet)
            .Include(d => d.District)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doctor == null)
        {
            return NotFound(); 
        }

        var result = new
        {
            Id = doctor.Id,
            Name = doctor.FullName,
            SpecializationId = doctor.Specialization.Id,
            RoomId = doctor.Cabinet.Id, 
            DistrictId = doctor.DistrictId 
        };

        return Ok(result);
    }
}
