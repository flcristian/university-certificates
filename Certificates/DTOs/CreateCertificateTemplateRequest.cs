using System.ComponentModel.DataAnnotations;

namespace UniversityCertificates.Certificates.DTOs;

public record CreateCertificateTemplateRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required IFormFile File { get; set; }
}
