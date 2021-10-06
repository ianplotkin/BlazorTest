﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace BlazorTest.Data.Models
{
    public partial class GroceryContext : DbContext
    {
        public GroceryContext()
        {
        }

        public GroceryContext(DbContextOptions<GroceryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Grocery> Grocery { get; set; }
        public virtual DbSet<Store> Store { get; set; }
        public virtual DbSet<StoreArea> StoreArea { get; set; }
        public virtual DbSet<StoreAreaMember> StoreAreaMember { get; set; }
        public virtual DbSet<WeatherForecast> WeatherForecast { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Grocery>(entity =>
            {
                entity.Property(e => e.DefaultUnit)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Grocery)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Grocery_Category");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<StoreArea>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.StoreArea)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreArea_Store");
            });

            modelBuilder.Entity<StoreAreaMember>(entity =>
            {
                entity.HasKey(e => new { e.GroceryId, e.StoreAreaId });

                entity.HasOne(d => d.Grocery)
                    .WithMany(p => p.StoreAreaMember)
                    .HasForeignKey(d => d.GroceryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreAreaMember_Grocery");

                entity.HasOne(d => d.StoreArea)
                    .WithMany(p => p.StoreAreaMember)
                    .HasForeignKey(d => d.StoreAreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreAreaMember_StoreArea");
            });

            modelBuilder.Entity<WeatherForecast>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Summary).HasMaxLength(50);

                entity.Property(e => e.UserName).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}