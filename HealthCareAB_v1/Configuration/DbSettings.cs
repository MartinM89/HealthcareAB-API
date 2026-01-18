using System.Diagnostics.CodeAnalysis;

namespace HealthCareAB_v1.Configuration;

[ExcludeFromCodeCoverage]
public class DbSettings
{
    public const string SectionName = "DbConnectionStrings";
    public string ConnectionString { get; set; } = string.Empty;
}
