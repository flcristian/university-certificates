namespace UniversityCertificates.Register.Services.Interfaces;

public interface IRegisterEntryDocumentsService
{
    public Task<byte[]> GenerateCertificateForRegisterEntryByIdAsync(int id, byte[] qrCode);
}
