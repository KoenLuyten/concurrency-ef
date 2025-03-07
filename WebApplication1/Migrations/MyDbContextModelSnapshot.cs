﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace WebApplication1.Migrations
{
    [DbContext(typeof(MyDbContext))]
    partial class MyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WebApplication1.Models.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("Companies");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ConcurrencyToken = new byte[] { 67, 203, 109, 46, 25, 99, 181, 71, 148, 73, 212, 175, 102, 56, 254, 94 },
                            Name = "Acme Corp"
                        },
                        new
                        {
                            Id = 2,
                            ConcurrencyToken = new byte[] { 65, 11, 181, 255, 104, 107, 68, 69, 187, 153, 78, 196, 249, 153, 171, 53 },
                            Name = "Globex Inc"
                        });
                });

            modelBuilder.Entity("WebApplication1.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("Persons");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ConcurrencyToken = new byte[] { 79, 225, 194, 211, 60, 187, 1, 65, 139, 128, 156, 221, 44, 147, 139, 13 },
                            Name = "John Doe"
                        },
                        new
                        {
                            Id = 2,
                            ConcurrencyToken = new byte[] { 203, 76, 6, 2, 152, 234, 221, 72, 180, 138, 42, 183, 154, 56, 68, 248 },
                            Name = "Jane Smith"
                        });
                });

            modelBuilder.Entity("WebApplication1.Models.PersonCompany", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<byte[]>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<DateTime>("FromDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PersonId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ToDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("PersonId");

                    b.ToTable("PersonCompany");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CompanyId = 1,
                            ConcurrencyToken = new byte[] { 49, 230, 138, 128, 185, 225, 136, 77, 182, 44, 4, 29, 89, 29, 11, 190 },
                            FromDate = new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            PersonId = 1
                        },
                        new
                        {
                            Id = 2,
                            CompanyId = 2,
                            ConcurrencyToken = new byte[] { 121, 31, 19, 74, 42, 133, 107, 75, 162, 207, 243, 37, 140, 68, 66, 22 },
                            FromDate = new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            PersonId = 1
                        });
                });

            modelBuilder.Entity("WebApplication1.Models.PersonCompany", b =>
                {
                    b.HasOne("WebApplication1.Models.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApplication1.Models.Person", null)
                        .WithMany("PersonCompanies")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");
                });

            modelBuilder.Entity("WebApplication1.Models.Person", b =>
                {
                    b.Navigation("PersonCompanies");
                });
#pragma warning restore 612, 618
        }
    }
}
