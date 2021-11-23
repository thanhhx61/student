using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Models.Accounts
{
    public class Login
    {
        [Required(ErrorMessage = "必須項目が未入力")]
        [MaxLength(50)]
        public string Username { get; set; }
        [Required(ErrorMessage = "必須項目が未入力")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
