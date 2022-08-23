using ManageMoneyServer.Models;
using ManageMoneyServer.Repositories;
using ManageMoneyServer.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManageMoneyServer.Services
{
    public class ContextService : IContextService
    {
        private const string DEFAULTLANGUAGESYMBOL = "EN";
        private Language defaultLanguage = null;
        public Language DefaultLanguage => defaultLanguage;
        public Language currentLanguage = null;
        public Language CurrentLanguage => currentLanguage;
        private List<AssetType> assetTypes = null;
        public List<AssetType> AssetTypes => assetTypes;
        private IHttpContextAccessor HttpContext { get; set; }
        private IRepository<Language> LanguageRepository { get; set; }
        private IRepository<AssetType> AssetTypesRepository { get; set; }
        public ContextService(IRepository<Language> languageRepository,
            IRepository<AssetType> assetTypesRepository,
            IHttpContextAccessor httpContext)
        {
            HttpContext = httpContext;
            LanguageRepository = languageRepository;
            AssetTypesRepository = assetTypesRepository;

            Task.Run(() => Init()).Wait();
        }
        private async Task Init()
        {
            IRequestCultureFeature rqf = HttpContext.HttpContext.Request.HttpContext.Features.Get<IRequestCultureFeature>();

            currentLanguage = await LanguageRepository.FindAsync(l => l.Symbol == rqf.RequestCulture.Culture.Name.ToUpper());
            defaultLanguage = await LanguageRepository.FindAsync(l => l.Symbol == DEFAULTLANGUAGESYMBOL);

            assetTypes = await AssetTypesRepository.GetListAsync();
        }
    }
}
