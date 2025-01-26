using FluentMigrator;

namespace UniversityCertificates.Data.Migrations;

[Migration(250120251)]
public class CreateCertificateTemplatesTable : Migration
{
    public override void Up()
    {
        Create
            .Table("CertificateTemplates")
            .WithColumn("id")
            .AsInt32()
            .PrimaryKey()
            .Identity()
            .WithColumn("name")
            .AsString(64)
            .NotNullable()
            .WithColumn("description")
            .AsString(256)
            .Nullable()
            .WithColumn("active")
            .AsBoolean()
            .WithDefaultValue(true)
            .NotNullable();
    }

    public override void Down()
    {
        Delete.Table("CertificateTemplates");
    }
}
