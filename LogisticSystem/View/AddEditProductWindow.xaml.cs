using LogisticSystem.Data;
using LogisticSystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<StockItem> StockItems { get; set; }

        public AddEditProductWindow(LogisticsContext context, Product product = null)
        {
            InitializeComponent();
            db = context;
            currentProduct = product ?? new Product();
            isEdit = product != null;

            StockItems = new ObservableCollection<StockItem>();

            LoadWarehousesAndStocks();
            DataContext = this; // для привязки StockItems

            if (isEdit)
            {
                Title = "Редактирование товара";
                tbName.Text = currentProduct.Name;
                tbSKU.Text = currentProduct.SKU;
                // Загружаем существующие Stocks
                var existingStocks = db.Stocks.Where(s => s.ProductId == currentProduct.Id).ToList();
                foreach (var stockItem in StockItems)
                {
                    var existing = existingStocks.FirstOrDefault(s => s.WarehouseId == stockItem.WarehouseId);
                    stockItem.Quantity = existing?.Quantity ?? 0;
                }
            }
            else
            {
                Title = "Новый товар";
                // Для нового товара все количества = 0
            }
        }

        private void LoadWarehousesAndStocks()
        {
            var warehouses = db.Warehouses.ToList();
            foreach (var w in warehouses)
            {
                StockItems.Add(new StockItem
                {
                    WarehouseId = w.Id,
                    WarehouseName = w.Name,
                    Quantity = 0
                });
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                MessageBox.Show("Введите название товара", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            currentProduct.Name = tbName.Text;
            currentProduct.SKU = tbSKU.Text;

            // Сохраняем товар сначала, чтобы получить Id для нового
            if (!isEdit)
                db.Products.Add(currentProduct);
            db.SaveChanges(); // теперь Id у currentProduct есть

            // Обновляем Stocks
            foreach (var item in StockItems)
            {
                var stock = db.Stocks.FirstOrDefault(s => s.ProductId == currentProduct.Id && s.WarehouseId == item.WarehouseId);
                if (item.Quantity > 0)
                {
                    if (stock == null)
                    {
                        stock = new ProductWarehouse
                        {
                            ProductId = currentProduct.Id,
                            WarehouseId = item.WarehouseId,
                            Quantity = item.Quantity
                        };
                        db.Stocks.Add(stock);
                    }
                    else
                    {
                        stock.Quantity = item.Quantity;
                    }
                }
                else
                {
                    // Если количество = 0, можно удалить запись, но лучше оставить с 0
                    if (stock != null)
                        stock.Quantity = 0;
                }
            }
            db.SaveChanges();

            // Вычисляем общее количество как сумму по складам
            currentProduct.Quantity = StockItems.Sum(i => i.Quantity);
            db.SaveChanges();

            DialogResult = true;
            Close();
        }
    }

    public class StockItem
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int Quantity { get; set; }
    }
}
