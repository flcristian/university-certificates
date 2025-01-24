using FluentMigrator;

namespace UniversityCertificates.Data.Migrations;

[Migration(240120251)]
public class CreateStudentsTable : Migration
{
    public override void Up()
    {
        Create
            .Table("students")
            .WithColumn("serial_number")
            .AsInt32()
            .PrimaryKey()
            .Identity()
            .WithColumn("first_name")
            .AsString(64)
            .NotNullable()
            .WithColumn("last_name")
            .AsString(64)
            .NotNullable()
            .WithColumn("study_year")
            .AsInt16()
            .NotNullable()
            .WithColumn("degree_type")
            .AsDecimal()
            .NotNullable()
            .WithColumn("department")
            .AsString(64)
            .NotNullable();
    }

    public override void Down()
    {
        Delete.Table("students");
    }
}
