using UniversityCertificates.Students.Models;

namespace UniversityCertificates.Students.DTOs;

public record CreateStudentRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required int StudyYear { get; set; }
    public required DegreeType DegreeType { get; set; }
    public required string Department { get; set; }
}
