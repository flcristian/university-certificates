using UniversityCertificates.Students.DTOs;
using UniversityCertificates.Students.Models;

namespace UniversityCertificates.Students.Services.Interfaces;

public interface IStudentsCommandService
{
    Task<Student> AddStudentAsync(CreateStudentRequest request);

    Task<Student> UpdateStudentAsync(UpdateStudentRequest request);

    Task<Student> DeleteStudentAsync(int serialNumber);
}
