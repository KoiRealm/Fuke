// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.IO;
using System.Linq;
using Fuke.Common;
using Fuke.Common.IO;
using Fuke.Common.Tooling;
using Fuke.Common.Utilities.Collections;
using SkiaSharp;
using static Fuke.Common.IO.HttpTasks;

partial class Build
{
    const string JetBrainsMonoVersion = "2.304";

    string[] FontDownloadUrls =>
        new[]
        {
            "https://github.com/googlefonts/roboto/releases/latest/download/roboto-unhinted.zip",
            $"https://github.com/JetBrains/JetBrainsMono/releases/download/v{JetBrainsMonoVersion}/JetBrainsMono-{JetBrainsMonoVersion}.zip"
        };

    AbsolutePath FontDirectory => TemporaryDirectory / "fonts";

    Target InstallFonts => _ => _
        .Executes(() =>
        {
            FontDownloadUrls.ForEach(x => HttpDownloadFile(x, FontDirectory / new Uri(x).Segments.Last()));
            FontDirectory.GlobFiles("*.zip").ForEach(x => x.UncompressTo(FontDirectory / x.NameWithoutExtension));
        });

    AbsolutePath WatermarkImageFile => RootDirectory / "images" / "logo-watermark.png";
    AbsolutePath ReleaseImageFile => TemporaryDirectory / "release-image.png";

    Target ReleaseImage => _ => _
        .DependsOn(InstallFonts)
        .Executes(() =>
        {
            const int width = 1200;
            const int height = 675;
            const float logoScale = 0.37f;
            using var bitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(new SKColor(25, 25, 25));

            using var logo = SKBitmap.Decode(WatermarkImageFile)
                ?? throw new InvalidDataException($"Could not decode the watermark image at {WatermarkImageFile}.");
            var logoWidth = (int) (logo.Width * logoScale);
            var logoHeight = (int) (logo.Height * logoScale);
            var logoBounds = SKRect.Create((width - logoWidth) / 2, (height - logoHeight) / 2, logoWidth, logoHeight);
            using var logoPaint = new SKPaint { Color = SKColors.White.WithAlpha(13) };
            canvas.DrawBitmap(logo, logoBounds, new SKSamplingOptions(SKCubicResampler.Mitchell), logoPaint);

            using var textPaint = new SKPaint { Color = SKColors.WhiteSmoke, IsAntialias = true };
            DrawCenteredText("NEW RELEASE", "JetBrainsMono-Thin.ttf", 100, height / 2f - 120);
            DrawCenteredText(ReleaseVersion, "JetBrainsMono-ExtraBold.ttf", 230, height / 2f + 60);

            using var fileStream = new FileStream(ReleaseImageFile, FileMode.Create);
            if (!bitmap.Encode(fileStream, SKEncodedImageFormat.Png, 100))
                throw new IOException($"Could not encode the release image at {ReleaseImageFile}.");

            void DrawCenteredText(string text, string fontFile, float fontSize, float centerY)
            {
                var fontPath = FontDirectory.GlobFiles($"**/{fontFile}").Single();
                using var typeface = SKTypeface.FromFile(fontPath)
                    ?? throw new InvalidDataException($"Could not load the font at {fontPath}.");
                using var font = new SKFont(typeface, fontSize);
                font.MeasureText(text, out var bounds);
                canvas.DrawText(text, width / 2f - bounds.MidX, centerY - bounds.MidY, SKTextAlign.Left, font, textPaint);
            }
        });
}
