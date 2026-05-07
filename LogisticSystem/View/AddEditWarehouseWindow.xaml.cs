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
    /// Логика взаимодействия для AddEditWarehouseWindow.xaml
    /// </summary>
    public partial class AddEditWarehouseWindow : Window
    {
        private LogisticsContext db;
        private Warehouse currentWarehouse;
        private bool isEdit;

        public AddEditWarehouseWindow(LogisticsContext context, Warehouse warehouse = null)
        {
            InitializeComponent();
            db = context;
            currentWarehouse = warehouse ?? new Warehouse();
            isEdit = warehouse != null;

            if (isEdit)
            {
                Title = "Редактирование склада";
                tbName.Text = currentWarehouse.Name;
                tbLocation.Text = currentWarehouse.Location;
            }
            else
            {
                Title = "Новый склад";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                MessageBox.Show("Введите название склада", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            currentWarehouse.Name = tbName.Text.Trim();
            currentWarehouse.Location = tbLocation.Text.Trim();

            if (!isEdit)
                db.Warehouses.Add(currentWarehouse);

            db.SaveChanges();
            DialogResult = true;
            Close();
        }
    }
}
