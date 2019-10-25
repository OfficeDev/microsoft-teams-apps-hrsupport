// <copyright file="Startup.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR
{
    using System.IO;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.IdentityModel.Protocols;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.Teams.Apps.AskHR.Bots;
    using Microsoft.Teams.Apps.AskHR.Common.Providers;
    using Microsoft.Teams.Apps.AskHR.Helper;
    using Microsoft.Teams.Apps.AskHR.Services;

    /// <summary>
    /// This a Startup class for this Bot.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">Startup Configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets Configurations Interfaces.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"> Service Collection Interface.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var storageConnectionString = this.Configuration["StorageConnectionString"];
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<Common.Providers.IConfigurationProvider>(new Common.Providers.ConfigurationProvider(storageConnectionString));
            services.AddHttpClient();
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();
            services.AddSingleton<ITicketsProvider>(new TicketsProvider(storageConnectionString));
            services.AddSingleton<IHelpDataProvider>(new HelpDataProvider(storageConnectionString));
            services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();
            services.AddSingleton(new MicrosoftAppCredentials(this.Configuration["MicrosoftAppId"], this.Configuration["MicrosoftAppPassword"]));
            services.AddTransient<IBot>((provider) => new AskHRBot(
                provider.GetRequiredService<TelemetryClient>(),
                provider.GetRequiredService<Common.Providers.IConfigurationProvider>(),
                provider.GetRequiredService<IHelpDataProvider>(),
                provider.GetRequiredService<IQnAMakerFactory>(),
                provider.GetRequiredService<MessagingExtension>(),
                this.Configuration["AppBaseUri"],
                this.Configuration["TenantId"],
                provider.GetRequiredService<MicrosoftAppCredentials>(),
                provider.GetRequiredService<ITicketsProvider>()));
            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<IQnAMakerFactory, QnAMakerFactory>();
            services.AddSingleton<ISearchService, SearchService>();
            services.AddSingleton<MessagingExtension>();
            services.AddMemoryCache();

            services.AddSingleton<IConfigurationManager<OpenIdConnectConfiguration>>(
                new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"https://login.microsoftonline.com/{this.Configuration["TenantId"]}/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever()));

            services.AddSingleton<IAuthManager>((provider) => new AuthManager(
                provider.GetRequiredService<IConfigurationManager<OpenIdConnectConfiguration>>(),
                this.Configuration["TenantId"],
                this.Configuration["MicrosoftAppId"]));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application Builder.</param>
        /// <param name="env">Hosting Environment.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // This will add "Libs" as another valid static content location
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                     Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/dist")),
                RequestPath = new PathString("/wwwroot/dist")
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Help}/{action=Index}/{id?}");
            });
        }
    }
}