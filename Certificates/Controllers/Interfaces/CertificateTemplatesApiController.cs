using Microsoft.AspNetCore.Mvc;
using UniversityCertificates.Certificates.DTOs;
using UniversityCertificates.Certificates.Models;
using UniversityCertificates.System.Utility.DTOs;

namespace UniversityCertificates.Certificates.Controllers.Interfaces;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class CertificateTemplatesApiController : ControllerBase
{
    [HttpGet("all")]
    [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<CertificateTemplate>))]
    [ProducesResponseType(statusCode: 404, type: typeof(string))]
    public abstract Task<ActionResult<IEnumerable<CertificateTemplate>>> GetCertificateTemplates();

    [HttpGet("certificate-template/{id}")]
    [ProducesResponseType(statusCode: 200, type: typeof(CertificateTemplate))]
    [ProducesResponseType(statusCode: 404, type: typeof(string))]
    public abstract Task<ActionResult<CertificateTemplate>> GetCertificateTemplateById(
        [FromRoute] int id
    );

    [HttpGet("certificate-template/{id}/file")]
    [ProducesResponseType(statusCode: 200, type: typeof(GetFileRequest))]
    [ProducesResponseType(statusCode: 404, type: typeof(string))]
    public abstract Task<ActionResult> GetCertificateTemplateFileById([FromRoute] int id);

    [HttpPost("create")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(statusCode: 201, type: typeof(CertificateTemplate))]
    [ProducesResponseType(statusCode: 400, type: typeof(string))]
    [ProducesResponseType(statusCode: 409, type: typeof(string))]
    public abstract Task<ActionResult<CertificateTemplate>> CreateCertificateTemplate(
        [FromForm] CreateCertificateTemplateRequest request
    );

    [HttpPut("update")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(statusCode: 202, type: typeof(CertificateTemplate))]
    [ProducesResponseType(statusCode: 400, type: typeof(string))]
    [ProducesResponseType(statusCode: 404, type: typeof(string))]
    [ProducesResponseType(statusCode: 409, type: typeof(string))]
    public abstract Task<ActionResult<CertificateTemplate>> UpdateCertificateTemplate(
        [FromForm] UpdateCertificateTemplateRequest request
    );

    [HttpDelete("hard-delete/{id}")]
    [ProducesResponseType(statusCode: 202, type: typeof(CertificateTemplate))]
    [ProducesResponseType(statusCode: 404, type: typeof(string))]
    public abstract Task<ActionResult<CertificateTemplate>> DeleteCertificateTemplate(
        [FromRoute] int id
    );

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(statusCode: 202, type: typeof(CertificateTemplate))]
    [ProducesResponseType(statusCode: 404, type: typeof(string))]
    public abstract Task<ActionResult<CertificateTemplate>> SoftDeleteCertificateTemplate(
        [FromRoute] int id
    );
}
