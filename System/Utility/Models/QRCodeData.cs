namespace UniversityCertificates.System.Utility.Models;

public record QRCodeData
{
    public required int SerialNumber { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required int StudyYear { get; set; }
    public required string DegreeType { get; set; }
    public required string Department { get; set; }
    public required string DateOfIssue { get; set; }
    public required string Reason { get; set; }
}
