using ManageMoneyServer.Api.Exchanges;
using ManageMoneyServer.Models;
using ManageMoneyServer.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManageMoneyServer.Services.Interfaces
{
    public interface ISourceService : IEnumerable<ISource>
    {
        public ISource this[string index] { get; }
        public bool Contains(string slug, AssetTypes type);
        public bool Contains(string slug);
        public Task<Dictionary<string, SynchronizedViewModel<Asset>>> SynchronizeAssets();
        public Task<Dictionary<string, SynchronizedViewModel<Asset>>> SynchronizeAssets(AssetTypes type);
        public Task<SynchronizedViewModel<Asset>> SynchronizeAssets(string source);
    }
}
