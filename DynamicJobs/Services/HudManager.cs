using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DynamicJobs.Storage;
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
        Task TryRegisterService(UnturnedUser user, string service);
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

        public async Task TryRegisterService(UnturnedUser user, string service)
        {
            var jobs = m_plugin.JobConfig.Jobs;
            var steamId = user.SteamId;

            // Find the job in the configuration
            var job = jobs.FirstOrDefault(j => j.Perm.Equals(service, StringComparison.OrdinalIgnoreCase) && j.RequiresService);
            if (job == null)
            {
                m_Logger.LogWarning($"Job {service} not found in configuration.");
                return;
            }

            // Check if the user has the required permission for the job
            var permissionResult = await m_PermissionChecker.CheckPermissionAsync(user, job.Perm);
            if (permissionResult != PermissionGrantResult.Grant)
            {
                await user.PrintMessageAsync($"You don't have permission for job {service}.");
                return;
            }

            // Check if the player already has the service
            if (m_plugin.ServiceStorage.Services.TryGetValue(steamId, out var userServices) && userServices.Contains(service))
            {
                // The player already has the service, so remove it
                userServices.Remove(service);
                if (userServices.Count == 0)
                {
                    m_plugin.ServiceStorage.Services.Remove(steamId);
                }

                await m_plugin.JobsStorage.RemoveJobCount(job.Perm);

                // Remove the job from the user's list of jobs
                if (m_plugin.JobsStorage.PlayerJobs.ContainsKey(steamId))
                {
                    m_plugin.JobsStorage.PlayerJobs[steamId].Remove(job.Perm);
                    if (m_plugin.JobsStorage.PlayerJobs[steamId].Count == 0)
                    {
                        m_plugin.JobsStorage.PlayerJobs.Remove(steamId);
                    }
                }

                m_Logger.LogInformation($"Job {service} deregistered for user {user.DisplayName}.");
            }
            else
            {
                // The player does not have the service, so add it
                if (!m_plugin.ServiceStorage.Services.ContainsKey(steamId))
                {
                    m_plugin.ServiceStorage.Services[steamId] = new List<string>();
                }
                m_plugin.ServiceStorage.Services[steamId].Add(service);

                if (!m_plugin.JobsStorage.PlayerJobs.ContainsKey(steamId))
                {
                    m_plugin.JobsStorage.PlayerJobs[steamId] = new List<string>();
                }
                await m_plugin.JobsStorage.AddJobCount(job.Perm);

                if (!m_plugin.JobsStorage.PlayerJobs[steamId].Contains(job.Perm))
                {
                    m_plugin.JobsStorage.PlayerJobs[steamId].Add(job.Perm);
                }

                m_Logger.LogInformation($"Job {service} registered for user {user.DisplayName}.");
            }

            // Reload the UI for all players to reflect the updated job counts
            await ReloadUIForAllPlayers();
        }

        public async Task RegisterCount(UnturnedUser user)
        {
            var jobs = m_plugin.JobConfig.Jobs;

            var steamId = user.SteamId;

            if (!m_plugin.JobsStorage.PlayerJobs.ContainsKey(steamId))
            {
                m_plugin.JobsStorage.PlayerJobs[steamId] = new List<string>();
            }

            for (int i = 0; i < jobs.Count; i++)
            {
                if (jobs[i].RequiresService) continue;

                if (await m_PermissionChecker.CheckPermissionAsync(user, jobs[i].Perm) == PermissionGrantResult.Grant)
                {
                    await m_plugin.JobsStorage.AddJobCount(jobs[i].Perm);

                    if (!m_plugin.JobsStorage.PlayerJobs[steamId].Contains(jobs[i].Perm))
                    {
                        m_plugin.JobsStorage.PlayerJobs[steamId].Add(jobs[i].Perm);
                    }
                }
            }
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
            await UniTask.SwitchToMainThread();
            foreach (var provider in Provider.clients)
            {
                for (int i = 0; i < jobs.Count; i++)
                {
                    EffectManager.sendUIEffectVisibility(13,
                        provider.transportConnection,
                        true,
                        $"Canvas/GameObject/GameObject (1)/uGUI Box/UI_JobCount_{i}",
                        true);
                    string count = "";
                    if(m_plugin.JobsStorage.Jobs[jobs[i].Perm] > 0)
                    {
                        count = $"<color={jobs[i].Color}>{m_plugin.JobsStorage.Jobs[jobs[i].Perm]}</color>";
                    }
                    else
                    {
                        count = $"<color=#D83939>0</color>";
                    }
                    EffectManager.sendUIEffectText(13,
                        provider.transportConnection,
                        true,
                        $"Canvas/GameObject/GameObject (1)/uGUI Box/UI_JobCount_{i}/RawImage/UI_JobCount_Count",
                        count);
                }
            }
        }
    }
}

