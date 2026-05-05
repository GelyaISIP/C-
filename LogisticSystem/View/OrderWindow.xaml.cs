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
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        private LogisticsContext db;
        private ObservableCollection<OrderItem> orderItems = new ObservableCollection<OrderItem>();
        public OrderWindow()
        {
            InitializeComponent();
            db = new LogisticsContext();
            LoadClients();
            dgOrderItems.ItemsSource = orderItems;
        }
        private void LoadClients()
        {
            var clients = db.Clients.OrderBy(c => c.Name).ToList();
            cbClients.ItemsSource = clients;
            if (clients.Any()) cbClients.SelectedIndex = 0;
        }
        private void btnAddProduct_Click(Object sender, RoutedEventArgs e)
        {
            var addProductWindow = new AddProductToOrderWindow(db);
            if (addProductWindow.ShowDialog() == true)
            {
                var newItem = addProductWindow.SelectedOrderItem;
                if (newItem != null)
                    orderItems.Add(newItem);
            }
        }
        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            if (cbClients.SelectedValue == null)
            {
                MessageBox.Show("Выберите клиента", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (dpOrderDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату заказа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (orderItems.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один товар", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int clientId = (int)cbClients.SelectedValue;
            DateTime orderDate = dpOrderDate.SelectedDate.Value;

            var order = new Order
            {
                ClientId = clientId,
                OrderDate = orderDate,
                Status = "New"
            };

            using (var context = new LogisticsContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Orders.Add(order);
                        context.SaveChanges();

                        foreach (var item in orderItems)
                        {
                            // Проверяем, достаточно ли товара на выбранном складе
                            var stock = context.Stocks.FirstOrDefault(s => s.ProductId == item.ProductId && s.WarehouseId == item.WarehouseId);
                            if (stock == null || stock.Quantity < item.Quantity)
                            {
                                MessageBox.Show($"Недостаточно товара '{item.ProductName}' на складе", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                transaction.Rollback();
                                return;
                            }

                            // Резервируем (уменьшаем количество)
                            stock.Quantity -= item.Quantity;

                            context.OrderProducts.Add(new OrderProduct
                            {
                                OrderId = order.Id,
                                ProductId = item.ProductId,
                                Quantity = item.Quantity
                            });
                        }
                        context.SaveChanges();
                        transaction.Commit();
                        MessageBox.Show("Заказ успешно создан", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Ошибка при сохранении заказа: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        public class OrderItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public int WarehouseId { get; set; }
            public string WarehouseName { get; set; }
        }

    }
}
