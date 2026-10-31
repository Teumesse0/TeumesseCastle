//
// Copyright (c) 2026 Teumesse — TeumesseCastle project.
// Licensed under the Non-Profit OSL 3.0. See LICENSE.md in the project root for full license information.
//
// Opt-in check for a newer TeumesseCastle release on GitHub (--check-update).
// This is the only outbound HTTP call of the product that is not directed at the audited infrastructure.
// It is never run automatically, it downloads nothing, and it sends nothing but a User-Agent header.
// See docs/NETWORK.md.
//
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace TeumesseCastle.misc
{
    /// <summary>
    /// Compares the running version with the latest GitHub release of TeumesseCastle and prints the result.
    /// </summary>
    public static class UpdateChecker
    {
        /// <summary>GitHub REST endpoint returning the latest non-draft, non-prerelease release.</summary>
        public const string LatestReleaseUrl = "https://api.github.com/repos/Teumesse0/TeumesseCastle/releases/latest";

        /// <summary>Human-readable page for the latest release, printed when an update is available.</summary>
        public const string LatestReleasePage = "https://github.com/Teumesse0/TeumesseCastle/releases/latest";

        /// <summary>Network timeout, in milliseconds (5 seconds as required by the project rules).</summary>
        public const int TimeoutMilliseconds = 5000;

        /// <summary>
        /// Runs the update check and prints a one-line result on the console.
        /// </summary>
        /// <param name="current">The version of the running executable.</param>
        /// <param name="interactive">
        /// When true (interactive menu), network errors are reported to the user; when false (command line),
        /// they are only written to the trace and the method stays silent.
        /// </param>
        /// <returns>True when a newer release exists, false otherwise (including on error).</returns>
        public static bool Run(Version current, bool interactive)
        {
            bool newer;
            string message = Describe(current, interactive, out newer);
            if (message != null)
            {
                Console.WriteLine(message);
            }
            return newer;
        }

        /// <summary>
        /// Runs the update check and returns the message to show to the user.
        /// </summary>
        /// <param name="current">The version of the running executable.</param>
        /// <param name="interactive">See <see cref="Run"/>: when false, a network error yields a null message.</param>
        /// <param name="newerAvailable">Set to true when a newer release exists.</param>
        /// <returns>The message to display, or null when the check failed in non-interactive mode.</returns>
        public static string Describe(Version current, bool interactive, out bool newerAvailable)
        {
            newerAvailable = false;
            string tag;
            try
            {
                tag = GetLatestReleaseTag();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Update check failed: " + ex.Message);
                return interactive ? "Unable to check for updates (" + ex.Message + ")." : null;
            }

            string currentText = current.Major + "." + current.Minor + "." + current.Build;
            int comparison;
            try
            {
                comparison = CompareSemVer(tag, currentText);
            }
            catch (FormatException ex)
            {
                Trace.WriteLine("Update check failed: " + ex.Message);
                return interactive ? "Unable to check for updates (" + ex.Message + ")." : null;
            }
            if (comparison > 0)
            {
                newerAvailable = true;
                return "A new version of TeumesseCastle is available: " + tag + " (you are running " + currentText + ")."
                    + Environment.NewLine + "Nothing has been downloaded. Get it from " + LatestReleasePage;
            }
            return "TeumesseCastle " + currentText + " is up to date (latest release: " + tag + ").";
        }

        /// <summary>
        /// Queries the GitHub API and returns the <c>tag_name</c> of the latest release (for example <c>v1.0.0</c>).
        /// </summary>
        /// <exception cref="WebException">The request failed or timed out.</exception>
        /// <exception cref="InvalidDataException">The answer does not contain a tag name.</exception>
        public static string GetLatestReleaseTag()
        {
            // GitHub only accepts TLS 1.2 or later; .NET Framework 4.5.2 does not enable it by default.
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var request = (HttpWebRequest)WebRequest.Create(LatestReleaseUrl);
            request.Method = "GET";
            request.Timeout = TimeoutMilliseconds;
            request.ReadWriteTimeout = TimeoutMilliseconds;
            request.Accept = "application/vnd.github+json";
            request.UserAgent = "TeumesseCastle/" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            request.AllowAutoRedirect = true;

            string body;
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                body = reader.ReadToEnd();
            }

            var json = JObject.Parse(body);
            var tag = (string)json["tag_name"];
            if (string.IsNullOrEmpty(tag))
            {
                throw new InvalidDataException("the GitHub answer has no tag_name");
            }
            return tag;
        }

        /// <summary>
        /// Compares two semantic versions (<c>MAJOR.MINOR.PATCH[-prerelease]</c>, an optional leading <c>v</c> is ignored).
        /// </summary>
        /// <returns>
        /// A negative number when <paramref name="left"/> is older than <paramref name="right"/>, zero when they are
        /// equal, a positive number when <paramref name="left"/> is newer. A pre-release is older than the same
        /// version without a pre-release tag (SemVer 2.0 §11).
        /// </returns>
        /// <exception cref="FormatException">One of the inputs is not a semantic version.</exception>
        public static int CompareSemVer(string left, string right)
        {
            var l = ParseSemVer(left);
            var r = ParseSemVer(right);
            for (int i = 0; i < 3; i++)
            {
                if (l.Numbers[i] != r.Numbers[i])
                    return l.Numbers[i].CompareTo(r.Numbers[i]);
            }
            if (string.IsNullOrEmpty(l.PreRelease) && string.IsNullOrEmpty(r.PreRelease))
                return 0;
            if (string.IsNullOrEmpty(l.PreRelease))
                return 1;
            if (string.IsNullOrEmpty(r.PreRelease))
                return -1;
            return string.CompareOrdinal(l.PreRelease, r.PreRelease);
        }

        private static readonly Regex SemVerPattern = new Regex(@"^v?(\d+)\.(\d+)(?:\.(\d+))?(?:-([0-9A-Za-z.-]+))?(?:\+[0-9A-Za-z.-]+)?$", RegexOptions.Compiled);

        private struct SemVer
        {
            public int[] Numbers;
            public string PreRelease;
        }

        private static SemVer ParseSemVer(string text)
        {
            if (text == null)
                throw new FormatException("null is not a semantic version");
            var m = SemVerPattern.Match(text.Trim());
            if (!m.Success)
                throw new FormatException("'" + text + "' is not a semantic version");
            return new SemVer
            {
                Numbers = new[]
                {
                    int.Parse(m.Groups[1].Value),
                    int.Parse(m.Groups[2].Value),
                    m.Groups[3].Success ? int.Parse(m.Groups[3].Value) : 0,
                },
                PreRelease = m.Groups[4].Success ? m.Groups[4].Value : null,
            };
        }
    }
}
