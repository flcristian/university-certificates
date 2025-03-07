using UniversityCertificates.Certificates.DTOs;
using UniversityCertificates.Certificates.Models;

namespace UniversityCertificates.Certificates.Repository.Interfaces;

public interface ICertificateTemplatesRepository
{
    public Task<IEnumerable<CertificateTemplate>> GetAllCertificateTemplatesAsync();

    public Task<CertificateTemplate?> GetCertificateTemplateByIdAsync(int id);

    public Task<CertificateTemplate> AddCertificateTemplateAsync(
        CreateCertificateTemplateRequest request
    );

    public Task<CertificateTemplate> UpdateCertificateTemplateAsync(
        UpdateCertificateTemplateRequest request
    );

    public Task<CertificateTemplate> DeleteCertificateTemplateAsync(int id);

    public Task<CertificateTemplate> SoftDeleteCertificateTemplateAsync(int id);
}
