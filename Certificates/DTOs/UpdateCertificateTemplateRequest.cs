using System.ComponentModel.DataAnnotations;

namespace UniversityCertificates.Certificates.DTOs;

public record UpdateCertificateTemplateRequest
{
    public required int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? Active { get; set; }
    public IFormFile? File { get; set; }
}
