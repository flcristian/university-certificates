using UniversityCertificates.Certificates.DTOs;
using UniversityCertificates.Certificates.Models;
using UniversityCertificates.Certificates.Repository.Interfaces;
using UniversityCertificates.Certificates.Services.Interfaces;
using UniversityCertificates.System.Constants;
using UniversityCertificates.System.Exceptions;

namespace UniversityCertificates.Certificates.Services;

public class CertificateTemplatesCommandService : ICertificateTemplatesCommandService
{
    private readonly ICertificateTemplatesRepository _certificateTemplatesRepository;
    private readonly ICertificateTemplateFilesRepository _certificateTemplateFilesRepository;

    public CertificateTemplatesCommandService(
        ICertificateTemplatesRepository certificateTemplatesRepository,
        ICertificateTemplateFilesRepository certificateTemplateFilesRepository
    )
    {
        _certificateTemplatesRepository = certificateTemplatesRepository;
        _certificateTemplateFilesRepository = certificateTemplateFilesRepository;
    }

    public async Task<CertificateTemplate> AddCertificateTemplateAsync(
        CreateCertificateTemplateRequest request
    )
    {
        if (
            (await _certificateTemplatesRepository.GetAllCertificateTemplatesAsync()).Any(ct =>
                ct.Name.Equals(request.Name) && ct.Active
            )
        )
        {
            throw new ItemAlreadyExistsException(Constants.CERTIFICATE_TEMPLATE_ALREADY_EXISTS);
        }

        if (Path.GetExtension(request.File.FileName) != ".docx")
        {
            throw new InvalidValueException(Constants.CERTIFICATE_TEMPLATE_FILE_EXTENSION_INVALID);
        }

        CertificateTemplate certificateTemplate =
            await _certificateTemplatesRepository.AddCertificateTemplateAsync(request);

        await _certificateTemplateFilesRepository.AddCertificateTemplateFileAsync(
            new CreateCertificateTemplateFileRequest
            {
                Id = certificateTemplate.Id,
                File = request.File,
            }
        );

        return certificateTemplate;
    }

    public async Task<CertificateTemplate> UpdateCertificateTemplateAsync(
        UpdateCertificateTemplateRequest request
    )
    {
        CertificateTemplate? certificateTemplate =
            await _certificateTemplatesRepository.GetCertificateTemplateByIdAsync(request.Id);

        if (certificateTemplate == null)
        {
            throw new ItemDoesNotExistException(Constants.CERTIFICATE_TEMPLATE_DOES_NOT_EXIST);
        }

        if (request.File != null && Path.GetExtension(request.File.FileName) != ".docx")
        {
            throw new InvalidValueException(Constants.CERTIFICATE_TEMPLATE_FILE_EXTENSION_INVALID);
        }

        if (
            request.Name != null
            && !request.Name.Equals(certificateTemplate.Name)
            && (await _certificateTemplatesRepository.GetAllCertificateTemplatesAsync()).Any(ct =>
                ct.Name.Equals(request.Name)
            )
        )
        {
            throw new ItemAlreadyExistsException(Constants.CERTIFICATE_TEMPLATE_ALREADY_EXISTS);
        }

        certificateTemplate.Name = request.Name ?? certificateTemplate.Name;
        certificateTemplate.Description = request.Description ?? certificateTemplate.Description;

        CertificateTemplate updatedCertificateTemplate =
            await _certificateTemplatesRepository.UpdateCertificateTemplateAsync(request);

        if (request.File != null)
        {
            await _certificateTemplateFilesRepository.UpdateCertificateTemplateFileAsync(
                new UpdateCertificateTemplateFileRequest
                {
                    Id = updatedCertificateTemplate.Id,
                    File = request.File,
                }
            );
        }

        return updatedCertificateTemplate;
    }

    public async Task<CertificateTemplate> DeleteCertificateTemplateAsync(int id)
    {
        CertificateTemplate? certificateTemplate =
            await _certificateTemplatesRepository.GetCertificateTemplateByIdAsync(id);

        if (certificateTemplate == null)
        {
            throw new ItemDoesNotExistException(Constants.CERTIFICATE_TEMPLATE_DOES_NOT_EXIST);
        }

        await _certificateTemplateFilesRepository.DeleteCertificateTemplateFileAsync(id);

        return await _certificateTemplatesRepository.DeleteCertificateTemplateAsync(id);
    }

    public async Task<CertificateTemplate> SoftDeleteCertificateTemplateAsync(int id)
    {
        CertificateTemplate? certificateTemplate =
            await _certificateTemplatesRepository.GetCertificateTemplateByIdAsync(id);

        if (certificateTemplate == null)
        {
            throw new ItemDoesNotExistException(Constants.CERTIFICATE_TEMPLATE_DOES_NOT_EXIST);
        }
        if (!certificateTemplate.Active)
        {
            throw new InvalidValueException(Constants.CERTIFICATE_TEMPLATE_NOT_ACTIVE);
        }

        return await _certificateTemplatesRepository.SoftDeleteCertificateTemplateAsync(id);
    }
}
