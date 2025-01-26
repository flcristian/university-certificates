namespace UniversityCertificates.Register.DTOs;

public record CreateRegisterEntryRequest
{
    public required int StudentSerialNumber { get; set; }
    public required DateTime DateOfIssue { get; set; }
    public required string Reason { get; set; }
}
