// <copyright file="WelcomeMessageViewModel.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR.Configuration.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    /// <summary>
    /// Welcome message view model
    /// </summary>
    public class WelcomeMessageViewModel
    {
        /// <summary>
        /// Gets or sets welcome message text box to be used in View
        /// </summary>
        [Required(ErrorMessage = "Enter a welcome message.")]
        [StringLength(maximumLength: 300, ErrorMessage = "Enter welcome message which should contain less than 300 characters.", MinimumLength = 2)]
        [DataType(DataType.Text)]
        [Display(Name = "Welcome message")]
        [AllowHtml]
        public string WelcomeMessage { get; set; }
    }
}