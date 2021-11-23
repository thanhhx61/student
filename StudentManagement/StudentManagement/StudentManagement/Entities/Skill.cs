using System;
using System.Collections.Generic;

#nullable disable

namespace StudentManagement.Entities
{
    public partial class Skill
    {
        public Skill()
        {
            Messages = new HashSet<Message>();
        }

        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string Style { get; set; }
        public string Tags { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}
