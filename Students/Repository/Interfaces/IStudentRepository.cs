namespace UniversityCertificates.Students.Repository.Interfaces;

using UniversityCertificates.Students.DTOs;
using UniversityCertificates.Students.Models;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetStudentsAsync();

    Task<Student?> GetStudentAsync(int serialNumber);

    Task<Student> AddStudentAsync(CreateStudentRequest request);

    Task<Student> UpdateStudentAsync(UpdateStudentRequest request);

    Task<Student> DeleteStudentAsync(int serialNumber);
}
