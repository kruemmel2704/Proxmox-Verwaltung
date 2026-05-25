using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace ProxmoxVEGui
{
    public static class Updater
    {
        public static readonly Version CurrentVersion = typeof(Program).Assembly.GetName().Version;

        public static string GetSkippedVersion()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\ProxmoxVEGui"))
                {
                    return key?.GetValue("SkippedVersion")?.ToString() ?? "";
                }
            }
            catch
            {
                return "";
            }
        }

        public static void SaveSkippedVersion(string version)
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\ProxmoxVEGui"))
                {
                    key?.SetValue("SkippedVersion", version);
                }
            }
            catch
            {
                // Registry access might fail in rare environments
            }
        }

        private static Version ParseVersion(string tagName)
        {
            if (string.IsNullOrEmpty(tagName)) return new Version(0, 0, 0);
            string clean = tagName.TrimStart('v', 'V');
            if (Version.TryParse(clean, out Version parsed))
            {
                return parsed;
            }
            return new Version(0, 0, 0);
        }

        public static async Task CheckForUpdatesAsync(Form parentForm, bool silent)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // GitHub API requires a User-Agent header
                    client.DefaultRequestHeaders.Add("User-Agent", "ProxmoxVEGui-Updater");

                    // Set a reasonable timeout so background check doesn't hang
                    client.Timeout = TimeSpan.FromSeconds(15);

                    string url = "https://api.github.com/repos/kruemmel2704/Proxmox-Verwaltung/releases/latest";
                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        if (!silent)
                        {
                            MessageBox.Show($"Check for updates failed: GitHub returned status code {response.StatusCode}.", 
                                            "Update Check", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        return;
                    }

                    string json = await response.Content.ReadAsStringAsync();
                    var release = JObject.Parse(json);

                    string latestTag = release["tag_name"]?.ToString() ?? "";
                    Version latestVersion = ParseVersion(latestTag);
                    Version currentVersion = CurrentVersion;

                    // Standardize versions to 3 components for comparison (e.g. 1.0.6)
                    if (latestVersion > currentVersion)
                    {
                        string skipped = GetSkippedVersion();
                        if (silent && skipped == latestVersion.ToString())
                        {
                            // User chose to skip this specific version
                            return;
                        }

                        // Extract download URL and size for the Windows Installer (.exe)
                        string downloadUrl = "";
                        long fileSize = 0;
                        var assets = release["assets"] as JArray;
                        if (assets != null)
                        {
                            foreach (var asset in assets)
                            {
                                string name = asset["name"]?.ToString() ?? "";
                                if (name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                                {
                                    downloadUrl = asset["browser_download_url"]?.ToString() ?? "";
                                    fileSize = asset["size"]?.ToObject<long>() ?? 0;
                                    break;
                                }
                            }
                        }

                        // Fallback if no EXE installer asset was found
                        if (string.IsNullOrEmpty(downloadUrl))
                        {
                            downloadUrl = release["html_url"]?.ToString() ?? "";
                        }

                        string releaseNotes = release["body"]?.ToString() ?? "No release notes available.";

                        // Launch UI update dialog on main thread
                        parentForm.BeginInvoke(new Action(() =>
                        {
                            try
                            {
                                using (var dialog = new UpdateDialog(latestTag, releaseNotes, downloadUrl, fileSize))
                                {
                                    dialog.ShowDialog(parentForm);
                                }
                            }
                            catch (Exception ex)
                            {
                                if (!silent)
                                {
                                    MessageBox.Show($"Could not open update dialog: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }));
                    }
                    else
                    {
                        if (!silent)
                        {
                            MessageBox.Show($"Your application is up to date!\n\nCurrent Version: {currentVersion.ToString(3)}", 
                                            "Update Check", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    MessageBox.Show($"Error checking for updates: {ex.Message}\nMake sure you are connected to the Internet.", 
                                    "Update Check", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
