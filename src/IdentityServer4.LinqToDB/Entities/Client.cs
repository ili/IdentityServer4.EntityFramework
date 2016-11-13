// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using IdentityServer4.Models;
using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
    public class Client
    {
		[PrimaryKey, Identity]
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ProtocolType { get; set; } = IdentityServerConstants.ProtocolTypes.OpenIdConnect;
        public string ClientName { get; set; }
        public bool Enabled { get; set; } = true;
		[Association(ThisKey = nameof(Id), OtherKey = nameof(ClientSecret.ClientId))]
        public List<ClientSecret> ClientSecrets { get; set; }
        public bool RequireClientSecret { get; set; } = true;
        public string ClientUri { get; set; }
        public string LogoUri { get; set; }
        public bool RequireConsent { get; set; } = true;
        public bool AllowRememberConsent { get; set; } = true;
		[Association(ThisKey = nameof(Id), OtherKey = nameof(ClientGrantType.ClientId))]
        public List<ClientGrantType> AllowedGrantTypes { get; set; }
        public bool RequirePkce { get; set; }
        public bool AllowPlainTextPkce { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }
		[Association(ThisKey = nameof(Id), OtherKey = nameof(ClientRedirectUri.ClientId))]
        public List<ClientRedirectUri> RedirectUris { get; set; }
		[Association(ThisKey = nameof(Id), OtherKey = nameof(ClientPostLogoutRedirectUri.ClientId))]
        public List<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; set; }
        public string LogoutUri { get; set; }
        public bool LogoutSessionRequired { get; set; } = true;
        public bool AllowAccessToAllScopes { get; set; }
		[Association(ThisKey = nameof(Id), OtherKey = nameof(ClientScope.ClientId))]
        public List<ClientScope> AllowedScopes { get; set; }
        public int IdentityTokenLifetime { get; set; } = 300;
        public int AccessTokenLifetime { get; set; } = 3600;
        public int AuthorizationCodeLifetime { get; set; } = 300;
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;
        public int RefreshTokenUsage { get; set; } = (int)TokenUsage.OneTimeOnly;
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        public int RefreshTokenExpiration { get; set; } = (int)TokenExpiration.Absolute;
        public int AccessTokenType { get; set; } = (int)0; // AccessTokenType.Jwt;
        public bool EnableLocalLogin { get; set; } = true;
		[Association(ThisKey = nameof(Id), OtherKey = nameof(ClientIdPRestriction.ClientId))]
        public List<ClientIdPRestriction> IdentityProviderRestrictions { get; set; }
        public bool IncludeJwtId { get; set; }
		[Association(ThisKey = nameof(Id), OtherKey = nameof(ClientClaim.ClientId))]
        public List<ClientClaim> Claims { get; set; }
        public bool AlwaysSendClientClaims { get; set; }
        public bool PrefixClientClaims { get; set; } = true;
		[Association(ThisKey = nameof(Id), OtherKey = nameof(ClientCorsOrigin.ClientId))]
        public List<ClientCorsOrigin> AllowedCorsOrigins { get; set; }
    }
}