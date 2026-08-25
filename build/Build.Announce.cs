// Copyright 2026 KoiRealm and Fuke contributors.
// Distributed under the MIT License.
// See LICENSE in the repository root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;
using LinqToTwitter.OAuth;
using Fuke.Common;
using Fuke.Common.ChangeLog;
using Fuke.Common.Git;
using Fuke.Common.IO;
using Fuke.Common.Tools.Discord;
using Fuke.Common.Tools.GitHub;
using Fuke.Common.Tools.Mastodon;
using Fuke.Common.Tools.Slack;
using Fuke.Common.Utilities;
using Fuke.Components;
using static Fuke.Common.Tools.Discord.DiscordTasks;
using static Fuke.Common.Tools.Git.GitTasks;
using static Fuke.Common.Tools.Mastodon.MastodonTasks;
using static Fuke.Common.Tools.Slack.SlackTasks;

partial class Build
{
    Target Announce => _ => _
        .DependsOn(ReleaseImage)
        .WhenSkipped(DependencyBehavior.Skip)
        // .TriggeredBy<IPublish>()
        .OnlyWhenStatic(() => GitRepository.IsOnMainBranch());

    IEnumerable<string> ChangelogSectionNotes => ChangelogTasks.ExtractChangelogSectionNotes(From<IHazChangelog>().ChangelogFile);

    string AnnouncementTitle => $"FUKE {ReleaseVersion} RELEASED!";
    string AnnouncementLink => $"https://nuget.org/packages/Fuke.Common/{ReleaseVersion}";
    int AnnouncementColor => 0x00ACC1;

    string AnnouncementThumbnailUrl =>
        (Version.Parse(ReleaseVersion).Minor, Version.Parse(ReleaseVersion).Build, Version.Parse(ReleaseVersion).Revision) switch
        {
            (0, 0, 0) => "https://em-content.zobj.net/thumbs/320/apple/325/rocket_1f680.png",
            (_, 0, 0) => "https://em-content.zobj.net/thumbs/320/apple/325/wrapped-gift_1f381.png",
            _ => "https://em-content.zobj.net/thumbs/320/apple/325/package_1f4e6.png"
        };

    string AnnouncementComparisonUrl => $"{RepositoryUrl}/releases/tag/v{ReleaseVersion}";

    string AnnouncementReleaseNotes =>
        new StringBuilder()
            .AppendLine("*Release Notes*")
            .AppendLine("```")
            .AppendLine(ChangelogSectionNotes.Select(x => x.Replace("- ", "• ").Replace("`", string.Empty)).JoinNewLine())
            .AppendLine("```").ToString();

    (string CommitsText, IReadOnlyCollection<string> NotableCommmitters) AnnouncementGitInfo
    {
        get
        {
            var committers = Git($"show --no-patch --format=%an v{ReleaseVersion}", logInvocation: false, logOutput: false);
            var commitsText = $"{committers.Count} {(committers.Count == 1 ? "commit" : "commits")}";
            var notableCommitters = committers
                .Select(x => x.Text)
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .Select(x => x.Key)
                .Where(x => x != "Matthias Koch").ToList();
            return (commitsText, notableCommitters);
        }
    }

    IEnumerable<(string Text, string Url)> AnnouncementSponsors =>
        new (string Text, string Url)[]
        {
            ("Octopus Deploy", "https://octopus.com/"),
            ("Datadog", "https://datadoghq.com/"),
            ("Amazon Web Services", "https://aws.amazon.com/"),
        };

    // https://api.slack.com/apps/A050ZLH0V40/incoming-webhooks?
    [Parameter] [Secret] readonly string SlackWebhook;

    Target AnnounceSlack => _ => _
        .TriggeredBy(Announce)
        .ProceedAfterFailure()
        .Requires(() => SlackWebhook)
        .Executes(async () =>
        {
            await SendSlackMessageAsync(_ => _
                    .AddAttachments(_ => _
                        .SetFallback(AnnouncementTitle)
                        .SetAuthorName(AnnouncementTitle)
                        .SetAuthorLink(AnnouncementLink)
                        .SetColor($"#{AnnouncementColor:x8}")
                        .SetThumbUrl(AnnouncementThumbnailUrl)
                        .SetText(new StringBuilder()
                            .Append($"<!channel>, this new release includes *<{AnnouncementComparisonUrl}|{AnnouncementGitInfo.CommitsText}>*")
                            .AppendLine(AnnouncementGitInfo.NotableCommmitters.Count > 0
                                ? $" with notable contributions from {AnnouncementGitInfo.NotableCommmitters.JoinCommaAnd()}. A round of applause for them! :clap:"
                                : ". No contributions this time. :sweat_smile:")
                            .AppendLine()
                            .AppendLine("Remember that you can call `fuke :update` to update your builds! :bulb:")
                            .AppendLine()
                            .AppendLine(AnnouncementReleaseNotes).ToString())
                        .SetFooter($"Powered by {AnnouncementSponsors.Select(x => $"*<{x.Url}|{x.Text}>*").JoinCommaAnd()}.")),
                SlackWebhook);
        });

    // Server settings | Apps | Integrations | Webhooks | FUKE
    [Parameter] [Secret] readonly string DiscordWebhook;

    Target AnnounceDiscord => _ => _
        .TriggeredBy(Announce)
        .ProceedAfterFailure()
        .Requires(() => DiscordWebhook)
        .Executes(async () =>
        {
            await SendDiscordMessageAsync(_ => _
                    .SetContent("@everyone")
                    .AddEmbeds(_ => _
                        .SetTitle(AnnouncementTitle)
                        .SetColor(AnnouncementColor)
                        .SetThumbnail(_ => _
                            .SetUrl(AnnouncementThumbnailUrl))
                        .SetDescription(new StringBuilder()
                            .Append($"This new release includes *[{AnnouncementGitInfo.CommitsText}]({AnnouncementComparisonUrl})*")
                            .AppendLine(AnnouncementGitInfo.NotableCommmitters.Count > 0
                                ? $" with notable contributions from {AnnouncementGitInfo.NotableCommmitters.JoinCommaAnd()}. A round of applause for them! 👏"
                                : ". No contributions this time. 😅")
                            .AppendLine()
                            .AppendLine("Remember that you can call `fuke :update` to update your builds! 💡")
                            .AppendLine()
                            .AppendLine(AnnouncementReleaseNotes).ToString()
                            .Replace("*", "**"))
                        .SetFooter(_ => _
                            .SetText($"Powered by {AnnouncementSponsors.Select(x => x.Text).JoinCommaAnd()}.")
                            .SetIconUrl("https://cdn.discordapp.com/emojis/674275938757771306.webp?size=240&quality=lossless"))),
                DiscordWebhook);
        });

    string AnnouncementTweetText =>
        new StringBuilder()
            .AppendLine($"🔥 Check out the new {ReleaseVersion} release! 🏗")
            .AppendLine()
            .AppendLine($"More information at 👉 {GitRepository.GetGitHubBrowseUrl(From<IHazChangelog>().ChangelogFile)}").ToString();

    Target AnnounceTwitter => _ => _
        .TriggeredBy(Announce)
        .ProceedAfterFailure()
        .Requires(() => TwitterCredentials.ConsumerKey)
        .Requires(() => TwitterCredentials.ConsumerSecret)
        .Requires(() => TwitterCredentials.AccessToken)
        .Requires(() => TwitterCredentials.AccessTokenSecret)
        .Executes(async () =>
        {
            var context = new TwitterContext(
                new SingleUserAuthorizer
                {
                    CredentialStore =
                        new SingleUserInMemoryCredentialStore
                        {
                            ConsumerKey = TwitterCredentials.ConsumerKey,
                            ConsumerSecret = TwitterCredentials.ConsumerSecret,
                            AccessToken = TwitterCredentials.AccessToken,
                            AccessTokenSecret = TwitterCredentials.AccessTokenSecret
                        }
                });

            var media = await context.UploadMediaAsync(
                media: ReleaseImageFile.ReadAllBytes(),
                mediaType: "image/png",
                mediaCategory: "tweet_image");

            await context.TweetMediaAsync(AnnouncementTweetText, mediaIds: new[] { media.NotNull().MediaID.ToString() });
        });

    string AnnouncementTootText => AnnouncementTweetText;
    // https://dotnet.social/settings/applications/496
    [Parameter] [Secret] readonly string MastodonAccessToken;

    Target AnnounceMastodon => _ => _
        .TriggeredBy(Announce)
        .ProceedAfterFailure()
        .Requires(() => MastodonAccessToken)
        .Executes(async () =>
        {
            await SendMastodonMessageAsync(_ => _
                    .SetText(AnnouncementTootText)
                    .AddMediaFiles(ReleaseImageFile),
                "https://dotnet.social",
                MastodonAccessToken);
        });
}
