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
        private RoundedButton btnApply;
        private RoundedButton btnReload;

        // Content scrollable area
        private Panel panelScroll;

        // Section labels
        private Label lblSectionGeneral;
        private Label lblSectionSystem;
        private Label lblSectionHardware;
        private Label lblSectionLxcHardware;
        private Label lblSectionLxcFeatures;
        private Label lblSectionNetwork;
        private Label lblSectionLxcDns;
        private Label lblSectionBoot;

        // --- General ---
        private Label lblCfgName;      private TextBox txtCfgName;
        private Label lblCfgDesc;      private TextBox txtCfgDesc;
        private Label lblCfgOnboot;    private CheckBox chkCfgOnboot;
        private Label lblCfgProtect;   private CheckBox chkCfgProtect;
        private Label lblCfgTags;      private TextBox txtCfgTags;
        private Label lblCfgStartupOrder; private NumericUpDown numCfgStartupOrder;
        private Label lblCfgStartupUp;    private NumericUpDown numCfgStartupUp;
        private Label lblCfgStartupDown;  private NumericUpDown numCfgStartupDown;

        // --- VM System ---
        private Label lblCfgMachine;   private ComboBox cmbCfgMachine;
        private Label lblCfgBios;      private ComboBox cmbCfgBios;
        private Label lblCfgScsiHw;    private ComboBox cmbCfgScsiHw;

        // --- Hardware (VM) ---
        private Label lblCfgCores;     private NumericUpDown numCfgCores;
        private Label lblCfgSockets;   private NumericUpDown numCfgSockets;
        private Label lblCfgCpuType;   private ComboBox cmbCfgCpuType;
        private Label lblCfgNuma;      private CheckBox chkCfgNuma;
        private Label lblCfgMemory;    private NumericUpDown numCfgMemory;
        private Label lblCfgBalloon;   private NumericUpDown numCfgBalloon;
        private Label lblCfgVga;       private ComboBox cmbCfgVga;
        private Label lblCfgAgent;     private CheckBox chkCfgAgent;

        // --- Hardware (LXC specific) ---
        private Label lblCfgLxcCores;     private NumericUpDown numCfgLxcCores;
        private Label lblCfgLxcCpuLimit;  private NumericUpDown numCfgLxcCpuLimit;
        private Label lblCfgLxcCpuUnits;  private NumericUpDown numCfgLxcCpuUnits;
        private Label lblCfgLxcMemory;    private NumericUpDown numCfgLxcMemory;
        private Label lblCfgLxcSwap;      private NumericUpDown numCfgLxcSwap;

        // --- LXC Features & Privileges ---
        private Label lblCfgPriv;       private CheckBox chkCfgPriv;
        private Label lblCfgNesting;    private CheckBox chkCfgNesting;
        private Label lblCfgKeyctl;     private CheckBox chkCfgKeyctl;
        private Label lblCfgFuse;       private CheckBox chkCfgFuse;
        private Label lblCfgMknod;      private CheckBox chkCfgMknod;
        private Label lblCfgLxcTty;     private NumericUpDown numCfgLxcTty;
        private Label lblCfgLxcConsole; private CheckBox chkCfgLxcConsole;

        // --- Network ---
        private Label lblCfgNetBridge; private ComboBox cmbCfgNetBridge;
        private Label lblCfgNetModel;  private ComboBox cmbCfgNetModel;
        private Label lblCfgNetVlan;   private NumericUpDown numCfgNetVlan;
        private Label lblCfgNetFw;     private CheckBox chkCfgNetFw;
        private Label lblCfgNetMac;    private TextBox txtCfgNetMac;
        private Label lblCfgNetRate;   private NumericUpDown numCfgNetRate;
        private Label lblCfgLxcIp;     private TextBox txtCfgLxcIp;
        private Label lblCfgLxcGw;     private TextBox txtCfgLxcGw;
        private Label lblCfgLxcIp6;    private TextBox txtCfgLxcIp6;
        private Label lblCfgLxcGw6;    private TextBox txtCfgLxcGw6;

        // --- LXC DNS ---
        private Label lblCfgNameserver;   private TextBox txtCfgNameserver;
        private Label lblCfgSearchDomain; private TextBox txtCfgSearchDomain;

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
            this.BackColor = BgColor;
            this.Dock = DockStyle.Fill;

            // ─── Header bar ───────────────────────────────────────────────
            var panelHeader = new Panel
            {
                Height = 55,
                Dock = DockStyle.Top,
                BackColor = CardColor,
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
            y = AddSectionHeader(ref lblSectionGeneral, "⚙️  General & Startup", y);

            (lblCfgName, txtCfgName) = AddTextRow("Name / Hostname", y); y += 42;
            (lblCfgDesc, txtCfgDesc) = AddTextRow("Description", y); y += 42;
            (lblCfgTags, txtCfgTags) = AddTextRow("Tags  (comma separated)", y); y += 42;

            lblCfgOnboot = AddLabel("Start at boot", y);
            chkCfgOnboot = AddCheckBox(y); y += 42;

            lblCfgProtect = AddLabel("Protection mode", y);
            chkCfgProtect = AddCheckBox(y); y += 42;

            lblCfgStartupOrder = AddLabel("Startup order  (order=X)", y);
            numCfgStartupOrder = AddNumericUpDown(y, -1, 1000, 1); y += 42;

            lblCfgStartupUp = AddLabel("Startup delay  (up, sec)", y);
            numCfgStartupUp = AddNumericUpDown(y, 0, 9999, 1); y += 42;

            lblCfgStartupDown = AddLabel("Shutdown delay  (down, sec)", y);
            numCfgStartupDown = AddNumericUpDown(y, 0, 9999, 1); y += 55;

            // ─── Section: System (VM only) ────────────────────────────────
            y = AddSectionHeader(ref lblSectionSystem, "💿  System & OS", y);

            lblCfgMachine = AddLabel("Machine type", y);
            cmbCfgMachine = AddComboBox(y, new[] { "pc", "q35", "pc-i440fx-8.1", "pc-q35-8.1" }); y += 42;

            lblCfgBios = AddLabel("BIOS type", y);
            cmbCfgBios = AddComboBox(y, new[] { "seabios", "ovmf" }); y += 42;

            lblCfgScsiHw = AddLabel("SCSI Controller", y);
            cmbCfgScsiHw = AddComboBox(y, new[] { "virtio-scsi-pci", "virtio-scsi-single", "megasas", "lsi", "lsi53c895a" }); y += 42;

            lblCfgAgent = AddLabel("QEMU Guest Agent", y);
            chkCfgAgent = AddCheckBox(y); y += 55;

            // ─── Section: Hardware (VM only) ──────────────────────────────
            y = AddSectionHeader(ref lblSectionHardware, "🖥️  Hardware & Resources", y);

            lblCfgSockets = AddLabel("CPU Sockets", y);
            numCfgSockets = AddNumericUpDown(y, 1, 32, 1); y += 42;

            lblCfgCores = AddLabel("CPU Cores", y);
            numCfgCores = AddNumericUpDown(y, 1, 512, 1); y += 42;

            lblCfgCpuType = AddLabel("CPU Type", y);
            cmbCfgCpuType = AddComboBox(y, new[] { "host", "kvm64", "max", "qemu64", "x86-64-v2-AES" }); y += 42;

            lblCfgNuma = AddLabel("NUMA Support", y);
            chkCfgNuma = AddCheckBox(y); y += 42;

            lblCfgMemory = AddLabel("Memory (MB)", y);
            numCfgMemory = AddNumericUpDown(y, 16, 2097152, 512); y += 42;

            lblCfgBalloon = AddLabel("Balloon min. (MB)", y);
            numCfgBalloon = AddNumericUpDown(y, 0, 2097152, 512); y += 42;

            lblCfgVga = AddLabel("Display (VGA)", y);
            cmbCfgVga = AddComboBox(y, new[] { "std", "cirrus", "vmware", "qxl", "serial0", "none" }); y += 55;

            // ─── Section: LXC Hardware (LXC only) ─────────────────────────
            y = AddSectionHeader(ref lblSectionLxcHardware, "🖥️  Container Resources", y);

            lblCfgLxcCores = AddLabel("CPU Cores (LXC)", y);
            numCfgLxcCores = AddNumericUpDown(y, 1, 512, 1); y += 42;

            lblCfgLxcCpuLimit = AddLabel("CPU Limit  (cores, 0 = none)", y);
            numCfgLxcCpuLimit = AddNumericUpDown(y, 0, 512, 0.1m, 2); y += 42;

            lblCfgLxcCpuUnits = AddLabel("CPU Units  (weight)", y);
            numCfgLxcCpuUnits = AddNumericUpDown(y, 8, 500000, 1); y += 42;

            lblCfgLxcMemory = AddLabel("Memory (MB, LXC)", y);
            numCfgLxcMemory = AddNumericUpDown(y, 16, 2097152, 256); y += 42;

            lblCfgLxcSwap = AddLabel("Swap (MB)", y);
            numCfgLxcSwap = AddNumericUpDown(y, 0, 65536, 256); y += 55;

            // ─── Section: LXC Features (LXC only) ─────────────────────────
            y = AddSectionHeader(ref lblSectionLxcFeatures, "🔒  Features & Privileges", y);

            lblCfgPriv = AddLabel("Unprivileged container", y);
            chkCfgPriv = AddCheckBox(y); y += 42;

            lblCfgNesting = AddLabel("Nesting  (Docker inside LXC)", y);
            chkCfgNesting = AddCheckBox(y); y += 42;

            lblCfgKeyctl = AddLabel("Keyctl features", y);
            chkCfgKeyctl = AddCheckBox(y); y += 42;

            lblCfgFuse = AddLabel("FUSE mounts", y);
            chkCfgFuse = AddCheckBox(y); y += 42;

            lblCfgMknod = AddLabel("Mount Device Nodes  (mknod)", y);
            chkCfgMknod = AddCheckBox(y); y += 42;

            lblCfgLxcTty = AddLabel("TTY Count", y);
            numCfgLxcTty = AddNumericUpDown(y, 0, 6, 1); y += 42;

            lblCfgLxcConsole = AddLabel("Enable Console / Shell", y);
            chkCfgLxcConsole = AddCheckBox(y); y += 55;

            // ─── Section: Network ─────────────────────────────────────────
            y = AddSectionHeader(ref lblSectionNetwork, "🌐  Network  (net0)", y);

            lblCfgNetModel = AddLabel("Model  (VM only)", y);
            cmbCfgNetModel = AddComboBox(y, new[] { "virtio", "e1000", "rtl8139", "vmxnet3" }); y += 42;

            lblCfgNetBridge = AddLabel("Bridge", y);
            cmbCfgNetBridge = AddComboBox(y, new[] { "vmbr0", "vmbr1", "vmbr2", "vmbr3" }); y += 42;

            lblCfgNetVlan = AddLabel("VLAN Tag  (0 = none)", y);
            numCfgNetVlan = AddNumericUpDown(y, 0, 4094, 0); y += 42;

            lblCfgNetFw = AddLabel("Firewall", y);
            chkCfgNetFw = AddCheckBox(y); y += 42;

            (lblCfgNetMac, txtCfgNetMac) = AddTextRow("MAC / Hardware Address", y); y += 42;

            lblCfgNetRate = AddLabel("Rate Limit (MB/s, 0=none)", y);
            numCfgNetRate = AddNumericUpDown(y, 0, 10000, 1); y += 42;

            // LXC specific net0 configuration fields
            (lblCfgLxcIp, txtCfgLxcIp) = AddTextRow("IPv4 Address  (e.g. 192.168.1.50/24 or dhcp)", y); y += 42;
            (lblCfgLxcGw, txtCfgLxcGw) = AddTextRow("IPv4 Gateway", y); y += 42;
            (lblCfgLxcIp6, txtCfgLxcIp6) = AddTextRow("IPv6 Address  (e.g. fd00::1/64 or dhcp)", y); y += 42;
            (lblCfgLxcGw6, txtCfgLxcGw6) = AddTextRow("IPv6 Gateway", y); y += 55;

            // ─── Section: LXC DNS ─────────────────────────────────────────
            y = AddSectionHeader(ref lblSectionLxcDns, "🔍  DNS Settings", y);

            (lblCfgSearchDomain, txtCfgSearchDomain) = AddTextRow("DNS Search Domain", y); y += 42;
            (lblCfgNameserver, txtCfgNameserver) = AddTextRow("DNS Server  (Nameserver)", y); y += 55;

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
        private int LayoutRow(Label lbl, Control ctrl, int y)
        {
            if (lbl != null)
            {
                lbl.Location = new Point(4, y + 8);
                lbl.Visible = true;
            }
            if (ctrl != null)
            {
                int offset = 0;
                if (ctrl is TextBox) offset = 2;
                else if (ctrl is ComboBox) offset = 1;
                else if (ctrl is CheckBox) offset = 5;

                ctrl.Location = new Point(255, y + offset);
                ctrl.Visible = true;
            }
            return y + 42;
        }

        private int LayoutSectionHeader(Label lbl, int y)
        {
            if (lbl != null)
            {
                lbl.Location = new Point(0, y);
                lbl.Visible = true;
            }
            return y + 36;
        }

        // ─── Populate UI fields from API response ──────────────────────────────
        private void PopulateFields(string type)
        {
            HideAllFields();

            string V(string key) => _lastConfig.ContainsKey(key) ? _lastConfig[key]?.ToString() ?? "" : "";
            bool B(string key) => V(key) == "1";
            int I(string key, int def = 0) => int.TryParse(V(key), out int v) ? v : def;

            // Parse startup options (shared between VM and LXC)
            int order = -1;
            int up = 0;
            int down = 0;
            string startup = V("startup");
            if (!string.IsNullOrEmpty(startup))
            {
                foreach (var part in startup.Split(','))
                {
                    var kv = part.Split('=');
                    if (kv.Length == 2)
                    {
                        string key = kv[0].Trim();
                        string val = kv[1].Trim();
                        if (key == "order") int.TryParse(val, out order);
                        else if (key == "up") int.TryParse(val, out up);
                        else if (key == "down") int.TryParse(val, out down);
                    }
                }
            }

            // General inputs
            txtCfgName.Text = type == "vm" ? V("name") : V("hostname");
            txtCfgDesc.Text = V("description");
            txtCfgTags.Text = V("tags");
            chkCfgOnboot.Checked = B("onboot");
            chkCfgProtect.Checked = B("protection");
            numCfgStartupOrder.Value = order;
            numCfgStartupUp.Value = up;
            numCfgStartupDown.Value = down;

            // Network (net0) common parsing
            ParseNet0(V("net0"), type);

            int y = 5;

            // Layout General
            y = LayoutSectionHeader(lblSectionGeneral, y);
            y = LayoutRow(lblCfgName, txtCfgName, y);
            y = LayoutRow(lblCfgDesc, txtCfgDesc, y);
            y = LayoutRow(lblCfgTags, txtCfgTags, y);
            y = LayoutRow(lblCfgOnboot, chkCfgOnboot, y);
            y = LayoutRow(lblCfgProtect, chkCfgProtect, y);
            y = LayoutRow(lblCfgStartupOrder, numCfgStartupOrder, y);
            y = LayoutRow(lblCfgStartupUp, numCfgStartupUp, y);
            y = LayoutRow(lblCfgStartupDown, numCfgStartupDown, y);
            y += 15;

            if (type == "vm")
            {
                // VM System
                string machine = V("machine");
                if (string.IsNullOrEmpty(machine)) cmbCfgMachine.SelectedIndex = -1;
                else cmbCfgMachine.Text = machine;

                string bios = V("bios");
                if (string.IsNullOrEmpty(bios)) cmbCfgBios.SelectedIndex = -1;
                else cmbCfgBios.Text = bios;

                string scsihw = V("scsihw");
                if (string.IsNullOrEmpty(scsihw)) cmbCfgScsiHw.SelectedIndex = -1;
                else cmbCfgScsiHw.Text = scsihw;

                chkCfgAgent.Checked = V("agent").StartsWith("1") || V("agent").Contains("enabled=1");

                // VM Hardware
                numCfgSockets.Value = Math.Max(1, I("sockets", 1));
                numCfgCores.Value = Math.Max(1, I("cores", 1));
                string cpuType = ParseField(V("cpu"), 0);
                if (string.IsNullOrEmpty(cpuType)) cmbCfgCpuType.SelectedIndex = -1;
                else cmbCfgCpuType.Text = cpuType;

                chkCfgNuma.Checked = B("numa");
                numCfgMemory.Value = Math.Max(16, I("memory", 512));
                numCfgBalloon.Value = I("balloon", 0);
                string vga = ParseVgaType(V("vga"));
                if (string.IsNullOrEmpty(vga)) cmbCfgVga.SelectedIndex = -1;
                else cmbCfgVga.Text = vga;

                txtCfgBootOrder.Text = V("boot");

                // Layout VM specific sections
                y = LayoutSectionHeader(lblSectionSystem, y);
                y = LayoutRow(lblCfgMachine, cmbCfgMachine, y);
                y = LayoutRow(lblCfgBios, cmbCfgBios, y);
                y = LayoutRow(lblCfgScsiHw, cmbCfgScsiHw, y);
                y = LayoutRow(lblCfgAgent, chkCfgAgent, y);
                y += 15;

                y = LayoutSectionHeader(lblSectionHardware, y);
                y = LayoutRow(lblCfgSockets, numCfgSockets, y);
                y = LayoutRow(lblCfgCores, numCfgCores, y);
                y = LayoutRow(lblCfgCpuType, cmbCfgCpuType, y);
                y = LayoutRow(lblCfgNuma, chkCfgNuma, y);
                y = LayoutRow(lblCfgMemory, numCfgMemory, y);
                y = LayoutRow(lblCfgBalloon, numCfgBalloon, y);
                y = LayoutRow(lblCfgVga, cmbCfgVga, y);
                y += 15;

                y = LayoutSectionHeader(lblSectionNetwork, y);
                y = LayoutRow(lblCfgNetModel, cmbCfgNetModel, y);
                y = LayoutRow(lblCfgNetBridge, cmbCfgNetBridge, y);
                y = LayoutRow(lblCfgNetVlan, numCfgNetVlan, y);
                y = LayoutRow(lblCfgNetFw, chkCfgNetFw, y);
                y = LayoutRow(lblCfgNetMac, txtCfgNetMac, y);
                y = LayoutRow(lblCfgNetRate, numCfgNetRate, y);
                y += 15;

                y = LayoutSectionHeader(lblSectionBoot, y);
                y = LayoutRow(lblCfgBootOrder, txtCfgBootOrder, y);
            }
            else // lxc
            {
                // LXC Hardware
                numCfgLxcCores.Value = Math.Max(1, I("cores", 1));
                numCfgLxcCpuLimit.Value = decimal.TryParse(V("cpulimit"), out decimal cl) ? cl : 0;
                numCfgLxcCpuUnits.Value = decimal.TryParse(V("cpuunits"), out decimal cu) ? Math.Max(8, cu) : 1024;
                numCfgLxcMemory.Value = Math.Max(16, I("memory", 512));
                numCfgLxcSwap.Value = I("swap", 512);

                // LXC Features & Privileges
                chkCfgPriv.Checked = B("unprivileged");
                string features = V("features");
                chkCfgNesting.Checked = features.Contains("nesting=1");
                chkCfgKeyctl.Checked = features.Contains("keyctl=1");
                chkCfgFuse.Checked = features.Contains("fuse=1");
                chkCfgMknod.Checked = features.Contains("mknod=1");
                numCfgLxcTty.Value = Math.Max(0, I("tty", 2));
                chkCfgLxcConsole.Checked = V("console") != "0";

                // DNS
                txtCfgSearchDomain.Text = V("searchdomain");
                txtCfgNameserver.Text = V("nameserver");

                // Layout LXC specific sections
                y = LayoutSectionHeader(lblSectionLxcHardware, y);
                y = LayoutRow(lblCfgLxcCores, numCfgLxcCores, y);
                y = LayoutRow(lblCfgLxcCpuLimit, numCfgLxcCpuLimit, y);
                y = LayoutRow(lblCfgLxcCpuUnits, numCfgLxcCpuUnits, y);
                y = LayoutRow(lblCfgLxcMemory, numCfgLxcMemory, y);
                y = LayoutRow(lblCfgLxcSwap, numCfgLxcSwap, y);
                y += 15;

                y = LayoutSectionHeader(lblSectionLxcFeatures, y);
                y = LayoutRow(lblCfgPriv, chkCfgPriv, y);
                y = LayoutRow(lblCfgNesting, chkCfgNesting, y);
                y = LayoutRow(lblCfgKeyctl, chkCfgKeyctl, y);
                y = LayoutRow(lblCfgFuse, chkCfgFuse, y);
                y = LayoutRow(lblCfgMknod, chkCfgMknod, y);
                y = LayoutRow(lblCfgLxcTty, numCfgLxcTty, y);
                y = LayoutRow(lblCfgLxcConsole, chkCfgLxcConsole, y);
                y += 15;

                y = LayoutSectionHeader(lblSectionNetwork, y);
                y = LayoutRow(lblCfgNetBridge, cmbCfgNetBridge, y);
                y = LayoutRow(lblCfgNetVlan, numCfgNetVlan, y);
                y = LayoutRow(lblCfgNetFw, chkCfgNetFw, y);
                y = LayoutRow(lblCfgNetMac, txtCfgNetMac, y);
                y = LayoutRow(lblCfgNetRate, numCfgNetRate, y);
                y = LayoutRow(lblCfgLxcIp, txtCfgLxcIp, y);
                y = LayoutRow(lblCfgLxcGw, txtCfgLxcGw, y);
                y = LayoutRow(lblCfgLxcIp6, txtCfgLxcIp6, y);
                y = LayoutRow(lblCfgLxcGw6, txtCfgLxcGw6, y);
                y += 15;

                y = LayoutSectionHeader(lblSectionLxcDns, y);
                y = LayoutRow(lblCfgSearchDomain, txtCfgSearchDomain, y);
                y = LayoutRow(lblCfgNameserver, txtCfgNameserver, y);
            }

            panelScroll.AutoScrollMinSize = new Size(0, y + 40);
        }

        private void ParseNet0(string net0, string type)
        {
            // Set defaults
            cmbCfgNetModel.Text = "virtio";
            cmbCfgNetBridge.Text = "vmbr0";
            numCfgNetVlan.Value = 0;
            chkCfgNetFw.Checked = false;
            txtCfgNetMac.Text = "";
            numCfgNetRate.Value = 0;
            txtCfgLxcIp.Text = "";
            txtCfgLxcGw.Text = "";
            txtCfgLxcIp6.Text = "";
            txtCfgLxcGw6.Text = "";

            if (string.IsNullOrEmpty(net0)) return;

            var parts = net0.Split(',').Select(p => p.Trim()).ToList();
            foreach (var part in parts)
            {
                if (part.StartsWith("bridge="))
                {
                    cmbCfgNetBridge.Text = part.Substring(7);
                }
                else if (part.StartsWith("firewall="))
                {
                    chkCfgNetFw.Checked = part.EndsWith("1");
                }
                else if (part.StartsWith("tag="))
                {
                    if (int.TryParse(part.Substring(4), out int vlan))
                        numCfgNetVlan.Value = vlan;
                }
                else if (part.StartsWith("rate="))
                {
                    if (decimal.TryParse(part.Substring(5), out decimal rate))
                        numCfgNetRate.Value = Math.Max(0, Math.Min(10000, rate));
                }
                else if (type == "lxc" && part.StartsWith("hwaddr="))
                {
                    txtCfgNetMac.Text = part.Substring(7);
                }
                else if (type == "lxc" && part.StartsWith("ip="))
                {
                    txtCfgLxcIp.Text = part.Substring(3);
                }
                else if (type == "lxc" && part.StartsWith("gw="))
                {
                    txtCfgLxcGw.Text = part.Substring(3);
                }
                else if (type == "lxc" && part.StartsWith("ip6="))
                {
                    txtCfgLxcIp6.Text = part.Substring(4);
                }
                else if (type == "lxc" && part.StartsWith("gw6="))
                {
                    txtCfgLxcGw6.Text = part.Substring(4);
                }
                else if (type == "vm" && part.Contains("=") && 
                         !part.StartsWith("bridge=") && 
                         !part.StartsWith("tag=") && 
                         !part.StartsWith("firewall=") && 
                         !part.StartsWith("rate="))
                {
                    var segs = part.Split('=');
                    if (segs.Length == 2)
                    {
                        cmbCfgNetModel.Text = segs[0];
                        txtCfgNetMac.Text = segs[1];
                    }
                }
            }
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
                var deleteList = new List<string>();

                // Build startup order string shared
                var startupList = new List<string>();
                if (numCfgStartupOrder.Value >= 0) startupList.Add($"order={numCfgStartupOrder.Value}");
                if (numCfgStartupUp.Value > 0) startupList.Add($"up={numCfgStartupUp.Value}");
                if (numCfgStartupDown.Value > 0) startupList.Add($"down={numCfgStartupDown.Value}");

                string startupStr = string.Join(",", startupList);

                if (_currentType == "vm")
                {
                    parameters["name"] = txtCfgName.Text.Trim();
                    parameters["description"] = txtCfgDesc.Text.Trim();
                    parameters["tags"] = txtCfgTags.Text.Trim();
                    parameters["onboot"] = chkCfgOnboot.Checked ? "1" : "0";
                    parameters["protection"] = chkCfgProtect.Checked ? "1" : "0";
                    parameters["sockets"] = numCfgSockets.Value.ToString();
                    parameters["cores"] = numCfgCores.Value.ToString();
                    parameters["memory"] = numCfgMemory.Value.ToString();
                    parameters["agent"] = chkCfgAgent.Checked ? "enabled=1" : "enabled=0";

                    if (!string.IsNullOrEmpty(startupStr))
                        parameters["startup"] = startupStr;
                    else
                        deleteList.Add("startup");

                    if (string.IsNullOrEmpty(cmbCfgMachine.Text))
                        deleteList.Add("machine");
                    else
                        parameters["machine"] = cmbCfgMachine.Text;

                    if (string.IsNullOrEmpty(cmbCfgBios.Text))
                        deleteList.Add("bios");
                    else
                        parameters["bios"] = cmbCfgBios.Text;

                    if (string.IsNullOrEmpty(cmbCfgScsiHw.Text))
                        deleteList.Add("scsihw");
                    else
                        parameters["scsihw"] = cmbCfgScsiHw.Text;

                    if (string.IsNullOrEmpty(cmbCfgCpuType.Text))
                        deleteList.Add("cpu");
                    else
                        parameters["cpu"] = cmbCfgCpuType.Text;

                    parameters["numa"] = chkCfgNuma.Checked ? "1" : "0";

                    if (numCfgBalloon.Value == 0)
                        deleteList.Add("balloon");
                    else
                        parameters["balloon"] = numCfgBalloon.Value.ToString();

                    if (string.IsNullOrEmpty(cmbCfgVga.Text))
                        deleteList.Add("vga");
                    else
                        parameters["vga"] = cmbCfgVga.Text;

                    if (string.IsNullOrWhiteSpace(txtCfgBootOrder.Text))
                        deleteList.Add("boot");
                    else
                        parameters["boot"] = txtCfgBootOrder.Text.Trim();

                    // Build net0 string for VM
                    string mac = txtCfgNetMac.Text.Trim();
                    if (string.IsNullOrEmpty(mac))
                    {
                        mac = ExtractMac(_lastConfig.ContainsKey("net0") ? _lastConfig["net0"]?.ToString() : "");
                    }
                    string netModel = cmbCfgNetModel.Text;
                    if (string.IsNullOrEmpty(netModel)) netModel = "virtio";
                    
                    string netStr = $"{netModel}={mac},bridge={cmbCfgNetBridge.Text}";
                    if (numCfgNetVlan.Value > 0) netStr += $",tag={numCfgNetVlan.Value}";
                    if (chkCfgNetFw.Checked) netStr += ",firewall=1";
                    if (numCfgNetRate.Value > 0) netStr += $",rate={numCfgNetRate.Value}";
                    parameters["net0"] = netStr;
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
                    parameters["tty"] = numCfgLxcTty.Value.ToString();
                    parameters["console"] = chkCfgLxcConsole.Checked ? "1" : "0";

                    if (!string.IsNullOrEmpty(startupStr))
                        parameters["startup"] = startupStr;
                    else
                        deleteList.Add("startup");

                    if (numCfgLxcCpuLimit.Value == 0)
                        deleteList.Add("cpulimit");
                    else
                        parameters["cpulimit"] = numCfgLxcCpuLimit.Value.ToString();

                    parameters["cpuunits"] = numCfgLxcCpuUnits.Value.ToString();

                    // Features
                    var featuresList = new List<string>();
                    if (chkCfgNesting.Checked) featuresList.Add("nesting=1");
                    if (chkCfgKeyctl.Checked) featuresList.Add("keyctl=1");
                    if (chkCfgFuse.Checked) featuresList.Add("fuse=1");
                    if (chkCfgMknod.Checked) featuresList.Add("mknod=1");

                    if (featuresList.Count > 0)
                        parameters["features"] = string.Join(",", featuresList);
                    else
                        deleteList.Add("features");

                    // DNS Settings
                    if (string.IsNullOrWhiteSpace(txtCfgSearchDomain.Text))
                        deleteList.Add("searchdomain");
                    else
                        parameters["searchdomain"] = txtCfgSearchDomain.Text.Trim();

                    if (string.IsNullOrWhiteSpace(txtCfgNameserver.Text))
                        deleteList.Add("nameserver");
                    else
                        parameters["nameserver"] = txtCfgNameserver.Text.Trim();

                    // Build net0 string for LXC
                    string mac = txtCfgNetMac.Text.Trim();
                    if (string.IsNullOrEmpty(mac))
                    {
                        mac = ExtractMac(_lastConfig.ContainsKey("net0") ? _lastConfig["net0"]?.ToString() : "");
                    }
                    
                    string lxcNet = $"name=eth0,bridge={cmbCfgNetBridge.Text},hwaddr={mac}";
                    if (numCfgNetVlan.Value > 0) lxcNet += $",tag={numCfgNetVlan.Value}";
                    if (chkCfgNetFw.Checked) lxcNet += ",firewall=1";
                    if (numCfgNetRate.Value > 0) lxcNet += $",rate={numCfgNetRate.Value}";

                    string ip = txtCfgLxcIp.Text.Trim();
                    if (!string.IsNullOrEmpty(ip)) lxcNet += $",ip={ip}";
                    
                    string gw = txtCfgLxcGw.Text.Trim();
                    if (!string.IsNullOrEmpty(gw)) lxcNet += $",gw={gw}";
                    
                    string ip6 = txtCfgLxcIp6.Text.Trim();
                    if (!string.IsNullOrEmpty(ip6)) lxcNet += $",ip6={ip6}";
                    
                    string gw6 = txtCfgLxcGw6.Text.Trim();
                    if (!string.IsNullOrEmpty(gw6)) lxcNet += $",gw6={gw6}";

                    parameters["net0"] = lxcNet;
                }

                if (deleteList.Count > 0)
                {
                    parameters["delete"] = string.Join(",", deleteList);
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
                var trimmed = part.Trim();
                if (trimmed.StartsWith("hwaddr="))
                {
                    return trimmed.Substring(7);
                }
                if (trimmed.Contains("=") && 
                    !trimmed.StartsWith("bridge=") && 
                    !trimmed.StartsWith("tag=") && 
                    !trimmed.StartsWith("firewall=") && 
                    !trimmed.StartsWith("rate=") && 
                    !trimmed.StartsWith("ip=") && 
                    !trimmed.StartsWith("ip6=") && 
                    !trimmed.StartsWith("gw=") && 
                    !trimmed.StartsWith("gw6=") && 
                    !trimmed.StartsWith("name="))
                {
                    var segments = trimmed.Split('=');
                    if (segments.Length == 2 && segments[1].Contains(":"))
                        return segments[1];
                }
            }
            return "de:ad:be:ef:00:01";
        }

        // ─── UI building helpers ───────────────────────────────────────────────
        private Color BgColor => Color.FromArgb(10, 15, 25);
        private Color CardColor => Color.FromArgb(17, 24, 39);
        private Color LabelColor => Color.FromArgb(229, 231, 235);
        private Color AccentColor => Color.FromArgb(249, 115, 22);
        private Color InputBgColor => Color.FromArgb(12, 18, 31);

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

        private NumericUpDown AddNumericUpDown(int y, decimal min, decimal max, decimal increment, int decimalPlaces = 0)
        {
            var num = new NumericUpDown
            {
                Location = new Point(255, y),
                Size = new Size(180, 28),
                Minimum = min,
                Maximum = max,
                Increment = increment,
                DecimalPlaces = decimalPlaces,
                BackColor = InputBgColor,
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
                BackColor = InputBgColor,
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
                BackColor = InputBgColor,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10F)
            };
            panelScroll.Controls.Add(txt);
            return (lbl, txt);
        }

        private RoundedButton MakeButton(string text, Color bg, Point loc)
        {
            var btn = new RoundedButton
            {
                Text = text,
                Location = loc,
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                BorderRadius = 10
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // ─── Field show/hide helpers ───────────────────────────────────────────
        private void HideAllFields()
        {
            var controlsToHide = new Control[]
            {
                lblSectionGeneral, lblSectionSystem, lblSectionHardware, lblSectionLxcHardware,
                lblSectionLxcFeatures, lblSectionNetwork, lblSectionLxcDns, lblSectionBoot,
                lblCfgName, txtCfgName, lblCfgDesc, txtCfgDesc, lblCfgTags, txtCfgTags,
                lblCfgOnboot, chkCfgOnboot, lblCfgProtect, chkCfgProtect,
                lblCfgStartupOrder, numCfgStartupOrder, lblCfgStartupUp, numCfgStartupUp, lblCfgStartupDown, numCfgStartupDown,
                lblCfgMachine, cmbCfgMachine, lblCfgBios, cmbCfgBios, lblCfgScsiHw, cmbCfgScsiHw, lblCfgAgent, chkCfgAgent,
                lblCfgSockets, numCfgSockets, lblCfgCores, numCfgCores, lblCfgCpuType, cmbCfgCpuType, lblCfgNuma, chkCfgNuma,
                lblCfgMemory, numCfgMemory, lblCfgBalloon, numCfgBalloon, lblCfgVga, cmbCfgVga,
                lblCfgLxcCores, numCfgLxcCores, lblCfgLxcCpuLimit, numCfgLxcCpuLimit, lblCfgLxcCpuUnits, numCfgLxcCpuUnits,
                lblCfgLxcMemory, numCfgLxcMemory, lblCfgLxcSwap, numCfgLxcSwap,
                lblCfgPriv, chkCfgPriv, lblCfgNesting, chkCfgNesting, lblCfgKeyctl, chkCfgKeyctl, lblCfgFuse, chkCfgFuse, lblCfgMknod, chkCfgMknod,
                lblCfgLxcTty, numCfgLxcTty, lblCfgLxcConsole, chkCfgLxcConsole,
                lblCfgNetModel, cmbCfgNetModel, lblCfgNetBridge, cmbCfgNetBridge, lblCfgNetVlan, numCfgNetVlan, lblCfgNetFw, chkCfgNetFw,
                lblCfgNetMac, txtCfgNetMac, lblCfgNetRate, numCfgNetRate,
                lblCfgLxcIp, txtCfgLxcIp, lblCfgLxcGw, txtCfgLxcGw, lblCfgLxcIp6, txtCfgLxcIp6, lblCfgLxcGw6, txtCfgLxcGw6,
                lblCfgSearchDomain, txtCfgSearchDomain, lblCfgNameserver, txtCfgNameserver,
                lblCfgBootOrder, txtCfgBootOrder
            };
            foreach (var c in controlsToHide)
            {
                if (c != null) c.Visible = false;
            }
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
