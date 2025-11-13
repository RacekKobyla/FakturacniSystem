using FakturacniSystem.Models;
using System.Windows;

namespace FakturacniSystem.Windows
{
    /// <summary>
    /// Interakční logika pro newCreateInvoice.xaml
    /// </summary>
    public partial class newCreateInvoiceWindow : Window
    {

        public newCreateInvoiceWindow()
        {
            InitializeComponent();
            var db = ContextManager.GetContext();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveInfo(object sender, RoutedEventArgs e)
        {
            var db = ContextManager.GetContext();

            var invoice = new Invoice();
            {
                string odberatel = OdberatelJmeno.Text;
                string adresa = OdberatelAdresa.Text;
                int ic = int.Parse(OdberatelIC.Text);
                string dic = OdberatelDIC.Text;
                string vystavil = VystavilComboBox.SelectedItem.ToString();
                DateTime vytvoreno = DateTime.Now;
            };

            db.Invoices.Add(invoice);
            db.SaveChanges();
        }
    }
}
