using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using StudentManagement.Entities;
using StudentManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Models.Students
{
    public class EditEvents
    {
        public int EventId { get; set; }
        public string Activities { get; set; }
        public string Act { get; set; }
        public string PowerExerted { get; set; }
        public string PowerDev { get; set; }
        public string Think { get; set; }
        public int? ListEventId { get; set; }
        public EventStatus? Status { get; set; }
        public int SchoolYearId { get; set; }

        public User User { get; set; }
        public string UserId { get; set; }
        public ICollection<Entities.Message> Messages { get; set; }
        public ListEvent ListEvent { get; set; }
    }
}
