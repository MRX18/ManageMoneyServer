using ManageMoneyServer.Models;

namespace ManageMoneyServer.Services.Interfaces
{
    public interface IContextService
    {
        public Language DefaultLanguage { get; }
        public Language CurrentLanguage { get; }
    }
}
