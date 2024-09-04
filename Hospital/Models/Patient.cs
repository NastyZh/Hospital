namespace Hospital.Models;

public class Patient
{
    public int Id { get; set; }
    public string Surname { get; set; }
    public string Name { get; set; }
    public string Patronymic { get; set; }
    public string Address { get; set; }
    public DateTime DateBirth { get; set; }
    public string Gender { get; set; }
    public District  District  { get; set; }
}