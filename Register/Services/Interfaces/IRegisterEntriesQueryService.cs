using UniversityCertificates.Register.Models;

namespace UniversityCertificates.Register.Services.Interfaces;

public interface IRegisterEntriesQueryService
{
    public Task<IEnumerable<RegisterEntry>> GetRegisterEntriesAsync();

    public Task<RegisterEntry> GetRegisterEntryAsync(int id);
}
