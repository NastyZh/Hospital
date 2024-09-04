namespace Hospital.Models;

public class Doctor
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public Cabinet Cabinet { get; set; }
    public Specialization Specialization { get; set; }
    public int? DistrictId { get; set; } 
    public District  District  { get; set; }
    
}