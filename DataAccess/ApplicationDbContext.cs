using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BusinessObject;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.Department)
                    .WithMany(d => d.Categories)
                    .HasForeignKey(e => e.DepartmentId);
            });
            modelBuilder.Entity<Response>(entity =>
            {
                entity.Property(e => e.Content).IsRequired();
                entity.HasOne(e => e.Form)
                    .WithMany(f => f.Responses)
                    .HasForeignKey(e => e.FormId);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Responses)
                    .HasForeignKey(e => e.StaffId);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });
            modelBuilder.Entity<Form>(entity =>
            {
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Status).HasDefaultValue(FormStatus.Pending);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Forms)
                    .HasForeignKey(e => e.StudentId);
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Forms)
                    .HasForeignKey(e => e.CategoryId);
            });
            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}