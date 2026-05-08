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
    /// Логика взаимодействия для AdminPanelWindow.xaml
    /// </summary>
    public partial class AdminPanelWindow : Window
    {
        private LogisticsContext db;
        private Product selectedProduct;
        private Warehouse selectedWarehouse;

        public AdminPanelWindow()
        {
            InitializeComponent();
            db = new LogisticsContext();
            LoadProducts();
            LoadWarehouses();
        }

        private void LoadProducts()
        {
            var products = db.Products.Include("ProductWarehouse").ToList();
            dgProducts.ItemsSource = products;
        }

        private void LoadWarehouses()
        {
            dgWarehouses.ItemsSource = db.Warehouses.ToList();
        }

        // ========== Товары ==========
        private void dgProducts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedProduct = dgProducts.SelectedItem as Product;
            btnEditProduct.IsEnabled = selectedProduct != null;
            btnDeleteProduct.IsEnabled = selectedProduct != null;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var addWin = new AddEditProductWindow(db);
            if (addWin.ShowDialog() == true)
                LoadProducts();
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (selectedProduct == null) return;
            var editWin = new AddEditProductWindow(db, selectedProduct);
            if (editWin.ShowDialog() == true)
                LoadProducts();
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (selectedProduct == null) return;
            if (MessageBox.Show("Удалить товар?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                db.Products.Remove(selectedProduct);
                db.SaveChanges();
                LoadProducts();
            }
        }

        // ========== Склады ==========
        private void dgWarehouses_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedWarehouse = dgWarehouses.SelectedItem as Warehouse;
            btnEditWarehouse.IsEnabled = selectedWarehouse != null;
            btnDeleteWarehouse.IsEnabled = selectedWarehouse != null;
        }

        private void AddWarehouse_Click(object sender, RoutedEventArgs e)
        {
            var addWin = new AddEditWarehouseWindow(db);
            if (addWin.ShowDialog() == true)
                LoadWarehouses();
        }

        private void EditWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (selectedWarehouse == null) return;
            var editWin = new AddEditWarehouseWindow(db, selectedWarehouse);
            if (editWin.ShowDialog() == true)
                LoadWarehouses();
        }

        private void DeleteWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (selectedWarehouse == null) return;
            if (MessageBox.Show("Удалить склад? Это может нарушить отгрузки!", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                db.Warehouses.Remove(selectedWarehouse);
                db.SaveChanges();
                LoadWarehouses();
            }
        }
    }
}
