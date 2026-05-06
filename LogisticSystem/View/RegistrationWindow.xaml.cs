using LogisticSystem.Data;
using LogisticSystem.Models;
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
using System.Windows.Shapes;

namespace LogisticSystem.View
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }
        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtApprovePassword.Password;

            // Проверка на пустые поля
            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Введите логин", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new LogisticsContext())
            {
                // Проверка, существует ли уже такой логин
                if (db.Users.Any(u => u.Login == login))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Определяем роль: если в БД нет пользователей - назначаем Manager, иначе Client
                string role = db.Users.Any() ? "Client" : "Manager";

                // Хешируем пароль
                string passwordHash = HashPassword(password);

                // Создаём нового пользователя
                var newUser = new User
                {
                    Login = login,
                    PasswordHash = passwordHash,
                    Role = role,
                    RegistrationDate = DateTime.Now
                };

                try
                {
                    db.Users.Add(newUser);
                    db.SaveChanges();

                    if (role == "Client")
                    {
                        var newClient = new Client
                        {
                            Name = login,
                            UserId = newUser.Id
                        };
                        db.Clients.Add(newClient);
                        db.SaveChanges();
                    }

                        string roleMessage = role == "Manager"
                        ? "Вы зарегистрированы как менеджер (первый пользователь системы)"
                        : "Вы зарегистрированы как клиент";

                    MessageBox.Show($"Регистрация успешна!\n{roleMessage}", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Закрываем окно регистрации и открываем окно авторизации
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при регистрации: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
        private void btnExitReg_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
