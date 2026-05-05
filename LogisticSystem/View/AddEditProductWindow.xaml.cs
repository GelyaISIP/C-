using LogisticSystem.Data;
using LogisticSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LogisticSystem.View
{
    /// <summary>
    /// Логика взаимодействия для AddEditProductWindow.xaml
    /// </summary>
    public partial class AddEditProductWindow : Window
    {
        private LogisticsContext db;
        private Product currentProduct;
        private bool isEdit;

        public AddEditProductWindow(LogisticsContext context, Product product = null)
        {
            InitializeComponent();
            db = context;
            currentProduct = product ?? new Product();
            isEdit = product != null;

            if (isEdit)
            {
                Title = "Редактирование товара";
                tbName.Text = currentProduct.Name;
                tbSKU.Text = currentProduct.SKU;
                tbQuantity.Text = currentProduct.Quantity.ToString();
            }
            else
            {
                Title = "Новый товар";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                MessageBox.Show("Введите название");
                return;
            }
            if (!int.TryParse(tbQuantity.Text, out int qty))
                qty = 0;

            currentProduct.Name = tbName.Text;
            currentProduct.SKU = tbSKU.Text;
            currentProduct.Quantity = qty;

            if (!isEdit)
                db.Products.Add(currentProduct);

            db.SaveChanges();
            DialogResult = true;
            Close();
        }
    }
}
