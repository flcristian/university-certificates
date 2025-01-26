namespace UniversityCertificates.Register.Services.Interfaces;

public interface IRegisterEntryQRCodesService
{
    public Task<byte[]> GenerateQRCodeForRegisterEntryByIdAsync(int id);
}
