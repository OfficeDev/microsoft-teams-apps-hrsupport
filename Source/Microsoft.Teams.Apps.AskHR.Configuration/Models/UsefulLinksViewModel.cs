// <copyright file="UsefulLinksViewModel.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR.Configuration.Models
{
    using System.Collections.Generic;
    using Microsoft.Teams.Apps.AskHR.Common.Models;

    /// <summary>
    /// Holds UsefulLinks urls.
    /// </summary>
    public class UsefulLinksViewModel
    {
        /// <summary>
        ///  Gets or sets listUsefulLinks
        /// </summary>
        public List<HelpInfoEntity> HelpInfoTiles { get; set; }

        /// <summary>
        /// Gets or sets listImageUrl
        /// </summary>
        public List<ImageFileUrl> TileImageUrls { get; set; }
    }
}