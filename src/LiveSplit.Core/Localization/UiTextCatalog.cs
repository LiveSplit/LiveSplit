using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace LiveSplit.Localization;

public static class UiTextCatalog
{
    private const string LocalizationDirectoryName = "Localization";

    private static readonly object SyncRoot = new();
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private static Dictionary<string, CatalogEntry> catalogs = new(StringComparer.OrdinalIgnoreCase)
    {
        [AppLanguage.English.Code] = CatalogEntry.CreateDefault()
    };

    private static IReadOnlyList<AppLanguage> languages = new[] { AppLanguage.English };
    private static bool initialized;
    private static string initializedBaseDirectory;

    public static IReadOnlyList<AppLanguage> Languages
    {
        get
        {
            EnsureLoaded();
            return languages;
        }
    }

    public static AppLanguage DefaultLanguage
    {
        get
        {
            EnsureLoaded();
            return catalogs.TryGetValue(AppLanguage.English.Code, out CatalogEntry catalog)
                ? catalog.Language
                : AppLanguage.English;
        }
    }

    public static void Initialize(string baseDirectory = null)
    {
        string normalizedBaseDirectory = NormalizeBaseDirectory(baseDirectory);

        lock (SyncRoot)
        {
            if (initialized && string.Equals(initializedBaseDirectory, normalizedBaseDirectory, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            catalogs = LoadCatalogs(normalizedBaseDirectory);
            languages = catalogs.Values
                .Select(x => x.Language)
                .OrderBy(x => x.IsDefault ? 0 : 1)
                .ThenBy(x => x.DisplayName, StringComparer.CurrentCultureIgnoreCase)
                .ToArray();
            initializedBaseDirectory = normalizedBaseDirectory;
            initialized = true;
        }
    }

    public static AppLanguage FindLanguage(string languageCode)
    {
        EnsureLoaded();

        if (string.IsNullOrWhiteSpace(languageCode))
        {
            return null;
        }

        string normalizedCode = languageCode.Trim();
        if (catalogs.TryGetValue(normalizedCode, out CatalogEntry exactMatch))
        {
            return exactMatch.Language;
        }

        string twoLetterIsoLanguageName = TryGetTwoLetterIsoLanguageName(normalizedCode);
        if (string.IsNullOrEmpty(twoLetterIsoLanguageName))
        {
            return null;
        }

        return catalogs.Values
            .Select(x => x.Language)
            .FirstOrDefault(x => string.Equals(TryGetTwoLetterIsoLanguageName(x.CultureName), twoLetterIsoLanguageName, StringComparison.OrdinalIgnoreCase));
    }

    public static AppLanguage MatchLanguage(CultureInfo culture)
    {
        EnsureLoaded();

        if (culture == null)
        {
            return DefaultLanguage;
        }

        AppLanguage exactMatch = FindLanguage(culture.Name);
        if (exactMatch != null)
        {
            return exactMatch;
        }

        string twoLetterIsoLanguageName = TryGetTwoLetterIsoLanguageName(culture.Name);
        if (!string.IsNullOrEmpty(twoLetterIsoLanguageName))
        {
            AppLanguage languageMatch = catalogs.Values
                .Select(x => x.Language)
                .FirstOrDefault(x => string.Equals(TryGetTwoLetterIsoLanguageName(x.CultureName), twoLetterIsoLanguageName, StringComparison.OrdinalIgnoreCase));
            if (languageMatch != null)
            {
                return languageMatch;
            }
        }

        return DefaultLanguage;
    }

    public static bool TryGetTranslation(string source, AppLanguage language, out string translated)
    {
        translated = null;
        EnsureLoaded();

        if (language == null || !language.RequiresLocalization || string.IsNullOrEmpty(source))
        {
            return false;
        }

        return catalogs.TryGetValue(language.Code, out CatalogEntry catalog)
            && catalog.Sources.TryGetValue(source, out translated);
    }

    public static bool TryGetKeyTranslation(AppLanguage language, string key, out string translated)
    {
        translated = null;
        EnsureLoaded();

        if (string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        if (language != null &&
            !language.IsAuto &&
            catalogs.TryGetValue(language.Code, out CatalogEntry catalog) &&
            catalog.Keys.TryGetValue(key, out translated))
        {
            return true;
        }

        return catalogs.TryGetValue(DefaultLanguage.Code, out CatalogEntry defaultCatalog)
            && defaultCatalog.Keys.TryGetValue(key, out translated);
    }

    private static void EnsureLoaded()
    {
        if (!initialized)
        {
            Initialize();
        }
    }

    private static string NormalizeBaseDirectory(string baseDirectory)
    {
        string normalizedPath;

        if (!string.IsNullOrWhiteSpace(baseDirectory))
        {
            normalizedPath = Path.GetFullPath(baseDirectory.Trim());
        }
        else
        {
            normalizedPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        return normalizedPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    private static Dictionary<string, CatalogEntry> LoadCatalogs(string baseDirectory)
    {
        var loadedCatalogs = new Dictionary<string, CatalogEntry>(StringComparer.OrdinalIgnoreCase)
        {
            [AppLanguage.English.Code] = CatalogEntry.CreateDefault()
        };

        string localizationDirectory = Path.Combine(baseDirectory, LocalizationDirectoryName);
        if (!Directory.Exists(localizationDirectory))
        {
            return loadedCatalogs;
        }

        foreach (string filePath in Directory.EnumerateFiles(localizationDirectory, "*.json").OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
        {
            try
            {
                LocalizationFileDefinition definition = JsonSerializer.Deserialize<LocalizationFileDefinition>(File.ReadAllText(filePath), JsonOptions);
                AppLanguage language = CreateLanguage(definition);
                if (language == null || language.IsAuto)
                {
                    continue;
                }

                loadedCatalogs[language.Code] = new CatalogEntry(
                    language,
                    CreateMap(definition?.Keys, StringComparer.Ordinal),
                    CreateMap(definition?.Sources, StringComparer.Ordinal));
            }
            catch
            {
                // Ignore invalid locale files and keep the rest of the catalog usable.
            }
        }

        return loadedCatalogs;
    }

    private static AppLanguage CreateLanguage(LocalizationFileDefinition definition)
    {
        if (definition == null || string.IsNullOrWhiteSpace(definition.Code))
        {
            return null;
        }

        string code = definition.Code.Trim();
        string displayName = string.IsNullOrWhiteSpace(definition.DisplayName) ? code : definition.DisplayName.Trim();
        string cultureName = string.IsNullOrWhiteSpace(definition.CultureName) ? code : definition.CultureName.Trim();
        return new AppLanguage(code, displayName, cultureName);
    }

    private static Dictionary<string, string> CreateMap(IDictionary<string, string> source, IEqualityComparer<string> comparer)
    {
        var map = new Dictionary<string, string>(comparer);
        if (source == null)
        {
            return map;
        }

        foreach (KeyValuePair<string, string> pair in source)
        {
            if (string.IsNullOrWhiteSpace(pair.Key) || pair.Value == null)
            {
                continue;
            }

            map[pair.Key] = pair.Value;
        }

        return map;
    }

    private static string TryGetTwoLetterIsoLanguageName(string cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
        {
            return null;
        }

        try
        {
            return CultureInfo.GetCultureInfo(cultureName).TwoLetterISOLanguageName;
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
    }

    private sealed class LocalizationFileDefinition
    {
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public string CultureName { get; set; }
        public Dictionary<string, string> Keys { get; set; }
        public Dictionary<string, string> Sources { get; set; }
    }

    private sealed class CatalogEntry
    {
        public AppLanguage Language { get; }
        public Dictionary<string, string> Keys { get; }
        public Dictionary<string, string> Sources { get; }

        public CatalogEntry(AppLanguage language, Dictionary<string, string> keys, Dictionary<string, string> sources)
        {
            Language = language;
            Keys = keys ?? new Dictionary<string, string>(StringComparer.Ordinal);
            Sources = sources ?? new Dictionary<string, string>(StringComparer.Ordinal);
        }

        public static CatalogEntry CreateDefault()
        {
            return new CatalogEntry(
                AppLanguage.English,
                new Dictionary<string, string>(StringComparer.Ordinal),
                new Dictionary<string, string>(StringComparer.Ordinal));
        }
    }
}
