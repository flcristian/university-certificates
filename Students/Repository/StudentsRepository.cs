using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UniversityCertificates.Data;
using UniversityCertificates.Students.DTOs;
using UniversityCertificates.Students.Models;
using UniversityCertificates.Students.Repository.Interfaces;

namespace UniversityCertificates.Students.Repository;

public class StudentsRepository : IStudentsRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public StudentsRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Student>> GetStudentsAsync()
    {
        return await _context.Students.Include(s => s.RegisterEntries).ToListAsync();
    }

    public async Task<Student?> GetStudentBySerialNumberAsync(int serialNumber)
    {
        return await _context
            .Students.Include(s => s.RegisterEntries)
            .FirstOrDefaultAsync(s => s.SerialNumber == serialNumber);
    }

    public async Task<Student> AddStudentAsync(CreateStudentRequest request)
    {
        Student student = _mapper.Map<Student>(request);

        Student addedStudent = _context.Students.Add(student).Entity;
        await _context.SaveChangesAsync();
        return addedStudent;
    }

    public async Task<Student> UpdateStudentAsync(UpdateStudentRequest request)
    {
        Student student = (await _context.Students.FindAsync(request.SerialNumber))!;

        student.FirstName = request.FirstName ?? student.FirstName;
        student.LastName = request.LastName ?? student.LastName;
        student.Email = request.Email ?? student.Email;
        student.StudyYear = request.StudyYear ?? student.StudyYear;
        student.DegreeType = request.DegreeType ?? student.DegreeType;
        student.Department = request.Department ?? student.Department;

        Student updatedStudent = _context.Students.Update(student).Entity;
        await _context.SaveChangesAsync();
        return updatedStudent;
    }

    public async Task<Student> DeleteStudentAsync(int serialNumber)
    {
        Student student = (await _context.Students.FindAsync(serialNumber))!;
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return student;
    }
}
