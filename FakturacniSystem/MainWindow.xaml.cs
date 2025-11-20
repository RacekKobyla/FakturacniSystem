using FakturacniSystem.Models;
using FakturacniSystem.Windows;
using System.Windows;

namespace FakturacniSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateInvoice(object sender, RoutedEventArgs e)
        {
            var newCreateInvoiceWindow = new newCreateInvoiceWindow();
            newCreateInvoiceWindow.Show();
        }

        private void ShowInvoices(object sender, RoutedEventArgs e)
        {
            newShowInvoicesWindow newShowInvoicesWindow = new newShowInvoicesWindow();
            newShowInvoicesWindow.Show();
        }

        private void CustomerRegistration(object sender, RoutedEventArgs e)
        {
            newRegistrateCustomer newRegistrateCustomer = new newRegistrateCustomer();
            newRegistrateCustomer.Show();
        }
    }
}