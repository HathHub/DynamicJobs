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
using OpenMod.Unturned.Players.UI.Events;
using OpenMod.Unturned.Plugins;
using OpenMod.Unturned.Users;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

// For more, visit https://openmod.github.io/openmod-docs/devdoc/guides/getting-started.html
namespace DynamicJobs.Events
{
    public class UnturnedPlayerButtonClickedEventListener : IEventListener<UnturnedPlayerButtonClickedEvent>
    {
        private readonly IMenuManager m_MenuManager;
        public UnturnedPlayerButtonClickedEventListener(IMenuManager menuManager, IServiceProvider serviceProvider)
        {
            m_MenuManager = menuManager;
        }

        public async Task HandleEventAsync(object sender, UnturnedPlayerButtonClickedEvent @event)
        {
            Player player = @event.Player.Player;
            if(@event.ButtonName == "Menu_Jobs_Close")
            {
                m_MenuManager.CloseJobsMenu(@event.Player.Player);
            }
        }
    }
}
