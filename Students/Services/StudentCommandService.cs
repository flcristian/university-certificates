using UniversityCertificates.Students.DTOs;
using UniversityCertificates.Students.Models;
using UniversityCertificates.Students.Repository.Interfaces;
using UniversityCertificates.Students.Services.Interfaces;
using UniversityCertificates.System.Constants;
using UniversityCertificates.System.Exceptions;

namespace UniversityCertificates.Students.Services;

public class StudentCommandService : IStudentCommandService
{
    private readonly IStudentRepository _studentRepository;

    public StudentCommandService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Student> AddStudentAsync(CreateStudentRequest request)
    {
        return await _studentRepository.AddStudentAsync(request);
    }

    public async Task<Student> UpdateStudentAsync(UpdateStudentRequest request)
    {
        Student? student = await _studentRepository.GetStudentAsync(request.SerialNumber);

        if (student == null)
        {
            throw new ItemDoesNotExistException(Constants.STUDENT_DOES_NOT_EXIST);
        }

        return await _studentRepository.UpdateStudentAsync(request);
    }

    public async Task<Student> DeleteStudentAsync(int serialNumber)
    {
        Student? student = await _studentRepository.GetStudentAsync(serialNumber);

        if (student == null)
        {
            throw new ItemDoesNotExistException(Constants.STUDENT_DOES_NOT_EXIST);
        }

        return await _studentRepository.DeleteStudentAsync(serialNumber);
    }
}
