namespace UniversityCertificates.System.Utility.DTOs;

public record GetFileRequest
{
    public required byte[] FileContents { get; set; }
    public required string ContentType { get; set; }
    public required string FileName { get; set; }
}
