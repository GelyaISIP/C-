using LogisticSystem.Data;
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
    /// Логика взаимодействия для ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        private LogisticsContext db;

        public ReportWindow()
        {
            InitializeComponent();
            db = new LogisticsContext();
            // Устанавливаем даты по умолчанию
            dpStart.SelectedDate = DateTime.Now.AddMonths(-1);
            dpEnd.SelectedDate = DateTime.Now;
        }

        private void CbReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgReport != null)
            {
                dgReport.ItemsSource = null;
                dgReport.Columns.Clear();
            }
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            string selectedType = (cbReportType.SelectedItem as ComboBoxItem)?.Content.ToString();
            switch (selectedType)
            {
                case "Отгрузки":
                    ShowShipmentsReport();
                    break;
                case "Заказы":
                    ShowOrdersReport();
                    break;
                case "Пользователи":
                    ShowUsersReport();
                    break;
            }
        }

        private void ShowShipmentsReport()
        {
            if (!dpStart.SelectedDate.HasValue || !dpEnd.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите период");
                return;
            }
            var start = dpStart.SelectedDate.Value;
            var end = dpEnd.SelectedDate.Value.AddDays(1);

            var query = from s in db.Shipments
                        where s.ShipmentDate >= start && s.ShipmentDate < end
                        group s by s.Warehouse.Name into g
                        select new { WarehouseName = g.Key, Count = g.Count() };

            dgReport.Columns.Clear();
            dgReport.AutoGenerateColumns = false;
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Склад", Binding = new System.Windows.Data.Binding("WarehouseName") });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Количество отгрузок", Binding = new System.Windows.Data.Binding("Count") });
            dgReport.ItemsSource = query.ToList();
        }

        private void ShowOrdersReport()
        {
            if (!dpStart.SelectedDate.HasValue || !dpEnd.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите период", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var start = dpStart.SelectedDate.Value;
            var end = dpEnd.SelectedDate.Value.AddDays(1);

            var query = from o in db.Orders
                        where o.OrderDate >= start && o.OrderDate < end
                        select new
                        {
                            CustomerName = (o.Client.Name == "Служебный (менеджер)") ? "Менеджер" : o.Client.Name,
                            o.Status
                        };

            var grouped = query.GroupBy(x => x.CustomerName)
                               .Select(g => new { CustomerName = g.Key, OrderCount = g.Count() })
                               .ToList();

            dgReport.Columns.Clear();
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Заказчик", Binding = new Binding("CustomerName") });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Количество заказов", Binding = new Binding("OrderCount") });
            dgReport.ItemsSource = grouped;
        }

        private void ShowUsersReport()
        {
            var query = from u in db.Users
                        group u by u.Role into g
                        select new { Role = g.Key, Count = g.Count() };

            dgReport.Columns.Clear();
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Роль", Binding = new Binding("Role") });
            dgReport.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new Binding("Count") });
            dgReport.ItemsSource = query.ToList();
        }
    }
}
