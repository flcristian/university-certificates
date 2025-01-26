using UniversityCertificates.Certificates.Models;
using UniversityCertificates.Certificates.Repository.Interfaces;
using UniversityCertificates.Certificates.Services.Interfaces;
using UniversityCertificates.System.Constants;
using UniversityCertificates.System.Exceptions;
using UniversityCertificates.System.Utility.DTOs;

namespace UniversityCertificates.Certificates.Services;

public class CertificateTemplatesQueryService : ICertificateTemplatesQueryService
{
    private readonly ICertificateTemplatesRepository _certificateTemplatesRepository;
    private readonly ICertificateTemplateFilesRepository _certificateTemplateFilesRepository;

    public CertificateTemplatesQueryService(
        ICertificateTemplatesRepository certificateTemplatesRepository,
        ICertificateTemplateFilesRepository certificateTemplateFilesRepository
    )
    {
        _certificateTemplatesRepository = certificateTemplatesRepository;
        _certificateTemplateFilesRepository = certificateTemplateFilesRepository;
    }

    public async Task<IEnumerable<CertificateTemplate>> GetAllCertificateTemplatesAsync()
    {
        IEnumerable<CertificateTemplate> certificateTemplates =
            await _certificateTemplatesRepository.GetAllCertificateTemplatesAsync();

        if (certificateTemplates.Count() == 0)
        {
            throw new ItemsDoNotExistException(Constants.CERTIFICATE_TEMPLATES_DO_NOT_EXIST);
        }

        return certificateTemplates;
    }

    public async Task<CertificateTemplate> GetCertificateTemplateByIdAsync(int id)
    {
        CertificateTemplate? certificateTemplate =
            await _certificateTemplatesRepository.GetCertificateTemplateByIdAsync(id);

        if (certificateTemplate == null)
        {
            throw new ItemDoesNotExistException(Constants.CERTIFICATE_TEMPLATE_DOES_NOT_EXIST);
        }

        return certificateTemplate;
    }

    public async Task<GetFileRequest> GetCertificateTemplateFileByIdAsync(int id)
    {
        CertificateTemplate? certificateTemplate =
            await _certificateTemplatesRepository.GetCertificateTemplateByIdAsync(id);

        if (certificateTemplate == null)
        {
            throw new ItemDoesNotExistException(Constants.CERTIFICATE_TEMPLATE_DOES_NOT_EXIST);
        }

        GetFileRequest? fileRequest =
            await _certificateTemplateFilesRepository.GetCertificateTemplateFileByIdAsync(id);

        if (fileRequest == null)
        {
            throw new ItemDoesNotExistException(Constants.CERTIFICATE_TEMPLATE_FILE_DOES_NOT_EXIST);
        }

        return fileRequest;
    }
}
