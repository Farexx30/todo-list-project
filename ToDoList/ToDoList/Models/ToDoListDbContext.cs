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
                 .UsingEntity<CategoryAssignment>(

                    j => j.HasOne(c => c.Category)
                          .WithMany()
                          .HasForeignKey(fk => fk.CategoryId),

                    j => j.HasOne(a => a.Assignment)
                          .WithMany()
                          .HasForeignKey(fk => fk.AssignmentId),

                    j => j.HasKey(pk => new { pk.CategoryId, pk.AssignmentId })
                 );

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
        }
    }
}