using System;
using System.Collections.Generic;

#nullable disable

namespace StudentManagement.Entities
{
    public partial class ListEvent
    {
        public ListEvent()
        {
            Events = new HashSet<Event>();
        }

        public int ListEventId { get; set; }
        public string ListEventName { get; set; }
        public string Icon { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
