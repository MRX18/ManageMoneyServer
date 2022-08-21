using ManageMoneyServer.Api.Exchanges;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ManageMoneyServer.Services.Interfaces;

namespace ManageMoneyServer.Models
{
    public static class DataSeed
    {
        public static void Seed(IServiceCollection services)
        {
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ApplicationContext context = serviceProvider.GetRequiredService<ApplicationContext>();
            if (context.Languages.Count() == 0)
            {
                ISourceService sourceService = serviceProvider.GetRequiredService<ISourceService>();
                UserManager<User> userManager = serviceProvider.GetRequiredService<UserManager<User>>();

                #region Clear all tables
                context.Users.RemoveRange(context.Users);

                context.AssetTypes.RemoveRange(context.AssetTypes);
                context.Database.ExecuteSqlInterpolated($"DBCC CHECKIDENT ('[ManageMoney].[dbo].[AssetTypes]', RESEED, 0);");

                context.Sources.RemoveRange(context.Sources);
                context.Database.ExecuteSqlInterpolated($"DBCC CHECKIDENT ('[ManageMoney].[dbo].[Sources]', RESEED, 0);");

                context.Languages.RemoveRange(context.Languages);
                context.Database.ExecuteSqlInterpolated($"DBCC CHECKIDENT ('[ManageMoney].[dbo].[Languages]', RESEED, 0);");

                context.SaveChanges();
                #endregion

                #region User
                var createUser = Task.Run(async () => await userManager.CreateAsync(new User
                {
                    UserName = "Test",
                    Email = "test@gmial.com"
                }, "Test1234."));
                createUser.Wait();
                #endregion

                #region Language
                Language[] languages = new Language[] {
                    new Language
                    {
                        Name = "English",
                        Symbol = "EN"
                    },
                    new Language
                    {
                        Name = "Українська",
                        Symbol = "UK"
                    }
                };
                context.Languages.AddRange(languages);
                #endregion

                #region Asset type
                AssetType[] assetTypes = new AssetType[] {
                    new AssetType
                    {
                        Language = languages[0],
                        Name = "Cryptocurrency",
                        Slug = "cryptocurrency",
                        Value = (int)AssetTypes.Cryptocurrency
                    },
                    new AssetType
                    {
                        Language = languages[0],
                        Name = "Fiat",
                        Slug = "fiat",
                        Value = (int)AssetTypes.Fiat
                    },
                    new AssetType
                    {
                        Language = languages[0],
                        Name = "Stocks",
                        Slug = "stocks",
                        Value = (int)AssetTypes.Stocks
                    }
                };
                context.AssetTypes.AddRange(assetTypes);
                #endregion

                #region Source asset
                List<Source> sources = new List<Source>();
                foreach (ISource source in sourceService)
                {
                    sources.Add(new Source
                    {
                        Name = source.SourceName,
                        Slug = source.Slug,
                        AssetTypes = new List<AssetType> { assetTypes[0] },
                        Language = languages[0]
                    });
                }
                context.AddRange(sources);
                #endregion

                context.SaveChanges();
            }
        }
    }
}
