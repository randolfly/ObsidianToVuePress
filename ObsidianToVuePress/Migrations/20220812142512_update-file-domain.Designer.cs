﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ObsidianToVuePress.Context;

#nullable disable

namespace ObsidianToVuePress.Migrations
{
    [DbContext(typeof(ObsidianFileInfoContext))]
    [Migration("20220812142512_update-file-domain")]
    partial class updatefiledomain
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.7");

            modelBuilder.Entity("ObsidianToVuePress.Domain.ObsidianFileInfo", b =>
                {
                    b.Property<string>("Path")
                        .HasColumnType("TEXT");

                    b.Property<string>("DestDir")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Sha256")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SrcDir")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Path");

                    b.ToTable("Files");
                });
#pragma warning restore 612, 618
        }
    }
}