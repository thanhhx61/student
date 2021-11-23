using System;
using System.Collections.Generic;

#nullable disable

namespace StudentManagement.Entities
{
    public partial class BellNotification
    {
        public BellNotification()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool? IsRead { get; set; }
        public int? NotifiId { get; set; }

        public virtual Message Notifi { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
