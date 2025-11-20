using FakturacniSystem.Database;
using FakturacniSystem.Models;
using System;
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
