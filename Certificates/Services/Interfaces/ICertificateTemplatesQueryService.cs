using UniversityCertificates.Certificates.Models;
using UniversityCertificates.System.Utility.DTOs;

namespace UniversityCertificates.Certificates.Services.Interfaces;

public interface ICertificateTemplatesQueryService
{
    public Task<IEnumerable<CertificateTemplate>> GetAllCertificateTemplatesAsync();

    public Task<CertificateTemplate> GetCertificateTemplateByIdAsync(int id);

    public Task<GetFileRequest> GetCertificateTemplateFileByIdAsync(int id);
}
