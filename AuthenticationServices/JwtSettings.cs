﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AuthenticationServices;

public class JwtSettings
{
    public required string Secret { get; set; }

    public required string Issuer { get; set; }

    public required string Audience { get; set; }

    public required string AccessExpiryMinutes { get; set; }

    public required string PasswordResetExpiryMinutes { get; set; }
}
