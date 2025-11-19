using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakturacniSystem.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string Odberatel { get; set; }
        public string Adresa { get; set; }
        public int IC { get; set; }
        public string DIC { get; set; }
        public string Sluzba { get; set; }
        public int Castka { get; set; }
        public string Vystavil { get; set; }
        public DateTime Vytvoreno { get; set; }
    }
}
