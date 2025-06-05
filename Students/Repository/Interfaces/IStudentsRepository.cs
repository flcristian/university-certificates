namespace UniversityCertificates.Students.Repository.Interfaces;

using UniversityCertificates.Students.DTOs;
using UniversityCertificates.Students.Models;

public interface IStudentsRepository
{
    Task<IEnumerable<Student>> GetStudentsAsync();

    Task<Student?> GetStudentBySerialNumberAsync(int serialNumber);

    Task<Student> AddStudentAsync(CreateStudentRequest request);

    Task<Student> UpdateStudentAsync(UpdateStudentRequest request);

    Task<Student> DeleteStudentAsync(int serialNumber);

    Task ClearStudentsAsync();
}
