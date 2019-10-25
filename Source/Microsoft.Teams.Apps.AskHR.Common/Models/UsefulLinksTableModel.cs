// <copyright file="UsefulLinksTableModel.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR.Common.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Useful Links Model
    /// </summary>
    public class UsefulLinksTableModel : TableEntity
    {
        /// <summary>
        /// Gets or sets Title text box to be used in View
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Description text box to be used in View
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets Url text box to be used in View
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets or sets Image to be used in View
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets Image to be used in View
        /// </summary>
        public string Tags { get; set; }

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
