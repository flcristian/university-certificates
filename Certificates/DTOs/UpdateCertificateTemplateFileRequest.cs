using System.ComponentModel.DataAnnotations;

namespace UniversityCertificates.Certificates.DTOs;

public record UpdateCertificateTemplateFileRequest
{
    public required int Id { get; set; }

    [Required(ErrorMessage = "Template file is required")]
    [Display(Name = "Template File")]
    [FileExtensions(Extensions = ".docx", ErrorMessage = "Only .docx files are allowed")]
    public required IFormFile File { get; set; }
}
