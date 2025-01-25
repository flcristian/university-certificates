using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniversityCertificates.Register.Models;

namespace UniversityCertificates.Students.Models;

public class Student
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("serial_number")]
    public required int SerialNumber { get; set; }

    [Required]
    [Column("first_name")]
    public required string FirstName { get; set; }

    [Required]
    [Column("last_name")]
    public required string LastName { get; set; }

    [Required]
    [Column("study_year")]
    public required int StudyYear { get; set; }

    [Required]
    [Column("degree_type")]
    public required DegreeType DegreeType { get; set; }

    [Required]
    [Column("department")]
    public required string Department { get; set; }

    public virtual required IEnumerable<RegisterEntry> RegisterEntries { get; set; }
}
