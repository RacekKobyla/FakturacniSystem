using FakturacniSystem.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FakturacniSystem.Windows
{
    public partial class newCreateInvoiceWindow : Window
    {
        public newCreateInvoiceWindow()
        {
            InitializeComponent();
            var db = ContextManager.GetContext();
            db.Database.EnsureCreated();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Automatické číslo faktury
        private string GenerateInvoiceNumber()
        {
            var db = ContextManager.GetContext();

            int lastId = db.Invoices
                           .OrderByDescending(i => i.Id)
                           .Select(i => i.Id)
                           .FirstOrDefault();

            return (lastId + 1).ToString("00000"); // 00001, 00002...
        }

        private void SaveInfo(object sender, RoutedEventArgs e)
        {
            // Validace
            if (string.IsNullOrWhiteSpace(OdberatelJmeno.Text) ||
                string.IsNullOrWhiteSpace(OdberatelAdresa.Text) ||
                string.IsNullOrWhiteSpace(OdberatelIC.Text) ||
                string.IsNullOrWhiteSpace(OdberatelDIC.Text) ||
                VystavilComboBox.SelectedItem == null)
            {
                MessageBox.Show("Vyplňte prosím všechna pole.", "Chybí údaje", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(OdberatelIC.Text, out int ic))
            {
                MessageBox.Show("IČ musí být číslo!", "Chybný formát", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Opravené získání hodnoty z ComboBoxu
            string vystavil = "";

            if (VystavilComboBox.SelectedItem is ComboBoxItem item)
                vystavil = item.Content.ToString();
            else
                vystavil = VystavilComboBox.Text;

            var db = ContextManager.GetContext();

            // Nová faktura
            var invoice = new Invoice
            {
                InvoiceNumber = GenerateInvoiceNumber(),
                Odberatel = OdberatelJmeno.Text,
                Adresa = OdberatelAdresa.Text,
                IC = ic,
                DIC = OdberatelDIC.Text,
                Vystavil = vystavil,
                Vytvoreno = DateTime.Now
            };

            // Uložení do DB
            db.Invoices.Add(invoice);
            db.SaveChanges();

            // Export do TXT
            SaveInvoiceAsTxt(invoice);

            MessageBox.Show("Faktura byla úspěšně vytvořena a uložena.");
            this.Close();
        }

        private void SaveInvoiceAsTxt(Invoice invoice)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Textový soubor (*.txt)|*.txt";
            dialog.FileName = $"Faktura_{invoice.InvoiceNumber}.txt";

            if (dialog.ShowDialog() == true)
            {
                string content =
$@"FAKTURA
------------------------------
Číslo faktury: {invoice.InvoiceNumber}
Datum vytvoření: {invoice.Vytvoreno:dd.MM.yyyy HH:mm}

ODBĚRATEL
Jméno: {invoice.Odberatel}
Adresa: {invoice.Adresa}
IČ: {invoice.IC}
DIČ: {invoice.DIC}

Vystavil: {invoice.Vystavil}
";

                File.WriteAllText(dialog.FileName, content, System.Text.Encoding.UTF8);
            }
        }
    }
}
