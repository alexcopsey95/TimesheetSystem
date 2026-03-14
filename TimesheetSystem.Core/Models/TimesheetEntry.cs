using System;
using System.Collections.Generic;
using System.Text;

namespace TimesheetSystem.Core.Models
{
    public class TimesheetEntry
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public DateOnly Date { get; set; }
        public decimal Hours { get; set; }
        public string? Description { get; set; }
    }
}
