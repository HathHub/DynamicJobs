using System;
using System.Threading.Tasks;
using DynamicJobs.Services;
using OpenMod.API.Eventing;
using OpenMod.API.Permissions;
using SDG.Unturned;

// For more, visit https://openmod.github.io/openmod-docs/devdoc/guides/getting-started.html
namespace DynamicJobs.Events
{
    public class UnturnedUserConnectedEvent : IEventListener<OpenMod.Unturned.Users.Events.UnturnedUserConnectedEvent>
    {
        private readonly IHudManager m_EffectManagerService;
        public UnturnedUserConnectedEvent(IHudManager effectManagerService, IServiceProvider serviceProvider)
        {
            m_EffectManagerService = effectManagerService;
        }

        public async Task HandleEventAsync(object sender, OpenMod.Unturned.Users.Events.UnturnedUserConnectedEvent @event)
        {
            Player player = @event.User.Player.Player;
            await m_EffectManagerService.LoadJobCounter(player.channel.owner.transportConnection, player.skills.experience.ToString());
            await m_EffectManagerService.RegisterCount(@event.User);
        }
    }
}
