using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

// For more, visit https://openmod.github.io/openmod-docs/devdoc/guides/getting-started.html
namespace DynamicJobs.Events
{
    public class JobConfig
    {
        public string ServerName { get; set; }
        public List<Job> Jobs { get; set; }

        public class Job
        {
            public string Id { get; set; }
            public string Perm { get; set; }
            public string Url { get; set; }
            public string Color { get; set; }
        }
    }
}
