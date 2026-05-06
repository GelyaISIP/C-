using LogisticSystem.Data;
using LogisticSystem.Migrations;
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
    /// Логика взаимодействия для AddEditShipmentWindow.xaml
    /// </summary>
    public partial class AddEditShipmentWindow : Window
    {
        private LogisticsContext db;
        private Shipment currentShipment;
        private bool isEdit;

        public AddEditShipmentWindow(LogisticsContext context, Shipment shipment = null)
        {
            InitializeComponent();
            db = context;
            currentShipment = shipment ?? new Shipment();
            isEdit = shipment != null;

            LoadComboBoxes();

            if (isEdit)
            {
                Title = "Редактирование отгрузки";
                cbOrder.SelectedValue = currentShipment.OrderId;
                cbWarehouse.SelectedValue = currentShipment.WarehouseId;
                dpPlannedShipmentDate.SelectedDate = currentShipment.PlannedShipmentDate;
                dpActualShipmentDate.SelectedDate = currentShipment.ShipmentDate;
                LoadOrderDetails(currentShipment.OrderId);
            }
            else
            {
                Title = "Новая отгрузка";
                dpPlannedShipmentDate.SelectedDate = DateTime.Today.AddDays(2);
                dpActualShipmentDate.SelectedDate = null;
                dpActualShipmentDate.IsEnabled = false;
            }
        }

        private void LoadComboBoxes()
        {
            var orders = db.Orders
                .Include("Client")
                .Where(o => o.Status != "Completed" && !db.Shipments.Any(s => s.OrderId == o.Id))
                .ToList();
            cbOrder.ItemsSource = orders;
            cbOrder.SelectedValuePath = "Id";
            cbOrder.DisplayMemberPath = "Info";

            cbWarehouse.ItemsSource = db.Warehouses.ToList();
        }

        private void CbOrder_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cbOrder.SelectedValue != null)
            {
                int orderId = (int)cbOrder.SelectedValue;
                LoadOrderDetails(orderId);
                LoadStocksForOrder(orderId);
                var order = db.Orders.Find(orderId);
                if (order != null)
                    dpPlannedShipmentDate.SelectedDate = order.OrderDate.AddDays(2);
            }
        }

        private void LoadOrderDetails(int orderId)
        {
            var order = db.Orders
                .Include("Client")
                .Include("OrderProducts.Product")
                .FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                tbOrderInfo.Text =  $"{order.Client?.Name}";
                dgOrderProducts.ItemsSource = order.OrderProducts.ToList();
            }
        }

        private void LoadStocksForOrder(int orderId)
        {
            // Получаем уникальные товары в заказе
            var productsInOrder = db.OrderProducts
                .Where(op => op.OrderId == orderId)
                .Select(op => op.ProductId)
                .Distinct()
                .ToList();

            // Для каждого товара ищем остатки на всех складах
            var stocks = db.Stocks
                .Include("Product")
                .Include("Warehouse")
                .Where(pw => productsInOrder.Contains(pw.ProductId))
                .Select(pw => new
                {
                    ProductName = pw.Product.Name,
                    WarehouseName = pw.Warehouse.Name,
                    pw.Quantity
                })
                .ToList();

            dgStocks.ItemsSource = stocks;
        }

        private bool CheckStockAvailability(int orderId, int warehouseId)
        {
            var orderProducts = db.OrderProducts.Where(op => op.OrderId == orderId).ToList();
            foreach (var op in orderProducts)
            {
                var stock = db.Stocks.FirstOrDefault(pw => pw.ProductId == op.ProductId && pw.WarehouseId == warehouseId);
                if (stock == null || stock.Quantity < op.Quantity)
                    return false;
            }
            return true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (cbOrder.SelectedValue == null || cbWarehouse.SelectedValue == null || dpPlannedShipmentDate.SelectedDate == null)
            {
                MessageBox.Show("Заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int orderId = (int)cbOrder.SelectedValue;
            int warehouseId = (int)cbWarehouse.SelectedValue;

            // Проверка: хватает ли на выбранном складе всех товаров заказа
            if (!CheckStockAvailability(orderId, warehouseId))
            {
                MessageBox.Show("На выбранном складе недостаточно товаров для отгрузки. Ознакомьтесь с остатками на вкладке 'Остатки на складах' и выберите другой склад или пополните запасы.",
                    "Недостаточно товаров", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            currentShipment.OrderId = orderId;
            currentShipment.WarehouseId = warehouseId;
            currentShipment.PlannedShipmentDate = dpPlannedShipmentDate.SelectedDate.Value;

            if (dpActualShipmentDate.SelectedDate != null)
            {
                currentShipment.ShipmentDate = dpActualShipmentDate.SelectedDate.Value;
                var order = db.Orders.Find(orderId);
                if (order != null && order.Status != "Completed")
                    order.Status = "Shipped";
            }
            else
            {
                currentShipment.ShipmentDate = null;
            }

            if (!isEdit)
                db.Shipments.Add(currentShipment);

            db.SaveChanges();
            DialogResult = true;
            Close();
        }
    }
}
