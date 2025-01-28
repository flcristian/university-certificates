namespace UniversityCertificates.Register.Services.Interfaces;

public interface IRegisterEntryDocxService
{
    public Task<byte[]> GenerateCertificateForRegisterEntryByIdAsync(int id, byte[] qrCode);
}
