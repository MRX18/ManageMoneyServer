using ManageMoneyServer.Models;
using System.Collections.Generic;

namespace ManageMoneyServer.Services.Interfaces
{
    public interface IContextService
    {
        public Language DefaultLanguage { get; }
        public Language CurrentLanguage { get; }
        public List<AssetType> AssetTypes { get; }
        public User User { get; }
    }
}
