using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniversityCertificates.Certificates.Models;
using UniversityCertificates.Students.Models;

namespace UniversityCertificates.Register.Models;

public class RegisterEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public required int Id { get; set; }

    [Required]
    [Column("student_serial_number")]
    public required int StudentSerialNumber { get; set; }

    [Required]
    [Column("date_of_issue")]
    public required DateTime DateOfIssue { get; set; }

    [Required]
    [Column("reason")]
    public required string Reason { get; set; }

    [Required]
    [Column("reviewed")]
    public required bool Reviewed { get; set; }

    [Required]
    [Column("accepted")]
    public required bool Accepted { get; set; }

    [Column("selected_template_id")]
    public int? SelectedTemplateId { get; set; }

    [ForeignKey("StudentSerialNumber")]
    public virtual required Student Student { get; set; }

    [ForeignKey("SelectedTemplateId")]
    public virtual CertificateTemplate? SelectedTemplate { get; set; }
}
