using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        [StringLength(256)]         // Указываем максимальную длину (это ключевой момент)
        [Index(IsUnique = true)]    // Создаём уникальный индекс для логина
        public string Login { get; set; }

        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public virtual Client Client { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
