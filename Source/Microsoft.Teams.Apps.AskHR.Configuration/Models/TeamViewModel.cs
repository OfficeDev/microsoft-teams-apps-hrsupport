// <copyright file="TeamViewModel.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR.Configuration.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    /// <summary>
    /// Team view model
    /// </summary>
    public class TeamViewModel
    {
        /// <summary>
        /// Gets or sets team Id textbox to be used in View
        /// </summary>
        [Required(ErrorMessage ="Enter team ID.")]
        [MinLength(1)]
        [DataType(DataType.Text)]
        [Display(Name ="Team ID")]
        [RegularExpression(@"(\S)+", ErrorMessage = "Enter team ID which should not contain any whitespace.")]
        [AllowHtml]
        public string TeamId { get; set; }
    }
}