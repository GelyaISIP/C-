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
    /// Логика взаимодействия для OrdersManagementWindow.xaml
    /// </summary>
    public partial class OrdersManagementWindow : Window
    {
        private LogisticsContext db;
        private Order selectedOrder;

        public OrdersManagementWindow()
        {
            InitializeComponent();
            db = new LogisticsContext();
            LoadOrders();
        }

        private void LoadOrders()
        {
            var orders = db.Orders.Include("Client").OrderByDescending(o => o.Id).ToList();
            dgOrders.ItemsSource = orders;
        }

        private void dgOrders_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedOrder = dgOrders.SelectedItem as Order;
            btnDelete.IsEnabled = selectedOrder != null;
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadOrdersWithFilters();
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            cbStatusFilter.SelectedIndex = 0;
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;
            tbSearch.Text = "";
            LoadOrdersWithFilters();
        }

        private void LoadOrdersWithFilters()
        {
            var query = db.Orders.Include("Client").AsQueryable();

            // Фильтр по статусу
            var selectedStatus = (cbStatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (selectedStatus != null && selectedStatus != "Все")
            {
                string engStatus = selectedStatus == "Новый" ? "New" : (selectedStatus == "Отгружен" ? "Shipped" : "Completed");
                query = query.Where(o => o.Status == engStatus);
            }

            // Фильтр по датам
            if (dpStartDate.SelectedDate != null)
                query = query.Where(o => o.OrderDate >= dpStartDate.SelectedDate.Value);
            if (dpEndDate.SelectedDate != null)
                query = query.Where(o => o.OrderDate <= dpEndDate.SelectedDate.Value.AddDays(1));

            // Поиск по клиенту (имя или логин)
            if (!string.IsNullOrWhiteSpace(tbSearch.Text))
            {
                string search = tbSearch.Text.Trim();
                query = query.Where(o => o.Client.Name.Contains(search));
            }

            dgOrders.ItemsSource = query.OrderByDescending(o => o.OrderDate).ToList();
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (selectedOrder == null) return;
            if (MessageBox.Show("Удалить заказ? Это удалит также связанные позиции и отгрузки.", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                db.OrderProducts.RemoveRange(db.OrderProducts.Where(op => op.OrderId == selectedOrder.Id));
                db.Shipments.RemoveRange(db.Shipments.Where(s => s.OrderId == selectedOrder.Id));
                db.Orders.Remove(selectedOrder);
                db.SaveChanges();
                LoadOrders();
            }
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = dgOrders.SelectedItem as Order;
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ");
                return;
            }

            var dialog = new Window
            {
                Title = "Изменение статуса заказа",
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Margin = new Thickness(10),
                    Children =
            {
                new TextBlock { Text = "Выберите новый статус:" },
                new ComboBox { Name = "cbStatus", ItemsSource = new[] { "Новый", "Отгружен", "Завершён" }, SelectedIndex = 0 },
                new Button { Content = "Сохранить", Margin = new Thickness(0,10,0,0), HorizontalAlignment = HorizontalAlignment.Right, Width = 80 }
            }
                }
            };
            var btn = (dialog.Content as StackPanel).Children[2] as Button;
            btn.Click += (s, args) =>
            {
                var combo = (dialog.Content as StackPanel).Children[1] as ComboBox;
                string newStatusRus = combo.SelectedItem.ToString();
                string newStatusEng = newStatusRus == "Новый" ? "New" : (newStatusRus == "Отгружен" ? "Shipped" : "Completed");
                selectedOrder.Status = newStatusEng;
                db.SaveChanges();
                LoadOrdersWithFilters();
                dialog.Close();
            };
            dialog.ShowDialog();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }
    }
}
