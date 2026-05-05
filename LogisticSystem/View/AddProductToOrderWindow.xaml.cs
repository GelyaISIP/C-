using System.Linq;
using System.Windows;
using LogisticSystem.Data;
using static LogisticSystem.View.OrderWindow;

namespace LogisticSystem.View
{
    public partial class AddProductToOrderWindow : Window
    {
        private LogisticsContext db;
        public OrderItem SelectedOrderItem { get; private set; }


        public AddProductToOrderWindow(LogisticsContext context)
        {
            InitializeComponent();
            db = context;  
            LoadData();
        }

        private void LoadData()
        {
            var products = db.Products.OrderBy(p => p.Name).ToList();
            cbProduct.ItemsSource = products;
            if (products.Any()) cbProduct.SelectedIndex = 0;

            var warehouses = db.Warehouses.OrderBy(w => w.Name).ToList();
            cbWarehouse.ItemsSource = warehouses;
            if (warehouses.Any()) cbWarehouse.SelectedIndex = 0;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (cbProduct.SelectedValue == null || cbWarehouse.SelectedValue == null)
            {
                MessageBox.Show("Выберите товар и склад", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(tbQuantity.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Введите положительное количество", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int productId = (int)cbProduct.SelectedValue;
            int warehouseId = (int)cbWarehouse.SelectedValue;
            string productName = cbProduct.Text;
            string warehouseName = cbWarehouse.Text;

            SelectedOrderItem = new OrderItem
            {
                ProductId = productId,
                ProductName = productName,
                Quantity = qty,
                WarehouseId = warehouseId,
                WarehouseName = warehouseName
            };

            DialogResult = true;
            Close();
        }
    }
}