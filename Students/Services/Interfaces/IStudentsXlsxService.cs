using UniversityCertificates.Students.Models;

namespace UniversityCertificates.Students.Services.Interfaces;

public interface IStudentsXlsxService
{
    Task<byte[]> GenerateExcelForStudentsAsync();
    Task<IEnumerable<Student>> ImportStudentsFromExcelAsync(Stream excelFileStream);
} 