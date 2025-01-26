using UniversityCertificates.Certificates.DTOs;
using UniversityCertificates.System.Utility.DTOs;

namespace UniversityCertificates.Certificates.Repository.Interfaces;

public interface ICertificateTemplateFilesRepository
{
    public Task<GetFileRequest?> GetCertificateTemplateFileByIdAsync(int id);

    public Task<GetFileRequest> AddCertificateTemplateFileAsync(
        CreateCertificateTemplateFileRequest request
    );

    public Task<GetFileRequest> UpdateCertificateTemplateFileAsync(
        UpdateCertificateTemplateFileRequest request
    );

    public Task<GetFileRequest> DeleteCertificateTemplateFileAsync(int id);
}
