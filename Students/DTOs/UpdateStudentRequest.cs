using UniversityCertificates.Students.Models;

namespace UniversityCertificates.Students.DTOs;

public record UpdateStudentRequest
{
    public int SerialNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public int? StudyYear { get; set; }
    public DegreeType? DegreeType { get; set; }
    public string? Department { get; set; }
}
