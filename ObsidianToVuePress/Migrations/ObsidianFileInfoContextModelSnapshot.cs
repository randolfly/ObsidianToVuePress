﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ObsidianToVuePress.Context;

#nullable disable

namespace ObsidianToVuePress.Migrations
{
    [DbContext(typeof(ObsidianFileInfoContext))]
    partial class ObsidianFileInfoContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.7");

            modelBuilder.Entity("ObsidianToVuePress.Domain.ObsidianFileInfo", b =>
                {
                    b.Property<string>("SrcPath")
                        .HasColumnType("TEXT");

                    b.Property<string>("DestPath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Sha256")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("SrcPath");

                    b.ToTable("Files");
                });
#pragma warning restore 612, 618
        }
    }
}