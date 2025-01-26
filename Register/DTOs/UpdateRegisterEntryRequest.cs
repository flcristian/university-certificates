namespace UniversityCertificates.Register.DTOs;

public record UpdateRegisterEntryRequest
{
    public int Id { get; set; }
    public int? StudentSerialNumber { get; set; }
    public DateTime? DateOfIssue { get; set; }
    public string? Reason { get; set; }
    public bool? Reviewed { get; set; }
    public bool? Accepted { get; set; }
    public int? SelectedTemplate { get; set; }
}
