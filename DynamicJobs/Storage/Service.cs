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
    public class ServiceStorage
    {
        public Dictionary<CSteamID, List<string>> Services { get; private set; } = new Dictionary<CSteamID, List<string>>();

        public Task AddService(CSteamID steamId, string service)
        {
            if (!Services.ContainsKey(steamId))
            {
                Services[steamId] = new List<string>();
            }

            if (!Services[steamId].Contains(service))
            {
                Services[steamId].Add(service);
            }

            return Task.CompletedTask;
        }

        public Task RemoveService(CSteamID steamId, string service)
        {
            if (Services.ContainsKey(steamId))
            {
                Services[steamId].Remove(service);

                if (Services[steamId].Count == 0)
                {
                    Services.Remove(steamId);
                }
            }

            return Task.CompletedTask;
        }

        public Task<bool> HasService(CSteamID steamId, string service)
        {
            return Task.FromResult(Services.ContainsKey(steamId) && Services[steamId].Contains(service));
        }
    }
}

