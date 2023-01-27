# 🌏WinUI3Localizer
The WinUI3Localizer is a NuGet package that helps you localize your WinUI 3 app.

- Switch languages **without restarting**
- You/users can **edit** localized strings even after deployment
- You/users can **add** new languages even after deployment
- Use standard **Resources.resw**

## 🙌 Getting started

### Installing the WinUI3Localizer
Install the WinUI3Localizer from the NuGet Package Manager or using the command below.

```powershell
dotnet add package WinUI3Localizer
```

### Prepare the "Strings" folder
Create a "Strings" folder and populate it with your string resources files for the languages you need. For example, this is the basic structure of a "Strings" folder for English (en-US), es-ES (Spanish) and Japanese (ja).

- Strings
  - en-US
    - Resources.resw
  - es-ES
    - Resources.resw
  - ja
    - Resources.resw

The "Strings" folder can be anywhere as long the app can access it. Usually, aside the app executable for non-packaged apps, or in the LocalData folder for packaged-apps.
