using UniversityCertificates.Certificates.DTOs;
using UniversityCertificates.Certificates.Models;

namespace UniversityCertificates.Certificates.Services.Interfaces;

public interface ICertificateTemplatesCommandService
{
    public Task<CertificateTemplate> AddCertificateTemplateAsync(
        CreateCertificateTemplateRequest request
    );

    public Task<CertificateTemplate> UpdateCertificateTemplateAsync(
        UpdateCertificateTemplateRequest request
    );

    public Task<CertificateTemplate> DeleteCertificateTemplateAsync(int id);

    public Task<CertificateTemplate> SoftDeleteCertificateTemplateAsync(int id);
}
