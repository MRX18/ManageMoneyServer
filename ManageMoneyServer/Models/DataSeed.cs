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
                context.Portfolios.RemoveRange(context.Portfolios);
                context.Database.ExecuteSqlInterpolated($"DBCC CHECKIDENT ('[ManageMoney].[dbo].[Portfolios]', RESEED, 0);");

                context.Users.RemoveRange(context.Users);

                context.AssetTypes.RemoveRange(context.AssetTypes);
                context.Database.ExecuteSqlInterpolated($"DBCC CHECKIDENT ('[ManageMoney].[dbo].[AssetTypes]', RESEED, 0);");

                context.AssetTypeInfos.RemoveRange(context.AssetTypeInfos);
                context.Database.ExecuteSqlInterpolated($"DBCC CHECKIDENT ('[ManageMoney].[dbo].[AssetTypeInfos]', RESEED, 0);");

                context.Sources.RemoveRange(context.Sources);
                context.Database.ExecuteSqlInterpolated($"DBCC CHECKIDENT ('[ManageMoney].[dbo].[Sources]', RESEED, 0);");

                context.Languages.RemoveRange(context.Languages);
                context.Database.ExecuteSqlInterpolated($"DBCC CHECKIDENT ('[ManageMoney].[dbo].[Languages]', RESEED, 0);");

                context.SaveChanges();
                #endregion

                #region User
                string email = "test@gmial.com";
                var createUser = Task.Run(async () => await userManager.CreateAsync(new User
                {
                    UserName = "Test",
                    Email = email,
                }, "Test1234."));
                createUser.Wait();

                var taskUser = Task.Run(async () => await userManager.FindByEmailAsync(email));
                taskUser.Wait();

                User user = taskUser.Result;
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

                #region Asset Type Info
                AssetTypeInfo[] assetTypeInfos = new AssetTypeInfo[]
                {
                    new AssetTypeInfo
                    {
                        Language = languages[0],
                        Name = "Cryptocurrency"
                    },
                    new AssetTypeInfo
                    {
                        Language = languages[0],
                        Name = "Fiat",
                    },
                    new AssetTypeInfo
                    {
                        Language = languages[0],
                        Name = "Stocks",
                    }
                };
                context.AssetTypeInfos.AddRange(assetTypeInfos);
                #endregion

                #region Asset type
                AssetType[] assetTypes = new AssetType[] {
                    new AssetType
                    {
                        Value = (int)AssetTypes.Cryptocurrency,
                        Infos = new List<AssetTypeInfo> { assetTypeInfos[0] }
                    },
                    new AssetType
                    {
                        Value = (int)AssetTypes.Fiat,
                        Infos = new List<AssetTypeInfo> { assetTypeInfos[1] }
                    },
                    new AssetType
                    {
                        Value = (int)AssetTypes.Stocks,
                        Infos = new List<AssetTypeInfo> { assetTypeInfos[2] }
                    }
                };
                context.AssetTypes.AddRange(assetTypes);
                #endregion

                #region Portfolio
                Portfolio[] portfolio = new Portfolio[] {
                    new Portfolio
                    {
                        Name = "Portfolio crypto #1",
                        Description = "Description crypto #1",
                        AssetTypes = new List<AssetType> { assetTypes[0] },
                        UserId = user.Id
                    },
                    new Portfolio
                    {
                        Name = "Portfolio stocks #2",
                        Description = "Description stocks #2",
                        AssetTypes = new List<AssetType> { assetTypes[2] },
                        UserId = user.Id
                    }
                };
                context.Portfolios.AddRange(portfolio);
                #endregion

                #region Source asset
                List<Source> sources = new List<Source>();
                foreach (ISource source in sourceService)
                {
                    AssetType assetType = assetTypes.First(a => source.Types.Contains(a.Type));
                    sources.Add(new Source
                    {
                        Name = source.SourceName,
                        Slug = source.Slug,
                        AssetTypes = new List<AssetType> { assetType },
                    });
                }
                context.Sources.AddRange(sources);
                #endregion

                context.SaveChanges();
            }
        }
    }
}
