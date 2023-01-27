using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Documents;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using WinUI3Localizer;

namespace WinUI3Localizer.SampleApp;

public partial class App : Application
{
    private static IHost Host { get; } = BuildHost();

    public static string StringsFolderPath { get; private set; } = string.Empty;

    private Window? window;

    private string[] DefaultStringsResources { get; } = { "en-US", "es-ES", "ja" };

    public App()
    {
        InitializeComponent();
        RequestedTheme = ApplicationTheme.Dark;
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        await InitializeLocalizer();

        this.window = Host.Services.GetRequiredService<MainWindow>();
        this.window.Activate();
    }

    private async Task PrepareDefaultStringsResourcesFolderForPackagedApps(StorageFolder stringsFolder)
    {
        foreach (string language in DefaultStringsResources)
        {
            StorageFolder languageFolder = await stringsFolder.CreateFolderAsync(
                language,
                CreationCollisionOption.OpenIfExists);

            string resourcesFileName = "Resources.resw";

            if (await languageFolder.TryGetItemAsync(resourcesFileName) is null)
            {
                Uri resourcesFileUri = new($"ms-appx:///Strings/{language}/{resourcesFileName}");
                StorageFile defaultResourceFile = await StorageFile.GetFileFromApplicationUriAsync(resourcesFileUri);
                _ = await defaultResourceFile.CopyAsync(languageFolder);
            }
        }
    }

    /// <summary>
    /// Creates default Resources.resw files for the WinUI3Localizer.
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
        await PrepareDefaultStringsResourcesFolderForPackagedApps(stringsFolder);
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
}