using UniversityCertificates.Certificates.DTOs;
using UniversityCertificates.Certificates.Repository.Interfaces;
using UniversityCertificates.System.Utility.DTOs;

namespace UniversityCertificates.Certificates.Repository;

public class CertificateTemplateFilesRepository : ICertificateTemplateFilesRepository
{
    private readonly string _storageDir;

    public CertificateTemplateFilesRepository()
    {
        _storageDir = Path.Combine(
            Directory.GetCurrentDirectory(),
            "System",
            "Storage",
            "CertificateTemplates"
        );
        Directory.CreateDirectory(_storageDir);
    }

    public async Task<GetFileRequest?> GetCertificateTemplateFileByIdAsync(int id)
    {
        string filePath = Path.Combine(_storageDir, $"template-{id}.docx");

        try
        {
            byte[] fileContents = await File.ReadAllBytesAsync(filePath);

            return new GetFileRequest
            {
                FileContents = fileContents,
                ContentType =
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                FileName = $"template-{id}.docx",
            };
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    public async Task<GetFileRequest> AddCertificateTemplateFileAsync(
        CreateCertificateTemplateFileRequest request
    )
    {
        string filePath = Path.Combine(_storageDir, $"template-{request.Id}.docx");

        using (FileStream fileStream = new(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(fileStream);
        }

        return new GetFileRequest
        {
            FileContents = await File.ReadAllBytesAsync(filePath),
            ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            FileName = $"template-{request.Id}.docx",
        };
    }

    public async Task<GetFileRequest> UpdateCertificateTemplateFileAsync(
        UpdateCertificateTemplateFileRequest request
    )
    {
        string filePath = Path.Combine(_storageDir, $"template-{request.Id}.docx");

        using (FileStream fileStream = new(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(fileStream);
        }

        return new GetFileRequest
        {
            FileContents = await File.ReadAllBytesAsync(filePath),
            ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            FileName = $"template-{request.Id}.docx",
        };
    }

    public async Task<GetFileRequest> DeleteCertificateTemplateFileAsync(int id)
    {
        string filePath = Path.Combine(_storageDir, $"template-{id}.docx");
        byte[] fileContents = await File.ReadAllBytesAsync(filePath);
        File.Delete(filePath);

        return new GetFileRequest
        {
            FileContents = fileContents,
            ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            FileName = $"template-{id}.docx",
        };
    }
}
