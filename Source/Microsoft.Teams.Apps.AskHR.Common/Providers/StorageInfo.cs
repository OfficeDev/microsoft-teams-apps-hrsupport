// <copyright file="StorageInfo.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR.Common.Providers
{
    /// <summary>
    /// References to storage table.
    /// </summary>
    public class StorageInfo
    {
        /// <summary>
        /// Table name where configuration app details will be saved
        /// </summary>
        public const string ConfigurationTableName = "ConfigurationInfo";

        /// <summary>
        /// Table name where SME activity details from bot will be saved
        /// </summary>
        public const string TicketTableName = "Tickets";

        /// <summary>
        /// Table name where Tiles details from configuration app will be saved
        /// </summary>
        public const string HelpInfoTableName = "HelpInfo";

        /// <summary>
        /// 
        /// </summary>
        public const string HelpImageBlobContainer = "tileimages";
    }
}
