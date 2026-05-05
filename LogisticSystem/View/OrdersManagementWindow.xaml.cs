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
            // хз на подумать
            //btnEdit.IsEnabled = selectedOrder != null;
            btnDelete.IsEnabled = selectedOrder != null;
        }

        // Хз пока, добавлять ли возможность редактировать заказы клиентов или пока удаление оставить. Потом решу
        //private void EditOrder_Click(object sender, RoutedEventArgs e)
        //{
        //    if (selectedOrder == null) return;
        //    // Можно открыть окно редактирования заказа (изменение статуса, состава товаров)
        //    var editWin = new EditOrderWindow(db, selectedOrder);
        //    if (editWin.ShowDialog() == true)
        //        LoadOrders();
        //}

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

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }
    }
}
