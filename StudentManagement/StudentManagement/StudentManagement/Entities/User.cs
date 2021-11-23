using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

#nullable disable

namespace StudentManagement.Entities
{
    public partial class User : IdentityUser
    {
        public User()
        {
            Events = new HashSet<Event>();
            Messages = new HashSet<Message>();
            UserSchoolYears = new HashSet<UserSchoolYear>();
        }
        public string Phone { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public string Avatar { get; set; }
        public int? ScholasticId { get; set; }
        public int? ParentId { get; set; }
        public int? BellNotifiId { get; set; }
        public string StudentCode { get; set; }

        public virtual BellNotification BellNotifi { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<UserSchoolYear> UserSchoolYears { get; set; }
    }
}
