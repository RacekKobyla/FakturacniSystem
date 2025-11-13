using FakturacniSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace FakturacniSystem.Database
{
    public class InvoiceSystemContext : DbContext
    {
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=InvoiceSystemDatabase.db");
        }

        public InvoiceSystemContext()
        {
            Database.EnsureCreated();
        }
    }

}
