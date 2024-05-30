using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using SDG.NetTransport;
using OpenMod.API.Ioc;
using OpenMod.Unturned.Users;

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
}