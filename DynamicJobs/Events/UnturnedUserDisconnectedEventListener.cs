using System;
using System.Threading.Tasks;
using DynamicJobs.Services;
using OpenMod.API.Eventing;
using OpenMod.API.Permissions;
using OpenMod.Unturned.Players.Inventory.Events;
using OpenMod.Unturned.Users.Events;
using SDG.Unturned;

// For more, visit https://openmod.github.io/openmod-docs/devdoc/guides/getting-started.html
namespace DynamicJobs.Events
{
    public class UnturnedUserDisconnectedEventListener : IEventListener<UnturnedUserDisconnectedEvent>
    {
        private readonly IHudManager m_EffectManagerService;
        public UnturnedUserDisconnectedEventListener(IHudManager effectManagerService, IServiceProvider serviceProvider)
        {
            m_EffectManagerService = effectManagerService;
        }

        public async Task HandleEventAsync(object sender, UnturnedUserDisconnectedEvent @event)
        {
            await m_EffectManagerService.DeRegisterCount(@event.User);
        }
    }
}
