using System;
using System.Collections.Generic;

#nullable disable

namespace StudentManagement.Entities
{
    public partial class Message
    {
        public Message()
        {
            BellNotifications = new HashSet<BellNotification>();
        }

        public int MessagesId { get; set; }
        public string Content { get; set; }
        public DateTime? Timestamp { get; set; }
        public int? EventId { get; set; }
        public string UserId { get; set; }
        public int? SkillId { get; set; }

        public virtual Event Event { get; set; }
        public virtual Skill Skill { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<BellNotification> BellNotifications { get; set; }
    }
}
