using Hospital.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PatientController:ControllerBase
{
    private readonly ApplicationDbContext _context;
    public PatientController(ApplicationDbContext context)
    {
        _context = context;
    }

  
    [HttpPost]
    public IActionResult AddPatient([FromBody]Patient patient)
    {
        if (patient.District != null)
        {
            var district = _context.Districts.Find(patient.District.Id);
            if (district != null)
            {
                patient.District = district;
            }
        }

        _context.Patients.Add(patient);
        _context.SaveChanges();
        return Ok();
    }
    
    [HttpPut("{id}")] 
    public async Task<IActionResult> UpdatePatient(int id, [FromBody] Patient updatePatient)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

       
        patient.Name = updatePatient.Name;
        patient.Surname = updatePatient.Surname;
        patient.Address = updatePatient.Address;
        patient.Gender = updatePatient.Gender;
        patient.DateBirth = updatePatient.DateBirth;
        patient.Patronymic = updatePatient.Patronymic;

        
        var district = await _context.Districts.FindAsync(updatePatient.District?.Id);
        if (district != null)
        {
            patient.District = district;
        }
        
        await _context.SaveChangesAsync();

        return Ok(patient);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();

        return NoContent(); 
    }

    [HttpGet("/find")]
    public async Task<IActionResult> GetPatients(
        [FromQuery] string sortBy = "surname",   
        [FromQuery] bool descending = false,     
        [FromQuery] int pageNumber = 1,          
        [FromQuery] int pageSize = 10)           
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _context.Patients
            .Include(p => p.District)  
            .AsQueryable();

        
        query = sortBy.ToLower() switch
        {
            "name" => descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "surname" => descending ? query.OrderByDescending(p => p.Surname) : query.OrderBy(p => p.Surname),
            "datebirth" => descending ? query.OrderByDescending(p => p.DateBirth) : query.OrderBy(p => p.DateBirth),
            _ => descending ? query.OrderByDescending(p => p.Surname) : query.OrderBy(p => p.Surname)
        };
        
        var totalItems = await query.CountAsync();
        var patients = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        
        var result = patients.Select(p => new
        {
            p.Id,
            p.Surname,
            p.Name,
            p.Patronymic,
            p.DateBirth,
            p.Address,
            p.Gender,
            District = p.District != null ? p.District.Number : "No District"
        });

        var response = new
        {
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Patients = result
        };

        return Ok(response);
    }

    [HttpGet("{id}/edit")]
    public async Task<IActionResult> GetPatientForEdit(int id)
    {
        var patient = await _context.Patients
            .Include(p => p.District)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
        {
            return NotFound("Patient not found.");
        }
        
        var result = new
        {
            patient.Id,
            patient.Surname,
            patient.Name,
            patient.Patronymic,
            patient.Address,
            patient.DateBirth,
            patient.Gender,
            DistrictId = patient.District?.Id  
        };

        return Ok(result);
    }

}