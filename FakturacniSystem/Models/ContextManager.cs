using FakturacniSystem.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakturacniSystem.Models
{
    public class ContextManager
    {
        private readonly InvoiceSystemContext _iSContext;

        public ContextManager(InvoiceSystemContext iSContext)
        {
            _iSContext = iSContext;
        }
    }
}
