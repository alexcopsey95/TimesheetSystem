using System;
using System.Collections.Generic;
using System.Text;
using TimesheetSystem.Core.Interfaces;
using TimesheetSystem.Core.Models;

namespace TimesheetSystem.Infrastructure.Repositories
{
    public class TimesheetRepository : ITimesheetRepository
    {
        private readonly List<TimesheetEntry> _entries = new();

        public TimesheetEntry Add(TimesheetEntry entry)
        {
            _entries.Add(entry);
            return entry;
        }

        public TimesheetEntry Update(TimesheetEntry entry)
        {
            var existing = _entries.FirstOrDefault(e => e.Id == entry.Id)
                ?? throw new KeyNotFoundException($"Entry with ID {entry.Id} not found.");

            existing.UserId = entry.UserId;
            existing.ProjectId = entry.ProjectId;
            existing.Date = entry.Date;
            existing.Hours = entry.Hours;
            existing.Description = entry.Description;

            return existing;
        }

        public void Delete(Guid id)
        {
            var entry = _entries.FirstOrDefault(e => e.Id == id)
                ?? throw new KeyNotFoundException($"Entry with ID {id} not found.");

            _entries.Remove(entry);
        }

        public IEnumerable<TimesheetEntry> GetAll()
        {
            return _entries.AsReadOnly();
        }
    }
}
