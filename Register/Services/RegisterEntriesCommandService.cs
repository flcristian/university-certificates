using UniversityCertificates.Certificates.Models;
using UniversityCertificates.Certificates.Repository.Interfaces;
using UniversityCertificates.Register.DTOs;
using UniversityCertificates.Register.Models;
using UniversityCertificates.Register.Repository.Interfaces;
using UniversityCertificates.Register.Services.Interfaces;
using UniversityCertificates.Students.Models;
using UniversityCertificates.Students.Repository.Interfaces;
using UniversityCertificates.System.Constants;
using UniversityCertificates.System.Exceptions;
using UniversityCertificates.System.Utility.Services;

namespace UniversityCertificates.Register.Services;

public class RegisterEntriesCommandService : IRegisterEntriesCommandService
{
    private readonly IRegisterEntriesRepository _registerEntriesRepository;
    private readonly IStudentsRepository _studentsRepository;
    private readonly ICertificateTemplatesRepository _certificateTemplatesRepository;
    private readonly IRegisterEntryDocxService _registerEntryDocxService;
    private readonly IRegisterEntryQRCodesService _registerEntryQRCodesService;
    private readonly EmailService _emailService;

    public RegisterEntriesCommandService(
        IRegisterEntriesRepository registerEntriesRepository,
        IStudentsRepository studentsRepository,
        ICertificateTemplatesRepository certificateTemplatesRepository,
        IRegisterEntryDocxService registerEntryDocxService,
        IRegisterEntryQRCodesService registerEntryQRCodesService,
        EmailService emailService
    )
    {
        _registerEntriesRepository = registerEntriesRepository;
        _studentsRepository = studentsRepository;
        _certificateTemplatesRepository = certificateTemplatesRepository;
        _registerEntryDocxService = registerEntryDocxService;
        _registerEntryQRCodesService = registerEntryQRCodesService;
        _emailService = emailService;
    }

    public async Task<RegisterEntry> AddRegisterEntryAsync(CreateRegisterEntryRequest request)
    {
        Student? student = await _studentsRepository.GetStudentBySerialNumberAsync(
            request.StudentSerialNumber
        );

        if (student == null)
        {
            throw new ItemDoesNotExistException(ConstantMessages.STUDENT_DOES_NOT_EXIST);
        }

        if (
            (await _registerEntriesRepository.GetRegisterEntriesAsync()).Any(entry =>
                entry.StudentSerialNumber == request.StudentSerialNumber
                && entry.DateOfIssue.Year == request.DateOfIssue.Year
                && entry.DateOfIssue.DayOfYear == request.DateOfIssue.DayOfYear
                && entry.Reason.Equals(request.Reason)
            )
        )
        {
            throw new ItemAlreadyExistsException(ConstantMessages.REGISTER_ENTRY_ALREADY_EXISTS);
        }

        if (
            (await _registerEntriesRepository.GetRegisterEntriesAsync()).Any(entry =>
                entry.StudentSerialNumber == request.StudentSerialNumber
                && entry.DateOfIssue
                    >= request.DateOfIssue.AddMinutes(-ConstantValues.REGISTER_ENTRY_WAIT_MINUTES)
                && entry.DateOfIssue <= request.DateOfIssue
            )
        )
        {
            throw new ItemAlreadyExistsException(ConstantMessages.REGISTER_ENTRY_MUST_WAIT);
        }

        return await _registerEntriesRepository.AddRegisterEntryAsync(request);
    }

    public async Task<RegisterEntry> UpdateRegisterEntryAsync(UpdateRegisterEntryRequest request)
    {
        RegisterEntry? registerEntry = await _registerEntriesRepository.GetRegisterEntryByIdAsync(
            request.Id
        );

        if (registerEntry == null)
        {
            throw new ItemDoesNotExistException(ConstantMessages.REGISTER_ENTRY_DOES_NOT_EXIST);
        }

        if (
            request.StudentSerialNumber != null
            && request.StudentSerialNumber != registerEntry.StudentSerialNumber
        )
        {
            Student? student = await _studentsRepository.GetStudentBySerialNumberAsync(
                request.StudentSerialNumber!.Value
            );

            if (student == null)
            {
                throw new ItemDoesNotExistException(ConstantMessages.STUDENT_DOES_NOT_EXIST);
            }
        }

        if (
            request.SelectedTemplateId != null
            && request.SelectedTemplateId != registerEntry.SelectedTemplateId
        )
        {
            CertificateTemplate? certificateTemplate =
                await _certificateTemplatesRepository.GetCertificateTemplateByIdAsync(
                    request.SelectedTemplateId!.Value
                );

            if (certificateTemplate == null)
            {
                throw new ItemDoesNotExistException(
                    ConstantMessages.CERTIFICATE_TEMPLATE_DOES_NOT_EXIST
                );
            }
            if (!certificateTemplate.Active)
            {
                throw new InvalidValueException(ConstantMessages.CERTIFICATE_TEMPLATE_NOT_ACTIVE);
            }
        }

        RegisterEntry? existingExtry = (
            await _registerEntriesRepository.GetRegisterEntriesAsync()
        ).FirstOrDefault(entry =>
            entry.StudentSerialNumber
                == (request.StudentSerialNumber ?? registerEntry.StudentSerialNumber)
            && entry.DateOfIssue.Year == (request.DateOfIssue ?? registerEntry.DateOfIssue).Year
            && entry.DateOfIssue.DayOfYear
                == (request.DateOfIssue ?? registerEntry.DateOfIssue).DayOfYear
            && entry.Reason.Equals(
                request.Reason ?? registerEntry.Reason,
                StringComparison.OrdinalIgnoreCase
            )
            && entry.SelectedTemplateId
                == (request.SelectedTemplateId ?? registerEntry.SelectedTemplateId)
            && entry.Accepted == (request.Accepted ?? registerEntry.Accepted)
            && entry.Reviewed == (request.Reviewed ?? registerEntry.Reviewed)
        );

        if (existingExtry != null)
        {
            if (existingExtry.Id != request.Id)
            {
                throw new ItemAlreadyExistsException(
                    ConstantMessages.REGISTER_ENTRY_ALREADY_EXISTS
                );
            }
            else
            {
                throw new InvalidValueException(ConstantMessages.REGISTER_ENTRY_NOT_MODIFIED);
            }
        }

        bool accepted = registerEntry.Accepted; // Remember the state before updating
        registerEntry = await _registerEntriesRepository.UpdateRegisterEntryAsync(request);

        if (!accepted && registerEntry.Accepted && (registerEntry.SelectedTemplateId != null))
        {
            List<(byte[] fileContents, string fileName)> attachedFiles =
                new List<(byte[], string)>();

            byte[] qrCode =
                await _registerEntryQRCodesService.GenerateQRCodeForRegisterEntryByIdAsync(
                    registerEntry.Id
                );

            attachedFiles.Add(
                (
                    await _registerEntryDocxService.GenerateCertificateForRegisterEntryByIdAsync(
                        registerEntry.Id,
                        qrCode
                    ),
                    "Certificate.docx"
                )
            );

            await _emailService.SendEmail(
                registerEntry.Student.Email,
                ConstantEmails.CERTIFICATE_ACCEPTED_EMAIL_SUBJECT,
                ConstantEmails.CERTIFICATE_ACCEPTED_EMAIL_BODY,
                attachedFiles
            );
        }

        return registerEntry;
    }

    public async Task<RegisterEntry> DeleteRegisterEntryAsync(int id)
    {
        RegisterEntry? registerEntry = await _registerEntriesRepository.GetRegisterEntryByIdAsync(
            id
        );

        if (registerEntry == null)
        {
            throw new ItemDoesNotExistException(ConstantMessages.REGISTER_ENTRY_DOES_NOT_EXIST);
        }

        return await _registerEntriesRepository.DeleteRegisterEntryAsync(id);
    }
}
