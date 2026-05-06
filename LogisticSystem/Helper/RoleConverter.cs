using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LogisticSystem.Helper
{
    public class RoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string role = value as string;
            if (role == "Manager") return "Менеджер";
            if (role == "WarehouseKeeper") return "Кладовщик";
            if (role == "Client") return "Клиент";
            return role;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string rus = value as string;
            if (rus == "Менеджер") return "Manager";
            if (rus == "Кладовщик") return "WarehouseKeeper";
            if (rus == "Клиент") return "Client";
            return rus;
        }
    }
}
