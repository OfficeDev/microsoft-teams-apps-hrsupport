// <copyright file="HelpInfoEntity.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR.Common.Models
{
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Model Class to take data from azure table storage
    /// </summary>
    public class HelpInfoEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets ImageUrl
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets RedirectUrl
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets or sets Tags.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets number of TagsMatched.
        /// </summary>
        public int TagsMatched { get; set; }

        /// <summary>
        /// Gets or sets order Id to be used in View
        /// </summary>
        public int TileOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation is save or edit
        /// </summary>
        public bool IsEdit { get; set; }
    }
}
