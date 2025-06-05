using UniversityCertificates.Students.DTOs;
using UniversityCertificates.Students.Models;
using UniversityCertificates.Students.Repository.Interfaces;
using UniversityCertificates.Students.Services.Interfaces;
using UniversityCertificates.System.Constants;
using UniversityCertificates.System.Exceptions;

namespace UniversityCertificates.Students.Services;

public class StudentsCommandService : IStudentsCommandService
{
    private readonly IStudentsRepository _studentRepository;

    public StudentsCommandService(IStudentsRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Student> AddStudentAsync(CreateStudentRequest request)
    {
        if (
            (await _studentRepository.GetStudentsAsync()).Any(s =>
                s.Email.Equals(request.Email, StringComparison.InvariantCultureIgnoreCase)
                || s.SerialNumber == request.SerialNumber
            )
        )
        {
            throw new ItemAlreadyExistsException(ConstantMessages.STUDENT_ALREADY_EXISTS);
        }

        return await _studentRepository.AddStudentAsync(request);
    }

    public async Task<Student> UpdateStudentAsync(UpdateStudentRequest request)
    {
        Student? student = await _studentRepository.GetStudentBySerialNumberAsync(
            request.SerialNumber
        );

        if (student == null)
        {
            throw new ItemDoesNotExistException(ConstantMessages.STUDENT_DOES_NOT_EXIST);
        }

        if (
            (request.FirstName == null || request.FirstName == student.FirstName)
            && (request.LastName == null || request.LastName == student.LastName)
            && (request.StudyYear == null || request.StudyYear == student.StudyYear)
            && (request.DegreeType == null || request.DegreeType == student.DegreeType)
            && (request.Department == null || request.Department == student.Department)
        )
        {
            throw new InvalidValueException(ConstantMessages.STUDENT_NOT_MODIFIED);
        }

        return await _studentRepository.UpdateStudentAsync(request);
    }

    public async Task<Student> DeleteStudentAsync(int serialNumber)
    {
        Student? student = await _studentRepository.GetStudentBySerialNumberAsync(serialNumber);

        if (student == null)
        {
            throw new ItemDoesNotExistException(ConstantMessages.STUDENT_DOES_NOT_EXIST);
        }

        return await _studentRepository.DeleteStudentAsync(serialNumber);
    }
}
