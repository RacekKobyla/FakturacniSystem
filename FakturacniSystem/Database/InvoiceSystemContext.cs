using FakturacniSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FakturacniSystem.Database
{
    public class InvoiceSystemContext : DbContext
    {
        public InvoiceSystemContext()
        {
            Database.EnsureCreated();
        }
    }
}
