using UniversityCertificates.Students.Models;

namespace UniversityCertificates.Students.DTOs;

public class CreateStudentRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required int StudyYear { get; set; }
    public required DegreeType DegreeType { get; set; }
    public required string Department { get; set; }
}
