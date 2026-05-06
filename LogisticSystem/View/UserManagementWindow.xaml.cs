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
        private User currentUser; // текущий авторизованный менеджер

        public UserManagementWindow()
        {
            InitializeComponent();
            db = new LogisticsContext();
            LoadUsers();
            currentUser = Application.Current.Properties["CurrentUser"] as User;
        }

        private void LoadUsers()
        {
            dgUsers.ItemsSource = db.Users.ToList();
        }

        private void SaveRole_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button?.Tag as User;
            if (user == null) return;

            // Защита от изменения первого менеджера и себя
            if (user.Id == 1)
            {
                MessageBox.Show("Невозможно изменить роль первого менеджера.", "Запрещено", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var currentUser = Application.Current.Properties["CurrentUser"] as User;
            if (currentUser != null && user.Id == currentUser.Id)
            {
                MessageBox.Show("Вы не можете изменить свою собственную роль.", "Запрещено", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Поскольку ComboBox уже обновил user.Role через TwoWay binding, просто сохраняем
            db.SaveChanges();
            LoadUsers();
            MessageBox.Show("Роль успешно изменена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            db = new LogisticsContext();
            LoadUsers();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Вспомогательный метод для поиска ComboBox в строке DataGrid
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is T t) return t;
                var result = FindVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}