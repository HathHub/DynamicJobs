using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DynamicJobs.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Commands;
using OpenMod.API.Eventing;
using OpenMod.API.Plugins;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Players.Connections.Events;
using OpenMod.Unturned.Plugins;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using Steamworks;

// For more, visit https://openmod.github.io/openmod-docs/devdoc/guides/getting-started.html
namespace DynamicJobs.Commands
{
    [Command("service")]
    public class CommandService : OpenMod.Core.Commands.Command
    {
        private readonly IHudManager m_HudManager;

        public CommandService(IServiceProvider serviceProvider, IHudManager hudManager) : base(serviceProvider)
        {
            m_HudManager = hudManager;
        }

        protected override async Task OnExecuteAsync()
        {
            var user = (UnturnedUser)Context.Actor;
            var service = await Context.Parameters.GetAsync<string>(0);

            await m_HudManager.TryRegisterService(user, service);
            await UniTask.CompletedTask;
        }
    }

}
