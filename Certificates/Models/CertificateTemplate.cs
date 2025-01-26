using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniversityCertificates.Register.Models;

namespace UniversityCertificates.Certificates.Models;

public class CertificateTemplate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public required int Id { get; set; }

    [Required]
    [Column("name")]
    public required string Name { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [Column("active")]
    [DefaultValue(true)]
    public required bool Active { get; set; }

    public virtual required IEnumerable<RegisterEntry> RegisterEntries { get; set; }
}
