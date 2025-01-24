﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UniversityCertificates.Data;

#nullable disable

namespace aspnet_setup_template.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("UniversityCertificates.Students.Models.Student", b =>
                {
                    b.Property<int>("SerialNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("serial_number");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("SerialNumber"));

                    b.Property<int>("DegreeType")
                        .HasColumnType("int")
                        .HasColumnName("degree_type");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("department");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("last_name");

                    b.Property<int>("StudyYear")
                        .HasColumnType("int")
                        .HasColumnName("study_year");

                    b.HasKey("SerialNumber");

                    b.ToTable("Students");
                });
#pragma warning restore 612, 618
        }
    }
}
