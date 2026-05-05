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
            isEdit = (shipment != null);

            LoadComboBoxes();

            if (isEdit)
            {
                Title = "Редактирование отгрузки";
                cbOrder.SelectedValue = currentShipment.OrderId;
                cbWarehouse.SelectedValue = currentShipment.WarehouseId;
                dpShipmentDate.SelectedDate = currentShipment.ShipmentDate;
            }
            else
            {
                Title = "Новая отгрузка";
                dpShipmentDate.SelectedDate = DateTime.Today;
            }
        }

        private void LoadComboBoxes()
        {
            // Заказы, которые ещё не отгружены (статус New или Shipped? лучше Shipped)
            var orders = db.Orders.Where(o => o.Status != "Completed").ToList();
            cbOrder.ItemsSource = orders;
            cbOrder.DisplayMemberPath = "Id";
            cbOrder.SelectedValuePath = "Id";

            var warehouses = db.Warehouses.ToList();
            cbWarehouse.ItemsSource = warehouses;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (cbOrder.SelectedValue == null)
            {
                MessageBox.Show("Выберите заказ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cbWarehouse.SelectedValue == null)
            {
                MessageBox.Show("Выберите склад", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (dpShipmentDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату отгрузки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int orderId = (int)cbOrder.SelectedValue;
            var order = db.Orders.Find(orderId);
            if (dpShipmentDate.SelectedDate.Value < order.OrderDate)
            {
                MessageBox.Show("Дата отгрузки не может быть раньше даты заказа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!isEdit)
                db.Shipments.Add(currentShipment);

            currentShipment.OrderId = orderId;
            currentShipment.WarehouseId = (int)cbWarehouse.SelectedValue;
            currentShipment.ShipmentDate = dpShipmentDate.SelectedDate.Value;

            db.SaveChanges();
            DialogResult = true;
            Close();
        }
    }
}
