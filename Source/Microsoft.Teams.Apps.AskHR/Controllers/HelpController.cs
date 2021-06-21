// <copyright file="HelpController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Teams.Apps.AskHR.Common.Models;
    using Microsoft.Teams.Apps.AskHR.Common.Providers;
    using Microsoft.Teams.Apps.AskHR.Helper;

    /// <summary>
    /// This is a Static tab controller class which will be used to display Help
    /// details in the bot tab.
    /// </summary>
    public class HelpController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IHelpDataProvider helpDataProvider;
        private readonly TelemetryClient telemetryClient;
        private readonly IAuthManager authManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpController"/> class.
        /// HelpController
        /// </summary>
        /// <param name="config">application config settings</param>
        /// <param name="helpDataProvider">help data provider instance</param>
        /// <param name="authManager">auth manager for token management instance</param>
        /// <param name="telemetryClient">telemetry instance</param>
        public HelpController(IConfiguration config, IHelpDataProvider helpDataProvider, IAuthManager authManager, TelemetryClient telemetryClient)
        {
            this.configuration = config;
            this.helpDataProvider = helpDataProvider;
            this.telemetryClient = telemetryClient;
            this.authManager = authManager;
        }

        /// <summary>
        /// Display help tab.
        /// </summary>
        /// <param name="locale">Current locale of user</param>
        /// <returns>Help tab view</returns>
        public ActionResult Index(string locale)
        {
            this.ViewBag.clientId = this.configuration["MicrosoftAppId"];
            this.ViewBag.locale = this.FilterSelectedLocale(locale);

            return this.View();
        }

        /// <summary>
        /// Microsoft Teams Sign-In Flow
        /// </summary>
        /// <returns>AAD token</returns>
        public IActionResult SignIn()
        {
            this.ViewBag.clientId = this.configuration["MicrosoftAppId"];

            return this.View();
        }

        /// <summary>
        /// Microsoft Teams Sign-In Flow
        /// </summary>
        /// <returns>AAD token</returns>
        public IActionResult RenderData()
        {
            this.ViewBag.clientId = this.configuration["MicrosoftAppId"];
            return this.View();
        }

        /// <summary>
        /// method to get all the data from client
        /// </summary>
        /// <param name="authorization">authorization</param>
        /// <returns>JSON response</returns>
        [HttpPost]
        public async Task<ActionResult> GetHelpTiles([FromHeader] string authorization)
        {
            try
            {
                string token = string.Empty;
                if (string.IsNullOrEmpty(authorization) || authorization.StartsWith("Bearer") == false)
                {
                    return this.Unauthorized();
                }

                token = authorization.Replace("Bearer ", string.Empty);

                // validate token; throw exception in case of failure
                await this.authManager.ValidateTokenAsync(token);

                var helpTiles = await this.helpDataProvider.GetHelpTilesAsync();
                if (helpTiles != null)
                {
                    return this.Json(new { entities = helpTiles.OrderBy(x => x.TileOrder) });
                }

                return this.Json(new { entities = new List<HelpInfoEntity>() });
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex);
                return this.Unauthorized();
            }
        }

        /// <summary>
        /// Get Configuration json.
        /// </summary>
        /// <returns> Configuration json</returns>
        [HttpGet]
        public JsonResult GetConfigurationJson()
        {
            var configurationJson = new Dictionary<string, string>
            {
                { "APPINSIGHTS_INSTRUMENTATIONKEY", this.configuration["APPINSIGHTS_INSTRUMENTATIONKEY"] }
            };

            return this.Json(configurationJson);
        }

        /// <summary>
        /// Filter the locale selected by the user from configuration.
        /// </summary>
        /// <param name="locale">User selected locale.</param>
        /// <returns>Filtered locale.</returns>
        private string FilterSelectedLocale(string locale)
        {
            if (!string.IsNullOrEmpty(locale))
            {
                var supportedCultures = this.configuration["i18n:SupportedCultures"].Split(',');
                if (supportedCultures.Contains(locale, StringComparer.OrdinalIgnoreCase) || supportedCultures.Contains(locale.Split("-")[0], StringComparer.OrdinalIgnoreCase))
                {
                    return locale;
                }
            }

            // Return default culture if locale is not supported.
            return this.configuration["i18n:DefaultCulture"];
        }
    }
}