using System;
using System.Windows.Forms;

namespace ProxmoxVEGui
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            cmbRealm.SelectedIndex = 0; // Default to pam
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            // Reset status
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
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            lblStatus.Text = "Authenticating with Proxmox VE...";

            var client = new ProxmoxClient(host, port, ignoreSsl);
            bool success = await client.LoginAsync(username, password, realm);

            if (success)
            {
                lblStatus.ForeColor = System.Drawing.Color.FromArgb(34, 197, 94);
                lblStatus.Text = "Login successful!";

                // Hide login form and show main form
                this.Hide();
                var mainForm = new MainForm(client);
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
            }
            else
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Connect to Host";
                lblStatus.ForeColor = System.Drawing.Color.FromArgb(239, 68, 68);
                lblStatus.Text = "Authentication failed. Check details.";
            }
        }
    }
}
