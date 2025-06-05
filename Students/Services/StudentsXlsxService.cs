using System.Text.RegularExpressions;
using UniversityCertificates.Register.Models;
using UniversityCertificates.Students.DTOs;
using UniversityCertificates.Students.Models;
using UniversityCertificates.Students.Repository.Interfaces;
using UniversityCertificates.Students.Services.Interfaces;
using UniversityCertificates.System.Constants;
using UniversityCertificates.System.Exceptions;
using UniversityCertificates.System.Utility.Services;

namespace UniversityCertificates.Students.Services;

public partial class StudentsXlsxService(IStudentsRepository studentsRepository)
    : IStudentsXlsxService
{
    private readonly IStudentsRepository _studentsRepository = studentsRepository;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    private const int MIN_STUDY_YEAR = 1;
    private const int MAX_STUDY_YEAR = 5;

    public async Task<byte[]> GenerateExcelForStudentsAsync()
    {
        IEnumerable<Student> students = await _studentsRepository.GetStudentsAsync();

        if (!students.Any())
        {
            throw new ItemsDoNotExistException(ConstantMessages.STUDENTS_DO_NOT_EXIST);
        }

        List<StudentExcelRow> studentExcelRows = students
            .Select(student => new StudentExcelRow
            {
                SerialNumber = student.SerialNumber,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                StudyYear = student.StudyYear,
                DegreeType = student.DegreeType.ToString(),
                Department = student.Department,
            })
            .ToList();

        byte[] excelBytes = XlsxFileHandler.ExportToExcel(studentExcelRows);

        return excelBytes;
    }

    public async Task<IEnumerable<Student>> ImportStudentsFromExcelAsync(Stream excelFileStream)
    {
        List<StudentExcelRow> studentExcelRows = XlsxFileHandler.ImportFromExcel<StudentExcelRow>(
            excelFileStream
        );

        if (studentExcelRows.Count == 0)
        {
            throw new InvalidValueException(ConstantMessages.EMPTY_EXCEL_FILE);
        }

        ValidateSerialNumbers(studentExcelRows);
        ValidateEmails(studentExcelRows);
        ValidateDegreeTypesAndStudyYears(studentExcelRows);

        await _studentsRepository.ClearStudentsAsync();

        List<Student> students = studentExcelRows
            .Select(row => new Student
            {
                SerialNumber = row.SerialNumber,
                FirstName = row.FirstName,
                LastName = row.LastName,
                Email = row.Email,
                StudyYear = row.StudyYear,
                DegreeType = Enum.Parse<DegreeType>(row.DegreeType),
                Department = row.Department,
                RegisterEntries = [],
            })
            .ToList();

        foreach (var student in students)
        {
            await _studentsRepository.AddStudentAsync(
                new CreateStudentRequest
                {
                    SerialNumber = student.SerialNumber,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    Email = student.Email,
                    StudyYear = student.StudyYear,
                    DegreeType = student.DegreeType,
                    Department = student.Department,
                }
            );
        }

        return students;
    }

    private static void ValidateSerialNumbers(List<StudentExcelRow> studentExcelRows)
    {
        // Check for non-positive serial numbers
        var invalidRows = studentExcelRows
            .Select((row, index) => new { Row = row, Index = index + 2 }) // +2 because Excel is 1-based and we have a header row
            .Where(x => x.Row.SerialNumber <= 0)
            .ToList();

        if (invalidRows.Count != 0)
        {
            var errorDetails = string.Join(
                "\n",
                invalidRows.Select(x =>
                    $"Row {x.Index}: SerialNumber={x.Row.SerialNumber}, Name={x.Row.FirstName} {x.Row.LastName}, Email={x.Row.Email}"
                )
            );

            throw new InvalidValueException(
                $"Invalid serial numbers found in the following rows:\n{errorDetails}\nSerial numbers must be positive integers."
            );
        }

        // Check for duplicate serial numbers
        var duplicateGroups = studentExcelRows
            .Select((row, index) => new { Row = row, Index = index + 2 })
            .GroupBy(x => x.Row.SerialNumber)
            .Where(g => g.Count() > 1)
            .ToList();

        if (duplicateGroups.Count != 0)
        {
            var errorDetails = string.Join(
                "\n",
                duplicateGroups.Select(g =>
                    $"Serial Number {g.Key} appears in rows: {string.Join(", ", g.Select(x => x.Index))}"
                )
            );

            throw new InvalidValueException($"Duplicate serial numbers found:\n{errorDetails}");
        }
    }

    private static void ValidateEmails(List<StudentExcelRow> studentExcelRows)
    {
        // Check for duplicate emails
        var duplicateEmails = studentExcelRows
            .Select((row, index) => new { Row = row, Index = index + 2 })
            .GroupBy(x => x.Row.Email.ToLowerInvariant())
            .Where(g => g.Count() > 1)
            .Select(g => new { Email = g.Key, Rows = g.Select(x => x.Index).ToList() })
            .ToList();

        if (duplicateEmails.Count != 0)
        {
            var errorDetails = string.Join(
                "\n",
                duplicateEmails.Select(d =>
                    $"Email {d.Email} appears in rows: {string.Join(", ", d.Rows)}"
                )
            );
            throw new InvalidValueException($"Duplicate emails found:\n{errorDetails}");
        }

        // Check email format
        var invalidEmails = studentExcelRows
            .Select((row, index) => new { Row = row, Index = index + 2 })
            .Where(x => !EmailRegex().IsMatch(x.Row.Email))
            .Select(x => $"Row {x.Index}: {x.Row.Email}")
            .ToList();

        if (invalidEmails.Count != 0)
        {
            throw new InvalidValueException(
                $"Invalid email format found in the following rows:\n{string.Join("\n", invalidEmails)}"
            );
        }
    }

    private static void ValidateDegreeTypesAndStudyYears(List<StudentExcelRow> studentExcelRows)
    {
        var invalidDegreeTypes = studentExcelRows
            .Select((row, index) => new { Row = row, Index = index + 2 })
            .Where(x =>
                !Enum.TryParse<DegreeType>(x.Row.DegreeType, out _)
                || !Enum.IsDefined(Enum.Parse<DegreeType>(x.Row.DegreeType))
            )
            .Select(x => $"Row {x.Index}: {x.Row.DegreeType}")
            .ToList();

        if (invalidDegreeTypes.Count != 0)
        {
            throw new InvalidValueException(
                $"Invalid degree types found in the following rows:\n{string.Join("\n", invalidDegreeTypes)}\nValid values are: {string.Join(", ", Enum.GetNames<DegreeType>())}"
            );
        }

        // Validate study years
        var invalidStudyYears = studentExcelRows
            .Select((row, index) => new { Row = row, Index = index + 2 })
            .Where(x => x.Row.StudyYear < MIN_STUDY_YEAR || x.Row.StudyYear > MAX_STUDY_YEAR)
            .Select(x => $"Row {x.Index}: StudyYear={x.Row.StudyYear}")
            .ToList();

        if (invalidStudyYears.Count != 0)
        {
            throw new InvalidValueException(
                $"Invalid study years found in the following rows:\n{string.Join("\n", invalidStudyYears)}\nStudy year must be between {MIN_STUDY_YEAR} and {MAX_STUDY_YEAR}"
            );
        }
    }

    private class StudentExcelRow
    {
        public int SerialNumber { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int StudyYear { get; set; }
        public string DegreeType { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}
