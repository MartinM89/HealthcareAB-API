namespace HealthCareAB_v1.Configuration;

public class DbSettings
{
    public const string SectionName = "DbConnectionStrings";
    public string DbConnectionString { get; set; } = string.Empty;
}
