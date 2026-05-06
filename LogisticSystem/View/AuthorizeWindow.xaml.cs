using LogisticSystem.Data;
using LogisticSystem.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogisticSystem.View
{
    /// <summary>
    /// Логика взаимодействия для AuthorizeWindow.xaml
    /// </summary>
    public partial class AuthorizeWindow : Window
    {
        public AuthorizeWindow()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new LogisticsContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Login == login);
                if (user == null)
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string hash = HashPassword(password);
                if (user.PasswordHash != hash)
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Сохраняем текущего пользователя и его роль 
                Application.Current.Properties["CurrentUser"] = user;

                switch (user.Role)
                {
                    case "Manager":
                        var mainWin = new MainWindow();
                        mainWin.Show();
                        break;
                    case "WarehouseKeeper":
                        var shipmentsWin = new ShipmentsWindow();
                        shipmentsWin.Show();
                        break;
                    case "Client":
                        var orderWin = new OrderWindow();
                        orderWin.Show();
                        break;
                    default:
                        MessageBox.Show("Неизвестная роль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }
                this.Close();
            }
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            var registrationWin = new RegistrationWindow();
            registrationWin.Show();
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Система логистики\n\nРазработчики: Донова Ангелина и Туркина Екатерина\n\nГруппа: ИСиП-23-1п");
        }
    }
}
