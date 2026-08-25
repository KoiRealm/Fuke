// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fuke.Common;

internal enum ToolLanguage
{
    English,
    Chinese
}

internal sealed class GlobalToolSettings
{
    public ToolLanguage Language { get; set; } = ToolLanguage.English;

    public bool ShowLogo { get; set; }
}

internal static class GlobalToolSettingsStore
{
    private const string SettingsFileName = "settings.json";
    private static readonly object s_lock = new();
    private static readonly JsonSerializerOptions s_serializerOptions = CreateSerializerOptions();
    private static GlobalToolSettings s_current;

    internal static string SettingsDirectory
    {
        get
        {
            var applicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (string.IsNullOrWhiteSpace(applicationData))
                throw new InvalidOperationException("The operating system did not provide an application data directory.");

            return Path.Combine(applicationData, "KoiRealm", "Fuke");
        }
    }

    internal static string SettingsFile => Path.Combine(SettingsDirectory, SettingsFileName);

    internal static GlobalToolSettings Current
    {
        get
        {
            lock (s_lock)
                return s_current ??= LoadFrom(SettingsFile);
        }
    }

    internal static GlobalToolSettings LoadFrom(string path)
    {
        if (!File.Exists(path))
            return new GlobalToolSettings();

        try
        {
            var settings = JsonSerializer.Deserialize<GlobalToolSettings>(File.ReadAllText(path, Encoding.UTF8), s_serializerOptions);
            if (settings == null)
                throw new InvalidDataException($"The global settings file '{path}' contains JSON null instead of an object.");

            if (!Enum.IsDefined(typeof(ToolLanguage), settings.Language))
                throw new InvalidDataException($"The global settings file '{path}' contains an unsupported language value.");

            return settings;
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException($"The global settings file '{path}' is invalid: {exception.Message}", exception);
        }
    }

    internal static void Save(GlobalToolSettings settings)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        if (!Enum.IsDefined(typeof(ToolLanguage), settings.Language))
            throw new ArgumentOutOfRangeException(nameof(settings), settings.Language, "Unsupported tool language.");

        lock (s_lock)
        {
            Directory.CreateDirectory(SettingsDirectory);
            var json = JsonSerializer.Serialize(settings, s_serializerOptions) + Environment.NewLine;
            File.WriteAllText(SettingsFile, json, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            s_current = settings;
        }
    }

    internal static void SaveTo(GlobalToolSettings settings, string path)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));
        if (!Enum.IsDefined(typeof(ToolLanguage), settings.Language))
            throw new ArgumentOutOfRangeException(nameof(settings), settings.Language, "Unsupported tool language.");

        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(directory))
            throw new ArgumentException("The settings path must contain a directory.", nameof(path));

        Directory.CreateDirectory(directory);
        var json = JsonSerializer.Serialize(settings, s_serializerOptions) + Environment.NewLine;
        File.WriteAllText(path, json, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    internal static void Reset()
    {
        lock (s_lock)
        {
            if (File.Exists(SettingsFile))
                File.Delete(SettingsFile);

            s_current = new GlobalToolSettings();
        }
    }

    internal static void Reload()
    {
        lock (s_lock)
            s_current = LoadFrom(SettingsFile);
    }

    private static JsonSerializerOptions CreateSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        };
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}

public static class ToolLocalization
{
    public static string GetText(string english, string chinese)
    {
        return L(english, chinese);
    }

    internal static string L(string english, string chinese)
    {
        if (english == null)
            throw new ArgumentNullException(nameof(english));
        if (chinese == null)
            throw new ArgumentNullException(nameof(chinese));

        return GlobalToolSettingsStore.Current.Language switch
        {
            ToolLanguage.English => english,
            ToolLanguage.Chinese => chinese,
            var language => throw new NotSupportedException($"Unsupported tool language '{language}'.")
        };
    }
}
