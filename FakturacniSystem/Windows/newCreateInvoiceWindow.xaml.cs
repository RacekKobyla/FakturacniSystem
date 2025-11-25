using FakturacniSystem.Database;
using FakturacniSystem.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace FakturacniSystem.Windows
{
    public partial class newCreateInvoiceWindow : Window
    {
        public newCreateInvoiceWindow()
        {
            InitializeComponent();
            InitializeServiceCombo();
            LoadCustomers();

            // Nastavit datum vystavení na dnešek a zajistit, že "Vystavil" bude viditelný
            IssueDatePicker.SelectedDate = DateTime.Now.Date;
            VystavilComboBox.Visibility = Visibility.Visible;
        }

        private void InitializeServiceCombo()
        {
            var services = new[]
            {
                "Elektroinstalace 1h",
                "Elektroinstalace 2h",
                "IT služby 1h",
                "IT služby 2h"
            };

            SluzbaComboBox.ItemsSource = services;
            SluzbaComboBox.SelectionChanged += SluzbaComboBox_SelectionChanged;
        }

        private void SluzbaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var text = (SluzbaComboBox.SelectedItem as string) ?? SluzbaComboBox.Text ?? string.Empty;
            var key = text.Trim().ToLowerInvariant();

            int amount = key switch
            {
                "elektroinstalace 1h" => 2000,
                "elektroinstalace 2h" => 2000 * 2,
                "it služby 1h" => 2500,
                "it služby 2h" => 2500 * 2,
                _ => 0
            };

            CastkaBox.Text = amount > 0 ? amount.ToString() : string.Empty;
        }

        private void LoadCustomers()
        {
            var db = ContextManager.GetContext();
            var list = db.Customers.OrderBy(c => c.Prijmeni).ThenBy(c => c.Jmeno).ToList();
            CustomerComboBox.ItemsSource = list;
            // DisplayMemberPath je nastaven v XAML na "FullName"

            // Odregistrovat starý handler a zaregistrovat nový, aby nedošlo k duplicitnímu přidání
            CustomerComboBox.SelectionChanged -= CustomerComboBox_SelectionChanged;
            CustomerComboBox.SelectionChanged += CustomerComboBox_SelectionChanged;

            // Vyčistit preview (pokud není nic vybráno)
            UpdateSelectedCustomerPreview(null);
        }

        private string GenerateInvoiceNumber(InvoiceSystemContext db)
        {
            var lastInvoice = db.Invoices.OrderByDescending(i => i.Id).FirstOrDefault();
            int nextNumber = lastInvoice != null ? lastInvoice.Id + 1 : 1;
            return $"F{nextNumber:D6}";
        }

        private void OpenRegister_Click(object sender, RoutedEventArgs e)
        {
            var win = new newRegistrateCustomer();
            win.Owner = this;
            win.ShowDialog();
            LoadCustomers();
        }

        private void CloseWindow(object sender, RoutedEventArgs e) => this.Close();

        // Nový handler pro změnu výběru odběratele
        private void CustomerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomerComboBox.SelectedItem is not Customer selected)
            {
                UpdateSelectedCustomerPreview(null);
                return;
            }

            UpdateSelectedCustomerPreview(selected);
        }

        // Aktualizuje texty v preview panelu podle vybraného zákazníka
        private void UpdateSelectedCustomerPreview(Customer c)
        {
            if (c == null)
            {
                SelectedCustomerNameText.Text = "Žádný odběratel vybrán.";
                SelectedCustomerAddressText.Text = string.Empty;
                SelectedCustomerICText.Text = string.Empty;
                SelectedCustomerDICText.Text = string.Empty;
                return;
            }

            SelectedCustomerNameText.Text = $"{c.Jmeno} {c.Prijmeni} (ID: {c.Id})";
            SelectedCustomerAddressText.Text = $"{c.Ulice} {c.CP}, {c.PSC} {c.Mesto}";
            SelectedCustomerICText.Text = $"IČ: {c.IC}";
            SelectedCustomerDICText.Text = $"DIČ: {c.DIC}";
        }

        // Jediný tlačítko: uloží do DB a nabídne uložení TXT
        private void GenerateAndSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CustomerComboBox.SelectedItem is not Customer customer)
                {
                    MessageBox.Show("Vyberte odběratele.", "Neplatné údaje", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string sluzba = (SluzbaComboBox.SelectedItem as string) ?? SluzbaComboBox.Text ?? string.Empty;
                sluzba = sluzba.Trim();
                if (string.IsNullOrWhiteSpace(sluzba))
                {
                    MessageBox.Show("Zadejte službu.", "Neplatné údaje", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(CastkaBox.Text, out decimal castkaDecimal))
                {
                    MessageBox.Show("Částka musí být číslo.", "Neplatné údaje", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                int castka = (int)castkaDecimal;

                DateTime vystaven = IssueDatePicker.SelectedDate ?? DateTime.Now;
                DateTime splatnost = DueDatePicker.SelectedDate ?? vystaven.AddDays(14);
                string vystavil = (VystavilComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? VystavilComboBox.Text ?? "";

                var db = ContextManager.GetContext();

                var invoice = new Invoice
                {
                    InvoiceNumber = GenerateInvoiceNumber(db),
                    CustomerId = customer.Id,
                    Sluzba = sluzba,
                    Castka = castka,
                    Vystavil = vystavil,
                    Vytvoreno = vystaven,
                    Splatnost = splatnost
                };

                db.Invoices.Add(invoice);
                db.SaveChanges();

                // připrav exportní text
                var sb = new StringBuilder();
                sb.AppendLine($"Číslo faktury: {invoice.InvoiceNumber}");
                sb.AppendLine($"Vystaveno: {invoice.Vytvoreno:dd.MM.yyyy}");
                sb.AppendLine($"Splatnost: {invoice.Splatnost:dd.MM.yyyy}");
                sb.AppendLine();
                sb.AppendLine("ODBĚRATEL:");
                sb.AppendLine($"{customer.Jmeno} {customer.Prijmeni}");
                sb.AppendLine($"{customer.Ulice} {customer.CP}");
                sb.AppendLine($"{customer.PSC} {customer.Mesto}");
                sb.AppendLine($"IČ: {customer.IC}");
                sb.AppendLine($"DIČ: {customer.DIC}");
                sb.AppendLine();
                sb.AppendLine($"SLUŽBA: {invoice.Sluzba}");
                sb.AppendLine($"ČÁSTKA: {invoice.Castka} Kč");
                sb.AppendLine($"Vystavil: {invoice.Vystavil}");

                var dlg = new SaveFileDialog { FileName = $"Faktura_{invoice.InvoiceNumber}.txt", DefaultExt = ".txt", Filter = "Text files (*.txt)|*.txt" };
                if (dlg.ShowDialog() == true)
                {
                    File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Faktura uložena a exportována do souboru.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Faktura uložena v databázi (export zrušen).", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
