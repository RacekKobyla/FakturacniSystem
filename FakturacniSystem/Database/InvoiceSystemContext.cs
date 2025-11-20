using FakturacniSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FakturacniSystem.Database
{
    public class InvoiceSystemContext : DbContext
    {
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=InvoiceSystemDatabase.db");
        }

        public InvoiceSystemContext()
        {
            // při vývoji můžete ponechat EnsureCreated; pro produkci použij migrace
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // jednoduchá konfigurace FK
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
