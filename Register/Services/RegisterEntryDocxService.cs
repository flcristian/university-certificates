using UniversityCertificates.Certificates.Repository.Interfaces;
using UniversityCertificates.Register.Models;
using UniversityCertificates.Register.Repository.Interfaces;
using UniversityCertificates.Register.Services.Interfaces;
using UniversityCertificates.System.Constants;
using UniversityCertificates.System.Exceptions;
using UniversityCertificates.System.Utility.DTOs;
using UniversityCertificates.System.Utility.Services;

namespace UniversityCertificates.Register.Services;

public class RegisterEntryDocxService : IRegisterEntryDocxService
{
    private readonly IRegisterEntriesRepository _registerEntriesRepository;
    private readonly ICertificateTemplateFilesRepository _certificateTemplateFilesRepository;

    public RegisterEntryDocxService(
        IRegisterEntriesRepository registerEntriesRepository,
        ICertificateTemplateFilesRepository certificateTemplateFilesRepository
    )
    {
        _registerEntriesRepository = registerEntriesRepository;
        _certificateTemplateFilesRepository = certificateTemplateFilesRepository;
    }

    public async Task<byte[]> GenerateCertificateForRegisterEntryByIdAsync(int id, byte[] qrCode)
    {
        RegisterEntry? registerEntry = await _registerEntriesRepository.GetRegisterEntryByIdAsync(
            id
        );

        if (registerEntry == null)
        {
            throw new ItemDoesNotExistException(ConstantMessages.REGISTER_ENTRY_DOES_NOT_EXIST);
        }

        GetFileRequest? fileRequest =
            await _certificateTemplateFilesRepository.GetCertificateTemplateFileByIdAsync(
                registerEntry.SelectedTemplateId!.Value
            );

        if (fileRequest == null)
        {
            throw new ItemDoesNotExistException(
                ConstantMessages.CERTIFICATE_TEMPLATE_DOES_NOT_EXIST
            );
        }

        byte[] documentTemplateBytes = fileRequest.FileContents;
        using var templateStream = new MemoryStream(documentTemplateBytes);
        byte[] resultStream = DocxFileHandler.ReplaceTextWithImage(
            documentTemplateBytes,
            qrCode,
            "[QR]"
        );

        Dictionary<string, string> replacementMap = new Dictionary<string, string>
        {
            // Register information
            { "[ID]", registerEntry.Id.ToString() },
            { "[DATE_OF_ISSUE]", registerEntry.DateOfIssue.ToString("dd.MM.yyyy") },
            { "[REASON]", registerEntry.Reason },
            // Student information
            { "[SERIAL_NUMBER]", registerEntry.Student.SerialNumber.ToString() },
            { "[FIRST_NAME]", registerEntry.Student.FirstName },
            { "[LAST_NAME]", registerEntry.Student.LastName },
            { "[STUDY_YEAR]", registerEntry.Student.StudyYear.ToString() },
            { "[DEGREE_TYPE]", registerEntry.Student.DegreeType.ToString() },
            { "[DEPARTMENT]", registerEntry.Student.Department },
        };

        foreach (var (key, value) in replacementMap)
        {
            resultStream = DocxFileHandler.ReplaceText(resultStream, key, value);
        }

        return resultStream;
    }
}
