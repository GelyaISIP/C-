using LogisticSystem.Models;
using LogisticSystem.View;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogisticSystem.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User currentUser;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenOrders_Click(object sender, RoutedEventArgs e)
        {
            var orderWin = new OrderWindow();
            orderWin.Show();
        }

        private void OpenShipments_Click(object sender, RoutedEventArgs e)
        {
            var shipmentsWin = new ShipmentsWindow();
            shipmentsWin.Show();
        }

        private void OpenUserManagement_Click(object sender, RoutedEventArgs e)
        {
            var userMgmtWin = new UserManagementWindow();
            userMgmtWin.Show();
        }

        private void OpenReports_Click(object sender, RoutedEventArgs e)
        {
            var reportWin = new ReportWindow();
            reportWin.Show();
        }

        private void OpenAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            var adminPanel = new AdminPanelWindow();
            adminPanel.Show();
        }

        private void OpenOrdersManagement_Click(object sender, RoutedEventArgs e)
        {
            var ordersMgmt = new OrdersManagementWindow();
            ordersMgmt.Show();
        }

        private void OpenOrderWindow_Click(object sender, RoutedEventArgs e)
        {
            var orderWin = new OrderWindow();
            orderWin.Show();
        }
        private void btnAccount_CLick(object sender, RoutedEventArgs e)
        {
            var clientWindow = new ClientAccountWindow(currentUser);
            clientWindow.Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

