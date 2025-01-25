using UniversityCertificates.Register.DTOs;
using UniversityCertificates.Register.Models;

namespace UniversityCertificates.Register.Services.Interfaces;

public interface IRegisterEntriesCommandService
{
    public Task<RegisterEntry> AddRegisterEntryAsync(CreateRegisterEntryRequest registerEntry);
    public Task<RegisterEntry> UpdateRegisterEntryAsync(UpdateRegisterEntryRequest registerEntry);
    public Task<RegisterEntry> DeleteRegisterEntryAsync(int id);
}
