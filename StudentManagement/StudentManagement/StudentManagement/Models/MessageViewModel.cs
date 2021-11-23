using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Models
{
    public class MessageViewModel
    {
        [Required]
        public string Content { get; set; }
        public string From { get; set; }
        public int MessagesId { get; set; }
        public DateTime? Timestamp { get; set; }
        public string Timestamps
        {
            get
            {
                return Timestamp != null ? Timestamp.Value.ToString("hh:mm tt dd/MM/yyyy") : string.Empty;
            }
        }
        public int? EventId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public bool IsMe { get; set; }
        //[Required]
        //public string Room { get; set; }
        public string Avatar { get; set; }
    }
}
