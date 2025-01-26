using Microsoft.AspNetCore.Mvc;
using UniversityCertificates.Students.Controllers.Interfaces;
using UniversityCertificates.Students.DTOs;
using UniversityCertificates.Students.Models;
using UniversityCertificates.Students.Services.Interfaces;
using UniversityCertificates.System.Exceptions;

namespace UniversityCertificates.Students.Controllers;

public class StudentsController : StudentsApiController
{
    private readonly IStudentsQueryService _studentQueryService;
    private readonly IStudentsCommandService _studentCommandService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(
        IStudentsQueryService studentQueryService,
        IStudentsCommandService studentCommandService,
        ILogger<StudentsController> logger
    )
    {
        _studentQueryService = studentQueryService;
        _studentCommandService = studentCommandService;
        _logger = logger;
    }

    public override async Task<ActionResult<IEnumerable<Student>>> GetStudents()
    {
        try
        {
            IEnumerable<Student> students = await _studentQueryService.GetStudentsAsync();

            return Ok(students);
        }
        catch (ItemsDoNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
    }

    public override async Task<ActionResult<Student>> GetStudentBySerialNumber(int serialNumber)
    {
        try
        {
            Student student = await _studentQueryService.GetStudentBySerialNumberAsync(
                serialNumber
            );

            return Ok(student);
        }
        catch (ItemDoesNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
    }

    public override async Task<ActionResult<Student>> CreateStudent(CreateStudentRequest request)
    {
        try
        {
            Student student = await _studentCommandService.AddStudentAsync(request);

            return CreatedAtAction(
                nameof(GetStudentBySerialNumber),
                new { serialNumber = student.SerialNumber },
                student
            );
        }
        catch (ItemAlreadyExistsException e)
        {
            _logger.LogError(e.Message);
            return Conflict(e.Message);
        }
    }

    public override async Task<ActionResult<Student>> UpdateStudent(UpdateStudentRequest request)
    {
        try
        {
            Student student = await _studentCommandService.UpdateStudentAsync(request);

            return Accepted(student);
        }
        catch (ItemDoesNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
        catch (InvalidValueException e)
        {
            _logger.LogError(e.Message);
            return BadRequest(e.Message);
        }
    }

    public override async Task<ActionResult<Student>> DeleteStudent(int serialNumber)
    {
        try
        {
            Student student = await _studentCommandService.DeleteStudentAsync(serialNumber);

            return Accepted(student);
        }
        catch (ItemDoesNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
    }
}
