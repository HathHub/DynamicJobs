using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NuGet.Protocol;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.Unturned.Users;
using SDG.NetTransport;
using SDG.Unturned;

// For more, visit https://openmod.github.io/openmod-docs/devdoc/guides/getting-started.html
namespace DynamicJobs.Services
{
    [Service]
    public interface IHudManager
    {
        Task LoadJobCounter(ITransportConnection transport, string experience);
        Task RegisterCount(UnturnedUser user);
        Task DeRegisterCount(UnturnedUser user);
        Task UpdateExperience(ITransportConnection transport, string experience);
    }

    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class HudManager : IHudManager
    {
        private readonly ILogger<HudManager> m_Logger;
        private readonly IServiceProvider m_ServiceProvider;
        private readonly DynamicJobs m_plugin;
        private readonly IConfiguration m_Configuration;
        private readonly IPermissionChecker m_PermissionChecker;
        public HudManager(IPermissionChecker permissionChecker, IConfiguration configuration, DynamicJobs plugin, IServiceProvider serviceProvider, ILogger<HudManager> logger)
        {
            m_ServiceProvider = serviceProvider;
            m_Logger = logger;
            m_plugin = plugin;
            m_Configuration = configuration;
            m_PermissionChecker = permissionChecker;
        }

        public async Task RegisterCount(UnturnedUser user)
        {
            var jobs = m_plugin.JobConfig.Jobs;
            Console.WriteLine(m_plugin.JobsStorage.Jobs.ToJson());

            var steamId = user.SteamId;

            if (!m_plugin.JobsStorage.PlayerJobs.ContainsKey(steamId))
            {
                m_plugin.JobsStorage.PlayerJobs[steamId] = new List<string>();
            }

            for (int i = 0; i < jobs.Count; i++)
            {
                if (await m_PermissionChecker.CheckPermissionAsync(user, jobs[i].Perm) == PermissionGrantResult.Grant)
                {
                    await m_plugin.JobsStorage.AddJobCount(jobs[i].Perm);

                    if (!m_plugin.JobsStorage.PlayerJobs[steamId].Contains(jobs[i].Perm))
                    {
                        m_plugin.JobsStorage.PlayerJobs[steamId].Add(jobs[i].Perm);
                    }
                }
            }

            Console.WriteLine(m_plugin.JobsStorage.Jobs.ToJson());
            Console.WriteLine($"User {steamId} permissions: {string.Join(", ", m_plugin.JobsStorage.PlayerJobs[steamId])}");

            await ReloadUIForAllPlayers();
        }

        public async Task DeRegisterCount(UnturnedUser user)
        {
            var steamId = user.SteamId;

            if (m_plugin.JobsStorage.PlayerJobs.ContainsKey(steamId))
            {
                var permissions = m_plugin.JobsStorage.PlayerJobs[steamId];

                foreach (var perm in permissions)
                {
                    await m_plugin.JobsStorage.RemoveJobCount(perm);
                }

                m_plugin.JobsStorage.PlayerJobs.Remove(steamId);

                Console.WriteLine(m_plugin.JobsStorage.Jobs.ToJson());

                await ReloadUIForAllPlayers();
            }
            else
            {
                Console.WriteLine($"User {steamId} not found in permissions dictionary.");
            }
        }

        public async Task LoadJobCounter(ITransportConnection transport, string experience)
        {
            EffectManager.sendUIEffect(22000, 13, transport, true);
            EffectManager.sendUIEffectText(13,
                transport,
                true,
                "Canvas/GameObject/GameObject (1)/UI_JobCount_ServerName",
                m_plugin.JobConfig.ServerName);
            EffectManager.sendUIEffectText(13,
                transport,
                true,
                "Canvas/GameObject/GameObject (1)/uGUI Box/UI_Experience",
                experience);
            var jobs = m_plugin.JobConfig.Jobs;
            for (int i = 0; i < jobs.Count; i++)
            {
                EffectManager.sendUIEffectImageURL(13,
                transport,
                true,
                $"Canvas/GameObject/GameObject (1)/uGUI Box/UI_JobCount_{i}/UI_JobCount_Image",
                jobs[i].Url,
                true, true);
            }
        }
        public async Task UpdateExperience(ITransportConnection transport, string experience)
        {
            EffectManager.sendUIEffectText(13,
                transport,
                true,
                "Canvas/GameObject/GameObject (1)/uGUI Box/UI_Experience",
                experience);
        }

        public async Task ReloadUIForAllPlayers()
        {
            var jobs = m_plugin.JobConfig.Jobs;
            foreach (var provider in Provider.clients)
            {
                for (int i = 0; i < jobs.Count; i++)
                {
                    EffectManager.sendUIEffectVisibility(13,
                        provider.transportConnection,
                        true,
                        $"Canvas/GameObject/GameObject (1)/uGUI Box/UI_JobCount_{i}",
                        true);
                    EffectManager.sendUIEffectText(13,
                        provider.transportConnection,
                        true,
                        $"Canvas/GameObject/GameObject (1)/uGUI Box/UI_JobCount_{i}/RawImage/UI_JobCount_Count",
                        m_plugin.JobsStorage.Jobs[jobs[i].Perm].ToString());
                }
            }
        }
    }
}

