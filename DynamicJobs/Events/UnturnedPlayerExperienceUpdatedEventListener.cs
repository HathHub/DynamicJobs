using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DynamicJobs.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Commands;
using OpenMod.API.Eventing;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Players.Connections.Events;
using OpenMod.Unturned.Players.Skills.Events;
using OpenMod.Unturned.Plugins;
using OpenMod.Unturned.Users;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

// For more, visit https://openmod.github.io/openmod-docs/devdoc/guides/getting-started.html
namespace DynamicJobs.Events
{
    public class UnturnedPlayerExperienceUpdatedEventListener : IEventListener<UnturnedPlayerExperienceUpdatedEvent>
    {
        private readonly IHudManager m_EffectManagerService;
        public UnturnedPlayerExperienceUpdatedEventListener(IHudManager effectManagerService, IServiceProvider serviceProvider)
        {
            m_EffectManagerService = effectManagerService;
        }

        public async Task HandleEventAsync(object sender, UnturnedPlayerExperienceUpdatedEvent @event)
        {
            Player player = @event.Player.Player;
            await m_EffectManagerService.UpdateExperience(player.channel.owner.transportConnection, player.skills.experience.ToString());
        }
    }
}
