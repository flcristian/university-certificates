using UniversityCertificates.Register.Models;
using UniversityCertificates.Register.Repository.Interfaces;
using UniversityCertificates.Register.Services.Interfaces;
using UniversityCertificates.System.Constants;
using UniversityCertificates.System.Exceptions;

namespace UniversityCertificates.Register.Services;

public class RegisterEntriesQueryService : IRegisterEntriesQueryService
{
    private readonly IRegisterEntriesRepository _registerEntriesRepository;

    public RegisterEntriesQueryService(IRegisterEntriesRepository registerEntriesRepository)
    {
        _registerEntriesRepository = registerEntriesRepository;
    }

    public async Task<IEnumerable<RegisterEntry>> GetRegisterEntriesAsync()
    {
        IEnumerable<RegisterEntry> registerEntries =
            await _registerEntriesRepository.GetRegisterEntriesAsync();

        if (registerEntries.Count() == 0)
        {
            throw new ItemsDoNotExistException(ConstantMessages.REGISTER_ENTRIES_DO_NOT_EXIST);
        }

        return registerEntries;
    }

    public async Task<RegisterEntry> GetRegisterEntryAsync(int id)
    {
        RegisterEntry? registerEntry = await _registerEntriesRepository.GetRegisterEntryByIdAsync(
            id
        );

        if (registerEntry == null)
        {
            throw new ItemDoesNotExistException(ConstantMessages.REGISTER_ENTRY_DOES_NOT_EXIST);
        }

        return registerEntry;
    }
}
