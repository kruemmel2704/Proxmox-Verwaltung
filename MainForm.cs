using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace ProxmoxVEGui
{
    public partial class MainForm : Form
    {
        private readonly ProxmoxClient _client;
        private List<PveNode> _cachedNodes = new List<PveNode>();
        private bool _webViewInitialized = false;
        private string _lastSelectedKey = ""; // Track selected resource to prevent UI jumps
        private ConfigPanel _configPanel; // Live configuration editor panel

        public MainForm(ProxmoxClient client)
        {
            _client = client;
            InitializeComponent();
            IconHelper.ApplyIcon(this);
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

        private async void MainForm_Load(object sender, EventArgs e)
        {
            // Create and add ConfigPanel to content container
            _configPanel = new ConfigPanel(_client);
            panelContentContainer.Controls.Add(_configPanel);

            // Initial load
            await RefreshDataAsync();
            SelectDatacenterNode();
            
            // Start auto-refresh timer
            timerRefresh.Start();
        }

        private void SelectDatacenterNode()
        {
            if (treeResources.Nodes.Count > 0)
            {
                treeResources.SelectedNode = treeResources.Nodes[0];
            }
        }

        public async Task RefreshDataAsync()
        {
            lblSelectedResource.Text = "Loading cluster information...";
            treeResources.BeginUpdate();
            
            // Save scroll/expand states if possible, or just rebuild and select
            treeResources.Nodes.Clear();

            var datacenterNode = new TreeNode("🌐 Datacenter")
            {
                Tag = new ResourceTag { Type = "datacenter", Name = "Datacenter" }
            };
            treeResources.Nodes.Add(datacenterNode);

            // Fetch cluster nodes
            var apiNodes = await _client.GetNodesAsync();
            _cachedNodes = apiNodes;

            if (apiNodes.Count == 0)
            {
                lblSelectedResource.Text = "Error: No nodes detected (Unauthorized or connection issue)";
                lblSelectedResource.ForeColor = Color.FromArgb(239, 68, 68);
            }
            else
            {
                lblSelectedResource.ForeColor = Color.White;
            }

            foreach (var node in apiNodes)
            {
                var isOnline = node.Status == "online";
                string statusDot = isOnline ? "🟢" : "🔴";
                string nodeDisplayName = $"{statusDot} 🗄️ {node.Node}";
                
                var nodeTreeNode = new TreeNode(nodeDisplayName)
                {
                    Tag = new ResourceTag { Type = "node", NodeName = node.Node, Name = node.Node, Data = node }
                };
                datacenterNode.Nodes.Add(nodeTreeNode);

                if (isOnline)
                {
                    // Fetch VMs
                    var vms = await _client.GetVmsAsync(node.Node);
                    var vmsGroupNode = new TreeNode("🖥️ Virtual Machines")
                    {
                        Tag = new ResourceTag { Type = "group_vm", NodeName = node.Node }
                    };
                    nodeTreeNode.Nodes.Add(vmsGroupNode);

                    foreach (var vm in vms)
                    {
                        if (vm.IsTemplate) continue; // Skip templates for clarity, or show them with template icon

                        string vmStatusDot = vm.Status == "running" ? "🟢" : "🔴";
                        string vmDisplay = $"{vmStatusDot} 🖥️ [{vm.VmId}] {vm.Name}";
                        vmsGroupNode.Nodes.Add(new TreeNode(vmDisplay)
                        {
                            Tag = new ResourceTag { Type = "vm", NodeName = node.Node, VmId = vm.VmId, Name = vm.Name, Data = vm }
                        });
                    }

                    // Fetch LXCs
                    var lxcs = await _client.GetLxcsAsync(node.Node);
                    var lxcsGroupNode = new TreeNode("📦 Containers (LXC)")
                    {
                        Tag = new ResourceTag { Type = "group_lxc", NodeName = node.Node }
                    };
                    nodeTreeNode.Nodes.Add(lxcsGroupNode);

                    foreach (var lxc in lxcs)
                    {
                        string lxcStatusDot = lxc.Status == "running" ? "🟢" : "🔴";
                        string lxcDisplay = $"{lxcStatusDot} 📦 [{lxc.VmId}] {lxc.Name}";
                        lxcsGroupNode.Nodes.Add(new TreeNode(lxcDisplay)
                        {
                            Tag = new ResourceTag { Type = "lxc", NodeName = node.Node, VmId = lxc.VmId, Name = lxc.Name, Data = lxc }
                        });
                    }

                    // Fetch Storages
                    var storages = await _client.GetStorageAsync(node.Node);
                    var storageGroupNode = new TreeNode("💾 Storage Pools")
                    {
                        Tag = new ResourceTag { Type = "group_storage", NodeName = node.Node }
                    };
                    nodeTreeNode.Nodes.Add(storageGroupNode);

                    foreach (var store in storages)
                    {
                        string storeStatusDot = store.Active ? "🟢" : "🔴";
                        string storeDisplay = $"{storeStatusDot} 💾 {store.Storage} ({store.Type})";
                        storageGroupNode.Nodes.Add(new TreeNode(storeDisplay)
                        {
                            Tag = new ResourceTag { Type = "storage", NodeName = node.Node, Name = store.Storage, Data = store }
                        });
                    }
                }
            }

            treeResources.ExpandAll();
            treeResources.EndUpdate();
            
            lblSelectedResource.Text = "Cluster ready.";
            
            // Re-sync task logs
            await RefreshTasksLogAsync();
            
            // Refresh details of selected item
            if (treeResources.SelectedNode != null)
            {
                var tag = treeResources.SelectedNode.Tag as ResourceTag;
                UpdateUIForSelectedResource(tag);
            }
        }

        private async Task RefreshTasksLogAsync()
        {
            string fallbackNode = _cachedNodes.FirstOrDefault(n => n.Status == "online")?.Node;
            var tasks = await _client.GetTasksAsync(fallbackNode);

            gridTasks.Rows.Clear();
            foreach (var task in tasks)
            {
                string timeStr = DateTimeOffset.FromUnixTimeSeconds(task.StartTime).LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                string desc = $"{task.Type.ToUpper()} {task.Id}";
                
                int index = gridTasks.Rows.Add(timeStr, task.Node, task.User, desc, task.Status);
                var row = gridTasks.Rows[index];

                // Color row cells depending on task status
                if (task.Status == "OK")
                {
                    row.Cells[4].Style.ForeColor = Color.FromArgb(34, 197, 94); // Neon Green
                }
                else if (task.Status == "RUNNING")
                {
                    row.Cells[4].Style.ForeColor = Color.FromArgb(59, 130, 246); // Blue
                }
                else
                {
                    row.Cells[4].Style.ForeColor = Color.FromArgb(239, 68, 68); // Neon Red (Error)
                }
            }
        }

        private void treeResources_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var tag = e.Node.Tag as ResourceTag;
            if (tag == null) return;

            _lastSelectedKey = $"{tag.Type}_{tag.NodeName}_{tag.VmId}";
            UpdateUIForSelectedResource(tag);
        }

        private void UpdateUIForSelectedResource(ResourceTag tag)
        {
            if (tag == null) return;
            lblSelectedResource.Text = tag.Name ?? "Resource";
            UpdateActionButtonsState(tag);

            bool isConfigurable = tag.Type == "vm" || tag.Type == "lxc";
            if (btnTabConfig != null)
            {
                btnTabConfig.Enabled = isConfigurable;
                if (!isConfigurable && _configPanel != null && _configPanel.Visible)
                {
                    SwitchToTab("dashboard");
                }
            }

            if (tag.Type == "datacenter")
            {
                lblResourceType.Text = "Resource Type: Datacenter / Cluster";
                lblResourceID.Text = "Total Nodes: " + _cachedNodes.Count;
                lblSpecsCores.Text = "Total CPU Cores: " + _cachedNodes.Sum(n => n.MaxCpu);
                lblSpecsMemory.Text = "Total Memory: " + FormatBytes(_cachedNodes.Sum(n => n.MaxMem));
                
                long totalUptime = _cachedNodes.Count > 0 ? _cachedNodes.Max(n => n.Uptime) : 0;
                lblUptime.Text = "Cluster Max Uptime: " + FormatUptime(totalUptime);
                lblResourceStatus.Text = $"Nodes: {_cachedNodes.Count(n => n.Status == "online")} / {_cachedNodes.Count} Online";

                lblDetailNode.Text = "Host Node: N/A";
                lblDetailHa.Text = "HA State: Enabled";
                lblDetailIp.Text = "IP Addresses: Cluster Subnet";
                lblDetailDisk.Text = "Cluster Disk: " + FormatBytes(_cachedNodes.Sum(n => n.MaxDisk));

                // Aggregate metrics
                long usedMem = _cachedNodes.Sum(n => n.Mem);
                long totalMem = _cachedNodes.Sum(n => n.MaxMem);
                double avgCpu = _cachedNodes.Count > 0 ? _cachedNodes.Average(n => n.Cpu) : 0;

                chartCpu.AddValue(avgCpu * 100);
                chartRam.AddValue(totalMem > 0 ? ((double)usedMem / totalMem) * 100 : 0);

                btnTabConsole.Enabled = false;
                lblConsoleWarning.Text = "Select a specific node, VM or LXC to open an interactive console shell.";
                SwitchToTab("dashboard");
            }
            else if (tag.Type == "node")
            {
                var node = tag.Data as PveNode;
                if (node != null)
                {
                    lblResourceType.Text = "Resource Type: Physical Node";
                    lblResourceID.Text = "Node Name: " + node.Node;
                    lblSpecsCores.Text = "CPU Cores: " + node.MaxCpu;
                    lblSpecsMemory.Text = "Total Memory: " + FormatBytes(node.MaxMem);
                    lblUptime.Text = "Node Uptime: " + FormatUptime(node.Uptime);
                    lblResourceStatus.Text = "Status: " + node.Status.ToUpper();

                    lblDetailNode.Text = "Host Node: " + node.Node;
                    lblDetailHa.Text = "HA State: Local Node";
                    lblDetailIp.Text = "IP Address: " + _client.Host;
                    lblDetailDisk.Text = "Local Disk: " + FormatBytes(node.MaxDisk);

                    chartCpu.AddValue(node.Cpu * 100);
                    chartRam.AddValue(node.MaxMem > 0 ? ((double)node.Mem / node.MaxMem) * 100 : 0);
                }

                btnTabConsole.Enabled = true;
                btnTabConsole.Text = "Node Shell";
                lblConsoleWarning.Text = "Connecting to Node host shell...";
            }
            else if (tag.Type == "vm")
            {
                var vm = tag.Data as PveVm;
                if (vm != null)
                {
                    lblResourceType.Text = "Resource Type: Qemu VM";
                    lblResourceID.Text = "VM ID: " + vm.VmId;
                    lblSpecsCores.Text = "vCPU Cores: " + vm.MaxCpu;
                    lblSpecsMemory.Text = "RAM Allocated: " + FormatBytes(vm.MaxMem);
                    lblUptime.Text = "VM Uptime: " + FormatUptime(vm.Uptime);
                    lblResourceStatus.Text = "Status: " + vm.Status.ToUpper();

                    lblDetailNode.Text = "Host Node: " + tag.NodeName;
                    lblDetailHa.Text = "HA State: Managed";
                    lblDetailIp.Text = "IP Address: Querying...";
                    lblDetailDisk.Text = "Disk usage: N/A";

                    chartCpu.AddValue(vm.Cpu * 100);
                    chartRam.AddValue(vm.MaxMem > 0 ? ((double)vm.Mem / vm.MaxMem) * 100 : 0);

                    // Resolve VM IP Address asynchronously
                    FetchVmIpAddressAsync(tag.NodeName, vm.VmId);
                }

                btnTabConsole.Enabled = true;
                btnTabConsole.Text = "noVNC Console";
                lblConsoleWarning.Text = "Connecting to VNC console...";
            }
            else if (tag.Type == "lxc")
            {
                var lxc = tag.Data as PveLxc;
                if (lxc != null)
                {
                    lblResourceType.Text = "Resource Type: LXC Container";
                    lblResourceID.Text = "Container ID: " + lxc.VmId;
                    lblSpecsCores.Text = "CPU Cores: " + lxc.MaxCpu;
                    lblSpecsMemory.Text = "RAM Allocated: " + FormatBytes(lxc.MaxMem);
                    lblUptime.Text = "Container Uptime: " + FormatUptime(lxc.Uptime);
                    lblResourceStatus.Text = "Status: " + lxc.Status.ToUpper();

                    lblDetailNode.Text = "Host Node: " + tag.NodeName;
                    lblDetailHa.Text = "HA State: Managed";
                    lblDetailIp.Text = "IP Address: Querying...";
                    lblDetailDisk.Text = "Disk usage: N/A";

                    chartCpu.AddValue(lxc.Cpu * 100);
                    chartRam.AddValue(lxc.MaxMem > 0 ? ((double)lxc.Mem / lxc.MaxMem) * 100 : 0);

                    // Resolve LXC IP Address asynchronously
                    FetchLxcIpAddressAsync(tag.NodeName, lxc.VmId);
                }

                btnTabConsole.Enabled = true;
                btnTabConsole.Text = "Container Shell";
                lblConsoleWarning.Text = "Connecting to container terminal...";
            }
            else if (tag.Type == "storage")
            {
                var store = tag.Data as PveStorage;
                if (store != null)
                {
                    lblResourceType.Text = "Resource Type: Storage Pool";
                    lblResourceID.Text = "Storage: " + store.Storage;
                    lblSpecsCores.Text = "Type: " + store.Type;
                    lblSpecsMemory.Text = "Capacity: " + FormatBytes(store.Total);
                    lblUptime.Text = "Used: " + FormatBytes(store.Used);
                    lblResourceStatus.Text = "Status: " + (store.Active ? "ACTIVE" : "INACTIVE");

                    lblDetailNode.Text = "Host Node: " + tag.NodeName;
                    lblDetailHa.Text = "HA State: Local Storage";
                    lblDetailIp.Text = "Shared: Local Device";
                    lblDetailDisk.Text = "Free: " + FormatBytes(store.Total - store.Used);

                    chartCpu.AddValue(0);
                    chartRam.AddValue(store.Total > 0 ? ((double)store.Used / store.Total) * 100 : 0);
                }

                btnTabConsole.Enabled = false;
                lblConsoleWarning.Text = "Select a specific node, VM or LXC to open an interactive console shell.";
                SwitchToTab("dashboard");
            }

            // Reload VNC console if visible
            if (panelConsole.Visible)
            {
                LoadConsoleForSelectedResource();
            }
        }

        private async void FetchVmIpAddressAsync(string node, int vmid)
        {
            string ip = await _client.GetVmIpAsync(node, vmid);
            // Verify user hasn't switched selection in the meantime
            if (treeResources.SelectedNode != null)
            {
                var tag = treeResources.SelectedNode.Tag as ResourceTag;
                if (tag != null && tag.Type == "vm" && tag.VmId == vmid)
                {
                    lblDetailIp.Text = "IP Address: " + ip;
                }
            }
        }

        private async void FetchLxcIpAddressAsync(string node, int vmid)
        {
            string ip = await _client.GetLxcIpAsync(node, vmid);
            if (treeResources.SelectedNode != null)
            {
                var tag = treeResources.SelectedNode.Tag as ResourceTag;
                if (tag != null && tag.Type == "lxc" && tag.VmId == vmid)
                {
                    lblDetailIp.Text = "IP Address: " + ip;
                }
            }
        }

        private void UpdateActionButtonsState(ResourceTag tag)
        {
            if (tag == null || tag.Type == "datacenter" || tag.Type == "group_vm" || tag.Type == "group_lxc" || tag.Type == "group_storage" || tag.Type == "storage")
            {
                btnStart.Enabled = false;
                btnStop.Enabled = false;
                btnShutdown.Enabled = false;
                btnReboot.Enabled = false;
                btnDelete.Enabled = false;
                return;
            }

            if (tag.Type == "node")
            {
                btnStart.Enabled = false;
                btnStop.Enabled = false;
                btnShutdown.Enabled = false;
                btnReboot.Enabled = false;
                btnDelete.Enabled = false;
                return;
            }

            btnDelete.Enabled = true;
            string status = "";
            if (tag.Type == "vm" && tag.Data is PveVm vm) status = vm.Status;
            if (tag.Type == "lxc" && tag.Data is PveLxc lxc) status = lxc.Status;

            if (status == "running")
            {
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                btnShutdown.Enabled = true;
                btnReboot.Enabled = true;
            }
            else
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                btnShutdown.Enabled = false;
                btnReboot.Enabled = false;
            }
        }

        private void SwitchToTab(string tabName)
        {
            panelDashboard.Visible = false;
            panelConsole.Visible = false;
            if (_configPanel != null) _configPanel.Visible = false;

            ResetTabButtonColors();

            if (tabName == "dashboard")
            {
                panelDashboard.Visible = true;
                btnTabDashboard.BackColor = Color.FromArgb(249, 115, 22);
                btnTabDashboard.ForeColor = Color.White;
            }
            else if (tabName == "console")
            {
                panelConsole.Visible = true;
                btnTabConsole.BackColor = Color.FromArgb(249, 115, 22);
                btnTabConsole.ForeColor = Color.White;
                LoadConsoleForSelectedResource();
            }
            else if (tabName == "config")
            {
                if (_configPanel != null)
                {
                    _configPanel.Visible = true;
                    btnTabConfig.BackColor = Color.FromArgb(249, 115, 22);
                    btnTabConfig.ForeColor = Color.White;
                    LoadConfigForSelectedResource();
                }
            }
        }

        private void ResetTabButtonColors()
        {
            var transparent = Color.Transparent;
            var inactiveText = Color.FromArgb(203, 213, 225);

            btnTabDashboard.BackColor = transparent;
            btnTabDashboard.ForeColor = inactiveText;

            btnTabConsole.BackColor = transparent;
            btnTabConsole.ForeColor = inactiveText;

            if (btnTabConfig != null)
            {
                btnTabConfig.BackColor = transparent;
                btnTabConfig.ForeColor = inactiveText;
            }
        }

        private async void LoadConfigForSelectedResource()
        {
            var node = treeResources.SelectedNode;
            if (node == null || _configPanel == null) return;

            var tag = node.Tag as ResourceTag;
            if (tag == null || (tag.Type != "vm" && tag.Type != "lxc")) return;

            await _configPanel.LoadConfigAsync(tag.NodeName, tag.VmId, tag.Type);
        }

        private async void LoadConsoleForSelectedResource()
        {
            var node = treeResources.SelectedNode;
            if (node == null) return;

            var tag = node.Tag as ResourceTag;
            if (tag == null || tag.Type == "datacenter" || tag.Type == "group_vm" || tag.Type == "group_lxc" || tag.Type == "group_storage" || tag.Type == "storage")
            {
                webViewConsole.Visible = false;
                lblConsoleWarning.Visible = true;
                lblConsoleWarning.Text = "Select a VM, Container or Node to view its interactive Console/Shell.";
                return;
            }

            string consoleUrl = "";
            if (tag.Type == "node")
            {
                consoleUrl = $"https://{_client.Host}:{_client.Port}/?console=shell&novnc=1&node={tag.NodeName}";
            }
            else if (tag.Type == "vm")
            {
                consoleUrl = $"https://{_client.Host}:{_client.Port}/?console=kvm&novnc=1&vmid={tag.VmId}&node={tag.NodeName}";
            }
            else if (tag.Type == "lxc")
            {
                consoleUrl = $"https://{_client.Host}:{_client.Port}/?console=lxc&xtermjs=1&vmid={tag.VmId}&node={tag.NodeName}";
            }

            lblConsoleWarning.Visible = true;
            webViewConsole.Visible = false;

            try
            {
                if (!_webViewInitialized)
                {
                    var options = new CoreWebView2EnvironmentOptions("--ignore-certificate-errors");
                    string userDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ProxmoxVEGui", "WebView2");
                    var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);
                    await webViewConsole.EnsureCoreWebView2Async(env);
                    _webViewInitialized = true;
                }

                var cookie = webViewConsole.CoreWebView2.CookieManager.CreateCookie("PVEAuthCookie", _client.Ticket, _client.Host, "/");
                webViewConsole.CoreWebView2.CookieManager.AddOrUpdateCookie(cookie);

                webViewConsole.CoreWebView2.Navigate(consoleUrl);
                
                lblConsoleWarning.Visible = false;
                webViewConsole.Visible = true;
            }
            catch (Exception ex)
            {
                lblConsoleWarning.Text = $"Failed to load console: {ex.Message}";
            }
        }

        private void btnTabDashboard_Click(object sender, EventArgs e) => SwitchToTab("dashboard");

        private void btnTabConsole_Click(object sender, EventArgs e) => SwitchToTab("console");

        private void btnTabConfig_Click(object sender, EventArgs e) => SwitchToTab("config");

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await RefreshDataAsync();
        }

        private async void PerformPowerAction(string action)
        {
            var node = treeResources.SelectedNode;
            if (node == null) return;

            var tag = node.Tag as ResourceTag;
            if (tag == null || (tag.Type != "vm" && tag.Type != "lxc")) return;

            lblSelectedResource.Text = $"Executing power action ({action})...";
            bool success = await _client.VMActionAsync(tag.NodeName, tag.VmId, tag.Type == "vm" ? "qemu" : "lxc", action);
            if (success)
            {
                MessageBox.Show($"Command '{action}' sent successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await RefreshDataAsync();
            }
            else
            {
                MessageBox.Show($"Failed to send command '{action}'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await RefreshDataAsync();
            }
        }

        private void btnStart_Click(object sender, EventArgs e) => PerformPowerAction("start");

        private void btnStop_Click(object sender, EventArgs e) => PerformPowerAction("stop");

        private void btnShutdown_Click(object sender, EventArgs e) => PerformPowerAction("shutdown");

        private void btnReboot_Click(object sender, EventArgs e) => PerformPowerAction("reboot");

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var node = treeResources.SelectedNode;
            if (node == null) return;

            var tag = node.Tag as ResourceTag;
            if (tag == null || (tag.Type != "vm" && tag.Type != "lxc")) return;

            var confirm = MessageBox.Show($"Are you sure you want to permanently delete [{tag.VmId}] {tag.Name}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                lblSelectedResource.Text = "Deleting resource...";
                bool success = await _client.DeleteResourceAsync(tag.NodeName, tag.VmId, tag.Type == "vm" ? "qemu" : "lxc");
                if (success)
                {
                    MessageBox.Show("Resource deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await RefreshDataAsync();
                }
                else
                {
                    MessageBox.Show("Failed to delete resource. Ensure it is powered off.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await RefreshDataAsync();
                }
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCreateVm_Click(object sender, EventArgs e)
        {
            OpenCreateDialog("vm");
        }

        private void btnCreateLxc_Click(object sender, EventArgs e)
        {
            OpenCreateDialog("lxc");
        }

        private void OpenCreateDialog(string type)
        {
            var onlineNodes = _cachedNodes.Where(n => n.Status == "online").Select(n => n.Node).ToList();
            if (onlineNodes.Count == 0)
            {
                MessageBox.Show("No online nodes available to host new resources.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int nextId = 100;
            var allResourceIds = new HashSet<int>();
            foreach (var node in treeResources.Nodes[0].Nodes)
            {
                var tn = node as TreeNode;
                if (tn == null) continue;
                foreach (TreeNode group in tn.Nodes)
                {
                    foreach (TreeNode resource in group.Nodes)
                    {
                        var tag = resource.Tag as ResourceTag;
                        if (tag != null && (tag.Type == "vm" || tag.Type == "lxc"))
                        {
                            allResourceIds.Add(tag.VmId);
                        }
                    }
                }
            }

            if (allResourceIds.Count > 0)
            {
                nextId = allResourceIds.Max() + 1;
            }

            var dialog = new CreateResourceDialog(this, _client, type, onlineNodes, nextId);
            dialog.ShowDialog();
        }

        // Live telemetry refresh ticker (Updates charts, stats, and task list without closing tree branches)
        private async void timerRefresh_Tick(object sender, EventArgs e)
        {
            try
            {
                // Refresh task log
                await RefreshTasksLogAsync();

                // Refresh nodes data
                var apiNodes = await _client.GetNodesAsync();
                _cachedNodes = apiNodes;

                // Sync current selected node details
                if (treeResources.SelectedNode != null)
                {
                    var tag = treeResources.SelectedNode.Tag as ResourceTag;
                    if (tag == null) return;

                    if (tag.Type == "datacenter")
                    {
                        long usedMem = _cachedNodes.Sum(n => n.Mem);
                        long totalMem = _cachedNodes.Sum(n => n.MaxMem);
                        double avgCpu = _cachedNodes.Count > 0 ? _cachedNodes.Average(n => n.Cpu) : 0;

                        chartCpu.AddValue(avgCpu * 100);
                        chartRam.AddValue(totalMem > 0 ? ((double)usedMem / totalMem) * 100 : 0);
                        lblResourceStatus.Text = $"Nodes: {_cachedNodes.Count(n => n.Status == "online")} / {_cachedNodes.Count} Online";
                    }
                    else if (tag.Type == "node")
                    {
                        var freshNode = _cachedNodes.FirstOrDefault(n => n.Node == tag.NodeName);
                        if (freshNode != null)
                        {
                            tag.Data = freshNode;
                            lblUptime.Text = "Node Uptime: " + FormatUptime(freshNode.Uptime);
                            chartCpu.AddValue(freshNode.Cpu * 100);
                            chartRam.AddValue(freshNode.MaxMem > 0 ? ((double)freshNode.Mem / freshNode.MaxMem) * 100 : 0);
                        }
                    }
                    else if (tag.Type == "vm")
                    {
                        var vms = await _client.GetVmsAsync(tag.NodeName);
                        var freshVm = vms.FirstOrDefault(v => v.VmId == tag.VmId);
                        if (freshVm != null)
                        {
                            tag.Data = freshVm;
                            lblUptime.Text = "VM Uptime: " + FormatUptime(freshVm.Uptime);
                            lblResourceStatus.Text = "Status: " + freshVm.Status.ToUpper();
                            chartCpu.AddValue(freshVm.Cpu * 100);
                            chartRam.AddValue(freshVm.MaxMem > 0 ? ((double)freshVm.Mem / freshVm.MaxMem) * 100 : 0);
                            UpdateActionButtonsState(tag);
                        }
                    }
                    else if (tag.Type == "lxc")
                    {
                        var lxcs = await _client.GetLxcsAsync(tag.NodeName);
                        var freshLxc = lxcs.FirstOrDefault(l => l.VmId == tag.VmId);
                        if (freshLxc != null)
                        {
                            tag.Data = freshLxc;
                            lblUptime.Text = "Container Uptime: " + FormatUptime(freshLxc.Uptime);
                            lblResourceStatus.Text = "Status: " + freshLxc.Status.ToUpper();
                            chartCpu.AddValue(freshLxc.Cpu * 100);
                            chartRam.AddValue(freshLxc.MaxMem > 0 ? ((double)freshLxc.Mem / freshLxc.MaxMem) * 100 : 0);
                            UpdateActionButtonsState(tag);
                        }
                    }
                }
            }
            catch
            {
                // Silence errors during background refreshes to keep UI running smoothly
            }
        }

        // Helpers
        public static string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return string.Format("{0:n1} {1}", number, suffixes[counter]);
        }

        public static string FormatUptime(long seconds)
        {
            var time = TimeSpan.FromSeconds(seconds);
            if (time.TotalDays >= 1)
                return string.Format("{0}d {1}h {2}m", (int)time.TotalDays, time.Hours, time.Minutes);
            if (time.TotalHours >= 1)
                return string.Format("{0}h {1}m", time.Hours, time.Minutes);
            return string.Format("{0}m {1}s", time.Minutes, time.Seconds);
        }
    }

    public class ResourceTag
    {
        public string Type { get; set; }
        public string NodeName { get; set; }
        public int VmId { get; set; }
        public string Name { get; set; }
        public object Data { get; set; }
    }
}
