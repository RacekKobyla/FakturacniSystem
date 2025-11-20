using System.Collections.Generic;

namespace FakturacniSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public string Ulice { get; set; }
        public string CP { get; set; }   // č.p.
        public string Mesto { get; set; }
        public string PSC { get; set; }
        public int IC { get; set; }
        public string DIC { get; set; }
        // navigace
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        // computed property pro zobrazení celého jména v UI
        public string FullName => $"{Jmeno} {Prijmeni}";
    }
}