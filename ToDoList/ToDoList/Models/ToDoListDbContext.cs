﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Models.Entities;

namespace ToDoList.Models
{
    public class ToDoListDbContext(DbContextOptions<ToDoListDbContext> options) : DbContext(options)
    {
        public DbSet<Entities.User> Users { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Assignment> Assignments { get; set; } = default!;
        public DbSet<AssignmentStep> AssignmentSteps { get; set; } = default!;
        public DbSet<CategoryAssignment> CategoryAssignments { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assignment>(builder =>
            {
                builder.HasOne(u => u.User)
                 .WithMany(a => a.Assignments)
                 .HasForeignKey(fk => fk.UserId);

                builder.HasMany(c => c.Categories)
                 .WithMany(a => a.Assignments)
                 .UsingEntity<CategoryAssignment>();

                builder.HasMany(a => a.AssignmentSteps)
                .WithOne(a => a.Assignment)
                .HasForeignKey(fk => fk.AssignmentId);

                builder.Property(a => a.Name)
                .HasColumnType("nvarchar(28)")
                .HasMaxLength(28);
            });

            modelBuilder.Entity<Entities.User>()
                .Property(u => u.Name)
                .HasColumnType("nvarchar(20)")
                .HasMaxLength(20);

            modelBuilder.Entity<Category>(builder =>
            {
                builder.Property(c => c.Name)
                .HasColumnType("nvarchar(11)")
                .HasMaxLength(11);

                builder.HasData(new Category() { Id = 1, Name = "Pozostałe", IsBuiltIn = true });
            });
            
            modelBuilder.Entity<AssignmentStep>()
                .Property(a => a.Name)
                .HasColumnType("nvarchar(17)")
                .HasMaxLength(17);

            modelBuilder.Entity<CategoryAssignment>()
                .HasKey(pk => new { pk.CategoryId, pk.AssignmentId });
        }
    }
}