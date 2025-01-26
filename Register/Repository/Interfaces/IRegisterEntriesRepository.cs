using UniversityCertificates.Register.DTOs;
using UniversityCertificates.Register.Models;

namespace UniversityCertificates.Register.Repository.Interfaces;

public interface IRegisterEntriesRepository
{
    public Task<IEnumerable<RegisterEntry>> GetRegisterEntriesAsync();

    public Task<RegisterEntry?> GetRegisterEntryByIdAsync(int id);

    public Task<RegisterEntry> AddRegisterEntryAsync(CreateRegisterEntryRequest request);

    public Task<RegisterEntry> UpdateRegisterEntryAsync(UpdateRegisterEntryRequest request);

    public Task<RegisterEntry> DeleteRegisterEntryAsync(int id);
}
