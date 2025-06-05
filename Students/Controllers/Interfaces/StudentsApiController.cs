using Microsoft.AspNetCore.Mvc;
using UniversityCertificates.Students.DTOs;
using UniversityCertificates.Students.Models;

namespace UniversityCertificates.Students.Controllers.Interfaces;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class StudentsApiController : ControllerBase
{
    [HttpGet("all")]
    [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<Student>))]
    [ProducesResponseType(statusCode: 404, type: typeof(string))]
    public abstract Task<ActionResult<IEnumerable<Student>>> GetStudents();

    [HttpGet("student/{serialNumber}")]
    [ProducesResponseType(statusCode: 200, type: typeof(Student))]
    [ProducesResponseType(statusCode: 404, type: typeof(string))]
    public abstract Task<ActionResult<Student>> GetStudentBySerialNumber(
        [FromRoute] int serialNumber
    );

    [HttpPost("create")]
    [ProducesResponseType(statusCode: 201, type: typeof(Student))]
    [ProducesResponseType(statusCode: 409, type: typeof(string))]
    public abstract Task<ActionResult<Student>> CreateStudent(
        [FromBody] CreateStudentRequest request
    );

    [HttpPut("update")]
    [ProducesResponseType(statusCode: 202, type: typeof(Student))]
    [ProducesResponseType(statusCode: 400, type: typeof(string))]
    [ProducesResponseType(statusCode: 404, type: typeof(string))]
    public abstract Task<ActionResult<Student>> UpdateStudent(
        [FromBody] UpdateStudentRequest request
    );

    [HttpDelete("delete/{serialNumber}")]
    [ProducesResponseType(statusCode: 202, type: typeof(Student))]
    [ProducesResponseType(statusCode: 404, type: typeof(string))]
    public abstract Task<ActionResult<Student>> DeleteStudent([FromRoute] int serialNumber);

    [HttpGet("export")]
    [ProducesResponseType(statusCode: 200, type: typeof(FileContentResult))]
    [ProducesResponseType(statusCode: 404, type: typeof(string))]
    public abstract Task<ActionResult> ExportStudentsToExcel();

    [HttpPost("import")]
    [ProducesResponseType(statusCode: 201, type: typeof(IEnumerable<Student>))]
    [ProducesResponseType(statusCode: 400, type: typeof(string))]
    public abstract Task<ActionResult<IEnumerable<Student>>> ImportStudentsFromExcel(
        IFormFile file
    );
}
