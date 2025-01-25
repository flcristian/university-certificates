using UniversityCertificates.Register.DTOs;
using UniversityCertificates.Register.Models;
using UniversityCertificates.Register.Repository.Interfaces;
using UniversityCertificates.Register.Services.Interfaces;
using UniversityCertificates.System.Constants;
using UniversityCertificates.System.Exceptions;

namespace UniversityCertificates.Register.Services;

public class RegisterEntriesCommandService : IRegisterEntriesCommandService
{
    private readonly IRegisterEntriesRepository _registerEntriesRepository;

    public RegisterEntriesCommandService(IRegisterEntriesRepository registerEntriesRepository)
    {
        _registerEntriesRepository = registerEntriesRepository;
    }

    public async Task<RegisterEntry> AddRegisterEntryAsync(CreateRegisterEntryRequest request)
    {
        return await _registerEntriesRepository.AddRegisterEntryAsync(request);
    }

    public async Task<RegisterEntry> UpdateRegisterEntryAsync(UpdateRegisterEntryRequest request)
    {
        RegisterEntry? registerEntry = await _registerEntriesRepository.GetRegisterEntryAsync(
            request.Id
        );

        if (registerEntry == null)
        {
            throw new ItemDoesNotExistException(Constants.REGISTER_ENTRY_DOES_NOT_EXIST);
        }

        return await _registerEntriesRepository.UpdateRegisterEntryAsync(request);
    }

    public async Task<RegisterEntry> DeleteRegisterEntryAsync(int id)
    {
        RegisterEntry? registerEntry = await _registerEntriesRepository.GetRegisterEntryAsync(id);

        if (registerEntry == null)
        {
            throw new ItemDoesNotExistException(Constants.REGISTER_ENTRY_DOES_NOT_EXIST);
        }

        return await _registerEntriesRepository.DeleteRegisterEntryAsync(id);
    }
}
