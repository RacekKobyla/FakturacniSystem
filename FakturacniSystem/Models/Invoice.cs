using System;

namespace FakturacniSystem.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }

        // odkaz na odběratele
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public string Sluzba { get; set; }
        public int Castka { get; set; }
        public string Vystavil { get; set; }
        public DateTime Vytvoreno { get; set; } // datum vystavení
        public DateTime Splatnost { get; set; } // datum splatnosti
    }
}