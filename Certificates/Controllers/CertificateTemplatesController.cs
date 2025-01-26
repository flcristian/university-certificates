using Microsoft.AspNetCore.Mvc;
using UniversityCertificates.Certificates.Controllers.Interfaces;
using UniversityCertificates.Certificates.DTOs;
using UniversityCertificates.Certificates.Models;
using UniversityCertificates.Certificates.Services.Interfaces;
using UniversityCertificates.System.Exceptions;
using UniversityCertificates.System.Utility.DTOs;

namespace UniversityCertificates.Certificates.Controllers;

public class CertificateTemplatesController : CertificateTemplatesApiController
{
    private readonly ICertificateTemplatesQueryService _certificateTemplatesQueryService;
    private readonly ICertificateTemplatesCommandService _certificateTemplatesCommandService;
    private readonly ILogger<CertificateTemplatesController> _logger;

    public CertificateTemplatesController(
        ICertificateTemplatesQueryService certificateTemplatesQueryService,
        ICertificateTemplatesCommandService certificateTemplatesCommandService,
        ILogger<CertificateTemplatesController> logger
    )
    {
        _certificateTemplatesQueryService = certificateTemplatesQueryService;
        _certificateTemplatesCommandService = certificateTemplatesCommandService;
        _logger = logger;
    }

    public override async Task<
        ActionResult<IEnumerable<CertificateTemplate>>
    > GetCertificateTemplates()
    {
        try
        {
            IEnumerable<CertificateTemplate> certificateTemplates =
                await _certificateTemplatesQueryService.GetAllCertificateTemplatesAsync();
            return Ok(certificateTemplates);
        }
        catch (ItemsDoNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
    }

    public override async Task<ActionResult<CertificateTemplate>> GetCertificateTemplateById(int id)
    {
        try
        {
            CertificateTemplate certificateTemplate =
                await _certificateTemplatesQueryService.GetCertificateTemplateByIdAsync(id);
            return Ok(certificateTemplate);
        }
        catch (ItemDoesNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
    }

    public override async Task<ActionResult> GetCertificateTemplateFileById(int id)
    {
        try
        {
            GetFileRequest fileRequest =
                await _certificateTemplatesQueryService.GetCertificateTemplateFileByIdAsync(id);

            return File(
                fileContents: fileRequest.FileContents,
                contentType: fileRequest.ContentType,
                fileDownloadName: fileRequest.FileName
            );
        }
        catch (ItemDoesNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
    }

    public override async Task<ActionResult<CertificateTemplate>> CreateCertificateTemplate(
        CreateCertificateTemplateRequest request
    )
    {
        try
        {
            CertificateTemplate certificateTemplate =
                await _certificateTemplatesCommandService.AddCertificateTemplateAsync(request);
            return CreatedAtAction(
                nameof(GetCertificateTemplateById),
                new { id = certificateTemplate.Id },
                certificateTemplate
            );
        }
        catch (InvalidValueException e)
        {
            _logger.LogError(e.Message);
            return BadRequest(e.Message);
        }
        catch (ItemAlreadyExistsException e)
        {
            _logger.LogError(e.Message);
            return Conflict(e.Message);
        }
    }

    public override async Task<ActionResult<CertificateTemplate>> UpdateCertificateTemplate(
        UpdateCertificateTemplateRequest request
    )
    {
        try
        {
            CertificateTemplate certificateTemplate =
                await _certificateTemplatesCommandService.UpdateCertificateTemplateAsync(request);
            return Ok(certificateTemplate);
        }
        catch (InvalidValueException e)
        {
            _logger.LogError(e.Message);
            return BadRequest(e.Message);
        }
        catch (ItemDoesNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
        catch (ItemAlreadyExistsException e)
        {
            _logger.LogError(e.Message);
            return Conflict(e.Message);
        }
    }

    public override async Task<ActionResult<CertificateTemplate>> DeleteCertificateTemplate(int id)
    {
        try
        {
            CertificateTemplate certificateTemplate =
                await _certificateTemplatesCommandService.DeleteCertificateTemplateAsync(id);
            return Ok(certificateTemplate);
        }
        catch (ItemDoesNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
    }

    public override async Task<ActionResult<CertificateTemplate>> SoftDeleteCertificateTemplate(
        int id
    )
    {
        try
        {
            CertificateTemplate certificateTemplate =
                await _certificateTemplatesCommandService.SoftDeleteCertificateTemplateAsync(id);
            return Ok(certificateTemplate);
        }
        catch (ItemDoesNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
    }
}
