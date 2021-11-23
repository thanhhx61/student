using StudentManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Models
{
    public class Student
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Roles { get; set; }
        public string StudentCode { get; set; }
        public string SchoolYears { get; set; }
        public ICollection<UserSchoolYear> UserSchoolYears { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<Skill> Skills { get; set; }

    }
}
