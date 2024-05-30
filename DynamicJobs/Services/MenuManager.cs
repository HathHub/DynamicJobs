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
    public interface IMenuManager
    {
        Task OpenJobsMenu(ITransportConnection transport);
    }

    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class MenuManager : IMenuManager
    {
        private readonly ILogger<HudManager> m_Logger;
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

        public async Task OpenJobsMenu(ITransportConnection transport)
        {
            EffectManager.sendUIEffect(22001, 14, transport, true);
        }
    }
}

