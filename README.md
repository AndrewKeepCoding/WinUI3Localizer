# 🌏WinUI3Localizer

**WinUI3Localizer** is a NuGet package that helps you localize your WinUI 3 app.

- Switch languages **without app restarting**
- You/users can **edit** localized strings even after deployment
- You/users can **add** new languages even after deployment
- Use standard **Resources.resw** (see [Microsoft docs](https://learn.microsoft.com/en-us/windows/uwp/app-resources/localize-strings-ui-manifest))

## 🙌 Quick Start

> **Note**: This is a quick start guide. Check the sample app for details.

### **Install WinUI3Localizer**

Install **WinUI3Localizer** from the NuGet Package Manager.

### **Create localized strings**

Create a "Strings" folder in your app project and populate it with your string resources files for each language you need. For example, this is a basic structure for English (en-US), es-ES (Spanish) and Japanese (ja) resources files.

- Strings
  - en-US
    - Resources.resw
  - es-ES
    - Resources.resw
  - ja
    - Resources.resw

Add this ItemGroup in the project file (\*.csproj) of your app.

```xml
<!-- Copy all "Resources.resw" files in the "Strings" folder to the output folder. -->
<ItemGroup>
  <Content Include="Strings\**\*.resw">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

> **Note**: The "Strings" folder can be anywhere as long the app can access it. Usually, aside the app executable for non-packaged apps, or in the "LocalFolder" for packaged-apps.

### **Build WinUI3Localizer**

- Non-packaged apps:

  In App.xaml.cs, build **WinUI3Localizer** like this:

  ```csharp
  private async Task InitializeLocalizer()
  {
      // Initialize a "Strings" folder in the executables folder.
      StringsFolderPath StringsFolderPath = Path.Combine(AppContext.BaseDirectory, "Strings");
      StorageFolder stringsFolder = await StorageFolder.GetFolderFromPathAsync(StringsFolderPath);

      ILocalizer localizer = await new LocalizerBuilder()
          .AddStringResourcesFolderForLanguageDictionaries(StringsFolderPath)
          .SetOptions(options =>
          {
              options.DefaultLanguage = "en-US";
              options.UseUidWhenLocalizedStringNotFound = true;
          })
          .Build();
  }
  ```

- Packaged apps:

  In App.xaml.cs, build **WinUI3Localizer** like this:

  ```csharp
  private async Task InitializeLocalizer()
  {

      // Initialize a "Strings" folder in the "LocalFolder" for the packaged app.
      StorageFolder localFolder = ApplicationData.Current.LocalFolder;
      StorageFolder stringsFolder = await localFolder.CreateFolderAsync(
        "Strings",
         CreationCollisionOption.OpenIfExists);

      // Create string resources file from app resources if doesn't exists.
      string resourceFileName = "Resources.resw";
      await CreateStringResourceFileIfNotExists(stringsFolder, "en-US", resourceFileName);
      await CreateStringResourceFileIfNotExists(stringsFolder, "es-ES", resourceFileName);
      await CreateStringResourceFileIfNotExists(stringsFolder, "ja", resourceFileName);

      ILocalizer localizer = await new LocalizerBuilder()
          .AddStringResourcesFolderForLanguageDictionaries(stringsFolder.Path)
          .SetOptions(options =>
          {
              options.DefaultLanguage = "en-US";
              options.UseUidWhenLocalizedStringNotFound = true;
          })
          .Build();
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
  ```

### **Localizing controls**

This is an example of how to localize the `Content` of a `Button`.

First asign an `Uid` to the `Button`, then in each language resources file, add an item that corresponds to the `Uid`.

You can also have multiple string resources files. For example, besides the default **Resources.resw** file, you can have a **Messages.resw** for your messages file.
To just need to include `/<resources-file-name>/` before the string resource identifier.

```xml
<Page
    x:Class="WinUI3Localizer.SampleApp.TestPage"
    ...
    xmlns:l="using:WinUI3Localizer">
    <StackPanel>
        <Button l:Uids.Uid="TestPage_Button">
            <Button.Flyout>
                <Flyout>
                    <TextBlock l:Uids.Uid="/Messages/ButtonFlyoutMessage" />
                </Flyout>
            </Button.Flyout>
        </Button>
    </StackPanel>
</Page>
```

- en-US

  - Resources.resw

    | Name | Value |
    | ---- | ----- |
    | TestPageButton.Content | Awesome! |

  - Messages.resw

    | Name | Value |
    | ---- | ----- |
    | ButtonFlyoutMessage.Text | This is an awesome message! |

- es-ES:

  - Resources.resw

    | Name | Value |
    | ---- | ----- |
    | TestPageButton.Content | ¡Increíble! |

  - Messages.resw

    | Name | Value |
    | ---- | ----- |
    | ButtonFlyoutMessage.Text | ¡Esto es un mensaje increíble! |

- ja:

  - Resources.resw

    | Name | Value |
    | ---- | ----- |
    | TestPageButton.Content | 素晴らしい！ |

  - Messages.resw

    | Name | Value |
    | ---- | ----- |
    | ButtonFlyoutMessage.Text | これは素晴らしいメッセージです！ |

### **Getting localized strings**

If we need to localize strings in code-behind or in ViewModels, we can use the `GetLocalizedString()` method.

```csharp
List<string> colors = new()
{
    "Red",
    "Green",
    "Blue",
};

ILocalizer localizer = Localizer.Get();
List<string> localizedColors = colors
    .Select(x => localizer.GetLocalizedString(x))
    .ToList();
```

In this case, we just use the `Uid` as `Name`.

- en-US

  - Resources.resw

    | Name | Value |
    | ---- | ----- |
    | Red | Red |
    | Green | Green |
    | Blue | Blue |

- es-ES:

  - Resources.resw

    | Name | Value |
    | ---- | ----- |
    | Red | Rojo |
    | Green | Verde |
    | Blue | Azul |

- ja:

  - Resources.resw

    | Name | Value |
    | ---- | ----- |
    | Red | 赤 |
    | Green | 緑 |
    | Blue | 青 |
### **Minimal example**
Refer to [TemplateStudioWinUI3LocalizerSampleApp
](https://github.com/AndrewKeepCoding/TemplateStudioWinUI3LocalizerSampleApp)
