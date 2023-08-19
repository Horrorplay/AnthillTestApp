namespace AnthillTest.AuthService.Extensions;

public static class ConfigurationExtension
{
    static string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    public static IConfiguration AppConfig
    {
        get
        {
            return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"Configurations/appsettings.json", optional: false)
            .AddJsonFile($"Configurations/appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        }
    }
}
