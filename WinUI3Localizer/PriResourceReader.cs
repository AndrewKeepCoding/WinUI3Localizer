using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUI3Localizer;
internal class PriResourceReader
{
    private readonly ResourceManager resourceManager;

    internal PriResourceReader(ResourceManager resourceManager)
    {
        this.resourceManager = resourceManager;
    }

    public IEnumerable<LanguageDictionary.Item> GetItems(string language, string subTreeName = "Resources")
    {
        if (string.IsNullOrEmpty(subTreeName) || subTreeName == "/")
        {
            subTreeName = "Resources";
        }
        else if (subTreeName.EndsWith('/'))
        {
            subTreeName = subTreeName[..^1];
        }

        ResourceMap resourceMap = this.resourceManager.MainResourceMap.TryGetSubtree(subTreeName);
        if (resourceMap != null)
        {
            ResourceContext resourceContext = this.resourceManager.CreateResourceContext();
            resourceContext.QualifierValues[KnownResourceQualifierName.Language] = language;

            return GetItemsCore(resourceMap, subTreeName, resourceContext);
        }

        return Enumerable.Empty<LanguageDictionary.Item>();
    }


    private IEnumerable<LanguageDictionary.Item> GetItemsCore(ResourceMap resourceMap, string subTreeName, ResourceContext resourceContext)
    {
        bool isResourcesSubTree = string.Equals(subTreeName, "Resources", StringComparison.OrdinalIgnoreCase);
        uint count = resourceMap.ResourceCount;

        for (uint i = 0; i < count; i++)
        {
            (string key, ResourceCandidate? candidate) = resourceMap.GetValueByIndex(i, resourceContext);

            if (candidate != null && candidate.Kind == ResourceCandidateKind.String)
            {
                key = key.Replace('/', '.');
                if (!isResourcesSubTree)
                {
                    key = $"/{subTreeName}/{key}";
                }
                yield return LocalizerBuilder.CreateLanguageDictionaryItem(key, candidate.ValueAsString);
            }
        }
    }

}

internal class PriResourceReaderFactory
{
    private readonly Dictionary<string, PriResourceReader> readers = new Dictionary<string, PriResourceReader>();

    internal PriResourceReader GetPriResourceReader(string? priFile)
    {
        string? normalizedFilePath = string.Empty;

        if (!string.IsNullOrEmpty(priFile))
        {
            normalizedFilePath = System.IO.Path.GetFullPath(priFile);
        }

        if (!this.readers.TryGetValue(normalizedFilePath, out PriResourceReader? reader))
        {
            ResourceManager manager;
            if (string.IsNullOrEmpty(normalizedFilePath))
            {
                manager = new ResourceManager();
            }
            else
            {
                manager = new ResourceManager(normalizedFilePath);
            }
            reader = new PriResourceReader(manager);
            this.readers[normalizedFilePath] = reader;
        }

        return reader;
    }
}

