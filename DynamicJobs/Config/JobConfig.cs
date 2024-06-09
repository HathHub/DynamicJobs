using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Commands;
using OpenMod.API.Eventing;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Players.Connections.Events;
using OpenMod.Unturned.Plugins;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using Steamworks;

namespace DynamicJobs.Events
{
    public class JobConfig
    {
        public string ServerName { get; set; }
        public List<Job> Jobs { get; set; }

        public class Job
        {
            public string Perm { get; set; }
            public string Url { get; set; }
            public string Color { get; set; }
            public bool RequiresService { get; set; }
        }
    }

    public class MenuConfig
    {
        public List<MenuJob> Jobs { get; set; }

        public class MenuJob
        {
            public string Name { get; set; }
            public string Perm { get; set; }
            public string ShortDesc { get; set; }
            public string LongDesc { get; set; }
            public string ImageUrl { get; set; }
            public SalaryConfig Salary { get; set; }

            public class SalaryConfig
            {
                public bool Enabled { get; set; }
                public int Time { get; set; }
                public int Amount { get; set; }
            }
        }
    }
}
