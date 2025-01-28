using UniversityCertificates.Register.Models;
using UniversityCertificates.Register.Repository.Interfaces;
using UniversityCertificates.Register.Services.Interfaces;
using UniversityCertificates.System.Constants;
using UniversityCertificates.System.Exceptions;
using UniversityCertificates.System.Utility.Services;

namespace UniversityCertificates.Register.Services;

public class RegisterEntryXlsxService : IRegisterEntryXlsxService
{
    private readonly IRegisterEntriesRepository _registerEntriesRepository;

    public RegisterEntryXlsxService(IRegisterEntriesRepository registerEntriesRepository)
    {
        _registerEntriesRepository = registerEntriesRepository;
    }

    public async Task<byte[]> GenerateExcelForRegisterEntriesAsync()
    {
        IEnumerable<RegisterEntry> registerEntries =
            await _registerEntriesRepository.GetRegisterEntriesAsync();

        if (registerEntries.Count() == 0)
        {
            throw new ItemsDoNotExistException(ConstantMessages.REGISTER_ENTRIES_DO_NOT_EXIST);
        }

        List<RegisterEntryExcelRow> registerEntriesExcelRows = registerEntries
            .Select(registerEntry => new RegisterEntryExcelRow
            {
                Id = registerEntry.Id,
                DateOfIssue = registerEntry.DateOfIssue.ToString("dd.MM.yyyy"),
                Reason = registerEntry.Reason,
                SerialNumber = registerEntry.Student.SerialNumber,
                FirstName = registerEntry.Student.FirstName,
                LastName = registerEntry.Student.LastName,
                StudyYear = registerEntry.Student.StudyYear,
                DegreeType = registerEntry.Student.DegreeType.ToString(),
                Department = registerEntry.Student.Department,
            })
            .ToList();

        byte[] excelBytes = XlsxFileHandler.ExportToExcel(registerEntriesExcelRows);

        return excelBytes;
    }

    private class RegisterEntryExcelRow
    {
        public required int Id { get; set; }
        public required string DateOfIssue { get; set; }
        public required string Reason { get; set; }
        public required int SerialNumber { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required int StudyYear { get; set; }
        public required string DegreeType { get; set; }
        public required string Department { get; set; }
    }
}
