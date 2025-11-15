using FakturacniSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FakturacniSystem.Windows
{
    public partial class newShowInvoicesWindow : Window
    {
        private List<Invoice> allInvoices = new List<Invoice>();

        public newShowInvoicesWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var db = ContextManager.GetContext();

            // Načti všechny faktury
            allInvoices = db.Invoices.OrderByDescending(i => i.Vytvoreno).ToList();

            // Unikátní odběratelé včetně jejich údajů
            var customers = allInvoices
                .Select(i => new { i.Odberatel, i.Adresa, i.IC, i.DIC })
                .Distinct()
                .OrderBy(i => i.Odberatel)
                .ToList();

            CustomersListBox.ItemsSource = customers;
            CustomersListBox.DisplayMemberPath = "Odberatel";
        }

        private void CustomersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomersListBox.SelectedItem == null)
            {
                InvoicesDataGrid.ItemsSource = null;
                return;
            }

            dynamic customer = CustomersListBox.SelectedItem;

            // Zobraz faktury daného odběratele
            var customerInvoices = allInvoices
                .Where(i => i.Odberatel == customer.Odberatel)
                .OrderByDescending(i => i.Vytvoreno)
                .ToList();

            InvoicesDataGrid.ItemsSource = customerInvoices;

            // Vyplň detail odběratele
            DetailOdberatel.Text = customer.Odberatel;
            DetailAdresa.Text = customer.Adresa;
            DetailIC.Text = $"IČ: {customer.IC}";
            DetailDIC.Text = $"DIČ: {customer.DIC}";

            // Vyčisti detaily faktury
            DetailInvoiceNumber.Text = "";
            DetailDate.Text = "";
            DetailVystavil.Text = "";
        }

        private void InvoicesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InvoicesDataGrid.SelectedItem is not Invoice invoice)
                return;

            // Detail faktury
            DetailInvoiceNumber.Text = $"Číslo faktury: {invoice.InvoiceNumber}";
            DetailDate.Text = $"Datum: {invoice.Vytvoreno:dd.MM.yyyy HH:mm}";
            DetailVystavil.Text = $"Vystavil: {invoice.Vystavil}";

            // Detail odběratele
            DetailOdberatel.Text = invoice.Odberatel;
            DetailAdresa.Text = invoice.Adresa;
            DetailIC.Text = $"IČ: {invoice.IC}";
            DetailDIC.Text = $"DIČ: {invoice.DIC}";
        }
    }
}
