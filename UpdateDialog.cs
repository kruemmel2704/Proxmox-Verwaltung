using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProxmoxVEGui
{
    public class UpdateDialog : Form
    {
        private readonly string _latestVersion;
        private readonly string _releaseNotes;
        private readonly string _downloadUrl;
        private readonly long _fileSize;

        private Label lblHeaderTitle;
        private Label lblVersionInfo;
        private Label lblVersionCompare;
        private Label lblNotesHeader;
        private GlassPanel panelChangelogContainer;
        private TextBox txtChangelog;
        
        private Panel panelProgressTrack;
        private Panel panelProgressBar;
        private Label lblProgressText;

        private RoundedButton btnDownload;
        private RoundedButton btnLater;
        private RoundedButton btnSkip;

        private CancellationTokenSource _cts;

        public UpdateDialog(string latestVersion, string releaseNotes, string downloadUrl, long fileSize)
        {
            _latestVersion = latestVersion;
            _releaseNotes = releaseNotes;
            _downloadUrl = downloadUrl;
            _fileSize = fileSize;

            InitializeComponent();
            ApplyAesthetics();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 420);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Software Update";
            this.BackColor = Color.FromArgb(10, 15, 25);
            this.ForeColor = Color.FromArgb(226, 232, 240);

            // Icon helper application
            IconHelper.ApplyIcon(this);

            // 1. Header Title
            lblHeaderTitle = new Label
            {
                Text = "✨ Update Available",
                Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(249, 115, 22),
                Location = new Point(20, 20),
                Size = new Size(460, 28),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // 2. Version Info Subtitle
            lblVersionInfo = new Label
            {
                Text = "A newer version of Proxmox VE Windows GUI is available.",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, 50),
                Size = new Size(460, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // 3. Version Comparison
            string currentVer = Updater.CurrentVersion.ToString(3);
            string newVer = _latestVersion.TrimStart('v', 'V');
            lblVersionCompare = new Label
            {
                Text = $"Installed:  v{currentVer}   ➔   Latest:  v{newVer}",
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 75),
                Size = new Size(460, 22),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // 4. Notes Header
            lblNotesHeader = new Label
            {
                Text = "RELEASE NOTES:",
                Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, 105),
                Size = new Size(460, 18),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // 5. Changelog Card
            panelChangelogContainer = new GlassPanel
            {
                Location = new Point(20, 125),
                Size = new Size(445, 160),
                BorderColor = Color.FromArgb(55, 65, 81),
                BackColor = Color.FromArgb(15, 23, 42),
                BorderRadius = 8
            };

            txtChangelog = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.FromArgb(226, 232, 240),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Location = new Point(8, 8),
                Size = new Size(429, 144),
                Text = _releaseNotes.Replace("\r\n", "\n").Replace("\n", "\r\n") // Ensure windows formatting
            };
            panelChangelogContainer.Controls.Add(txtChangelog);

            // 6. Custom Progress Bar Track
            panelProgressTrack = new Panel
            {
                Location = new Point(20, 305),
                Size = new Size(445, 8),
                BackColor = Color.FromArgb(30, 41, 59),
                Visible = false
            };

            // 7. Custom Progress Bar Fill
            panelProgressBar = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(0, 8),
                BackColor = Color.FromArgb(249, 115, 22)
            };
            panelProgressTrack.Controls.Add(panelProgressBar);

            // 8. Progress details text
            lblProgressText = new Label
            {
                Location = new Point(20, 318),
                Size = new Size(445, 20),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Visible = false,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // 9. Download button
            btnDownload = new RoundedButton
            {
                Text = "🚀 Install Now",
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                Location = new Point(345, 340),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(249, 115, 22),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnDownload.Click += btnDownload_Click;

            // 10. Later button
            btnLater = new RoundedButton
            {
                Text = "Remind Me",
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                Location = new Point(215, 340),
                Size = new Size(115, 30),
                BackColor = Color.FromArgb(55, 65, 81),
                ForeColor = Color.FromArgb(226, 232, 240),
                Cursor = Cursors.Hand
            };
            btnLater.Click += (s, e) => this.Close();

            // 11. Skip button
            btnSkip = new RoundedButton
            {
                Text = "Skip Version",
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                Location = new Point(20, 340),
                Size = new Size(115, 30),
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.FromArgb(148, 163, 184),
                Cursor = Cursors.Hand
            };
            btnSkip.Click += btnSkip_Click;

            // Add controls to Form
            this.Controls.Add(lblHeaderTitle);
            this.Controls.Add(lblVersionInfo);
            this.Controls.Add(lblVersionCompare);
            this.Controls.Add(lblNotesHeader);
            this.Controls.Add(panelChangelogContainer);
            this.Controls.Add(panelProgressTrack);
            this.Controls.Add(lblProgressText);
            this.Controls.Add(btnDownload);
            this.Controls.Add(btnLater);
            this.Controls.Add(btnSkip);
        }

        private void ApplyAesthetics()
        {
            // Center textbox selection so it doesn't default to select-all highlight
            txtChangelog.SelectionStart = 0;
            txtChangelog.SelectionLength = 0;
            
            // Adjust RoundedButtons styling
            btnDownload.BorderRadius = 6;
            btnLater.BorderRadius = 6;
            btnSkip.BorderRadius = 6;
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            string versionStr = _latestVersion.TrimStart('v', 'V');
            Updater.SaveSkippedVersion(versionStr);
            MessageBox.Show($"Version v{versionStr} will be skipped until the next version is released.", 
                            "Skip Version", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            // If download url is a web page, redirect browser and close application
            if (!_downloadUrl.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    Process.Start(new ProcessStartInfo(_downloadUrl) { UseShellExecute = true });
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not open download page: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            // UI feedback transition
            btnDownload.Visible = false;
            btnLater.Visible = false;
            btnSkip.Visible = false;

            panelProgressTrack.Visible = true;
            lblProgressText.Visible = true;
            lblProgressText.Text = "Connecting to download server...";

            _cts = new CancellationTokenSource();

            try
            {
                string tempDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ProxmoxVEGui", "Updates");
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                string tempFile = Path.Combine(tempDir, "ProxmoxVEGui-Setup.exe");
                
                // If installer already downloaded in previous attempts, delete it
                if (File.Exists(tempFile))
                {
                    try { File.Delete(tempFile); } catch { }
                }

                await Task.Run(() => DownloadInstallerAsync(_downloadUrl, tempFile, _cts.Token));

                lblProgressText.Text = "Starting installer and closing application...";
                await Task.Delay(800);

                // Run the setup executable
                Process.Start(new ProcessStartInfo(tempFile) { UseShellExecute = true });

                // Exit current GUI instance so files can be replaced
                Application.Exit();
            }
            catch (OperationCanceledException)
            {
                lblProgressText.Text = "Download cancelled.";
                ResetActionButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to download or run update: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetActionButtons();
            }
        }

        private async Task DownloadInstallerAsync(string url, string destinationPath, CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(5);

                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();

                    long? totalBytes = response.Content.Headers.ContentLength;
                    if (totalBytes == null || totalBytes == 0)
                    {
                        totalBytes = _fileSize; // Fallback to GitHub Release JSON asset size
                    }

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var buffer = new byte[8192];
                        long totalRead = 0;
                        int bytesRead;

                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                            totalRead += bytesRead;

                            if (totalBytes > 0)
                            {
                                double progressPercentage = (double)totalRead / totalBytes.Value;
                                int percent = (int)(progressPercentage * 100);

                                this.BeginInvoke(new Action(() =>
                                {
                                    panelProgressBar.Width = (int)(progressPercentage * panelProgressTrack.Width);
                                    lblProgressText.Text = $"Downloading update: {percent}% ({FormatBytes(totalRead)} / {FormatBytes(totalBytes.Value)})";
                                }));
                            }
                        }
                    }
                }
            }
        }

        private void ResetActionButtons()
        {
            panelProgressTrack.Visible = false;
            lblProgressText.Visible = false;

            btnDownload.Visible = true;
            btnLater.Visible = true;
            btnSkip.Visible = true;
        }

        private string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB" };
            int i = 0;
            double dblBytes = bytes;
            while (dblBytes >= 1024 && i < suffixes.Length - 1)
            {
                i++;
                dblBytes /= 1024;
            }
            return $"{dblBytes:0.1} {suffixes[i]}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}
