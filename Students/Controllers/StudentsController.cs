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
    private readonly IStudentsXlsxService _studentsXlsxService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(
        IStudentsQueryService studentQueryService,
        IStudentsCommandService studentCommandService,
        IStudentsXlsxService studentsXlsxService,
        ILogger<StudentsController> logger
    )
    {
        _studentQueryService = studentQueryService;
        _studentCommandService = studentCommandService;
        _studentsXlsxService = studentsXlsxService;
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

    public override async Task<ActionResult> ExportStudentsToExcel()
    {
        try
        {
            byte[] excelBytes = await _studentsXlsxService.GenerateExcelForStudentsAsync();

            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "students.xlsx"
            );
        }
        catch (ItemsDoNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
    }

    public override async Task<ActionResult<IEnumerable<Student>>> ImportStudentsFromExcel(
        IFormFile file
    )
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only .xlsx files are allowed.");
            }

            using var stream = file.OpenReadStream();
            IEnumerable<Student> importedStudents =
                await _studentsXlsxService.ImportStudentsFromExcelAsync(stream);

            return CreatedAtAction(nameof(GetStudents), importedStudents);
        }
        catch (InvalidValueException e)
        {
            _logger.LogError(e.Message);
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error importing students from Excel");
            return BadRequest("An error occurred while importing the Excel file.");
        }
    }
}
