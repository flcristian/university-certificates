using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

    public virtual required Student Student { get; set; }
}
