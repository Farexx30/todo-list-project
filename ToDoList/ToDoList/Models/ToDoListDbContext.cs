using Microsoft.EntityFrameworkCore;
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
        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentStep> AssignmentSteps { get; set; }
        public DbSet<CategoryAssignment> CategoryAssignments { get; set; }


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
                .HasColumnType("varchar(100)")
                .HasMaxLength(100);
            });

            modelBuilder.Entity<Entities.User>()
                .Property(u => u.Name)
                .HasColumnType("varchar(30)")
                .HasMaxLength(30);

            modelBuilder.Entity<Category>(builder =>
            {
                builder.Property(c => c.Name)
                .HasColumnType("varchar(15)")
                .HasMaxLength(15);

                builder.HasData(new Category() { Id = 1, Name = "Dom", IsBuiltIn = true },
                    new Category() { Id = 2, Name = "Praca", IsBuiltIn = true },
                    new Category() { Id = 3, Name = "Edukacja", IsBuiltIn = true },
                    new Category() { Id = 4, Name = "Zadania", IsBuiltIn = true });
            });
            
            modelBuilder.Entity<AssignmentStep>()
                .Property(a => a.Name)
                .HasColumnType("varchar(50)")
                .HasMaxLength(50);

            modelBuilder.Entity<CategoryAssignment>()
                .HasKey(pk => new { pk.CategoryId, pk.AssignmentId });
        }
    }
}