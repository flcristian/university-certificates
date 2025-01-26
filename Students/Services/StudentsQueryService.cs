using UniversityCertificates.Students.Models;
using UniversityCertificates.Students.Repository.Interfaces;
using UniversityCertificates.Students.Services.Interfaces;
using UniversityCertificates.System.Constants;
using UniversityCertificates.System.Exceptions;

namespace UniversityCertificates.Students.Services;

public class StudentsQueryService : IStudentsQueryService
{
    private readonly IStudentsRepository _studentRepository;

    public StudentsQueryService(IStudentsRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<IEnumerable<Student>> GetStudentsAsync()
    {
        IEnumerable<Student> students = await _studentRepository.GetStudentsAsync();

        if (students.Count() == 0)
        {
            throw new ItemsDoNotExistException(Constants.STUDENTS_DO_NOT_EXIST);
        }

        return students;
    }

    public async Task<Student> GetStudentBySerialNumberAsync(int serialNumber)
    {
        Student? student = await _studentRepository.GetStudentBySerialNumberAsync(serialNumber);

        if (student == null)
        {
            throw new ItemDoesNotExistException(Constants.STUDENT_DOES_NOT_EXIST);
        }

        return student;
    }
}
