using System;
using System.Collections.Generic;
using System.Text;
using TimesheetSystem.Core.Models;

namespace TimesheetSystem.Infrastructure.Data
{
    public static class SeedData
    {
        public static readonly List<User> Users = new()
        {
            new User { Id = 1, Name = "Alice" },
            new User { Id = 2, Name = "Bob" },
            new User { Id = 3, Name = "Charlie" }
        };

        public static readonly List<Project> Projects = new()
        {
            new Project { Id = 1, Name = "Website Redesign" },
            new Project { Id = 2, Name = "Mobile App" },
            new Project { Id = 3, Name = "Internal Tools" },
            new Project { Id = 4, Name = "Client Portal" }
        };
    }
}
