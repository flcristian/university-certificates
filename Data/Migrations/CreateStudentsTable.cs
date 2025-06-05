using DotNetEnv;
using FluentMigrator;

namespace UniversityCertificates.Data.Migrations;

[Migration(240120251)]
public class CreateStudentsTable : Migration
{
    public override void Up()
    {
        Create
            .Table("Students")
            .WithColumn("serial_number")
            .AsInt32()
            .PrimaryKey()
            .WithColumn("first_name")
            .AsString(64)
            .NotNullable()
            .WithColumn("last_name")
            .AsString(64)
            .NotNullable()
            .WithColumn("email")
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

        Create.Index("IX_Students_First_Name").OnTable("Students").OnColumn("first_name");

        Create.Index("IX_Students_Last_Name").OnTable("Students").OnColumn("last_name");

        Create.Index("IX_Students_Email").OnTable("Students").OnColumn("email");

        Create.Index("IX_Students_StudyYear").OnTable("Students").OnColumn("study_year");

        Create.Index("IX_Students_Department").OnTable("Students").OnColumn("department");

        Create.Index("IX_Students_DegreeType").OnTable("Students").OnColumn("degree_type");

        Env.GetString("STUDENT_EMAIL");
        if (Env.GetString("STUDENT_EMAIL") != null)
        {
            Insert
                .IntoTable("Students")
                .Row(
                    new
                    {
                        first_name = "Florescu",
                        last_name = "Cristian Ioan",
                        email = Env.GetString("STUDENT_EMAIL"),
                        study_year = 2,
                        degree_type = 0,
                        department = "Computer Science",
                    }
                );
        }
    }

    public override void Down()
    {
        Delete.Table("Students");
    }
}
