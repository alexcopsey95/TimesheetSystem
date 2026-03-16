using TimesheetSystem.Core.Models;
using TimesheetSystem.Core.Services;
using TimesheetSystem.Core.Interfaces;
using Moq;

namespace TimesheetSystem.Tests
{
    public class TimesheetServiceTests
    {
        private readonly Mock<ITimesheetRepository> _mockRepo;
        private readonly TimesheetService _service;
        private readonly List<TimesheetEntry> _entries;

        public TimesheetServiceTests()
        {
            _entries = new List<TimesheetEntry>();
            _mockRepo = new Mock<ITimesheetRepository>();

            _mockRepo.Setup(r => r.GetAll()).Returns(() => _entries);
            _mockRepo.Setup(r => r.Add(It.IsAny<TimesheetEntry>()))
                .Callback<TimesheetEntry>(e => _entries.Add(e))
                .Returns((TimesheetEntry e) => e);

            _service = new TimesheetService(_mockRepo.Object);
        }

        #region AddEntry Tests

        [Fact]
        public void AddEntry_ValidEntry_AddsSuccessfully()
        {
            var entry = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Hours = 7.5m
            };

            var result = _service.AddEntry(entry);

            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Single(_entries);
        }

        [Fact]
        public void AddEntry_ZeroHours_ThrowsArgumentException()
        {
            var entry = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Hours = 0
            };

            Assert.Throws<ArgumentException>(() => _service.AddEntry(entry));
        }

        [Fact]
        public void AddEntry_NegativeHours_ThrowsArgumentException()
        {
            var entry = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Hours = -1
            };

            Assert.Throws<ArgumentException>(() => _service.AddEntry(entry));
        }

        [Fact]
        public void AddEntry_HoursExceed24_ThrowsArgumentException()
        {
            var entry = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Hours = 25
            };

            Assert.Throws<ArgumentException>(() => _service.AddEntry(entry));
        }

        [Fact]
        public void AddEntry_FutureDate_ThrowsArgumentException()
        {
            var entry = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                Hours = 7.5m
            };

            Assert.Throws<ArgumentException>(() => _service.AddEntry(entry));
        }

        [Fact]
        public void AddEntry_DateOverAMonthAgo_ThrowsArgumentException()
        {
            var entry = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1).AddDays(-1)),
                Hours = 7.5m
            };

            Assert.Throws<ArgumentException>(() => _service.AddEntry(entry));
        }

        [Fact]
        public void AddEntry_DuplicateEntry_ThrowsInvalidOperationException()
        {
            var entry = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Hours = 7.5m
            };

            _service.AddEntry(entry);

            var duplicate = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Hours = 3m
            };

            Assert.Throws<InvalidOperationException>(() => _service.AddEntry(duplicate));
        }

        [Fact]
        public void AddEntry_SameUserDifferentProject_AddsSuccessfully()
        {
            var entry1 = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Hours = 4m
            };

            var entry2 = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 2,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Hours = 4m
            };

            _service.AddEntry(entry1);
            _service.AddEntry(entry2);

            Assert.Equal(2, _entries.Count);
        }

        #endregion

        #region UpdateEntry Tests

        [Fact]
        public void UpdateEntry_ValidUpdate_UpdatesSuccessfully()
        {
            var entry = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Hours = 7.5m
            };
            _service.AddEntry(entry);

            _mockRepo.Setup(r => r.Update(It.IsAny<TimesheetEntry>()))
                .Callback<TimesheetEntry>(updated =>
                {
                    var existing = _entries.First(e => e.Id == updated.Id);
                    existing.Hours = updated.Hours;
                    existing.Description = updated.Description;
                })
                .Returns((TimesheetEntry e) => e);

            entry.Hours = 6m;
            var result = _service.UpdateEntry(entry);

            Assert.Equal(6m, _entries.First().Hours);
        }

        [Fact]
        public void UpdateEntry_FutureDate_ThrowsArgumentException()
        {
            var entry = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                Hours = 7.5m
            };

            Assert.Throws<ArgumentException>(() => _service.UpdateEntry(entry));
        }

        [Fact]
        public void UpdateEntry_DateOverAMonthAgo_ThrowsArgumentException()
        {
            var entry = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1).AddDays(-1)),
                Hours = 7.5m
            };

            Assert.Throws<ArgumentException>(() => _service.UpdateEntry(entry));
        }

        [Fact]
        public void UpdateEntry_ZeroHours_ThrowsArgumentException()
        {
            var entry = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Hours = 0
            };

            Assert.Throws<ArgumentException>(() => _service.UpdateEntry(entry));
        }

        [Fact]
        public void UpdateEntry_HoursExceed24_ThrowsArgumentException()
        {
            var entry = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Hours = 25m
            };

            Assert.Throws<ArgumentException>(() => _service.UpdateEntry(entry));
        }

        [Fact]
        public void UpdateEntry_DuplicateEntry_ThrowsInvalidOperationException()
        {
            var entry1 = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Hours = 7.5m
            };
            _service.AddEntry(entry1);

            var entry2 = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
                Hours = 4m
            };
            _service.AddEntry(entry2);

            entry2.Date = entry1.Date;

            Assert.Throws<InvalidOperationException>(() => _service.UpdateEntry(entry2));
        }

        #endregion

        #region DeleteEntry Tests

        [Fact]
        public void DeleteEntry_ValidId_DeletesSuccessfully()
        {
            var entry = new TimesheetEntry
            {
                UserId = 1,
                ProjectId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Hours = 7.5m
            };
            _service.AddEntry(entry);

            _mockRepo.Setup(r => r.Delete(It.IsAny<Guid>()))
                .Callback<Guid>(id => _entries.RemoveAll(e => e.Id == id));

            _service.DeleteEntry(entry.Id);

            Assert.Empty(_entries);
        }

        [Fact]
        public void DeleteEntry_InvalidId_ThrowsKeyNotFoundException()
        {
            _mockRepo.Setup(r => r.Delete(It.IsAny<Guid>()))
                .Throws<KeyNotFoundException>();

            Assert.Throws<KeyNotFoundException>(() => _service.DeleteEntry(Guid.NewGuid()));
        }

        #endregion

        #region GetEntriesForUserAndWeek Tests

        [Fact]
        public void GetEntriesForUserAndWeek_ReturnsCorrectEntries()
        {
            var weekStart = new DateOnly(2025, 1, 6); // A Monday

            _entries.AddRange(new[]
            {
        new TimesheetEntry { Id = Guid.NewGuid(), UserId = 1, ProjectId = 1, Date = weekStart, Hours = 7.5m },
        new TimesheetEntry { Id = Guid.NewGuid(), UserId = 1, ProjectId = 2, Date = weekStart.AddDays(2), Hours = 4m },
        new TimesheetEntry { Id = Guid.NewGuid(), UserId = 1, ProjectId = 1, Date = weekStart.AddDays(7), Hours = 6m }, // next week
        new TimesheetEntry { Id = Guid.NewGuid(), UserId = 2, ProjectId = 1, Date = weekStart, Hours = 5m } // different user
    });

            var result = _service.GetEntriesForUserAndWeek(1, weekStart);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void GetEntriesForUserAndWeek_NoEntries_ReturnsEmpty()
        {
            var weekStart = new DateOnly(2025, 1, 6);

            var result = _service.GetEntriesForUserAndWeek(1, weekStart);

            Assert.Empty(result);
        }

        [Fact]
        public void GetEntriesForUserAndWeek_IncludesWeekEndDate()
        {
            var weekStart = new DateOnly(2025, 1, 6);

            _entries.Add(new TimesheetEntry
            {
                Id = Guid.NewGuid(),
                UserId = 1,
                ProjectId = 1,
                Date = weekStart.AddDays(6), // Sunday
                Hours = 4m
            });

            var result = _service.GetEntriesForUserAndWeek(1, weekStart);

            Assert.Single(result);
        }

        [Fact]
        public void GetEntriesForUserAndWeek_ExcludesEntryBeforeWeekStart()
        {
            var weekStart = new DateOnly(2025, 1, 6);

            _entries.Add(new TimesheetEntry
            {
                Id = Guid.NewGuid(),
                UserId = 1,
                ProjectId = 1,
                Date = weekStart.AddDays(-1), // day before week start
                Hours = 4m
            });

            var result = _service.GetEntriesForUserAndWeek(1, weekStart);

            Assert.Empty(result);
        }

        #endregion

        #region GetTotalHoursPerProject Tests

        [Fact]
        public void GetTotalHoursPerProject_ReturnsSumOfHours()
        {
            var weekStart = new DateOnly(2025, 1, 6);

            _entries.AddRange(new[]
            {
        new TimesheetEntry { Id = Guid.NewGuid(), UserId = 1, ProjectId = 1, Date = weekStart, Hours = 4m },
        new TimesheetEntry { Id = Guid.NewGuid(), UserId = 1, ProjectId = 1, Date = weekStart.AddDays(1), Hours = 3.5m },
        new TimesheetEntry { Id = Guid.NewGuid(), UserId = 1, ProjectId = 2, Date = weekStart, Hours = 5m } // different project
    });

            var result = _service.GetTotalHoursPerProject(1, 1, weekStart);

            Assert.Equal(7.5m, result);
        }

        [Fact]
        public void GetTotalHoursPerProject_NoEntries_ReturnsZero()
        {
            var weekStart = new DateOnly(2025, 1, 6);

            var result = _service.GetTotalHoursPerProject(1, 1, weekStart);

            Assert.Equal(0m, result);
        }

        [Fact]
        public void GetTotalHoursPerProject_ExcludesOtherUsers()
        {
            var weekStart = new DateOnly(2025, 1, 6);

            _entries.Add(new TimesheetEntry
            {
                Id = Guid.NewGuid(),
                UserId = 2,
                ProjectId = 1,
                Date = weekStart,
                Hours = 8m
            });

            var result = _service.GetTotalHoursPerProject(1, 1, weekStart);

            Assert.Equal(0m, result);
        }

        #endregion
    }
}
