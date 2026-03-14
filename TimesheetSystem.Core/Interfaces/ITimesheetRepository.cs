using System;
using System.Collections.Generic;
using System.Text;
using TimesheetSystem.Core.Models;

namespace TimesheetSystem.Core.Interfaces
{
    public interface ITimesheetRepository
    {
        TimesheetEntry Add(TimesheetEntry entry);
        TimesheetEntry Update(TimesheetEntry entry);
        void Delete(Guid id);
        IEnumerable<TimesheetEntry> GetAll();
    }
}
