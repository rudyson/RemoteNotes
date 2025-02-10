﻿namespace FPECS.ISTK.Shared.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public required string Secret { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int ExpirationMinutes { get; set; }
}
