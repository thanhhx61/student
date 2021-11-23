using System;
using System.Collections.Generic;

#nullable disable

namespace StudentManagement.Entities
{
    public partial class UserSchoolYear
    {
        public string UserId { get; set; }
        public int? SchoolYearId { get; set; }

        public virtual SchoolYear SchoolYear { get; set; }
        public virtual User User { get; set; }
    }
}
