using System;
using System.Collections.Generic;
using System.Text;
using TimesheetSystem.Core.Interfaces;
using TimesheetSystem.Core.Models;

namespace TimesheetSystem.Core.Services
{
    public class TimesheetService : ITimesheetService
    {
        private readonly ITimesheetRepository _repository;

        public TimesheetService(ITimesheetRepository repository)
        {
            _repository = repository;
        }

        public TimesheetEntry AddEntry(TimesheetEntry entry)
        {
            if (entry.Date > DateOnly.FromDateTime(DateTime.Today))
                throw new ArgumentException("Date cannot be in the future.");

            if (entry.Date < DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)))
                throw new ArgumentException("Date cannot be over a month in the past.");

            if (entry.Hours <= 0)
                throw new ArgumentException("Hours must be greater than zero.");

            if (entry.Hours > 24)
                throw new ArgumentException("Hours cannot exceed 24 in a single day.");

            if (_repository.GetAll().Any(e =>
                e.UserId == entry.UserId &&
                e.ProjectId == entry.ProjectId &&
                e.Date == entry.Date))
                throw new InvalidOperationException("An entry already exists for this user, project, and date.");

            entry.Id = Guid.NewGuid();
            return _repository.Add(entry);
        }

        public TimesheetEntry UpdateEntry(TimesheetEntry entry)
        {
            if (entry.Date > DateOnly.FromDateTime(DateTime.Today))
                throw new ArgumentException("Date cannot be in the future.");

            if (entry.Date < DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)))
                throw new ArgumentException("Date cannot be over a month in the past.");

            if (entry.Hours <= 0)
                throw new ArgumentException("Hours must be greater than zero.");

            if (entry.Hours > 24)
                throw new ArgumentException("Hours cannot exceed 24 in a single day.");

            if (_repository.GetAll().Any(e =>
                e.UserId == entry.UserId &&
                e.ProjectId == entry.ProjectId &&
                e.Date == entry.Date &&
                e.Id != entry.Id))
                throw new InvalidOperationException("An entry already exists for this user, project, and date.");

            return _repository.Update(entry);
        }

        public void DeleteEntry(Guid id)
        {
            _repository.Delete(id);
        }

        public IEnumerable<TimesheetEntry> GetEntriesForUserAndWeek(int userId, DateOnly weekStart)
        {
            var weekEnd = weekStart.AddDays(6);
            return _repository.GetAll()
                .Where(e => e.UserId == userId && e.Date >= weekStart && e.Date <= weekEnd);
        }

        public decimal GetTotalHoursPerProject(int userId, int projectId, DateOnly weekStart)
        {
            return GetEntriesForUserAndWeek(userId, weekStart)
                .Where(e => e.ProjectId == projectId)
                .Sum(e => e.Hours);
        }
    }
}
