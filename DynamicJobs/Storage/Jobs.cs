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
namespace DynamicJobs.Storage
{
    public class JobsStorage
    {
        public Dictionary<CSteamID, List<string>> PlayerJobs = new Dictionary<CSteamID, List<string>>();

        public Dictionary<string, int> Jobs = new Dictionary<string, int>();

        public Task AddJobCount(string name)
        {
            Jobs[name]++;
            return Task.CompletedTask;
        }
        public Task RemoveJobCount(string name)
        {
            Jobs[name]--;
            return Task.CompletedTask;
        }


    }
}
