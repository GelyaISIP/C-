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
        private User currentUser;
        private Client currentClient;

        public OrderWindow()
        {
            InitializeComponent();
            db = new LogisticsContext();
            LoadCurrentUser();
            dgOrderItems.ItemsSource = orderItems;
        }

        private void LoadCurrentUser()
        {
            currentUser = Application.Current.Properties["CurrentUser"] as User;
            if (currentUser == null)
            {
                MessageBox.Show("Ошибка авторизации. Закройте окно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddProductToOrderWindow(db);
            if (addWindow.ShowDialog() == true)
                orderItems.Add(addWindow.SelectedOrderItem);
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            if (orderItems.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один товар", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int clientId;

            if (currentUser.Role == "Client")
            {
                var client = db.Clients.FirstOrDefault(c => c.UserId == currentUser.Id);
                if (client == null)
                {
                    MessageBox.Show("Для вашего аккаунта не найден клиент. Обратитесь к администратору.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                clientId = client.Id;
            }
            else // Manager (или другие роли, которые могут оформлять заказы)
            {
                // Автоматическое создание служебного клиента, если его ещё нет
                var serviceClient = db.Clients.FirstOrDefault(c => c.Name == "Служебный (менеджер)");
                if (serviceClient == null)
                {
                    serviceClient = new Client { Name = "Служебный (менеджер)" };
                    db.Clients.Add(serviceClient);
                    db.SaveChanges();
                }
                clientId = serviceClient.Id;
            }

            var order = new Order
            {
                ClientId = clientId,
                OrderDate = dpOrderDate.SelectedDate ?? DateTime.Now,
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
                            var stock = context.Stocks.FirstOrDefault(s => s.ProductId == item.ProductId && s.WarehouseId == item.WarehouseId);
                            if (stock == null || stock.Quantity < item.Quantity)
                            {
                                MessageBox.Show($"Недостаточно товара '{item.ProductName}' на складе '{item.WarehouseName}'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                transaction.Rollback();
                                return;
                            }
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
                        MessageBox.Show("Заказ успешно оформлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при сохранении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void btnAccount_Click(object sender, EventArgs e)
        {
            var clientWindow = new ClientAccountWindow(currentUser);
            clientWindow.Show();
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
