using FluentMigrator;

namespace UniversityCertificates.Data.Migrations;

[Migration(250120251)]
public class CreateRegisterEntriesTable : Migration
{
    public override void Up()
    {
        Create
            .Table("RegisterEntries")
            .WithColumn("id")
            .AsInt32()
            .PrimaryKey()
            .Identity()
            .WithColumn("student_serial_number")
            .AsInt32()
            .NotNullable()
            .ForeignKey("FK_RegisterEntries_Students", "Students", "serial_number")
            .WithColumn("date_of_issue")
            .AsDateTime()
            .WithDefaultValue(SystemMethods.CurrentDateTime)
            .NotNullable()
            .WithColumn("reason")
            .AsString(64)
            .NotNullable();

        Create
            .Index("IX_RegisterEntries_StudentSerialNumber")
            .OnTable("RegisterEntries")
            .OnColumn("student_serial_number");

        Create
            .Index("IX_RegisterEntries_DateOfIssue")
            .OnTable("RegisterEntries")
            .OnColumn("date_of_issue");
    }

    public override void Down()
    {
        Delete.Table("RegisterEntries");
    }
}
