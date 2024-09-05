using Hospital;
using Hospital.Controllers;
using Hospital.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalTest;
[TestFixture]
public class DoctorControllerTests
{
    private ApplicationDbContext _context;
    private DoctorController _controller;
    
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new DoctorController(_context);
    }

    [Test]
    public void AddDoctor_ShouldReturnOk_WhenDoctorIsValid()
    {
        var doctor = new Doctor
        {
            FullName = "John Doe",
            Cabinet = new Cabinet { Number = "101" },
            Specialization = new Specialization { Name = "Cardiology" }
        };
        
        var result = _controller.AddDoctor(doctor);
        Assert.IsInstanceOf<OkObjectResult>(result);
        var addedDoctor = _context.Doctors.FirstOrDefault(d => d.FullName == "John Doe");
        Assert.IsNotNull(addedDoctor);
        Assert.AreEqual("John Doe", addedDoctor.FullName);
    }
}