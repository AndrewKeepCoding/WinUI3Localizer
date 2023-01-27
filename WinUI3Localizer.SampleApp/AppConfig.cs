using Microsoft.Extensions.Configuration;

namespace WinUI3Localizer.SampleApp;

public class AppConfig
{
    private readonly IConfigurationRoot configurationRoot;

    public AppConfig(string basePath)
    {
        this.configurationRoot = new ConfigurationBuilder()
            .SetBasePath("")
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
    }
}