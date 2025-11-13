using FakturacniSystem.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakturacniSystem.Models
{
    public static class ContextManager
    {
        private static InvoiceSystemContext _context;

        public static InvoiceSystemContext GetContext()
        {
            return _context ??= new InvoiceSystemContext();
        }
    }
}

