using FakturacniSystem.Database;
using FakturacniSystem.Models;
using System;
using System.Linq;
using System.Text;
using System.Windows;

namespace FakturacniSystem.Windows
{
    /// <summary>
    /// Interakční logika pro newRegistrateCustomer.xaml
    /// </summary>
    public partial class newRegistrateCustomer : Window
    {
        public newRegistrateCustomer()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            // jednoduchá validace
            string jmeno = JmenoBox.Text?.Trim() ?? "";
            string prijmeni = PrijmeniBox.Text?.Trim() ?? "";
            string ulice = UliceBox.Text?.Trim() ?? "";
            string cp = CPBox.Text?.Trim() ?? "";
            string mesto = MestoBox.Text?.Trim() ?? "";
            string psc = PSCBox.Text?.Trim() ?? "";
            string icText = ICBox.Text?.Trim() ?? "";
            string dic = DICBox.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(jmeno) || string.IsNullOrWhiteSpace(prijmeni))
            {
                MessageBox.Show("Jméno a příjmení jsou povinné.", "Neplatné údaje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // IČ musí být číslo a délka 8
            if (!int.TryParse(icText, out int ic) || icText.Length != 8)
            {
                MessageBox.Show("IČ musí být číslo a mít 8 číslic.", "Neplatné IČ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // DIČ délka minimálně 2, maximálně 12 (uprav dle potřeby)
            if (dic.Length < 2 || dic.Length > 12)
            {
                MessageBox.Show("DIČ musí mít délku 2–12 znaků.", "Neplatné DIČ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var db = ContextManager.GetContext();

            //DUPLICITY IČ ZAKÁZÁNY
            // DUPLICITY JMEN A ADRESY POVOLENY S VAROVÁNÍM

            // 1) Kontrola IČ v DB (pevná duplicita -> zamezit)
            if (db.Customers.Any(c => c.IC == ic))
            {
                MessageBox.Show("Odběratel s tímto IČ již v databázi existuje. Uložení zrušeno.", "Duplicitní IČ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2) Hledání možných duplicit podle jména + adresních údajů (pouze varování)
            var potentialDuplicates = db.Customers
                .Where(c => c.Jmeno == jmeno && c.Prijmeni == prijmeni
                            && ( ( !string.IsNullOrEmpty(psc) && c.PSC == psc )
                                 || ( !string.IsNullOrEmpty(ulice) && c.Ulice == ulice )
                                 || ( !string.IsNullOrEmpty(cp) && c.CP == cp ) ))
                .ToList();

            if (potentialDuplicates.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine("Nalezeni podobní odběratelé:");
                foreach (var pd in potentialDuplicates)
                {
                    sb.AppendLine($"{pd.FullName} | IČ: {pd.IC} | {pd.Ulice} {pd.CP}, {pd.Mesto} {pd.PSC} | DIČ: {pd.DIC}");
                }
                sb.AppendLine();
                sb.AppendLine("Chcete pokračovat a uložit nového odběratele i přesto?");

                var result = MessageBox.Show(sb.ToString(), "Možná duplicita", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;
            }

            var customer = new Customer
            {
                Jmeno = jmeno,
                Prijmeni = prijmeni,
                Ulice = ulice,
                CP = cp,
                Mesto = mesto,
                PSC = psc,
                IC = ic,
                DIC = dic
            };

            db.Customers.Add(customer);
            db.SaveChanges();

            MessageBox.Show("Odběratel zaregistrován.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
