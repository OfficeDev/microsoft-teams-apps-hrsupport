// <copyright file="AutofacConfig.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR.Configuration
{
    using System.Configuration;
    using System.Management;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Web.Mvc;
    using Autofac;
    using Autofac.Integration.Mvc;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker;
    using Microsoft.Teams.Apps.AskHR.Common.Providers;
    using ConfigurationManager = System.Configuration.ConfigurationManager;

    /// <summary>
    /// Autofac configuration
    /// </summary>
    public class AutofacConfig
    {
        /// <summary>
        /// Register Autofac dependencies
        /// </summary>
        /// <returns>Autofac container</returns>
        public static IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            var storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

            builder.Register(c => new ConfigurationProvider(storageConnectionString))
                .As<IConfigurationProvider>()
                .SingleInstance();

            var qnaMakerClient = new QnAMakerClient(
                new ApiKeyServiceClientCredentials(
                ConfigurationManager.AppSettings["QnAMakerSubscriptionKey"]))
            { Endpoint = StripRouteFromQnAMakerEndpoint(ConfigurationManager.AppSettings["QnAMakerApiEndpointUrl"]) };

            builder.Register(c => qnaMakerClient)
                .As<IQnAMakerClient>()
                .SingleInstance();

            builder.Register(c => new HelpDataProvider(storageConnectionString))
                .As<IHelpDataProvider>()
                .SingleInstance();

            var telemetryClient = new TelemetryClient()
            {
                InstrumentationKey = ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"]
            };

            builder.Register(c => telemetryClient)
                .As<TelemetryClient>()
                .SingleInstance();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            return container;
        }

        // Strip the route suffix from the endpoint
        private static string StripRouteFromQnAMakerEndpoint(string endpoint)
        {
            const string apiRoute = "/qnamaker/v4.0";

            if (endpoint.EndsWith(apiRoute, System.StringComparison.OrdinalIgnoreCase))
            {
                endpoint = endpoint.Substring(0, endpoint.Length - apiRoute.Length);
            }

            return endpoint;
        }
    }
}