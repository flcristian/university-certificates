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
    private readonly ILogger<RegisterEntriesController> _logger;

    public RegisterEntriesController(
        IRegisterEntriesQueryService registerEntriesQueryService,
        IRegisterEntriesCommandService registerEntriesCommandService,
        ILogger<RegisterEntriesController> logger
    )
    {
        _registerEntriesQueryService = registerEntriesQueryService;
        _registerEntriesCommandService = registerEntriesCommandService;
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
}
