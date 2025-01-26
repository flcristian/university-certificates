using UniversityCertificates.Register.DTOs;
using UniversityCertificates.Register.Models;

namespace UniversityCertificates.Register.Services.Interfaces;

public interface IRegisterEntriesCommandService
{
    public Task<RegisterEntry> AddRegisterEntryAsync(CreateRegisterEntryRequest request);
    public Task<RegisterEntry> UpdateRegisterEntryAsync(UpdateRegisterEntryRequest request);
    public Task<RegisterEntry> DeleteRegisterEntryAsync(int id);
}
