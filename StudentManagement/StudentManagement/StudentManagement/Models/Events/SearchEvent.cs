using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagement.DBContexts;
using StudentManagement.Entities;
using StudentManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Models.Events
{
    public class SearchEvent
    {
        public List<Event> events { get; set; }
        public SelectList SelectLists { get; set; }
        public string YearName { get; set; }
        public int SchoolYearId { get; set; }
        public string ListEvent { get; set; }
        public bool checkbox1 { get; set; }
        public bool checkbox2 { get; set; }
        public bool checkbox3 { get; set; }
    }
}
