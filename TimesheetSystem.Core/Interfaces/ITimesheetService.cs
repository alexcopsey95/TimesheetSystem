using System;
using System.Collections.Generic;
using System.Text;
using TimesheetSystem.Core.Models;

namespace TimesheetSystem.Core.Interfaces
{
    public interface ITimesheetService
    {
        TimesheetEntry AddEntry(TimesheetEntry entry);
        TimesheetEntry UpdateEntry(TimesheetEntry entry);
        void DeleteEntry(Guid id);
        IEnumerable<TimesheetEntry> GetEntriesForUserAndWeek(int userId, DateOnly weekStart);
        decimal GetTotalHoursPerProject(int userId, int projectId, DateOnly weekStart);
    }
}
