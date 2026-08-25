// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using FluentAssertions;
using Fuke.Common;
using Xunit;

namespace Fuke.GlobalTool.Tests;

public class GlobalToolSettingsTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "fuke-settings-tests", Guid.NewGuid().ToString("N"));

    private string SettingsFile => Path.Combine(_directory, "settings.json");

    [Fact]
    public void MissingFileUsesDocumentedDefaults()
    {
        var settings = GlobalToolSettingsStore.LoadFrom(SettingsFile);

        settings.Language.Should().Be(ToolLanguage.English);
        settings.ShowLogo.Should().BeFalse();
    }

    [Theory]
    [InlineData("English", false)]
    [InlineData("Chinese", true)]
    public void SettingsRoundTrip(string languageName, bool showLogo)
    {
        var language = Enum.Parse<ToolLanguage>(languageName);
        var expected = new GlobalToolSettings
        {
            Language = language,
            ShowLogo = showLogo
        };

        GlobalToolSettingsStore.SaveTo(expected, SettingsFile);
        var actual = GlobalToolSettingsStore.LoadFrom(SettingsFile);

        actual.Language.Should().Be(language);
        actual.ShowLogo.Should().Be(showLogo);
    }

    [Theory]
    [InlineData("null")]
    [InlineData("{\"language\":\"German\",\"showLogo\":false}")]
    [InlineData("{\"language\":\"English\",\"showLogo\":false,\"unexpected\":true}")]
    [InlineData("{not-json}")]
    public void InvalidSettingsAreRejected(string json)
    {
        Directory.CreateDirectory(_directory);
        File.WriteAllText(SettingsFile, json);

        var action = () => GlobalToolSettingsStore.LoadFrom(SettingsFile);

        action.Should().Throw<InvalidDataException>();
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, recursive: true);
    }
}
