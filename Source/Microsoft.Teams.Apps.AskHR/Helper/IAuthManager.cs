// <copyright file="IAuthManager.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR.Helper
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for Authentication management for JWT tokens
    /// </summary>
    public interface IAuthManager
    {
        /// <summary>
        /// Validate JSON web token against validation parameters provided by issuer configuration
        /// </summary>
        /// <param name="token">JSON web token.</param>
        /// <returns>true if the token is valid</returns>
        Task ValidateTokenAsync(string token);
    }
}
