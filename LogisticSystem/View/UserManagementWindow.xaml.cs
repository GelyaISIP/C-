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
    /// Логика взаимодействия для UserManagementWindow.xaml
    /// </summary>
    public partial class UserManagementWindow : Window
    {
        private LogisticsContext db;

        public UserManagementWindow()
        {
            InitializeComponent();
            db = new LogisticsContext();
            LoadUsers();
        }

        private void LoadUsers()
        {
            var users = db.Users.ToList();
            dgUsers.ItemsSource = users;
        }

        private void ChangeRole_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button == null) return;
            string newRole = button.Tag.ToString();
            var user = (button.DataContext as User);
            if (user == null) return;

            user.Role = newRole;
            db.SaveChanges();
            LoadUsers(); // обновить таблицу
            MessageBox.Show($"Роль пользователя {user.Login} изменена на {newRole}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
