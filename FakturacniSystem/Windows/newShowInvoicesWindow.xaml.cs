using FakturacniSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FakturacniSystem.Windows
{
    public partial class newShowInvoicesWindow : Window
    {
        private List<Invoice> allInvoices = new();

        public newShowInvoicesWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var db = ContextManager.GetContext();
            allInvoices = db.Invoices.Include(i => i.Customer).OrderByDescending(i => i.Vytvoreno).ToList();

            InvoicesDataGrid.ItemsSource = allInvoices;

            // seznam zákazníků - zobrazit celé jméno (FullName)
            var customers = db.Customers.OrderBy(c => c.Prijmeni).ThenBy(c => c.Jmeno).ToList();
            CustomersListBox.ItemsSource = customers;
            CustomersListBox.DisplayMemberPath = "FullName";
        }

        private void CustomersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomersListBox.SelectedItem is not Customer c)
            {
                InvoicesDataGrid.ItemsSource = allInvoices;
                return;
            }

            InvoicesDataGrid.ItemsSource = allInvoices.Where(i => i.CustomerId == c.Id).ToList();

            DetailOdberatel.Text = $"{c.Jmeno} {c.Prijmeni}";
            DetailAdresa.Text = $"{c.Ulice} {c.CP}";
            DetailIC.Text = $"IČ: {c.IC}";
            DetailDIC.Text = $"DIČ: {c.DIC}";
        }

        private void InvoicesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InvoicesDataGrid.SelectedItem is not Invoice invoice)
                return;

            DetailInvoiceNumber.Text = $"Číslo faktury: {invoice.InvoiceNumber}";
            DetailDate.Text = $"Datum vystavení: {invoice.Vytvoreno:dd.MM.yyyy}";
            DetailSluzba.Text = $"Služba: {invoice.Sluzba}";
            DetailCastka.Text = $"Částka: {invoice.Castka} Kč";
            DetailVystavil.Text = $"Vystavil: {invoice.Vystavil}";

            if (invoice.Customer != null)
            {
                DetailOdberatel.Text = $"{invoice.Customer.Jmeno} {invoice.Customer.Prijmeni}";
                DetailAdresa.Text = $"{invoice.Customer.Ulice} {invoice.Customer.CP}";
                DetailIC.Text = $"IČ: {invoice.Customer.IC}";
                DetailDIC.Text = $"DIČ: {invoice.Customer.DIC}";
            }
        }

        private void ShowAllInvoices_Click(object sender, RoutedEventArgs e)
        {
            InvoicesDataGrid.ItemsSource = allInvoices;
        }
    }
}
