// <copyright file="AuthManager.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR.Helper
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Protocols;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// AuthManager.
    /// </summary>
    public class AuthManager : IAuthManager
    {
        private readonly IConfigurationManager<OpenIdConnectConfiguration> configurationManager;
        private readonly string expectedTenantId;
        private readonly string appId;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthManager"/> class.
        /// AuthManager constructor
        /// </summary>
        /// <param name="configurationManager">configurationManager</param>
        /// <param name="tenantId">tenantId</param>
        /// <param name="appId">appId</param>
        public AuthManager(IConfigurationManager<OpenIdConnectConfiguration> configurationManager, string tenantId, string appId)
        {
            this.configurationManager = configurationManager;
            this.expectedTenantId = tenantId;
            this.appId = appId;
        }

        /// <summary>
        /// Validate token against OIDC configuration values
        /// </summary>
        /// <param name="token">JWT</param>
        /// <returns>valid user name claim</returns>
        public async Task ValidateTokenAsync(string token)
        {
            var openIdConfig = await this.configurationManager.GetConfigurationAsync(CancellationToken.None);

            // Configure the TokenValidationParameters. Also set the Issuer and Audience(s) to validate
            TokenValidationParameters validationParameters =
                new TokenValidationParameters
                {
                    ValidIssuer = $"https://sts.windows.net/{this.expectedTenantId}/",
                    IssuerSigningKeys = openIdConfig.SigningKeys,
                    ValidAudience = this.appId,
                    ValidateIssuer = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

            // If the token is not valid for any reason, an exception will be thrown by the method
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
        }
    }
}
