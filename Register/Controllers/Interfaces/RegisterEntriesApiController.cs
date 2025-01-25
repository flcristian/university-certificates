using Microsoft.AspNetCore.Mvc;
using UniversityCertificates.Register.DTOs;
using UniversityCertificates.Register.Models;

namespace UniversityCertificates.Register.Controllers.Interfaces;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class RegisterEntriesApiController : ControllerBase
{
    [HttpGet("all")]
    [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<RegisterEntry>))]
    [ProducesResponseType(statusCode: 404, type: typeof(String))]
    public abstract Task<ActionResult<IEnumerable<RegisterEntry>>> GetRegisterEntries();

    [HttpGet("register-entry/{id}")]
    [ProducesResponseType(statusCode: 200, type: typeof(RegisterEntry))]
    [ProducesResponseType(statusCode: 404, type: typeof(String))]
    public abstract Task<ActionResult<RegisterEntry>> GetRegisterEntryById([FromRoute] int id);

    [HttpPost("create")]
    [ProducesResponseType(statusCode: 201, type: typeof(RegisterEntry))]
    [ProducesResponseType(statusCode: 409, type: typeof(String))]
    public abstract Task<ActionResult<RegisterEntry>> CreateRegisterEntry(
        [FromBody] CreateRegisterEntryRequest request
    );

    [HttpPut("update")]
    [ProducesResponseType(statusCode: 202, type: typeof(RegisterEntry))]
    [ProducesResponseType(statusCode: 404, type: typeof(String))]
    public abstract Task<ActionResult<RegisterEntry>> UpdateRegisterEntry(
        [FromBody] UpdateRegisterEntryRequest request
    );

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(statusCode: 202, type: typeof(RegisterEntry))]
    [ProducesResponseType(statusCode: 404, type: typeof(String))]
    public abstract Task<ActionResult<RegisterEntry>> DeleteRegisterEntry([FromRoute] int id);
}
