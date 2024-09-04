using Hospital.Models;
using Microsoft.EntityFrameworkCore;
namespace Hospital;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Cabinet> Cabinets { get; set; }
    public DbSet<Specialization> Specializations { get; set; }
    public DbSet<District> Districts { get; set; }

    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
}