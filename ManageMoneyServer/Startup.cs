using ManageMoneyServer.Filters;
using ManageMoneyServer.Models;
using ManageMoneyServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ManageMoneyServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            //services.AddControllers(o => {
            //    o.Filters.Add(typeof(LocalizerActionFilter));
            //}).AddDataAnnotationsLocalization();

            services.AddControllers().AddDataAnnotationsLocalization();

            services.AddCors();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ManageMoneyServer", Version = "v1" });
            });

            services.AddDbContext<ApplicationContext>(option => {
                option.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]);
            });

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) 
                .AddJwtBearer(option => {
                    option.RequireHttpsMetadata = false;
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        // ????????, ????? ?? ?????????????? ???????? ??? ????????? ??????
                        ValidateIssuer = true,
                        // ??????, ?????????????? ????????
                        ValidIssuer = Configuration["AuthOptions:ISSUER"],
                        // ????? ?? ?????????????? ??????????? ??????
                        ValidateAudience = true,
                        // ????????? ??????????? ??????
                        ValidAudience = Configuration["AuthOptions:AUDIENCE"],
                        // ????? ?? ?????????????? ????? ?????????????
                        ValidateLifetime = true,
                        // ????????? ????? ????????????
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["AuthOptions:KEY"])),
                        // ????????? ????? ????????????
                        ValidateIssuerSigningKey = true
                    };
                });

            services.AddSingleton<ResourceService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ManageMoneyServer v1"));
            }

            List<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en"),
                new CultureInfo("uk")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions { 
                DefaultRequestCulture = new RequestCulture("uk"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(options => {
                //options.AllowAnyOrigin();
                options.WithOrigins("http://localhost:4200");
                options.AllowCredentials();
                options.AllowAnyHeader();
                options.AllowAnyMethod();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
