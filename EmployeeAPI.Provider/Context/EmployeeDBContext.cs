using System;
using Microsoft.EntityFrameworkCore;
using EmployeeAPI.Contracts.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAPI.Provider.Context
{
    public class EmployeeDBContext : DbContext
    {
        public EmployeeDBContext(DbContextOptions options) : base(options) { }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasIndex(e => e.Email).IsUnique();
            modelBuilder.Entity<Department>().HasIndex(e => e.Name).IsUnique();


            // Department can have multiple employees
            modelBuilder.Entity<Department>()
                .HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .HasPrincipalKey(d => d.Id)
                .OnDelete(DeleteBehavior.Restrict); // Set to Restrict to avoid multiple cascade paths

            // Employees can be assigned multiple todos
            modelBuilder.Entity<Todo>()
                .HasOne(t => t.AssignedTo)
                .WithMany(e => e.AssignedTodos)
                .HasForeignKey(t => t.AssignedToId)
              // **Change to NoAction to prevent cascading deletes**
              .OnDelete(DeleteBehavior.NoAction);

            // Employees can create multiple todos
            modelBuilder.Entity<Todo>()
                .HasOne(t => t.AssignedBy)
                .WithMany(e => e.CreatedTodos)
                .HasForeignKey(t => t.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Employees can create multiple notifications
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.AssignedBy)
                .WithMany(e => e.CreatedNotifications)
                .HasForeignKey(n => n.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Employees can receive multiple notifications
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.AssignedTo)
                .WithMany(e => e.GotNotifications)
                .HasForeignKey(n => n.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            // Todos can have multiple notifications
            modelBuilder.Entity<Todo>()
                .HasMany(t => t.Notifications)
                .WithOne(n => n.Todo)
                .HasForeignKey(n => n.TodoId)
                .OnDelete(DeleteBehavior.Restrict); // Kept as Cascade
        }
    }
}
