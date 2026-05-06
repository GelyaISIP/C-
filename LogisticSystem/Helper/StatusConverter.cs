using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LogisticSystem.Helper
{

    /// <summary>
    /// Класс контейнер для того, чтобы переводить латинские статусы на кирилицу
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (status == "New") return "Новый";
            if (status == "Shipped") return "Отгружен";
            if (status == "Completed") return "Завершён";
            return status ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string rus = value as string;
            if (rus == "Новый") return "New";
            if (rus == "Отгружен") return "Shipped";
            if (rus == "Завершён") return "Completed";
            return rus ?? "";
        }
    }
}
