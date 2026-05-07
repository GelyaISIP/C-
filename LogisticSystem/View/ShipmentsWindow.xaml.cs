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
            cbWarehouseFilter.ItemsSource = db.Warehouses.ToList();
        }

        private void LoadShipments()
        {
            var query = db.Shipments
                         .Include("Order")
                         .Include("Order.Client")
                         .Include("Warehouse")
                         .AsQueryable();

            if (cbWarehouseFilter.SelectedValue != null && (int)cbWarehouseFilter.SelectedValue > 0)
                query = query.Where(s => s.WarehouseId == (int)cbWarehouseFilter.SelectedValue);

            if (!string.IsNullOrWhiteSpace(tbClientSearch.Text))
            {
                string search = tbClientSearch.Text.Trim();
                query = query.Where(s => s.Order.Client.Name.Contains(search));
            }

            string selectedStatus = (cbStatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (selectedStatus != null && selectedStatus != "Все")
            {
                string engStatus = selectedStatus == "Новый" ? "New" : (selectedStatus == "Отгружен" ? "Shipped" : "Completed");
                query = query.Where(s => s.Order.Status == engStatus);
            }

            if (dpStartDate.SelectedDate != null)
                query = query.Where(s => s.PlannedShipmentDate >= dpStartDate.SelectedDate);
            if (dpEndDate.SelectedDate != null)
                query = query.Where(s => s.PlannedShipmentDate <= dpEndDate.SelectedDate.Value.AddDays(1));

            dgShipments.ItemsSource = query.OrderByDescending(s => s.PlannedShipmentDate).ToList();
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadShipments();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            cbWarehouseFilter.SelectedValue = null;
            tbClientSearch.Text = "";
            cbStatusFilter.SelectedIndex = 0;
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;
            LoadShipments();
        }

        private void dgShipments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedShipment = dgShipments.SelectedItem as Shipment;
            btnEdit.IsEnabled = selectedShipment != null;
            btnDelete.IsEnabled = selectedShipment != null;
            btnConfirm.IsEnabled = selectedShipment != null && selectedShipment.ShipmentDate == null;
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

        private void ConfirmShipment_Click(object sender, RoutedEventArgs e)
        {
            if (selectedShipment == null || selectedShipment.ShipmentDate != null) return;
            selectedShipment.ShipmentDate = DateTime.Now;
            var order = selectedShipment.Order;
            if (order != null && order.Status != "Completed")
                order.Status = "Shipped";
            db.SaveChanges();
            LoadShipments();
        }

        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            var currentUser = Application.Current.Properties["CurrentUser"] as User;
            if (currentUser == null)
            {
                MessageBox.Show("Ошибка авторизации. Пожалуйста, войдите заново.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var clientWindow = new ClientAccountWindow(currentUser);
            clientWindow.Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}