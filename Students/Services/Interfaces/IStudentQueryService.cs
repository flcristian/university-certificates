using UniversityCertificates.Students.Models;

namespace UniversityCertificates.Students.Services.Interfaces;

public interface IStudentQueryService
{
    Task<IEnumerable<Student>> GetStudentsAsync();

    Task<Student> GetStudentAsync(int serialNumber);
}
