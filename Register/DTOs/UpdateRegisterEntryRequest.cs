namespace UniversityCertificates.Register.DTOs;

public class UpdateRegisterEntryRequest
{
    public int Id { get; set; }
    public int? StudentSerialNumber { get; set; }
    public DateTime? DateOfIssue { get; set; }
    public string? Reason { get; set; }
}
