using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProxmoxVEGui
{
    /// <summary>
    /// A self-contained configuration panel that loads VM or LXC config from Proxmox,
    /// displays it in editable fields, and submits changes via the API.
    /// </summary>
    public class ConfigPanel : Panel
    {
        private readonly ProxmoxClient _client;

        // Header
        private Label lblConfigTitle;
        private Label lblConfigSubtitle;
        private Button btnApply;
        private Button btnReload;

        // Content scrollable area
        private Panel panelScroll;

        // Section labels
        private Label lblSectionGeneral;
        private Label lblSectionHardware;
        private Label lblSectionNetwork;
        private Label lblSectionBoot;

        // --- General ---
        private Label lblCfgName;      private TextBox txtCfgName;
        private Label lblCfgDesc;      private TextBox txtCfgDesc;
        private Label lblCfgOnboot;    private CheckBox chkCfgOnboot;
        private Label lblCfgProtect;   private CheckBox chkCfgProtect;
        private Label lblCfgTags;      private TextBox txtCfgTags;

        // --- Hardware (VM) ---
        private Label lblCfgCores;     private NumericUpDown numCfgCores;
        private Label lblCfgSockets;   private NumericUpDown numCfgSockets;
        private Label lblCfgCpuType;   private ComboBox cmbCfgCpuType;
        private Label lblCfgMemory;    private NumericUpDown numCfgMemory;
        private Label lblCfgBalloon;   private NumericUpDown numCfgBalloon;
        private Label lblCfgVga;       private ComboBox cmbCfgVga;
        private Label lblCfgAgent;     private CheckBox chkCfgAgent;

        // --- Hardware (LXC specific) ---
        private Label lblCfgLxcCores;   private NumericUpDown numCfgLxcCores;
        private Label lblCfgLxcMemory;  private NumericUpDown numCfgLxcMemory;
        private Label lblCfgLxcSwap;    private NumericUpDown numCfgLxcSwap;
        private Label lblCfgPriv;       private CheckBox chkCfgPriv;
        private Label lblCfgNesting;    private CheckBox chkCfgNesting;

        // --- Network ---
        private Label lblCfgNetBridge; private ComboBox cmbCfgNetBridge;
        private Label lblCfgNetModel;  private ComboBox cmbCfgNetModel;
        private Label lblCfgNetVlan;   private NumericUpDown numCfgNetVlan;
        private Label lblCfgNetFw;     private CheckBox chkCfgNetFw;

        // --- Boot ---
        private Label lblCfgBootOrder; private TextBox txtCfgBootOrder;

        // State
        private string _currentNode;
        private int _currentVmId;
        private string _currentType; // "vm" or "lxc"
        private Dictionary<string, object> _lastConfig = new Dictionary<string, object>();

        public ConfigPanel(ProxmoxClient client)
        {
            _client = client;
            BuildUI();
        }

        private void BuildUI()
        {
            this.BackColor = Color.FromArgb(15, 23, 42);
            this.Dock = DockStyle.Fill;

            // ─── Header bar ───────────────────────────────────────────────
            var panelHeader = new Panel
            {
                Height = 55,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(15, 0, 15, 0)
            };

            lblConfigTitle = new Label
            {
                Text = "Configuration",
                Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(249, 115, 22),
                AutoSize = false,
                Location = new Point(15, 8),
                Size = new Size(400, 24)
            };

            lblConfigSubtitle = new Label
            {
                Text = "Select a VM or Container to edit its configuration.",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = false,
                Location = new Point(15, 32),
                Size = new Size(500, 18)
            };

            btnApply = MakeButton("💾  Apply Changes", Color.FromArgb(34, 197, 94), new Point(680, 12));
            btnApply.Size = new Size(150, 32);
            btnApply.Enabled = false;
            btnApply.Click += async (s, e) => await ApplyChangesAsync();

            btnReload = MakeButton("↺  Reload", Color.FromArgb(71, 85, 105), new Point(530, 12));
            btnReload.Size = new Size(140, 32);
            btnReload.Enabled = false;
            btnReload.Click += async (s, e) => await LoadConfigAsync(_currentNode, _currentVmId, _currentType);

            panelHeader.Controls.Add(lblConfigTitle);
            panelHeader.Controls.Add(lblConfigSubtitle);
            panelHeader.Controls.Add(btnApply);
            panelHeader.Controls.Add(btnReload);
            this.Controls.Add(panelHeader);

            // ─── Scrollable content area ──────────────────────────────────
            panelScroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20, 15, 20, 20)
            };
            this.Controls.Add(panelScroll);

            int y = 5;

            // ─── Section: General ─────────────────────────────────────────
            y = AddSectionHeader(ref lblSectionGeneral, "⚙️  General", y);

            (lblCfgName, txtCfgName) = AddTextRow("Name / Hostname", y); y += 42;
            (lblCfgDesc, txtCfgDesc) = AddTextRow("Description", y); y += 42;
            (lblCfgTags, txtCfgTags) = AddTextRow("Tags  (comma separated)", y); y += 42;

            lblCfgOnboot = AddLabel("Start at boot", y);
            chkCfgOnboot = AddCheckBox(y); y += 42;

            lblCfgProtect = AddLabel("Protection mode", y);
            chkCfgProtect = AddCheckBox(y); y += 55;

            // ─── Section: Hardware (VM) ───────────────────────────────────
            y = AddSectionHeader(ref lblSectionHardware, "🖥️  Hardware", y);

            lblCfgSockets = AddLabel("CPU Sockets", y);
            numCfgSockets = AddNumericUpDown(y, 1, 32, 1); y += 42;

            lblCfgCores = AddLabel("CPU Cores", y);
            numCfgCores = AddNumericUpDown(y, 1, 512, 1); y += 42;

            lblCfgCpuType = AddLabel("CPU Type", y);
            cmbCfgCpuType = AddComboBox(y,
                new[] { "host", "kvm64", "max", "qemu64", "x86-64-v2-AES" }); y += 42;

            lblCfgMemory = AddLabel("Memory (MB)", y);
            numCfgMemory = AddNumericUpDown(y, 16, 2097152, 512); y += 42;

            lblCfgBalloon = AddLabel("Balloon min. (MB)", y);
            numCfgBalloon = AddNumericUpDown(y, 0, 2097152, 512); y += 42;

            lblCfgVga = AddLabel("Display (VGA)", y);
            cmbCfgVga = AddComboBox(y,
                new[] { "std", "cirrus", "vmware", "qxl", "serial0", "none" }); y += 42;

            lblCfgAgent = AddLabel("QEMU Guest Agent", y);
            chkCfgAgent = AddCheckBox(y); y += 55;

            // ─── Section: Hardware (LXC) ──────────────────────────────────
            lblCfgLxcCores = AddLabel("CPU Cores (LXC)", y);
            numCfgLxcCores = AddNumericUpDown(y, 1, 512, 1); y += 42;

            lblCfgLxcMemory = AddLabel("Memory (MB, LXC)", y);
            numCfgLxcMemory = AddNumericUpDown(y, 16, 2097152, 256); y += 42;

            lblCfgLxcSwap = AddLabel("Swap (MB)", y);
            numCfgLxcSwap = AddNumericUpDown(y, 0, 65536, 256); y += 42;

            lblCfgPriv = AddLabel("Unprivileged container", y);
            chkCfgPriv = AddCheckBox(y); y += 42;

            lblCfgNesting = AddLabel("Nesting  (Docker inside LXC)", y);
            chkCfgNesting = AddCheckBox(y); y += 55;

            // ─── Section: Network ─────────────────────────────────────────
            y = AddSectionHeader(ref lblSectionNetwork, "🌐  Network  (net0)", y);

            lblCfgNetModel = AddLabel("Model", y);
            cmbCfgNetModel = AddComboBox(y,
                new[] { "virtio", "e1000", "rtl8139", "vmxnet3" }); y += 42;

            lblCfgNetBridge = AddLabel("Bridge", y);
            cmbCfgNetBridge = AddComboBox(y,
                new[] { "vmbr0", "vmbr1", "vmbr2", "vmbr3" }); y += 42;

            lblCfgNetVlan = AddLabel("VLAN Tag  (0 = none)", y);
            numCfgNetVlan = AddNumericUpDown(y, 0, 4094, 0); y += 42;

            lblCfgNetFw = AddLabel("Firewall", y);
            chkCfgNetFw = AddCheckBox(y); y += 55;

            // ─── Section: Boot ────────────────────────────────────────────
            y = AddSectionHeader(ref lblSectionBoot, "🚀  Boot Order", y);

            (lblCfgBootOrder, txtCfgBootOrder) = AddTextRow("Boot order  (e.g. order=scsi0;net0)", y); y += 55;

            // Set total height of scrollable content
            panelScroll.AutoScrollMinSize = new Size(0, y + 40);

            HideAllFields();
        }

        // ─── Public: load config for a resource ───────────────────────────────
        public async Task LoadConfigAsync(string node, int vmid, string type)
        {
            _currentNode = node;
            _currentVmId = vmid;
            _currentType = type;

            lblConfigSubtitle.Text = $"Loading config for {type.ToUpper()} {vmid} on node {node}...";
            SetFieldsEnabled(false);

            try
            {
                _lastConfig = await _client.GetConfigAsync(node, vmid, type);
                PopulateFields(type);

                lblConfigSubtitle.Text = $"{type.ToUpper()} {vmid}  —  node: {node}  —  editable configuration";
                btnApply.Enabled = true;
                btnReload.Enabled = true;
                SetFieldsEnabled(true);
            }
            catch (Exception ex)
            {
                lblConfigSubtitle.Text = $"⚠️  Failed to load config: {ex.Message}";
                btnApply.Enabled = false;
                btnReload.Enabled = true;
            }
        }

        // ─── Populate UI fields from API response ──────────────────────────────
        private void PopulateFields(string type)
        {
            HideAllFields();

            string V(string key) => _lastConfig.ContainsKey(key) ? _lastConfig[key]?.ToString() ?? "" : "";
            bool B(string key) => V(key) == "1";
            int I(string key, int def = 0) => int.TryParse(V(key), out int v) ? v : def;

            // ─ General ─
            ShowRow(lblCfgName, txtCfgName);    txtCfgName.Text = type == "vm" ? V("name") : V("hostname");
            ShowRow(lblCfgDesc, txtCfgDesc);    txtCfgDesc.Text = V("description");
            ShowRow(lblCfgTags, txtCfgTags);    txtCfgTags.Text = V("tags");
            ShowRow(lblCfgOnboot, chkCfgOnboot); chkCfgOnboot.Checked = B("onboot");
            ShowRow(lblCfgProtect, chkCfgProtect); chkCfgProtect.Checked = B("protection");

            if (type == "vm")
            {
                // ─ VM Hardware ─
                ShowRow(lblCfgSockets, numCfgSockets);  numCfgSockets.Value = Math.Max(1, I("sockets", 1));
                ShowRow(lblCfgCores, numCfgCores);      numCfgCores.Value = Math.Max(1, I("cores", 1));
                ShowComboRow(lblCfgCpuType, cmbCfgCpuType, ParseField(V("cpu"), 0));
                ShowRow(lblCfgMemory, numCfgMemory);    numCfgMemory.Value = Math.Max(16, I("memory", 512));
                ShowRow(lblCfgBalloon, numCfgBalloon);  numCfgBalloon.Value = I("balloon", 0);
                ShowComboRow(lblCfgVga, cmbCfgVga, ParseVgaType(V("vga")));
                ShowRow(lblCfgAgent, chkCfgAgent);      chkCfgAgent.Checked = V("agent").StartsWith("1");

                // ─ Network (net0) ─
                ParseAndShowNet0(V("net0"), "vm");
                ShowTextRow(lblCfgBootOrder, txtCfgBootOrder, V("boot"));
            }
            else // lxc
            {
                // ─ LXC Hardware ─
                ShowRow(lblCfgLxcCores, numCfgLxcCores);   numCfgLxcCores.Value = Math.Max(1, I("cores", 1));
                ShowRow(lblCfgLxcMemory, numCfgLxcMemory); numCfgLxcMemory.Value = Math.Max(16, I("memory", 512));
                ShowRow(lblCfgLxcSwap, numCfgLxcSwap);     numCfgLxcSwap.Value = I("swap", 0);
                ShowRow(lblCfgPriv, chkCfgPriv);            chkCfgPriv.Checked = B("unprivileged");

                // features: nesting=1
                string features = V("features");
                ShowRow(lblCfgNesting, chkCfgNesting);
                chkCfgNesting.Checked = features.Contains("nesting=1");

                // ─ Network (net0) ─
                ParseAndShowNet0(V("net0"), "lxc");
            }
        }

        private void ParseAndShowNet0(string net0, string type)
        {
            // net0 looks like: "virtio=AA:BB:CC:DD:EE:FF,bridge=vmbr0,firewall=1,tag=100"
            var parts = net0.Split(',').Select(p => p.Trim()).ToList();

            string model = "";
            string bridge = "vmbr0";
            int vlan = 0;
            bool fw = false;

            foreach (var part in parts)
            {
                if (part.StartsWith("bridge=")) bridge = part.Substring(7);
                else if (part.StartsWith("firewall=")) fw = part.EndsWith("1");
                else if (part.StartsWith("tag=")) int.TryParse(part.Substring(4), out vlan);
                else if (part.Contains("=") && !part.StartsWith("ip=") && !part.StartsWith("rate="))
                {
                    // model is e.g. "virtio=AA:BB..." or "e1000=AA:..."
                    model = part.Split('=')[0];
                }
                // LXC net0 format: "name=eth0,bridge=vmbr0,ip=dhcp"
                else if (type == "lxc" && part.StartsWith("name="))
                {
                    // skip
                }
            }

            ShowComboRow(lblCfgNetModel, cmbCfgNetModel, model);
            ShowComboRow(lblCfgNetBridge, cmbCfgNetBridge, bridge);
            ShowRow(lblCfgNetVlan, numCfgNetVlan); numCfgNetVlan.Value = vlan;
            ShowRow(lblCfgNetFw, chkCfgNetFw); chkCfgNetFw.Checked = fw;
        }

        private string ParseField(string raw, int partIndex)
        {
            if (string.IsNullOrEmpty(raw)) return "";
            var parts = raw.Split(',');
            return parts.Length > partIndex ? parts[partIndex].Split('=')[0].Trim() : raw;
        }

        private string ParseVgaType(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return "std";
            return raw.Split(',')[0].Split('=').Last().Trim();
        }

        // ─── Apply changes back to Proxmox ────────────────────────────────────
        private async Task ApplyChangesAsync()
        {
            btnApply.Enabled = false;
            btnApply.Text = "Saving...";

            try
            {
                var parameters = new Dictionary<string, string>();

                if (_currentType == "vm")
                {
                    parameters["name"] = txtCfgName.Text.Trim();
                    parameters["description"] = txtCfgDesc.Text.Trim();
                    parameters["tags"] = txtCfgTags.Text.Trim();
                    parameters["onboot"] = chkCfgOnboot.Checked ? "1" : "0";
                    parameters["protection"] = chkCfgProtect.Checked ? "1" : "0";
                    parameters["sockets"] = numCfgSockets.Value.ToString();
                    parameters["cores"] = numCfgCores.Value.ToString();
                    parameters["cpu"] = cmbCfgCpuType.Text;
                    parameters["memory"] = numCfgMemory.Value.ToString();
                    parameters["balloon"] = numCfgBalloon.Value.ToString();
                    parameters["vga"] = cmbCfgVga.Text;
                    parameters["agent"] = chkCfgAgent.Checked ? "enabled=1" : "enabled=0";

                    // Build net0 string
                    string mac = ExtractMac(_lastConfig.ContainsKey("net0") ? _lastConfig["net0"]?.ToString() : "");
                    string netModel = cmbCfgNetModel.Text;
                    string netStr = $"{netModel}={mac},bridge={cmbCfgNetBridge.Text}";
                    if (numCfgNetVlan.Value > 0) netStr += $",tag={numCfgNetVlan.Value}";
                    if (chkCfgNetFw.Checked) netStr += ",firewall=1";
                    parameters["net0"] = netStr;

                    if (!string.IsNullOrWhiteSpace(txtCfgBootOrder.Text))
                        parameters["boot"] = txtCfgBootOrder.Text.Trim();
                }
                else // lxc
                {
                    parameters["hostname"] = txtCfgName.Text.Trim();
                    parameters["description"] = txtCfgDesc.Text.Trim();
                    parameters["tags"] = txtCfgTags.Text.Trim();
                    parameters["onboot"] = chkCfgOnboot.Checked ? "1" : "0";
                    parameters["protection"] = chkCfgProtect.Checked ? "1" : "0";
                    parameters["cores"] = numCfgLxcCores.Value.ToString();
                    parameters["memory"] = numCfgLxcMemory.Value.ToString();
                    parameters["swap"] = numCfgLxcSwap.Value.ToString();
                    parameters["unprivileged"] = chkCfgPriv.Checked ? "1" : "0";

                    // Features
                    parameters["features"] = chkCfgNesting.Checked ? "nesting=1" : "";

                    // Build net0 string for LXC
                    string ip = ExtractLxcIp(_lastConfig.ContainsKey("net0") ? _lastConfig["net0"]?.ToString() : "");
                    string lxcNet = $"name=eth0,bridge={cmbCfgNetBridge.Text},{ip}";
                    if (numCfgNetVlan.Value > 0) lxcNet += $",tag={numCfgNetVlan.Value}";
                    if (chkCfgNetFw.Checked) lxcNet += ",firewall=1";
                    parameters["net0"] = lxcNet;
                }

                bool ok = await _client.UpdateConfigAsync(_currentNode, _currentVmId, _currentType, parameters);

                if (ok)
                {
                    lblConfigSubtitle.Text = $"✅  Configuration saved successfully at {DateTime.Now:HH:mm:ss}";
                    lblConfigSubtitle.ForeColor = Color.FromArgb(163, 230, 53);
                }
                else
                {
                    lblConfigSubtitle.Text = "⚠️  Failed to save configuration. Check API logs.";
                    lblConfigSubtitle.ForeColor = Color.FromArgb(249, 115, 22);
                }
            }
            catch (Exception ex)
            {
                lblConfigSubtitle.Text = $"⚠️  Error: {ex.Message}";
                lblConfigSubtitle.ForeColor = Color.FromArgb(239, 68, 68);
            }
            finally
            {
                btnApply.Enabled = true;
                btnApply.Text = "💾  Apply Changes";
            }
        }

        // ─── Helpers: extract parts from existing config strings ──────────────
        private string ExtractMac(string net0)
        {
            if (string.IsNullOrEmpty(net0)) return "de:ad:be:ef:00:01";
            foreach (var part in net0.Split(','))
            {
                if (part.Contains("=") && !part.StartsWith("bridge=") && !part.StartsWith("tag=")
                    && !part.StartsWith("firewall=") && !part.StartsWith("rate=")
                    && !part.StartsWith("ip="))
                {
                    var segments = part.Split('=');
                    if (segments.Length == 2 && segments[1].Contains(":"))
                        return segments[1];
                }
            }
            return "de:ad:be:ef:00:01";
        }

        private string ExtractLxcIp(string net0)
        {
            if (string.IsNullOrEmpty(net0)) return "ip=dhcp";
            foreach (var part in net0.Split(','))
            {
                if (part.StartsWith("ip=")) return part;
                if (part.StartsWith("ip6=")) return part;
            }
            return "ip=dhcp";
        }

        // ─── UI building helpers ───────────────────────────────────────────────
        private Color BgColor => Color.FromArgb(15, 23, 42);
        private Color CardColor => Color.FromArgb(30, 41, 59);
        private Color LabelColor => Color.FromArgb(203, 213, 225);
        private Color AccentColor => Color.FromArgb(249, 115, 22);

        private int AddSectionHeader(ref Label lbl, string text, int y)
        {
            lbl = new Label
            {
                Text = text,
                AutoSize = false,
                Location = new Point(0, y),
                Size = new Size(860, 26),
                Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold),
                ForeColor = AccentColor,
                BackColor = CardColor,
                Padding = new Padding(8, 4, 0, 0)
            };
            panelScroll.Controls.Add(lbl);
            return y + 36;
        }

        private Label AddLabel(string text, int y)
        {
            var lbl = new Label
            {
                Text = text,
                AutoSize = false,
                Location = new Point(4, y + 8),
                Size = new Size(240, 22),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = LabelColor
            };
            panelScroll.Controls.Add(lbl);
            return lbl;
        }

        private CheckBox AddCheckBox(int y)
        {
            var chk = new CheckBox
            {
                Location = new Point(255, y + 5),
                Size = new Size(24, 24),
                ForeColor = Color.White
            };
            panelScroll.Controls.Add(chk);
            return chk;
        }

        private NumericUpDown AddNumericUpDown(int y, decimal min, decimal max, decimal increment)
        {
            var num = new NumericUpDown
            {
                Location = new Point(255, y),
                Size = new Size(180, 28),
                Minimum = min,
                Maximum = max,
                Increment = increment,
                BackColor = CardColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };
            panelScroll.Controls.Add(num);
            return num;
        }

        private ComboBox AddComboBox(int y, string[] items)
        {
            var cmb = new ComboBox
            {
                Location = new Point(255, y + 1),
                Size = new Size(300, 28),
                BackColor = CardColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F)
            };
            cmb.Items.AddRange(items);
            if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
            panelScroll.Controls.Add(cmb);
            return cmb;
        }

        private (Label lbl, TextBox txt) AddTextRow(string labelText, int y)
        {
            var lbl = AddLabel(labelText, y);
            var txt = new TextBox
            {
                Location = new Point(255, y + 2),
                Size = new Size(560, 28),
                BackColor = CardColor,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F)
            };
            panelScroll.Controls.Add(txt);
            return (lbl, txt);
        }

        private Button MakeButton(string text, Color bg, Point loc)
        {
            var btn = new Button
            {
                Text = text,
                Location = loc,
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // ─── Field show/hide helpers ───────────────────────────────────────────
        private void HideAllFields()
        {
            var controlsToHide = new Control[]
            {
                lblCfgName, txtCfgName, lblCfgDesc, txtCfgDesc,
                lblCfgTags, txtCfgTags, lblCfgOnboot, chkCfgOnboot,
                lblCfgProtect, chkCfgProtect,
                lblCfgCores, numCfgCores, lblCfgSockets, numCfgSockets,
                lblCfgCpuType, cmbCfgCpuType, lblCfgMemory, numCfgMemory,
                lblCfgBalloon, numCfgBalloon, lblCfgVga, cmbCfgVga,
                lblCfgAgent, chkCfgAgent,
                lblCfgLxcCores, numCfgLxcCores, lblCfgLxcMemory, numCfgLxcMemory,
                lblCfgLxcSwap, numCfgLxcSwap, lblCfgPriv, chkCfgPriv,
                lblCfgNesting, chkCfgNesting,
                lblCfgNetModel, cmbCfgNetModel, lblCfgNetBridge, cmbCfgNetBridge,
                lblCfgNetVlan, numCfgNetVlan, lblCfgNetFw, chkCfgNetFw,
                lblCfgBootOrder, txtCfgBootOrder,
                lblSectionHardware, lblSectionNetwork, lblSectionBoot
            };
            foreach (var c in controlsToHide)
                if (c != null) c.Visible = false;
        }

        private void ShowRow(Label lbl, Control ctrl)
        {
            if (lbl != null) lbl.Visible = true;
            if (ctrl != null) ctrl.Visible = true;
        }

        private void ShowComboRow(Label lbl, ComboBox cmb, string selectedValue)
        {
            ShowRow(lbl, cmb);
            if (!string.IsNullOrEmpty(selectedValue))
            {
                int idx = cmb.FindStringExact(selectedValue);
                if (idx >= 0) cmb.SelectedIndex = idx;
                else cmb.Text = selectedValue;
            }
        }

        private void ShowTextRow(Label lbl, TextBox txt, string value)
        {
            ShowRow(lbl, txt);
            txt.Text = value;
        }

        private void SetFieldsEnabled(bool enabled)
        {
            btnApply.Enabled = enabled;
            foreach (Control c in panelScroll.Controls)
            {
                if (c is TextBox || c is NumericUpDown || c is ComboBox || c is CheckBox)
                    c.Enabled = enabled;
            }
        }
    }
}
