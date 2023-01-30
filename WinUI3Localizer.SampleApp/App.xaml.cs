using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace WinUI3Localizer.SampleApp;

public partial class App : Application
{
    private Window? window;

    public App()
    {
        InitializeComponent();
        RequestedTheme = ApplicationTheme.Dark;
    }

    public static string StringsFolderPath { get; private set; } = string.Empty;

    private static IHost Host { get; } = BuildHost();

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        await InitializeLocalizer();

        this.window = Host.Services.GetRequiredService<MainWindow>();
        this.window.Activate();
    }

    private static async Task CreateStringResourceFileIfNotExists(StorageFolder stringsFolder, string language, string resourceFileName)
    {
        StorageFolder languageFolder = await stringsFolder.CreateFolderAsync(
            language,
            CreationCollisionOption.OpenIfExists);

        if (await languageFolder.TryGetItemAsync(resourceFileName) is null)
        {
            string resourceFilePath = Path.Combine(stringsFolder.Name, language, resourceFileName);
            StorageFile resourceFile = await LoadStringResourcesFileFromAppResource(resourceFilePath);
            _ = await resourceFile.CopyAsync(languageFolder);
        }
    }

    private static async Task<StorageFile> LoadStringResourcesFileFromAppResource(string filePath)
    {
        Uri resourcesFileUri = new($"ms-appx:///{filePath}");
        return await StorageFile.GetFileFromApplicationUriAsync(resourcesFileUri);
    }

    private static IHost BuildHost()
    {
        return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .UseSerilog((context, service, configuration) =>
            {
                _ = configuration
                    .MinimumLevel.Verbose()
                    .WriteTo.File(
                        "WinUI3Localizer.log",
                        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose,
                        rollingInterval: RollingInterval.Month);
            })
            .ConfigureServices((context, services) =>
            {
                _ = services
                    .AddLogging(configure =>
                    {
                        _ = configure
                            .SetMinimumLevel(LogLevel.Trace)
                            .AddSerilog()
                            .AddDebug();
                    })
                    .AddSingleton<MainWindow>()
                    //.AddSingleton<ILocalizer>(factory =>
                    //{
                    //    return new LocalizerBuilder()
                    //        .AddStringResourcesFolderForLanguageDictionaries(StringsFolderPath)
                    //        .SetLogger(Host.Services
                    //            .GetRequiredService<ILoggerFactory>()
                    //            .CreateLogger<Localizer>())
                    //        .SetOptions(options =>
                    //        {
                    //            options.DefaultLanguage = "ja";
                    //            options.UseUidWhenLocalizedStringNotFound = true;
                    //        })
                    //        .Build()
                    //        .GetAwaiter()
                    //        .GetResult();
                    //})
                    ;
            })
            .Build();
    }

    /// <summary>
    /// Creates default Resources.resw files for WinUI3Localizer.
    /// </summary>
    private async Task InitializeLocalizer()
    {
#if IS_NON_PACKAGED
        // Initialize a "Strings" folder in the executables folder.
        StringsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Strings");

        //StorageFolder localFolder = await StorageFolder.GetFolderFromPathAsync(Directory.GetCurrentDirectory());
        StorageFolder stringsFolder = await StorageFolder.GetFolderFromPathAsync(StringsFolderPath);
#else
        // Initialize a "Strings" folder in the "LocalFolder" for the packaged app.
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        StorageFolder stringsFolder = await localFolder.CreateFolderAsync("Strings", CreationCollisionOption.OpenIfExists);
        StringsFolderPath = stringsFolder.Path;

        // Create string resources file from app resources if doesn't exists.
        string resourceFileName = "Resources.resw";
        await CreateStringResourceFileIfNotExists(stringsFolder, "en-US", resourceFileName);
        await CreateStringResourceFileIfNotExists(stringsFolder, "es-ES", resourceFileName);
        await CreateStringResourceFileIfNotExists(stringsFolder, "ja", resourceFileName);
#endif

        ILocalizer localizer = await new LocalizerBuilder()
            .AddStringResourcesFolderForLanguageDictionaries(StringsFolderPath)
            //.SetLogger(Host.Services
            //    .GetRequiredService<ILoggerFactory>()
            //    .CreateLogger<Localizer>())
            .SetOptions(options =>
            {
                options.DefaultLanguage = "en-US";
                options.UseUidWhenLocalizedStringNotFound = true;
            })
            //.AddLocalizationAction(new LocalizationActionItem(typeof(Hyperlink), arguments =>
            //{
            //    if (arguments.DependencyObject is Hyperlink target && target.Inlines.Count is 0)
            //    {
            //        target.Inlines.Clear();
            //        target.Inlines.Add(new Run() { Text = arguments.Value });
            //    }
            //}))
            //.AddLocalizationAction(new LocalizationActionItem(typeof(Run), arguments =>
            //{
            //    if (arguments.DependencyObject is Run target)
            //    {
            //        target.Text = arguments.Value;
            //    }
            //}))
            .Build();
    }
}