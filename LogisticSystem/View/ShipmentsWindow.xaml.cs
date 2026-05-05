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
    /// Логика взаимодействия для ShipmentsWindow.xaml
    /// </summary>
    public partial class ShipmentsWindow : Window
    {
        private LogisticsContext db;
        private Shipment selectedShipment;

        public ShipmentsWindow()
        {
            InitializeComponent();
            db = new LogisticsContext();
            LoadFilters();
            LoadShipments();
        }

        private void LoadFilters()
        {
            // Загрузка складов для фильтра
            var warehouses = db.Warehouses.ToList();
            cbWarehouseFilter.ItemsSource = warehouses;
            cbWarehouseFilter.DisplayMemberPath = "Name";
            cbWarehouseFilter.SelectedValuePath = "Id";

            // Загрузка клиентов для фильтра
            var clients = db.Clients.OrderBy(c => c.Name).ToList();
            cbClientFilter.ItemsSource = clients;
            cbClientFilter.DisplayMemberPath = "Name";
            cbClientFilter.SelectedValuePath = "Id";
        }

        private void LoadShipments()
        {
            var query = db.Shipments
                         .Include("Order")
                         .Include("Order.Client")
                         .Include("Warehouse")
                         .AsQueryable();

            // Фильтр по складу
            if (cbWarehouseFilter.SelectedValue != null && (int)cbWarehouseFilter.SelectedValue > 0)
                query = query.Where(s => s.WarehouseId == (int)cbWarehouseFilter.SelectedValue);

            // Фильтр по клиенту (через связанный заказ)
            if (cbClientFilter.SelectedValue != null && (int)cbClientFilter.SelectedValue > 0)
                query = query.Where(s => s.Order.ClientId == (int)cbClientFilter.SelectedValue);

            // Фильтр по статусу заказа
            var statusFilter = (cbStatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (statusFilter != null && statusFilter != "Все")
                query = query.Where(s => s.Order.Status == statusFilter);

            // Фильтр по начальной дате
            if (dpStartDate.SelectedDate != null)
                query = query.Where(s => s.ShipmentDate >= dpStartDate.SelectedDate);

            // Фильтр по конечной дате
            if (dpEndDate.SelectedDate != null)
                query = query.Where(s => s.ShipmentDate <= dpEndDate.SelectedDate.Value.AddDays(1));

            dgShipments.ItemsSource = query.OrderByDescending(s => s.ShipmentDate).ToList();
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadShipments();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            cbWarehouseFilter.SelectedValue = null;
            cbClientFilter.SelectedValue = null;
            cbStatusFilter.SelectedIndex = 0; // "Все"
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;
            LoadShipments();
        }

        private void dgShipments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedShipment = dgShipments.SelectedItem as Shipment;
            btnEdit.IsEnabled = selectedShipment != null;
            btnDelete.IsEnabled = selectedShipment != null;
        }

        private void AddShipment_Click(object sender, RoutedEventArgs e)
        {
            var addWin = new AddEditShipmentWindow(db);
            if (addWin.ShowDialog() == true)
                LoadShipments();
        }

        private void EditShipment_Click(object sender, RoutedEventArgs e)
        {
            if (selectedShipment == null) return;
            var editWin = new AddEditShipmentWindow(db, selectedShipment);
            if (editWin.ShowDialog() == true)
                LoadShipments();
        }

        private void DeleteShipment_Click(object sender, RoutedEventArgs e)
        {
            if (selectedShipment == null) return;
            if (MessageBox.Show("Удалить отгрузку?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                db.Shipments.Remove(selectedShipment);
                db.SaveChanges();
                LoadShipments();
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadShipments();
        }
    }
}
