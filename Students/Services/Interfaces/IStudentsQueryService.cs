using UniversityCertificates.Students.Models;

namespace UniversityCertificates.Students.Services.Interfaces;

public interface IStudentsQueryService
{
    Task<IEnumerable<Student>> GetStudentsAsync();

    Task<Student> GetStudentBySerialNumberAsync(int serialNumber);
}
