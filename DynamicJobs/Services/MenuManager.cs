using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NuGet.Protocol;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Users;
using SDG.NetTransport;
using SDG.Unturned;

// For more, visit https://openmod.github.io/openmod-docs/devdoc/guides/getting-started.html
namespace DynamicJobs.Services
{
    [Service]
    public interface IMenuManager
    {
        Task OpenJobsMenu(UnturnedUser user);
        Task CloseJobsMenu(Player user);
    }

    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class MenuManager : IMenuManager
    {
        private readonly ILogger<MenuManager> m_Logger;
        private readonly IServiceProvider m_ServiceProvider;
        private readonly DynamicJobs m_plugin;
        private readonly IConfiguration m_Configuration;
        private readonly IPermissionChecker m_PermissionChecker;
        public MenuManager(IPermissionChecker permissionChecker, IConfiguration configuration, DynamicJobs plugin, IServiceProvider serviceProvider, ILogger<MenuManager> logger)
        {
            m_ServiceProvider = serviceProvider;
            m_Logger = logger;
            m_plugin = plugin;
            m_Configuration = configuration;
            m_PermissionChecker = permissionChecker;
        }

        public async Task OpenJobsMenu(UnturnedUser user)
        {
            await UniTask.SwitchToMainThread();
            EffectManager.sendUIEffect(22001, 14, user.Player.Player.channel.owner.transportConnection, true);
            user.Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
            user.Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, true);
            await DisplayJobs(user.Player.Player.channel.owner.transportConnection);
        }
        public async Task CloseJobsMenu(Player user)
        {
            await UniTask.SwitchToMainThread();
            EffectManager.askEffectClearByID(22001, user.channel.owner.transportConnection);
            user.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
            user.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, false);

        }
        public async Task DisplayJobs(ITransportConnection transport)
        {
            await UniTask.SwitchToMainThread();
            var jobs = m_plugin.MenuConfig.Jobs;
            Console.WriteLine(jobs.ToJson());
            for (int i = 0; i < jobs.Count; i++)
            {
                EffectManager.sendUIEffectVisibility(14,
                    transport,
                    true,
                    $"Canvas/GameObject/uGUI Box/GameObject/uGUI ScrollView/Viewport/Content/Menu_Jobs_{i}",
                    true);
                string count = "";
                if (m_plugin.JobsStorage.Jobs[jobs[i].Perm] > 0)
                {
                    count = $"{m_plugin.JobsStorage.Jobs[jobs[i].Perm]} online";
                }
                else
                {
                    count = $"<color=#D83939>0 online</color>";
                }
                EffectManager.sendUIEffectText(14,
                    transport,
                    true,
                    $"Canvas/GameObject/uGUI Box/GameObject/uGUI ScrollView/Viewport/Content/Menu_Jobs_{i}/RawImage/OnlineAmount",
                    count);
                EffectManager.sendUIEffectImageURL(14,
                    transport,
                    true,
                    $"Canvas/GameObject/uGUI Box/GameObject/uGUI ScrollView/Viewport/Content/Menu_Jobs_{i}/RawImage/JobIcon",
                    jobs[i].ImageUrl);
                EffectManager.sendUIEffectText(14,
                    transport,
                    true,
                    $"Canvas/GameObject/uGUI Box/GameObject/uGUI ScrollView/Viewport/Content/Menu_Jobs_{i}/GameObject/Name",
                    jobs[i].Name);
                EffectManager.sendUIEffectText(14,
                    transport,
                    true,
                    $"Canvas/GameObject/uGUI Box/GameObject/uGUI ScrollView/Viewport/Content/Menu_Jobs_{i}/GameObject/ShortDesc",
                    jobs[i].ShortDesc);
            }
        }
    }
}

