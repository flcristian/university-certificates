using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UniversityCertificates.Register.Controllers.Interfaces;
using UniversityCertificates.Register.DTOs;
using UniversityCertificates.Register.Models;
using UniversityCertificates.Register.Services.Interfaces;
using UniversityCertificates.System.Exceptions;

namespace UniversityCertificates.Register.Controllers;

public class RegisterEntriesController : RegisterEntriesApiController
{
    private readonly IRegisterEntriesQueryService _registerEntriesQueryService;
    private readonly IRegisterEntriesCommandService _registerEntriesCommandService;
    private readonly IRegisterEntryQRCodesService _registerEntryQRCodesService;
    private readonly IRegisterEntryDocxService _registerEntryDocumentsService;
    private readonly IRegisterEntryXlsxService _registerEntryXlsxService;
    private readonly ILogger<RegisterEntriesController> _logger;

    public RegisterEntriesController(
        IRegisterEntriesQueryService registerEntriesQueryService,
        IRegisterEntriesCommandService registerEntriesCommandService,
        IRegisterEntryQRCodesService registerEntryQRCodesService,
        IRegisterEntryDocxService registerEntryDocumentsService,
        IRegisterEntryXlsxService registerEntryXlsxService,
        ILogger<RegisterEntriesController> logger
    )
    {
        _registerEntriesQueryService = registerEntriesQueryService;
        _registerEntriesCommandService = registerEntriesCommandService;
        _registerEntryQRCodesService = registerEntryQRCodesService;
        _registerEntryDocumentsService = registerEntryDocumentsService;
        _registerEntryXlsxService = registerEntryXlsxService;
        _logger = logger;
    }

    public override async Task<ActionResult<IEnumerable<RegisterEntry>>> GetRegisterEntries()
    {
        try
        {
            IEnumerable<RegisterEntry> registerEntries =
                await _registerEntriesQueryService.GetRegisterEntriesAsync();

            return Ok(registerEntries);
        }
        catch (ItemsDoNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
    }

    public override async Task<ActionResult<RegisterEntry>> GetRegisterEntryById([FromRoute] int id)
    {
        try
        {
            RegisterEntry registerEntry = await _registerEntriesQueryService.GetRegisterEntryAsync(
                id
            );

            return Ok(registerEntry);
        }
        catch (ItemDoesNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
    }

    public override async Task<ActionResult<RegisterEntry>> CreateRegisterEntry(
        [FromBody] CreateRegisterEntryRequest request
    )
    {
        try
        {
            RegisterEntry registerEntry =
                await _registerEntriesCommandService.AddRegisterEntryAsync(request);

            return CreatedAtAction(
                nameof(GetRegisterEntryById),
                new { id = registerEntry.Id },
                registerEntry
            );
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

    public override async Task<ActionResult<RegisterEntry>> UpdateRegisterEntry(
        [FromBody] UpdateRegisterEntryRequest request
    )
    {
        try
        {
            RegisterEntry registerEntry =
                await _registerEntriesCommandService.UpdateRegisterEntryAsync(request);

            return Accepted(registerEntry);
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
        catch (ItemAlreadyExistsException e)
        {
            _logger.LogError(e.Message);
            return Conflict(e.Message);
        }
    }

    public override async Task<ActionResult<RegisterEntry>> DeleteRegisterEntry([FromRoute] int id)
    {
        try
        {
            RegisterEntry registerEntry =
                await _registerEntriesCommandService.DeleteRegisterEntryAsync(id);

            return Accepted(registerEntry);
        }
        catch (ItemDoesNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
    }

    public override async Task<ActionResult> GenerateQRCodeForRegisterEntryById([FromRoute] int id)
    {
        try
        {
            byte[] qrCode =
                await _registerEntryQRCodesService.GenerateQRCodeForRegisterEntryByIdAsync(id);

            if (qrCode == null || qrCode.Length == 0)
            {
                return BadRequest("Failed to generate QR code");
            }

            return File(qrCode, "image/png");
        }
        catch (ItemDoesNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
    }

    public override async Task<ActionResult> GenerateCertificateForRegisterEntryById(
        [FromRoute] int id
    )
    {
        try
        {
            byte[] qrCode =
                await _registerEntryQRCodesService.GenerateQRCodeForRegisterEntryByIdAsync(id);

            byte[] certificate =
                await _registerEntryDocumentsService.GenerateCertificateForRegisterEntryByIdAsync(
                    id,
                    qrCode
                );

            if (certificate == null || certificate.Length == 0)
            {
                return BadRequest("Failed to generate certificate");
            }

            return File(
                certificate,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "certificate.docx"
            );
        }
        catch (ItemDoesNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
    }

    public override async Task<ActionResult> GenerateRegisterEntriesExcel()
    {
        try
        {
            byte[] excelBytes =
                await _registerEntryXlsxService.GenerateExcelForRegisterEntriesAsync();

            if (excelBytes == null || excelBytes.Length == 0)
            {
                return BadRequest("Failed to generate Excel file");
            }

            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "register-export.xlsx"
            );
        }
        catch (ItemsDoNotExistException e)
        {
            _logger.LogError(e.Message);
            return NotFound(e.Message);
        }
    }
}
