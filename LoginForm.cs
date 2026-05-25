using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ProxmoxVEGui
{
    public partial class LoginForm : Form
    {
        private readonly List<SavedConnectionProfile> savedProfiles = new List<SavedConnectionProfile>();

        private GlassPanel panelSavedProfiles;
        private Label lblSavedProfilesTitle;
        private ProfileListControl profileList;
        private ProfileActionButton btnSaveProfile;
        private ProfileActionButton btnLoadProfile;
        private ProfileActionButton btnDeleteProfile;

        private CheckBox chkStayLoggedIn;
        private RememberLoginState rememberLoginState;
        private bool rememberLoginApplied = false;
        private bool autoLoginStarted = false;
        private bool suppressRememberCheckboxEvent = false;

        private static readonly byte[] RememberEntropy = Encoding.UTF8.GetBytes("ProxmoxVEGui.RememberLogin.v1");

        private string ProfilesFolder
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "ProxmoxVEGui"
                );
            }
        }

        private string ProfilesFile
        {
            get
            {
                return Path.Combine(ProfilesFolder, "saved_profiles.xml");
            }
        }

        private string RememberLoginFile
        {
            get
            {
                return Path.Combine(ProfilesFolder, "remember_login.xml");
            }
        }

        public LoginForm()
        {
            InitializeComponent();

            ApplyApplicationIcon();

            InitializeStayLoggedInUi();
            InitializeSavedProfilesUi();

            LoadSavedProfilesFromDisk();
            LoadRememberLoginFromDisk();

            RefreshSavedProfilesList();

            this.Shown += LoginForm_Shown_CustomProfiles;
            this.Shown += LoginForm_Shown_RememberLogin;
            this.Resize += LoginForm_Resize_CustomProfiles;

            SetTransparentBgForLabels(this);
        }

        private void SetTransparentBgForLabels(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Label label)
                {
                    if (label.Parent is GlassPanel)
                    {
                        label.BackColor = Color.Transparent;
                    }
                }
                else if (ctrl.HasChildren)
                {
                    SetTransparentBgForLabels(ctrl);
                }
            }
        }

        private void ApplyApplicationIcon()
        {
            IconHelper.ApplyIcon(this);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            if (cmbRealm.SelectedIndex < 0)
            {
                cmbRealm.SelectedIndex = 0;
            }

            ApplyRememberLoginToForm(false);
        }

        private void LoginForm_Shown_CustomProfiles(object sender, EventArgs e)
        {
            PositionSavedProfilesPanel();
        }

        private void LoginForm_Shown_RememberLogin(object sender, EventArgs e)
        {
            if (autoLoginStarted) return;
            if (!rememberLoginApplied) return;
            if (chkStayLoggedIn == null || !chkStayLoggedIn.Checked) return;
            if (string.IsNullOrWhiteSpace(txtHost.Text)) return;
            if (string.IsNullOrWhiteSpace(txtPort.Text)) return;
            if (string.IsNullOrWhiteSpace(txtUsername.Text)) return;
            if (string.IsNullOrWhiteSpace(txtPassword.Text)) return;

            autoLoginStarted = true;

            BeginInvoke(new Action(() =>
            {
                btnLogin_Click(btnLogin, EventArgs.Empty);
            }));
        }

        private void LoginForm_Resize_CustomProfiles(object sender, EventArgs e)
        {
            PositionSavedProfilesPanel();
        }

        private void InitializeStayLoggedInUi()
        {
            chkStayLoggedIn = new CheckBox();
            chkStayLoggedIn.AutoSize = chkIgnoreSsl != null ? chkIgnoreSsl.AutoSize : true;
            chkStayLoggedIn.Text = "Angemeldet bleiben";
            chkStayLoggedIn.Cursor = Cursors.Hand;
            chkStayLoggedIn.CheckedChanged += ChkStayLoggedIn_CheckedChanged;

            Control parent = this;

            if (chkIgnoreSsl != null && chkIgnoreSsl.Parent != null)
            {
                parent = chkIgnoreSsl.Parent;

                chkStayLoggedIn.Font = chkIgnoreSsl.Font;
                chkStayLoggedIn.ForeColor = chkIgnoreSsl.ForeColor;
                chkStayLoggedIn.BackColor = chkIgnoreSsl.BackColor;
                chkStayLoggedIn.FlatStyle = chkIgnoreSsl.FlatStyle;
                chkStayLoggedIn.UseVisualStyleBackColor = chkIgnoreSsl.UseVisualStyleBackColor;
                chkStayLoggedIn.Padding = chkIgnoreSsl.Padding;
                chkStayLoggedIn.Margin = chkIgnoreSsl.Margin;
                chkStayLoggedIn.TextAlign = chkIgnoreSsl.TextAlign;
                chkStayLoggedIn.CheckAlign = chkIgnoreSsl.CheckAlign;
                chkStayLoggedIn.Height = chkIgnoreSsl.Height;

                chkStayLoggedIn.FlatAppearance.BorderSize = chkIgnoreSsl.FlatAppearance.BorderSize;
                chkStayLoggedIn.FlatAppearance.BorderColor = chkIgnoreSsl.FlatAppearance.BorderColor;
                chkStayLoggedIn.FlatAppearance.CheckedBackColor = chkIgnoreSsl.FlatAppearance.CheckedBackColor;
                chkStayLoggedIn.FlatAppearance.MouseDownBackColor = chkIgnoreSsl.FlatAppearance.MouseDownBackColor;
                chkStayLoggedIn.FlatAppearance.MouseOverBackColor = chkIgnoreSsl.FlatAppearance.MouseOverBackColor;
            }
            else if (panelCard != null)
            {
                parent = panelCard;

                chkStayLoggedIn.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
                chkStayLoggedIn.ForeColor = Color.FromArgb(229, 231, 235);
                chkStayLoggedIn.BackColor = Color.Transparent;
                chkStayLoggedIn.FlatStyle = FlatStyle.Flat;
            }

            parent.Controls.Add(chkStayLoggedIn);

            if (chkIgnoreSsl != null && chkIgnoreSsl.Parent == parent)
            {
                chkStayLoggedIn.Location = new Point(
                    chkIgnoreSsl.Right + 24,
                    chkIgnoreSsl.Top
                );
            }
            else
            {
                chkStayLoggedIn.Location = new Point(220, 390);
            }

            chkStayLoggedIn.BringToFront();
        }
        private void MoveLoginControlsDownForRememberCheckbox(Control parent)
        {
            int requiredTop = chkStayLoggedIn.Bottom + 12;
            int moveBy = 32;

            if (btnLogin != null && btnLogin.Parent == parent && btnLogin.Top < requiredTop)
            {
                btnLogin.Top += moveBy;
            }

            if (lblStatus != null && lblStatus.Parent == parent && lblStatus.Top < requiredTop)
            {
                lblStatus.Top += moveBy;
            }

            if (panelCard != null && parent == panelCard)
            {
                int neededHeight = 0;

                foreach (Control control in panelCard.Controls)
                {
                    neededHeight = Math.Max(neededHeight, control.Bottom + 24);
                }

                if (panelCard.Height < neededHeight)
                {
                    panelCard.Height = neededHeight;
                }
            }
        }

        private void ChkStayLoggedIn_CheckedChanged(object sender, EventArgs e)
        {
            if (suppressRememberCheckboxEvent) return;

            if (chkStayLoggedIn != null && !chkStayLoggedIn.Checked)
            {
                DeleteRememberLoginFromDisk(false);
            }
        }

        private void InitializeSavedProfilesUi()
        {
            panelSavedProfiles = new GlassPanel();
            lblSavedProfilesTitle = new Label();
            profileList = new ProfileListControl();
            btnSaveProfile = new ProfileActionButton();
            btnLoadProfile = new ProfileActionButton();
            btnDeleteProfile = new ProfileActionButton();

            panelSavedProfiles.BackColor = Color.FromArgb(17, 24, 39);
            panelSavedProfiles.BorderColor = Color.FromArgb(55, 65, 81);
            panelSavedProfiles.BorderRadius = 12;
            panelSavedProfiles.BorderSize = 1;
            panelSavedProfiles.Size = new Size(500, 530);
            panelSavedProfiles.Padding = new Padding(20);

            lblSavedProfilesTitle.AutoSize = true;
            lblSavedProfilesTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblSavedProfilesTitle.ForeColor = Color.White;
            lblSavedProfilesTitle.Location = new Point(28, 28);
            lblSavedProfilesTitle.Text = "Gespeicherte Hosts";

            profileList.Location = new Point(26, 90);
            profileList.Size = new Size(448, 280);
            profileList.BackColor = Color.FromArgb(12, 18, 31);
            profileList.BorderColor = Color.FromArgb(75, 85, 99);
            profileList.BorderRadius = 8;
            profileList.DoubleClick += ProfileList_DoubleClick;
            profileList.RenameRequested += ProfileList_RenameRequested;

            btnSaveProfile.NormalColor = Color.FromArgb(249, 115, 22);
            btnSaveProfile.HoverColor = Color.FromArgb(251, 146, 60);
            btnSaveProfile.DownColor = Color.FromArgb(194, 65, 12);
            btnSaveProfile.BorderRadius = 6;
            btnSaveProfile.ForeColor = Color.White;
            btnSaveProfile.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnSaveProfile.Location = new Point(26, 404);
            btnSaveProfile.Size = new Size(448, 40);
            btnSaveProfile.Text = "Aktuelle Daten speichern";
            btnSaveProfile.Cursor = Cursors.Hand;
            btnSaveProfile.Click += BtnSaveProfile_Click;

            btnLoadProfile.NormalColor = Color.FromArgb(31, 41, 55);
            btnLoadProfile.HoverColor = Color.FromArgb(55, 65, 81);
            btnLoadProfile.DownColor = Color.FromArgb(75, 85, 99);
            btnLoadProfile.BorderRadius = 6;
            btnLoadProfile.ForeColor = Color.White;
            btnLoadProfile.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnLoadProfile.Location = new Point(26, 458);
            btnLoadProfile.Size = new Size(215, 40);
            btnLoadProfile.Text = "Laden";
            btnLoadProfile.Cursor = Cursors.Hand;
            btnLoadProfile.Click += BtnLoadProfile_Click;

            btnDeleteProfile.NormalColor = Color.FromArgb(127, 29, 29);
            btnDeleteProfile.HoverColor = Color.FromArgb(153, 27, 27);
            btnDeleteProfile.DownColor = Color.FromArgb(100, 20, 20);
            btnDeleteProfile.BorderRadius = 6;
            btnDeleteProfile.ForeColor = Color.White;
            btnDeleteProfile.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnDeleteProfile.Location = new Point(259, 458);
            btnDeleteProfile.Size = new Size(215, 40);
            btnDeleteProfile.Text = "Löschen";
            btnDeleteProfile.Cursor = Cursors.Hand;
            btnDeleteProfile.Click += BtnDeleteProfile_Click;

            panelSavedProfiles.Controls.Add(lblSavedProfilesTitle);
            panelSavedProfiles.Controls.Add(profileList);
            panelSavedProfiles.Controls.Add(btnSaveProfile);
            panelSavedProfiles.Controls.Add(btnLoadProfile);
            panelSavedProfiles.Controls.Add(btnDeleteProfile);

            this.Controls.Add(panelSavedProfiles);
            panelSavedProfiles.BringToFront();
        }

        private void PositionSavedProfilesPanel()
        {
            if (panelSavedProfiles == null || panelCard == null || lblTitle == null || lblSubtitle == null) return;
            if (this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0) return;

            int spacing = 30;
            int margin = 30;
            int groupWidth = panelCard.Width + spacing + panelSavedProfiles.Width;

            if (groupWidth <= this.ClientSize.Width - (margin * 2))
            {
                int groupX = Math.Max(margin, (this.ClientSize.Width - groupWidth) / 2);
                int cardY = panelCard.Top;

                panelCard.Location = new Point(groupX, cardY);
                panelSavedProfiles.Location = new Point(panelCard.Right + spacing, cardY);
            }
            else
            {
                int centeredX = Math.Max(20, (this.ClientSize.Width - panelSavedProfiles.Width) / 2);
                int belowY = panelCard.Bottom + 25;

                if (belowY + panelSavedProfiles.Height <= this.ClientSize.Height - 20)
                {
                    panelSavedProfiles.Location = new Point(centeredX, belowY);
                }
                else
                {
                    panelSavedProfiles.Location = new Point(
                        Math.Max(20, this.ClientSize.Width - panelSavedProfiles.Width - 20),
                        Math.Max(20, panelCard.Top)
                    );
                }
            }

            lblTitle.Location = new Point(
                Math.Max(20, (this.ClientSize.Width - lblTitle.Width) / 2),
                lblTitle.Top
            );

            lblSubtitle.Location = new Point(
                Math.Max(20, (this.ClientSize.Width - lblSubtitle.Width) / 2),
                lblSubtitle.Top
            );
        }

        private void BtnSaveProfile_Click(object sender, EventArgs e)
        {
            string host = txtHost.Text.Trim();
            string port = txtPort.Text.Trim();
            string username = txtUsername.Text.Trim();

            if (string.IsNullOrWhiteSpace(host))
            {
                ShowProfileStatus("Host fehlt.", false);
                return;
            }

            if (string.IsNullOrWhiteSpace(port))
            {
                ShowProfileStatus("Port fehlt.", false);
                return;
            }

            if (!int.TryParse(port, out _))
            {
                ShowProfileStatus("Port ist ungültig.", false);
                return;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                ShowProfileStatus("Benutzername fehlt.", false);
                return;
            }

            string realm = cmbRealm.SelectedIndex == 0 ? "pam" : "pve";
            bool ignoreSsl = chkIgnoreSsl.Checked;

            SavedConnectionProfile existingProfile = savedProfiles.FirstOrDefault(p =>
                string.Equals(p.Host, host, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(p.Port, port, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(p.Username, username, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(p.Realm, realm, StringComparison.OrdinalIgnoreCase)
            );

            if (existingProfile != null)
            {
                existingProfile.Host = host;
                existingProfile.Port = port;
                existingProfile.Username = username;
                existingProfile.Realm = realm;
                existingProfile.IgnoreSsl = ignoreSsl;
                existingProfile.UpdatedAt = DateTime.Now;
            }
            else
            {
                savedProfiles.Add(new SavedConnectionProfile
                {
                    Host = host,
                    Port = port,
                    Username = username,
                    Realm = realm,
                    IgnoreSsl = ignoreSsl,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }

            savedProfiles.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));

            SaveSavedProfilesToDisk();
            RefreshSavedProfilesList();
            profileList.SelectProfile(host, port, username, realm);

            ShowProfileStatus("Profil gespeichert.", true);
        }

        private void BtnLoadProfile_Click(object sender, EventArgs e)
        {
            LoadSelectedProfileIntoForm();
        }

        private void BtnDeleteProfile_Click(object sender, EventArgs e)
        {
            SavedConnectionProfile selectedProfile = profileList.SelectedProfile;

            if (selectedProfile == null)
            {
                ShowProfileStatus("Bitte erst ein Profil auswählen.", false);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Möchtest du dieses Profil wirklich löschen?\n\n" + selectedProfile.DisplayName,
                "Profil löschen",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes) return;

            savedProfiles.Remove(selectedProfile);
            SaveSavedProfilesToDisk();
            RefreshSavedProfilesList();

            ShowProfileStatus("Profil gelöscht.", true);
        }

        private void ProfileList_DoubleClick(object sender, EventArgs e)
        {
            LoadSelectedProfileIntoForm();
        }

        private void ProfileList_RenameRequested(object sender, ProfileRenameRequestedEventArgs e)
        {
            if (e == null || e.Profile == null)
            {
                ShowProfileStatus("Bitte erst ein Profil auswählen.", false);
                return;
            }

            SavedConnectionProfile profile = e.Profile;

            using (ProfileRenameDialog dialog = new ProfileRenameDialog(profile.DisplayName, profile.ConnectionText))
            {
                DialogResult result = dialog.ShowDialog(this);

                if (result != DialogResult.OK) return;

                profile.Name = dialog.ProfileName.Trim();
                profile.UpdatedAt = DateTime.Now;

                savedProfiles.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));

                SaveSavedProfilesToDisk();
                RefreshSavedProfilesList();
                profileList.SelectProfile(profile.Host, profile.Port, profile.Username, profile.Realm);

                ShowProfileStatus(
                    string.IsNullOrWhiteSpace(profile.Name) ? "Name zurückgesetzt." : "Profil umbenannt.",
                    true
                );
            }
        }

        private void LoadSelectedProfileIntoForm()
        {
            SavedConnectionProfile selectedProfile = profileList.SelectedProfile;

            if (selectedProfile == null)
            {
                ShowProfileStatus("Bitte erst ein Profil auswählen.", false);
                return;
            }

            txtHost.Text = selectedProfile.Host;
            txtPort.Text = selectedProfile.Port;
            txtUsername.Text = selectedProfile.Username;
            chkIgnoreSsl.Checked = selectedProfile.IgnoreSsl;

            if (string.Equals(selectedProfile.Realm, "pam", StringComparison.OrdinalIgnoreCase))
            {
                cmbRealm.SelectedIndex = 0;
            }
            else
            {
                cmbRealm.SelectedIndex = 1;
            }

            txtPassword.Text = "";
            txtPassword.Focus();

            ShowProfileStatus("Profil geladen. Passwort eingeben.", true);
        }

        private void RefreshSavedProfilesList()
        {
            profileList.SetProfiles(savedProfiles);
        }

        private void LoadSavedProfilesFromDisk()
        {
            try
            {
                savedProfiles.Clear();

                if (!File.Exists(ProfilesFile)) return;

                XmlSerializer serializer = new XmlSerializer(typeof(List<SavedConnectionProfile>));

                using (FileStream stream = new FileStream(ProfilesFile, FileMode.Open, FileAccess.Read))
                {
                    List<SavedConnectionProfile> loadedProfiles = serializer.Deserialize(stream) as List<SavedConnectionProfile>;

                    if (loadedProfiles != null)
                    {
                        savedProfiles.AddRange(loadedProfiles);
                    }
                }

                savedProfiles.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                savedProfiles.Clear();
            }
        }

        private void SaveSavedProfilesToDisk()
        {
            Directory.CreateDirectory(ProfilesFolder);

            XmlSerializer serializer = new XmlSerializer(typeof(List<SavedConnectionProfile>));

            using (FileStream stream = new FileStream(ProfilesFile, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, savedProfiles);
            }
        }

        private void LoadRememberLoginFromDisk()
        {
            rememberLoginState = null;

            try
            {
                if (!File.Exists(RememberLoginFile)) return;

                XmlSerializer serializer = new XmlSerializer(typeof(RememberLoginState));

                using (FileStream stream = new FileStream(RememberLoginFile, FileMode.Open, FileAccess.Read))
                {
                    rememberLoginState = serializer.Deserialize(stream) as RememberLoginState;
                }
            }
            catch
            {
                rememberLoginState = null;
            }
        }

        private bool ApplyRememberLoginToForm(bool showStatus)
        {
            if (rememberLoginState == null) return false;

            try
            {
                string password = UnprotectString(rememberLoginState.EncryptedPassword);

                if (string.IsNullOrWhiteSpace(rememberLoginState.Host)) return false;
                if (string.IsNullOrWhiteSpace(rememberLoginState.Port)) return false;
                if (string.IsNullOrWhiteSpace(rememberLoginState.Username)) return false;
                if (string.IsNullOrEmpty(password)) return false;

                txtHost.Text = rememberLoginState.Host;
                txtPort.Text = rememberLoginState.Port;
                txtUsername.Text = rememberLoginState.Username;
                txtPassword.Text = password;
                chkIgnoreSsl.Checked = rememberLoginState.IgnoreSsl;

                if (string.Equals(rememberLoginState.Realm, "pam", StringComparison.OrdinalIgnoreCase))
                {
                    cmbRealm.SelectedIndex = 0;
                }
                else
                {
                    cmbRealm.SelectedIndex = 1;
                }

                suppressRememberCheckboxEvent = true;
                chkStayLoggedIn.Checked = true;
                suppressRememberCheckboxEvent = false;

                rememberLoginApplied = true;

                if (showStatus)
                {
                    ShowProfileStatus("Gespeicherte Anmeldung geladen.", true);
                }

                return true;
            }
            catch
            {
                rememberLoginApplied = false;

                suppressRememberCheckboxEvent = true;
                chkStayLoggedIn.Checked = false;
                suppressRememberCheckboxEvent = false;

                DeleteRememberLoginFromDisk(false);

                if (showStatus)
                {
                    ShowProfileStatus("Gespeicherte Anmeldung konnte nicht geladen werden.", false);
                }

                return false;
            }
        }

        private void SaveRememberLoginToDisk(string host, string port, string username, string realm, bool ignoreSsl, string password)
        {
            Directory.CreateDirectory(ProfilesFolder);

            rememberLoginState = new RememberLoginState
            {
                Host = host,
                Port = port,
                Username = username,
                Realm = realm,
                IgnoreSsl = ignoreSsl,
                EncryptedPassword = ProtectString(password),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            XmlSerializer serializer = new XmlSerializer(typeof(RememberLoginState));

            using (FileStream stream = new FileStream(RememberLoginFile, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, rememberLoginState);
            }

            rememberLoginApplied = true;
        }

        private void DeleteRememberLoginFromDisk(bool showStatus)
        {
            try
            {
                if (File.Exists(RememberLoginFile))
                {
                    File.Delete(RememberLoginFile);
                }
            }
            catch
            {
                // Löschen ist optional. Login darf dadurch nicht blockiert werden.
            }

            rememberLoginState = null;
            rememberLoginApplied = false;

            if (showStatus)
            {
                ShowProfileStatus("Gespeicherte Anmeldung entfernt.", true);
            }
        }

        private string ProtectString(string plainText)
        {
            if (plainText == null) plainText = "";

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = ProtectedData.Protect(
                plainBytes,
                RememberEntropy,
                DataProtectionScope.CurrentUser
            );

            return Convert.ToBase64String(encryptedBytes);
        }

        private string UnprotectString(string encryptedBase64)
        {
            if (string.IsNullOrWhiteSpace(encryptedBase64)) return "";

            byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);
            byte[] plainBytes = ProtectedData.Unprotect(
                encryptedBytes,
                RememberEntropy,
                DataProtectionScope.CurrentUser
            );

            return Encoding.UTF8.GetString(plainBytes);
        }

        private void ShowProfileStatus(string message, bool success)
        {
            lblStatus.ForeColor = success
                ? Color.FromArgb(34, 197, 94)
                : Color.FromArgb(239, 68, 68);

            lblStatus.Text = message;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";

            string host = txtHost.Text.Trim();
            string portStr = txtPort.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            bool ignoreSsl = chkIgnoreSsl.Checked;

            if (string.IsNullOrEmpty(host))
            {
                lblStatus.Text = "Please enter Host IP / Domain.";
                return;
            }

            if (!int.TryParse(portStr, out int port))
            {
                lblStatus.Text = "Invalid port number.";
                return;
            }

            if (string.IsNullOrEmpty(username))
            {
                lblStatus.Text = "Please enter username.";
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                lblStatus.Text = "Please enter password.";
                return;
            }

            string realm = cmbRealm.SelectedIndex == 0 ? "pam" : "pve";

            btnLogin.Enabled = false;
            btnLogin.Text = "Connecting...";
            lblStatus.ForeColor = Color.FromArgb(148, 163, 184);
            lblStatus.Text = "Authenticating with Proxmox VE...";

            var client = new ProxmoxClient(host, port, ignoreSsl);
            bool success = await client.LoginAsync(username, password, realm);

            if (success)
            {
                if (chkStayLoggedIn != null && chkStayLoggedIn.Checked)
                {
                    SaveRememberLoginToDisk(host, portStr, username, realm, ignoreSsl, password);
                }
                else
                {
                    DeleteRememberLoginFromDisk(false);
                }

                lblStatus.ForeColor = Color.FromArgb(34, 197, 94);
                lblStatus.Text = "Login successful!";

                this.Hide();
                var mainForm = new MainForm(client);
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
            }
            else
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Mit Host verbinden";
                lblStatus.ForeColor = Color.FromArgb(239, 68, 68);
                lblStatus.Text = "Authentication failed. Check details.";
            }
        }

        private class ProfileListControl : Control
        {
            private readonly List<SavedConnectionProfile> profiles = new List<SavedConnectionProfile>();

            private int selectedIndex = -1;
            private int hoverIndex = -1;
            private int renameHoverIndex = -1;
            private int scrollOffset = 0;

            public event EventHandler<ProfileRenameRequestedEventArgs> RenameRequested;

            public Color BorderColor { get; set; } = Color.FromArgb(75, 85, 99);
            public int BorderRadius { get; set; } = 13;

            private const int PaddingSize = 10;
            private const int ItemHeight = 64;

            public SavedConnectionProfile SelectedProfile
            {
                get
                {
                    if (selectedIndex < 0 || selectedIndex >= profiles.Count) return null;
                    return profiles[selectedIndex];
                }
            }

            public ProfileListControl()
            {
                this.SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.Selectable,
                    true
                );

                this.TabStop = true;
                this.Cursor = Cursors.Hand;
                this.BackColor = Color.FromArgb(12, 18, 31);
            }

            public void SetProfiles(IEnumerable<SavedConnectionProfile> values)
            {
                profiles.Clear();

                if (values != null)
                {
                    profiles.AddRange(values);
                }

                if (profiles.Count == 0)
                {
                    selectedIndex = -1;
                    hoverIndex = -1;
                    renameHoverIndex = -1;
                    scrollOffset = 0;
                }
                else
                {
                    if (selectedIndex >= profiles.Count) selectedIndex = profiles.Count - 1;
                    if (selectedIndex < 0) selectedIndex = 0;
                    ClampScroll();
                }

                Invalidate();
            }

            public void SelectProfile(string host, string port, string username, string realm)
            {
                for (int i = 0; i < profiles.Count; i++)
                {
                    SavedConnectionProfile profile = profiles[i];

                    if (
                        string.Equals(profile.Host, host, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(profile.Port, port, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(profile.Username, username, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(profile.Realm, realm, StringComparison.OrdinalIgnoreCase)
                    )
                    {
                        selectedIndex = i;
                        EnsureSelectedVisible();
                        Invalidate();
                        return;
                    }
                }
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                ClampScroll();
                Invalidate();
            }

            protected override void OnMouseWheel(MouseEventArgs e)
            {
                base.OnMouseWheel(e);

                if (profiles.Count <= GetVisibleItemCount()) return;

                if (e.Delta < 0)
                {
                    scrollOffset++;
                }
                else if (e.Delta > 0)
                {
                    scrollOffset--;
                }

                ClampScroll();
                Invalidate();
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);

                int newHoverIndex = HitTest(e.Location);
                int newRenameHoverIndex = HitTestRenameButton(e.Location);

                if (newHoverIndex != hoverIndex || newRenameHoverIndex != renameHoverIndex)
                {
                    hoverIndex = newHoverIndex;
                    renameHoverIndex = newRenameHoverIndex;
                    Invalidate();
                }

                this.Cursor = newHoverIndex >= 0 ? Cursors.Hand : Cursors.Default;
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);

                if (hoverIndex != -1 || renameHoverIndex != -1)
                {
                    hoverIndex = -1;
                    renameHoverIndex = -1;
                    Invalidate();
                }

                this.Cursor = Cursors.Hand;
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                this.Focus();

                int renameIndex = HitTestRenameButton(e.Location);

                if (renameIndex >= 0 && renameIndex < profiles.Count)
                {
                    selectedIndex = renameIndex;
                    EnsureSelectedVisible();
                    Invalidate();

                    RenameRequested?.Invoke(
                        this,
                        new ProfileRenameRequestedEventArgs(profiles[renameIndex])
                    );

                    return;
                }

                int hitIndex = HitTest(e.Location);

                if (hitIndex >= 0 && hitIndex < profiles.Count)
                {
                    selectedIndex = hitIndex;
                    EnsureSelectedVisible();
                    Invalidate();
                }
            }

            private int HitTest(Point point)
            {
                int visibleCount = GetVisibleItemCount();

                for (int visibleIndex = 0; visibleIndex < visibleCount; visibleIndex++)
                {
                    int realIndex = scrollOffset + visibleIndex;

                    if (realIndex < 0 || realIndex >= profiles.Count) continue;

                    Rectangle itemRect = GetItemRectangle(visibleIndex);

                    if (itemRect.Contains(point))
                    {
                        return realIndex;
                    }
                }

                return -1;
            }

            private int HitTestRenameButton(Point point)
            {
                int visibleCount = GetVisibleItemCount();

                for (int visibleIndex = 0; visibleIndex < visibleCount; visibleIndex++)
                {
                    int realIndex = scrollOffset + visibleIndex;

                    if (realIndex < 0 || realIndex >= profiles.Count) continue;

                    Rectangle itemRect = GetItemRectangle(visibleIndex);
                    Rectangle renameRect = GetRenameButtonRectangle(itemRect);

                    if (renameRect.Contains(point))
                    {
                        return realIndex;
                    }
                }

                return -1;
            }

            private Rectangle GetItemRectangle(int visibleIndex)
            {
                return new Rectangle(
                    PaddingSize,
                    PaddingSize + (visibleIndex * ItemHeight),
                    this.Width - (PaddingSize * 2),
                    ItemHeight - 8
                );
            }

            private Rectangle GetRenameButtonRectangle(Rectangle itemRect)
            {
                return new Rectangle(
                    itemRect.Right - 40,
                    itemRect.Y + 13,
                    28,
                    28
                );
            }

            private int GetVisibleItemCount()
            {
                int usableHeight = Math.Max(1, this.Height - (PaddingSize * 2));
                return Math.Max(1, usableHeight / ItemHeight);
            }

            private int GetMaxScrollOffset()
            {
                return Math.Max(0, profiles.Count - GetVisibleItemCount());
            }

            private void ClampScroll()
            {
                int max = GetMaxScrollOffset();

                if (scrollOffset < 0) scrollOffset = 0;
                if (scrollOffset > max) scrollOffset = max;
            }

            private void EnsureSelectedVisible()
            {
                if (selectedIndex < 0) return;

                int visibleCount = GetVisibleItemCount();

                if (selectedIndex < scrollOffset)
                {
                    scrollOffset = selectedIndex;
                }
                else if (selectedIndex >= scrollOffset + visibleCount)
                {
                    scrollOffset = selectedIndex - visibleCount + 1;
                }

                ClampScroll();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Rectangle outerRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

                using (GraphicsPath outerPath = GetProfileRoundedPath(outerRect, BorderRadius))
                using (SolidBrush backgroundBrush = new SolidBrush(this.BackColor))
                using (Pen borderPen = new Pen(BorderColor, 1F))
                {
                    e.Graphics.FillPath(backgroundBrush, outerPath);
                    e.Graphics.DrawPath(borderPen, outerPath);
                }

                Rectangle contentRect = new Rectangle(1, 1, this.Width - 2, this.Height - 2);

                using (GraphicsPath clipPath = GetProfileRoundedPath(contentRect, BorderRadius - 1))
                {
                    Region oldClip = e.Graphics.Clip;
                    e.Graphics.SetClip(clipPath);

                    if (profiles.Count == 0)
                    {
                        DrawEmptyText(e.Graphics);
                    }
                    else
                    {
                        DrawProfiles(e.Graphics);
                    }

                    e.Graphics.Clip = oldClip;
                }

                DrawScrollbar(e.Graphics);
            }

            private void DrawEmptyText(Graphics g)
            {
                Rectangle textRect = new Rectangle(14, 0, this.Width - 28, this.Height);

                using (Font emptyFont = new Font("Segoe UI", 9.5F))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    using (StringFormat sf = new StringFormat())
                    {
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Trimming = StringTrimming.EllipsisCharacter;
                        sf.FormatFlags = StringFormatFlags.NoWrap;

                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(156, 163, 175)))
                        {
                            g.DrawString("Keine Profile gespeichert", emptyFont, brush, textRect, sf);
                        }
                    }
                }
            }

            private void DrawProfiles(Graphics g)
            {
                int visibleCount = GetVisibleItemCount();

                for (int visibleIndex = 0; visibleIndex < visibleCount; visibleIndex++)
                {
                    int realIndex = scrollOffset + visibleIndex;

                    if (realIndex < 0 || realIndex >= profiles.Count) continue;

                    SavedConnectionProfile profile = profiles[realIndex];

                    bool selected = realIndex == selectedIndex;
                    bool hover = realIndex == hoverIndex;
                    bool renameHover = realIndex == renameHoverIndex;

                    Rectangle itemRect = GetItemRectangle(visibleIndex);
                    Rectangle renameRect = GetRenameButtonRectangle(itemRect);

                    Color backColor;

                    if (selected)
                    {
                        backColor = Color.FromArgb(249, 115, 22);
                    }
                    else if (hover)
                    {
                        backColor = Color.FromArgb(31, 41, 55);
                    }
                    else
                    {
                        backColor = Color.FromArgb(12, 18, 31);
                    }

                    Color borderColor = selected
                        ? Color.FromArgb(251, 146, 60)
                        : hover
                            ? Color.FromArgb(75, 85, 99)
                            : Color.FromArgb(31, 41, 55);

                    Color titleColor = Color.White;
                    Color subtitleColor = selected
                        ? Color.FromArgb(255, 237, 213)
                        : Color.FromArgb(156, 163, 175);

                    if (selected || hover)
                    {
                        RoundedButton.DrawLiquidGlass(g, itemRect, backColor, 6, hover && !selected, false, true);
                    }
                    else
                    {
                        using (GraphicsPath itemPath = GetProfileRoundedPath(itemRect, 6))
                        using (SolidBrush itemBrush = new SolidBrush(backColor))
                        using (Pen itemPen = new Pen(borderColor, 1F))
                        {
                            g.FillPath(itemBrush, itemPath);
                            g.DrawPath(itemPen, itemPath);
                        }
                    }

                    DrawRenameButton(g, renameRect, selected, renameHover);

                    Rectangle titleRect = new Rectangle(itemRect.X + 14, itemRect.Y + 7, itemRect.Width - 64, 18);
                    Rectangle subtitleRect = new Rectangle(itemRect.X + 14, itemRect.Y + 25, itemRect.Width - 64, 16);

                    using (Font titleFont = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold))
                    using (Font subtitleFont = new Font("Segoe UI", 8.5F))
                    {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                        using (StringFormat sf = new StringFormat())
                        {
                            sf.Alignment = StringAlignment.Near;
                            sf.LineAlignment = StringAlignment.Center;
                            sf.Trimming = StringTrimming.EllipsisCharacter;
                            sf.FormatFlags = StringFormatFlags.NoWrap;

                            using (SolidBrush titleBrush = new SolidBrush(titleColor))
                            {
                                g.DrawString(profile.DisplayName, titleFont, titleBrush, titleRect, sf);
                            }
                            using (SolidBrush subtitleBrush = new SolidBrush(subtitleColor))
                            {
                                g.DrawString(profile.SubTitle, subtitleFont, subtitleBrush, subtitleRect, sf);
                            }
                        }
                    }
                }
            }

            private void DrawRenameButton(Graphics g, Rectangle rect, bool selected, bool hover)
            {
                Color buttonBackColor;

                if (hover)
                {
                    buttonBackColor = selected
                        ? Color.FromArgb(234, 88, 12)
                        : Color.FromArgb(55, 65, 81);
                }
                else
                {
                    buttonBackColor = selected
                        ? Color.FromArgb(194, 65, 12)
                        : Color.FromArgb(17, 24, 39);
                }

                Color iconColor = selected
                    ? Color.White
                    : Color.FromArgb(209, 213, 219);

                using (GraphicsPath path = GetProfileRoundedPath(rect, 4))
                using (SolidBrush brush = new SolidBrush(buttonBackColor))
                using (Pen pen = new Pen(hover ? Color.FromArgb(251, 146, 60) : Color.FromArgb(75, 85, 99), 1F))
                {
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }

                using (Font iconFont = new Font("Segoe UI Symbol", 10.5F, FontStyle.Bold))
                {
                    TextRenderer.DrawText(
                        g,
                        "✎",
                        iconFont,
                        rect,
                        iconColor,
                        TextFormatFlags.HorizontalCenter |
                        TextFormatFlags.VerticalCenter |
                        TextFormatFlags.NoPadding
                    );
                }
            }

            private void DrawScrollbar(Graphics g)
            {
                int visibleCount = GetVisibleItemCount();

                if (profiles.Count <= visibleCount) return;

                int trackX = this.Width - 7;
                int trackY = PaddingSize + 4;
                int trackHeight = this.Height - ((PaddingSize + 4) * 2);

                if (trackHeight <= 20) return;

                int thumbHeight = Math.Max(24, (int)(trackHeight * (visibleCount / (float)profiles.Count)));
                int maxScroll = GetMaxScrollOffset();

                int thumbY = trackY;

                if (maxScroll > 0)
                {
                    thumbY = trackY + (int)((trackHeight - thumbHeight) * (scrollOffset / (float)maxScroll));
                }

                using (SolidBrush trackBrush = new SolidBrush(Color.FromArgb(17, 24, 39)))
                using (SolidBrush thumbBrush = new SolidBrush(Color.FromArgb(75, 85, 99)))
                {
                    Rectangle trackRect = new Rectangle(trackX, trackY, 3, trackHeight);
                    Rectangle thumbRect = new Rectangle(trackX, thumbY, 3, thumbHeight);

                    using (GraphicsPath trackPath = GetProfileRoundedPath(trackRect, 2))
                    using (GraphicsPath thumbPath = GetProfileRoundedPath(thumbRect, 2))
                    {
                        g.FillPath(trackBrush, trackPath);
                        g.FillPath(thumbBrush, thumbPath);
                    }
                }
            }
        }

        private class ProfileRenameRequestedEventArgs : EventArgs
        {
            public SavedConnectionProfile Profile { get; private set; }

            public ProfileRenameRequestedEventArgs(SavedConnectionProfile profile)
            {
                Profile = profile;
            }
        }

        private class ProfileRenameDialog : Form
        {
            private readonly TextBox txtName;
            private readonly Button btnOk;
            private readonly Button btnCancel;

            public string ProfileName
            {
                get
                {
                    return txtName.Text ?? "";
                }
            }

            public ProfileRenameDialog(string currentName, string connectionText)
            {
                this.Text = "Profil umbenennen";
                this.StartPosition = FormStartPosition.CenterParent;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.ShowIcon = false;
                this.ShowInTaskbar = false;
                this.ClientSize = new Size(420, 205);
                this.BackColor = Color.FromArgb(17, 24, 39);
                this.Font = new Font("Segoe UI", 10F);

                Label lblTitle = new Label();
                lblTitle.AutoSize = false;
                lblTitle.Location = new Point(24, 20);
                lblTitle.Size = new Size(372, 28);
                lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
                lblTitle.ForeColor = Color.White;
                lblTitle.Text = "Host benennen";

                Label lblConnection = new Label();
                lblConnection.AutoSize = false;
                lblConnection.Location = new Point(24, 52);
                lblConnection.Size = new Size(372, 22);
                lblConnection.Font = new Font("Segoe UI", 9F);
                lblConnection.ForeColor = Color.FromArgb(156, 163, 175);
                lblConnection.Text = connectionText;

                Label lblName = new Label();
                lblName.AutoSize = true;
                lblName.Location = new Point(24, 86);
                lblName.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
                lblName.ForeColor = Color.FromArgb(229, 231, 235);
                lblName.Text = "Anzeigename";

                txtName = new TextBox();
                txtName.Location = new Point(24, 112);
                txtName.Size = new Size(372, 28);
                txtName.BorderStyle = BorderStyle.FixedSingle;
                txtName.BackColor = Color.FromArgb(12, 18, 31);
                txtName.ForeColor = Color.White;
                txtName.Font = new Font("Segoe UI", 10.5F);
                txtName.Text = currentName ?? "";

                btnOk = new Button();
                btnOk.Location = new Point(224, 158);
                btnOk.Size = new Size(82, 34);
                btnOk.FlatStyle = FlatStyle.Flat;
                btnOk.FlatAppearance.BorderSize = 0;
                btnOk.BackColor = Color.FromArgb(249, 115, 22);
                btnOk.ForeColor = Color.White;
                btnOk.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
                btnOk.Text = "Speichern";
                btnOk.DialogResult = DialogResult.OK;

                btnCancel = new Button();
                btnCancel.Location = new Point(314, 158);
                btnCancel.Size = new Size(82, 34);
                btnCancel.FlatStyle = FlatStyle.Flat;
                btnCancel.FlatAppearance.BorderColor = Color.FromArgb(75, 85, 99);
                btnCancel.FlatAppearance.BorderSize = 1;
                btnCancel.BackColor = Color.FromArgb(31, 41, 55);
                btnCancel.ForeColor = Color.White;
                btnCancel.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
                btnCancel.Text = "Abbrechen";
                btnCancel.DialogResult = DialogResult.Cancel;

                this.Controls.Add(lblTitle);
                this.Controls.Add(lblConnection);
                this.Controls.Add(lblName);
                this.Controls.Add(txtName);
                this.Controls.Add(btnOk);
                this.Controls.Add(btnCancel);

                this.AcceptButton = btnOk;
                this.CancelButton = btnCancel;

                this.Shown += (s, e) =>
                {
                    txtName.Focus();
                    txtName.SelectAll();
                };
            }
        }

        // ProfileRoundedPanel replaced by global GlassPanel

        private class ProfileActionButton : Control
        {
            public int BorderRadius { get; set; } = 10;
            public Color NormalColor { get; set; } = Color.FromArgb(31, 41, 55);
            public Color HoverColor { get; set; } = Color.FromArgb(55, 65, 81);
            public Color DownColor { get; set; } = Color.FromArgb(75, 85, 99);

            private bool isHover = false;
            private bool isDown = false;

            public ProfileActionButton()
            {
                this.SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.Selectable,
                    true
                );

                this.TabStop = true;
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                isHover = true;
                this.Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                isHover = false;
                isDown = false;
                this.Invalidate();
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                this.Focus();

                if (e.Button == MouseButtons.Left)
                {
                    isDown = true;
                    this.Invalidate();
                }
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);

                if (isDown && e.Button == MouseButtons.Left)
                {
                    isDown = false;
                    this.Invalidate();

                    if (this.ClientRectangle.Contains(e.Location))
                    {
                        this.OnClick(EventArgs.Empty);
                    }
                }
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                base.OnKeyDown(e);

                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                {
                    this.OnClick(EventArgs.Empty);
                    e.Handled = true;
                }
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);

                if (this.Width > 0 && this.Height > 0)
                {
                    using (GraphicsPath path = GetProfileRoundedPath(new Rectangle(0, 0, this.Width, this.Height), BorderRadius))
                    {
                        this.Region = new Region(path);
                    }
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Color baseColor = isDown ? DownColor : isHover ? HoverColor : NormalColor;

                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    RoundedButton.DrawLiquidGlass(e.Graphics, rect, baseColor, BorderRadius, isHover, isDown, this.Enabled);
                }

                TextRenderer.DrawText(
                    e.Graphics,
                    this.Text,
                    this.Font,
                    new Rectangle(0, 0, this.Width, this.Height),
                    this.ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
                );
            }
        }

        private static GraphicsPath GetProfileRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            int diameter = radius * 2;

            if (diameter > rect.Width) diameter = rect.Width;
            if (diameter > rect.Height) diameter = rect.Height;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }

    public class SavedConnectionProfile
    {
        public string Name { get; set; } = "";
        public string Host { get; set; } = "";
        public string Port { get; set; } = "8006";
        public string Username { get; set; } = "";
        public string Realm { get; set; } = "pam";
        public bool IgnoreSsl { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [XmlIgnore]
        public string ConnectionText
        {
            get
            {
                return Host + ":" + Port;
            }
        }

        [XmlIgnore]
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Name))
                {
                    return Name.Trim();
                }

                return ConnectionText;
            }
        }

        [XmlIgnore]
        public string SubTitle
        {
            get
            {
                return ConnectionText + " · " + Username + "@" + Realm + " · SSL ignorieren: " + (IgnoreSsl ? "Ja" : "Nein");
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public class RememberLoginState
    {
        public string Host { get; set; } = "";
        public string Port { get; set; } = "8006";
        public string Username { get; set; } = "";
        public string Realm { get; set; } = "pam";
        public bool IgnoreSsl { get; set; } = true;
        public string EncryptedPassword { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}