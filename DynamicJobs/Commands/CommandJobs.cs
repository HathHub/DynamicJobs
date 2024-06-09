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
    [Command("jobsmenu")]
    public class CommandJobs : OpenMod.Core.Commands.Command
    {
        private readonly IMenuManager m_MenuManager;
        public CommandJobs(IServiceProvider serviceProvider, IMenuManager menuManager) : base(serviceProvider)
        {
            m_MenuManager = menuManager;
        }

        protected override async Task OnExecuteAsync()
        {
            UnturnedUser user = (UnturnedUser)Context.Actor;
            await m_MenuManager.OpenJobsMenu(user);
            await UniTask.CompletedTask;
        }
    }
}
