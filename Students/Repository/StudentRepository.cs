using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UniversityCertificates.Data;
using UniversityCertificates.Students.DTOs;
using UniversityCertificates.Students.Models;
using UniversityCertificates.Students.Repository.Interfaces;

namespace UniversityCertificates.Students.Repository;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public StudentRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Student?> GetStudentAsync(int serialNumber)
    {
        return await _context.Students.FindAsync(serialNumber);
    }

    public async Task<IEnumerable<Student>> GetStudentsAsync()
    {
        return await _context.Students.ToListAsync();
    }

    public async Task<Student> AddStudentAsync(CreateStudentRequest request)
    {
        Student student = _mapper.Map<Student>(request);

        EntityEntry<Student> addedStudent = _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return addedStudent.Entity;
    }

    public async Task<Student> UpdateStudentAsync(UpdateStudentRequest request)
    {
        Student student = (await _context.Students.FindAsync(request.SerialNumber))!;

        student.FirstName = request.FirstName ?? student.FirstName;
        student.LastName = request.LastName ?? student.LastName;
        student.StudyYear = request.StudyYear ?? student.StudyYear;
        student.DegreeType = request.DegreeType ?? student.DegreeType;
        student.Department = request.Department ?? student.Department;

        EntityEntry<Student> updatedStudent = _context.Students.Update(student);
        await _context.SaveChangesAsync();
        return updatedStudent.Entity;
    }

    public async Task<Student> DeleteStudentAsync(int serialNumber)
    {
        Student student = (await _context.Students.FindAsync(serialNumber))!;
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return student;
    }
}
