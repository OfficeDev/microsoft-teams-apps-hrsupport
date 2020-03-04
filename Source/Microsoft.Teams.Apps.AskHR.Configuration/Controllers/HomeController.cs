// <copyright file="HomeController.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR.Configuration.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Azure.CognitiveServices.Knowledge.QnAMaker;
    using Microsoft.Security.Application;
    using Microsoft.Teams.Apps.AskHR.Common.Models;
    using Microsoft.Teams.Apps.AskHR.Common.Providers;
    using Microsoft.Teams.Apps.AskHR.Configuration.Models;
    using Microsoft.Teams.Apps.AskHR.Configuration.Resources;

    /// <summary>
    /// Home Controller
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        private static int tileOrder;

        private readonly IConfigurationProvider configurationProvider;
        private readonly IHelpDataProvider helpDataProvider;
        private readonly IQnAMakerClient qnaMakerClient;

        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="configurationProvider">configurationProvider DI.</param>
        /// <param name="helpDataProvider">help content data provider instance.</param>
        /// <param name="qnaMakerClient">qnaMakerClient DI.</param>
        /// <param name="telemetryClient">telemetry client to trace logs.</param>
        public HomeController(IConfigurationProvider configurationProvider, IHelpDataProvider helpDataProvider, IQnAMakerClient qnaMakerClient, TelemetryClient telemetryClient)
        {
            this.configurationProvider = configurationProvider;
            this.qnaMakerClient = qnaMakerClient;
            this.helpDataProvider = helpDataProvider;
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// The landing page
        /// </summary>
        /// <returns>Default landing page view</returns>
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            try
            {
                var helpInfoTiles = await this.helpDataProvider.GetHelpTilesAsync();
                this.telemetryClient.TrackTrace("help info tiles are found");

                if (helpInfoTiles.Count > 0)
                {
                    tileOrder = helpInfoTiles.Max(d => d.TileOrder);
                }

                var viewModel = new UsefulLinksViewModel
                {
                    TileImageUrls = await this.helpDataProvider.GetImageFilesAsync(),
                    HelpInfoTiles = helpInfoTiles.OrderBy(c => c.TileOrder).ToList()
                };

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackTrace($"Error processing message: {ex.Message}", SeverityLevel.Error);
                this.telemetryClient.TrackException(ex);
                return this.View("Error");
            }
        }

        /// <summary>
        /// Parse team Id from first and then proceed to save it on success
        /// </summary>
        /// <param name="teamId">teamId is the unique string</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> ParseAndSaveTeamIdAsync(string teamId)
        {
            teamId = Sanitizer.GetSafeHtmlFragment(teamId);
            string teamIdAfterParse = this.ParseTeamIdFromDeepLink(teamId);
            if (!string.IsNullOrEmpty(teamIdAfterParse))
            {
                return await this.SaveOrUpdateTeamIdAsync(teamIdAfterParse);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ErrorMessages.TeamIdValidMessage);
            }
        }

        /// <summary>
        /// Save or update teamId in table storage which is received from View
        /// </summary>
        /// <param name="teamId">teamId is the unique deep link URL string associated with each team</param>
        /// <returns>View</returns>
        [HttpPost]
        public async Task<ActionResult> SaveOrUpdateTeamIdAsync(string teamId)
        {
            bool saved = await this.configurationProvider.SaveOrUpdateEntityAsync(teamId, ConfigurationEntityTypes.TeamId);
            if (saved)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ErrorMessages.UnableToSaveTeamId);
            }
        }

        /// <summary>
        /// Get already saved team Id from table storage
        /// </summary>
        /// <returns>Team Id</returns>
        [HttpGet]
        public async Task<string> GetSavedTeamIdAsync()
        {
            return await this.configurationProvider.GetSavedEntityDetailAsync(ConfigurationEntityTypes.TeamId);
        }

        /// <summary>
        /// Save or update knowledgeBaseId in table storage which is received from View
        /// </summary>
        /// <param name="knowledgeBaseId">knowledgeBaseId is the unique string knowledge Id</param>
        /// <returns>View</returns>
        public async Task<ActionResult> SaveOrUpdateKnowledgeBaseIdAsync(string knowledgeBaseId)
        {
            bool saved = await this.configurationProvider.SaveOrUpdateEntityAsync(knowledgeBaseId, ConfigurationEntityTypes.KnowledgeBaseId);
            if (saved)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ErrorMessages.UnableToSaveKnowledgeBaseId);
            }
        }

        /// <summary>
        /// Validate knowledge base Id from QnA Maker service first and then proceed to save it on success.
        /// The QnA Maker endpoint key is also refreshed as part of this process.
        /// </summary>
        /// <param name="knowledgeBaseId">knowledgeBaseId is the unique string knowledge Id</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> ValidateAndSaveKnowledgeBaseIdAsync(string knowledgeBaseId)
        {
            knowledgeBaseId = Sanitizer.GetSafeHtmlFragment(knowledgeBaseId);
            bool isValidKnowledgeBaseId = await this.IsKnowledgeBaseIdValid(knowledgeBaseId);
            if (isValidKnowledgeBaseId)
            {
                var endpointRefreshStatus = await this.RefreshQnAMakerEndpointKeyAsync();
                if (!endpointRefreshStatus)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ErrorMessages.UnabletoSaveQnAKey);
                }

                return await this.SaveOrUpdateKnowledgeBaseIdAsync(knowledgeBaseId);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ErrorMessages.InvalidKnowledgeBaseId);
            }
        }

        /// <summary>
        /// Get already saved knowledge base Id from table storage
        /// </summary>
        /// <returns>knowledge base Id</returns>
        [HttpGet]
        public async Task<string> GetSavedKnowledgeBaseIdAsync()
        {
            return await this.configurationProvider.GetSavedEntityDetailAsync(ConfigurationEntityTypes.KnowledgeBaseId);
        }

        /// <summary>
        /// Save or update welcome message to be used by bot in table storage which is received from View
        /// </summary>
        /// <param name="welcomeMessage">welcomeMessage</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> SaveWelcomeMessageAsync(string welcomeMessage)
        {
            bool saved = await this.configurationProvider.SaveOrUpdateEntityAsync(welcomeMessage, ConfigurationEntityTypes.WelcomeMessageText);
            if (saved)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ErrorMessages.UnableToSaveWelcomeMsg);
            }
        }

        /// <summary>
        /// Get already saved Welcome message from table storage
        /// </summary>
        /// <returns>Welcome message</returns>
        public async Task<string> GetSavedWelcomeMessageAsync()
        {
            var welcomeText = await this.configurationProvider.GetSavedEntityDetailAsync(ConfigurationEntityTypes.WelcomeMessageText);
            if (welcomeText.Equals(string.Empty))
            {
                await this.SaveWelcomeMessageAsync(Strings.DefaultWelcomeMessage);
            }

            return await this.configurationProvider.GetSavedEntityDetailAsync(ConfigurationEntityTypes.WelcomeMessageText);
        }

        /// <summary>
        /// Save or update welcome message to be used by bot in table storage which is received from View
        /// </summary>
        /// <param name="helpInfoEntity">UsefulLinks Object</param>
        /// <returns>View</returns>
        [HttpPost]
        public async Task<ActionResult> SaveDetailsAsync(HelpInfoEntity helpInfoEntity)
        {
            if (!helpInfoEntity.IsEdit)
            {
                helpInfoEntity.TileOrder = tileOrder + 1;
                tileOrder = helpInfoEntity.TileOrder;
            }

            if (helpInfoEntity.RowKey == null)
            {
                helpInfoEntity.RowKey = Guid.NewGuid().ToString();
            }

            bool isSuccess = await this.helpDataProvider.SaveOrUpdateEntityAsync(helpInfoEntity);
            if (isSuccess)
            {
                return this.Json(new { statusCode = HttpStatusCode.OK, rowKey = helpInfoEntity.RowKey, tileOrder = helpInfoEntity.TileOrder });
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ErrorMessages.UnableToSaveWelcomeMsg);
            }
        }

        /// <summary>
        /// Delete UsefulLinksRecord
        /// </summary>
        /// <param name="rowKey">Row Key</param>
        /// <returns>Deleted Record</returns>
        [HttpGet]
        public async Task<bool> DeleteUsefulLinksRecord(string rowKey)
        {
            return await this.helpDataProvider.DeleteEntityAsync(rowKey);
        }

        /// <summary>
        /// Based on deep link URL received find team id and return it to that it can be saved
        /// </summary>
        /// <param name="teamIdDeepLink">team Id deep link</param>
        /// <returns>team Id as string</returns>
        private string ParseTeamIdFromDeepLink(string teamIdDeepLink)
        {
            // team id regex match
            // for a pattern like https://teams.microsoft.com/l/team/19%3a64c719819fb1412db8a28fd4a30b581a%40thread.tacv2/conversations?groupId=53b4782c-7c98-4449-993a-441870d10af9&tenantId=72f988bf-86f1-41af-91ab-2d7cd011db47
            // regex checks for 19%3a64c719819fb1412db8a28fd4a30b581a%40thread.tacv2
            var match = Regex.Match(teamIdDeepLink, @"teams.microsoft.com/l/team/(\S+)/");

            if (!match.Success)
            {
                return string.Empty;
            }

            return HttpUtility.UrlDecode(match.Groups[1].Value);
        }

        /// <summary>
        /// Check if provided knowledgebase Id is valid or not.
        /// </summary>
        /// <param name="knowledgeBaseId">knowledge base id</param>
        /// <returns><see cref="Task"/> boolean value indicating provided knowledgebase Id is valid or not</returns>
        private async Task<bool> IsKnowledgeBaseIdValid(string knowledgeBaseId)
        {
            try
            {
                var kbIdDetail = await this.qnaMakerClient.Knowledgebase.GetDetailsAsync(knowledgeBaseId);
                return kbIdDetail.Id == knowledgeBaseId;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Update the saved endpoint key
        /// </summary>
        /// <returns>Tracking task</returns>
        private async Task<bool> RefreshQnAMakerEndpointKeyAsync()
        {
            try
            {
                var endpointKeys = await this.qnaMakerClient.EndpointKeys.GetKeysAsync();
                await this.configurationProvider.SaveOrUpdateEntityAsync(endpointKeys.PrimaryEndpointKey, ConfigurationEntityTypes.QnAMakerEndpointKey);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}