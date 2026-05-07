using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using LogisticSystem.Data;
using LogisticSystem.Models;

namespace LogisticSystem.View
{
    public partial class ClientAccountWindow : Window
    {
        private User currentUser;

        public ClientAccountWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            this.Title = $"Личный кабинет - {currentUser.Login}";
        }

        private void btnSavePass_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = txtNewPassword.Password.Trim();
            string confirmPassword = txtApprovNewPass.Password.Trim();

            // Проверка на пустые поля
            if (string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Введите новый пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Подтвердите новый пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка длины пароля
            if (newPassword.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка совпадения паролей
            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Обновление пароля в бд
            try
            {
                using (var db = new LogisticsContext())
                {
                    var userFromDb = db.Users.FirstOrDefault(u => u.Id == currentUser.Id);

                    if (userFromDb == null)
                    {
                        MessageBox.Show("Пользователь не найден", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string newPasswordHash = HashPassword(newPassword);

                    userFromDb.PasswordHash = newPasswordHash;

                    // Сохранение
                    db.SaveChanges();

                    MessageBox.Show("Пароль успешно изменён!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении пароля: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}