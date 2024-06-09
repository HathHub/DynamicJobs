using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DynamicJobs.Events;
using DynamicJobs.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NuGet.Protocol;
using OpenMod.API.Permissions;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;

// For more, visit https://openmod.github.io/openmod-docs/devdoc/guides/getting-started.html

[assembly: PluginMetadata("DynamicJobs", DisplayName = "DynamicJobs", Author = "Hath.")]

namespace DynamicJobs
{
    public class DynamicJobs : OpenModUnturnedPlugin
    {
        private readonly IConfiguration m_Configuration;
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly ILogger<DynamicJobs> m_Logger;
        private readonly IPermissionRegistry m_PermissionRegistry;

        public JobConfig JobConfig { get; private set; }
        public MenuConfig MenuConfig;
        public JobsStorage JobsStorage { get; set; }
        public ServiceStorage ServiceStorage { get; set; }

        public DynamicJobs(
            IConfiguration configuration,
            IStringLocalizer stringLocalizer,
            ILogger<DynamicJobs> logger,
            IServiceProvider serviceProvider,
            IPermissionRegistry permissionRegistry) : base(serviceProvider)
        {
            m_Configuration = configuration;
            m_StringLocalizer = stringLocalizer;
            m_Logger = logger;
            m_PermissionRegistry = permissionRegistry;
        }
        protected override async UniTask OnLoadAsync()
        {
            // await UniTask.SwitchToMainThread(); uncomment if you have to access Unturned or UnityEngine APIs
            m_Logger.LogInformation("Hello World!");

            JobConfig = new JobConfig();
            m_Configuration.GetSection("UI")
                .Bind(JobConfig);

            MenuConfig = new MenuConfig();
            m_Configuration.GetSection("Menu")
                .Bind(MenuConfig);

            m_Logger.LogInformation(JobConfig.ToJson());
            m_Logger.LogInformation($"Loaded {JobConfig.Jobs.Count()} job counters!");
            ServiceStorage = new ServiceStorage();
            JobsStorage = new JobsStorage();
            foreach(var item in JobConfig.Jobs)
            {
                m_PermissionRegistry.RegisterPermission(this,
                    item.Perm);
                JobsStorage.Jobs.Add(item.Perm, 0);
                Console.WriteLine(item.Perm);
            }
            // await UniTask.SwitchToThreadPool(); // you can switch back to a different thread
        }

        protected override async UniTask OnUnloadAsync()
        {
            // await UniTask.SwitchToMainThread(); uncomment if you have to access Unturned or UnityEngine APIs
            m_Logger.LogInformation(m_StringLocalizer["plugin_events:plugin_stop"]);
        }
    }
}
