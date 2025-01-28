namespace UniversityCertificates.Register.Services.Interfaces;

public interface IRegisterEntryXlsxService
{
    public Task<byte[]> GenerateExcelForRegisterEntriesAsync();
}
