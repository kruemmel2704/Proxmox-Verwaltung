using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProxmoxVEGui
{
    public partial class CreateResourceDialog : Form
    {
        private readonly MainForm _parent;
        private readonly ProxmoxClient _client;
        private readonly string _type; // "vm" or "lxc"
        private List<PveStorage> _cachedStorages = new List<PveStorage>();

        public CreateResourceDialog(MainForm parent, ProxmoxClient client, string type, List<string> nodes, int defaultVmId)
        {
            _parent = parent;
            _client = client;
            _type = type;

            InitializeComponent();
            IconHelper.ApplyIcon(this);

            // Set Title & Subtitle
            lblTitle.Text = type == "vm" ? "Create Virtual Machine" : "Create LXC Container";
            
            // Set default VMID
            numVmId.Value = defaultVmId;

            // Setup step layouts
            ConfigureTabWizard();
            PopulateComboBoxes();
            
            // Populate target nodes
            cmbNode.Items.Clear();
            foreach (var n in nodes)
            {
                cmbNode.Items.Add(n);
            }
            if (cmbNode.Items.Count > 0)
            {
                cmbNode.SelectedIndex = 0;
            }

            // Bind Node change event to load storage pools dynamically
            cmbNode.SelectedIndexChanged += CmbNode_SelectedIndexChanged;
            
            // Bind OS type change event to show/hide Windows autounattend options
            cmbOsType.SelectedIndexChanged += CmbOsType_SelectedIndexChanged;
            
            // Bind Disk storage change event to toggle format support dynamically
            cmbDiskStorage.SelectedIndexChanged += CmbDiskStorage_SelectedIndexChanged;
            
            // Initialize storage load
            _ = TriggerStorageLoad();

            // Setup first step
            UpdateWizardState();
        }

        private void ConfigureTabWizard()
        {
            // Set styles to hide tabs
            tabWizard.Appearance = TabAppearance.Buttons;
            tabWizard.ItemSize = new Size(0, 1);
            tabWizard.SizeMode = TabSizeMode.Fixed;

            // Configure VM/LXC specific control visibility and defaults
            if (_type == "vm")
            {
                // OS Page
                lblTemplatePath.Visible = false;
                cmbTemplatePath.Visible = false;
                lblOsType.Visible = true;
                cmbOsType.Visible = true;
                lblIsoImage.Visible = true;
                cmbIsoImage.Visible = true;
                UpdateAutounattendVisibility();

                // System Page
                lblScsiController.Visible = true;
                cmbScsiController.Visible = true;
                chkQemuAgent.Visible = true;
                chkUnprivileged.Visible = false;

                // Disks Page
                lblDiskFormat.Visible = true;
                cmbDiskFormat.Visible = true;
                numDiskSize.Value = 32;

                // CPU Page
                lblSockets.Visible = true;
                numSockets.Visible = true;
                lblCpuType.Visible = true;
                cmbCpuType.Visible = true;

                // Memory Page
                chkBallooning.Visible = true;
            }
            else // LXC
            {
                // OS Page
                lblTemplatePath.Visible = true;
                cmbTemplatePath.Visible = true;
                lblOsType.Visible = false;
                cmbOsType.Visible = false;
                lblIsoImage.Visible = false;
                cmbIsoImage.Visible = false;
                chkAutounattend.Visible = false;
                lblAutounattendIso.Visible = false;
                cmbAutounattendIso.Visible = false;

                // System Page
                lblScsiController.Visible = false;
                cmbScsiController.Visible = false;
                chkQemuAgent.Visible = false;
                chkUnprivileged.Visible = true;

                // Disks Page
                lblDiskFormat.Visible = false;
                cmbDiskFormat.Visible = false;
                numDiskSize.Value = 8;

                // CPU Page
                lblSockets.Visible = false;
                numSockets.Visible = false;
                lblCpuType.Visible = false;
                cmbCpuType.Visible = false;

                // Memory Page
                chkBallooning.Visible = false;
                numMemory.Value = 1024;
            }
        }

        private void PopulateComboBoxes()
        {
            // OS Types
            var osTypes = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("l26", "Linux 6.x - 2.6 Kernel"),
                new KeyValuePair<string, string>("win11", "Windows 11 / Server 2022"),
                new KeyValuePair<string, string>("win10", "Windows 10 / Server 2016 / 2019"),
                new KeyValuePair<string, string>("win8", "Windows 8 / Server 2012"),
                new KeyValuePair<string, string>("win7", "Windows 7 / Server 2008"),
                new KeyValuePair<string, string>("other", "Other OS / Generic Linux")
            };
            cmbOsType.DataSource = osTypes;
            cmbOsType.DisplayMember = "Value";
            cmbOsType.ValueMember = "Key";
            cmbOsType.SelectedIndex = 0;

            // SCSI Controllers
            var controllers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("virtio-scsi-pci", "VirtIO SCSI (Recommended)"),
                new KeyValuePair<string, string>("virtio-scsi-single", "VirtIO SCSI Single"),
                new KeyValuePair<string, string>("lsi", "LSI 53C895A"),
                new KeyValuePair<string, string>("megasas", "MegaRAID SAS 8708EM2")
            };
            cmbScsiController.DataSource = controllers;
            cmbScsiController.DisplayMember = "Value";
            cmbScsiController.ValueMember = "Key";
            cmbScsiController.SelectedIndex = 0;

            // CPU Types
            var cpuTypes = new List<string> { "kvm64", "host", "max", "qemu64", "x86-64-v2-AES" };
            cmbCpuType.DataSource = cpuTypes;
            cmbCpuType.SelectedIndex = 0;

            // Disk Formats
            var diskFormats = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("qcow2", "QEMU Image Format (qcow2)"),
                new KeyValuePair<string, string>("raw", "Raw disk image (raw)"),
                new KeyValuePair<string, string>("vmdk", "VMware image format (vmdk)")
            };
            cmbDiskFormat.DataSource = diskFormats;
            cmbDiskFormat.DisplayMember = "Value";
            cmbDiskFormat.ValueMember = "Key";
            cmbDiskFormat.SelectedIndex = 0;
        }

        private async void CmbNode_SelectedIndexChanged(object sender, EventArgs e)
        {
            await TriggerStorageLoad();
        }

        private void CmbDiskStorage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDiskStorage.SelectedItem == null) return;
            string selectedName = cmbDiskStorage.SelectedItem.ToString();
            
            // Find in cached storages list
            var storage = _cachedStorages.FirstOrDefault(s => s.Storage == selectedName);
            if (storage == null) return;

            // Block storages on Proxmox (lvm, lvmthin, zfspool, rbd, cephfs) only support raw format
            bool isBlockStorage = storage.Type == "lvmthin" || 
                                  storage.Type == "lvm" || 
                                  storage.Type == "zfspool" || 
                                  storage.Type == "rbd";

            if (isBlockStorage)
            {
                cmbDiskFormat.SelectedValue = "raw";
                if (cmbDiskFormat.SelectedValue == null || cmbDiskFormat.SelectedValue.ToString() != "raw")
                {
                    // Fallback to manual selection in case binding is loading
                    for (int i = 0; i < cmbDiskFormat.Items.Count; i++)
                    {
                        var item = (KeyValuePair<string, string>)cmbDiskFormat.Items[i];
                        if (item.Key == "raw")
                        {
                            cmbDiskFormat.SelectedIndex = i;
                            break;
                        }
                    }
                }
                cmbDiskFormat.Enabled = false;
            }
            else
            {
                cmbDiskFormat.Enabled = true;
            }
        }

        private async Task TriggerStorageLoad()
        {
            if (cmbNode.SelectedItem == null) return;
            string selectedNode = cmbNode.SelectedItem.ToString();
            
            try
            {
                cmbDiskStorage.Items.Clear();
                cmbDiskStorage.Items.Add("Loading storages...");
                cmbDiskStorage.SelectedIndex = 0;

                var storages = await _client.GetStorageAsync(selectedNode);
                _cachedStorages = storages;

                cmbDiskStorage.Items.Clear();
                foreach (var store in storages.Where(s => s.Active))
                {
                    cmbDiskStorage.Items.Add(store.Storage);
                }

                if (cmbDiskStorage.Items.Count > 0)
                {
                    // Select local-lvm if present, otherwise default to first
                    int defaultIdx = cmbDiskStorage.FindStringExact("local-lvm");
                    if (defaultIdx >= 0)
                        cmbDiskStorage.SelectedIndex = defaultIdx;
                    else
                        cmbDiskStorage.SelectedIndex = 0;
                }
                else
                {
                    cmbDiskStorage.Items.Add("local");
                    cmbDiskStorage.SelectedIndex = 0;
                }

                // Load ISOs or container templates dynamically
                await LoadStorageContentsAsync();
            }
            catch
            {
                cmbDiskStorage.Items.Clear();
                cmbDiskStorage.Items.Add("local");
                cmbDiskStorage.SelectedIndex = 0;
            }
        }

        private async Task LoadStorageContentsAsync()
        {
            if (cmbNode.SelectedItem == null) return;
            string selectedNode = cmbNode.SelectedItem.ToString();

            try
            {
                if (_type == "vm")
                {
                    cmbIsoImage.Items.Clear();
                    cmbIsoImage.Items.Add("Loading ISO images...");
                    cmbIsoImage.SelectedIndex = 0;
                }
                else
                {
                    cmbTemplatePath.Items.Clear();
                    cmbTemplatePath.Items.Add("Loading templates...");
                    cmbTemplatePath.SelectedIndex = 0;
                }

                var files = new List<string>();
                
                // Fetch content from all active storages
                foreach (var store in _cachedStorages.Where(s => s.Active))
                {
                    var contents = await _client.GetStorageContentAsync(selectedNode, store.Storage);
                    foreach (var item in contents)
                    {
                        if (_type == "vm" && item.Content == "iso")
                        {
                            files.Add(item.VolId);
                        }
                        else if (_type == "lxc" && item.Content == "vztmpl")
                        {
                            files.Add(item.VolId);
                        }
                    }
                }

                if (_type == "vm")
                {
                    cmbIsoImage.Items.Clear();
                    cmbAutounattendIso.Items.Clear();
                    foreach (var file in files.OrderBy(f => f))
                    {
                        cmbIsoImage.Items.Add(file);
                        cmbAutounattendIso.Items.Add(file);
                    }

                    if (cmbIsoImage.Items.Count > 0)
                    {
                        cmbIsoImage.SelectedIndex = 0;
                    }
                    else
                    {
                        // Fallback default
                        cmbIsoImage.Items.Add("local:iso/ubuntu-server-22.04.iso");
                        cmbIsoImage.SelectedIndex = 0;
                    }

                    if (cmbAutounattendIso.Items.Count > 0)
                    {
                        // Try to default to an autounattend or virtio iso if one exists
                        int defaultAutounattendIdx = -1;
                        for (int i = 0; i < cmbAutounattendIso.Items.Count; i++)
                        {
                            string itemStr = cmbAutounattendIso.Items[i].ToString().ToLower();
                            if (itemStr.Contains("autounattend") || itemStr.Contains("unattend") || itemStr.Contains("virtio"))
                            {
                                defaultAutounattendIdx = i;
                                break;
                            }
                        }
                        if (defaultAutounattendIdx >= 0)
                            cmbAutounattendIso.SelectedIndex = defaultAutounattendIdx;
                        else
                            cmbAutounattendIso.SelectedIndex = 0;
                    }
                    else
                    {
                        cmbAutounattendIso.Items.Add("local:iso/autounattend.iso");
                        cmbAutounattendIso.SelectedIndex = 0;
                    }
                }
                else
                {
                    cmbTemplatePath.Items.Clear();
                    foreach (var file in files.OrderBy(f => f))
                    {
                        cmbTemplatePath.Items.Add(file);
                    }

                    if (cmbTemplatePath.Items.Count > 0)
                    {
                        cmbTemplatePath.SelectedIndex = 0;
                    }
                    else
                    {
                        // Fallback default
                        cmbTemplatePath.Items.Add("local:vztmpl/debian-12-standard_12.2-1_amd64.tar.zst");
                        cmbTemplatePath.SelectedIndex = 0;
                    }
                }
            }
            catch
            {
                // Fallback default on error
                if (_type == "vm")
                {
                    cmbIsoImage.Items.Clear();
                    cmbIsoImage.Items.Add("local:iso/ubuntu-server-22.04.iso");
                    cmbIsoImage.SelectedIndex = 0;

                    cmbAutounattendIso.Items.Clear();
                    cmbAutounattendIso.Items.Add("local:iso/autounattend.iso");
                    cmbAutounattendIso.SelectedIndex = 0;
                }
                else
                {
                    cmbTemplatePath.Items.Clear();
                    cmbTemplatePath.Items.Add("local:vztmpl/debian-12-standard_12.2-1_amd64.tar.zst");
                    cmbTemplatePath.SelectedIndex = 0;
                }
            }
        }

        private void UpdateWizardState()
        {
            int currentStep = tabWizard.SelectedIndex;
            btnBack.Enabled = currentStep > 0;
            btnNext.Text = currentStep == 7 ? "Finish" : "Next >";

            // Render navigation text dynamically to highlight current active step
            string[] stepNames = { "General", "OS", "System", "Disks", "CPU", "Memory", "Network", "Confirm" };
            var sb = new StringBuilder();
            for (int i = 0; i < stepNames.Length; i++)
            {
                if (i > 0) sb.Append(" ➔ ");
                if (i == currentStep)
                {
                    sb.Append($"[{stepNames[i].ToUpper()}]");
                }
                else
                {
                    sb.Append(stepNames[i]);
                }
            }
            lblWizardSteps.Text = sb.ToString();

            // If on Confirm tab, compile properties summary
            if (currentStep == 7)
            {
                BuildSummaryText();
            }
        }

        private void BuildSummaryText()
        {
            var summary = new StringBuilder();
            summary.AppendLine("==========================================");
            summary.AppendLine($" TYPE:       {(_type == "vm" ? "Virtual Machine (Qemu)" : "LXC Container")}");
            summary.AppendLine($" NODE:       {cmbNode.SelectedItem}");
            summary.AppendLine($" VMID:       {numVmId.Value}");
            summary.AppendLine($" NAME:       {txtName.Text.Trim()}");
            summary.AppendLine("------------------------------------------");
            
            if (_type == "vm")
            {
                summary.AppendLine($" OS TYPE:    {((KeyValuePair<string, string>)cmbOsType.SelectedItem).Value}");
                summary.AppendLine($" ISO IMAGE:  {cmbIsoImage.Text.Trim()}");
                if (chkAutounattend.Visible && chkAutounattend.Checked)
                {
                    summary.AppendLine($" SEC. ISO:   {cmbAutounattendIso.Text.Trim()}");
                }
                summary.AppendLine($" SCSI CONT:  {((KeyValuePair<string, string>)cmbScsiController.SelectedItem).Value}");
                summary.AppendLine($" QEMU AGENT: {(chkQemuAgent.Checked ? "Enabled" : "Disabled")}");
                summary.AppendLine($" DISK:       {cmbDiskStorage.SelectedItem} ({numDiskSize.Value} GB, {((KeyValuePair<string, string>)cmbDiskFormat.SelectedItem).Key})");
                summary.AppendLine($" CPU:        {numSockets.Value} Sockets / {numCores.Value} Cores ({cmbCpuType.SelectedItem})");
                summary.AppendLine($" RAM:        {numMemory.Value} MB (Ballooning: {(chkBallooning.Checked ? "Yes" : "No")})");
            }
            else
            {
                summary.AppendLine($" TEMPLATE:   {cmbTemplatePath.Text.Trim()}");
                summary.AppendLine($" PRIVILEGE:  {(chkUnprivileged.Checked ? "Unprivileged" : "Privileged")}");
                summary.AppendLine($" ROOTFS:     {cmbDiskStorage.SelectedItem} ({numDiskSize.Value} GB)");
                summary.AppendLine($" CPU CORES:  {numCores.Value}");
                summary.AppendLine($" RAM:        {numMemory.Value} MB");
            }

            summary.AppendLine($" NET BRIDGE: {txtBridge.Text.Trim()}");
            summary.AppendLine($" VLAN TAG:   {(numVlan.Value > 0 ? numVlan.Value.ToString() : "No VLAN")}");
            summary.AppendLine($" FIREWALL:   {(chkFirewall.Checked ? "Enabled" : "Disabled")}");
            summary.AppendLine("==========================================");
            txtSummary.Text = summary.ToString();
        }

        private bool ValidateCurrentStep()
        {
            int currentStep = tabWizard.SelectedIndex;

            if (currentStep == 0) // General
            {
                if (cmbNode.SelectedItem == null)
                {
                    MessageBox.Show("Please select a target node.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                string name = txtName.Text.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("Please enter a name for the resource.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (!Regex.IsMatch(name, @"^[a-zA-Z][a-zA-Z0-9\-]*$"))
                {
                    MessageBox.Show("Name must start with a letter and contain only letters, numbers, and hyphens.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else if (currentStep == 1) // OS
            {
                if (_type == "vm" && string.IsNullOrEmpty(cmbIsoImage.Text.Trim()))
                {
                    MessageBox.Show("Please specify an ISO image path (e.g. local:iso/ubuntu.iso).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (_type == "vm" && chkAutounattend.Visible && chkAutounattend.Checked && string.IsNullOrEmpty(cmbAutounattendIso.Text.Trim()))
                {
                    MessageBox.Show("Please specify a secondary ISO image path (e.g. local:iso/autounattend.iso).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (_type == "lxc" && string.IsNullOrEmpty(cmbTemplatePath.Text.Trim()))
                {
                    MessageBox.Show("Please specify a container template path (e.g. local:vztmpl/debian.tar.zst).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else if (currentStep == 3) // Disks
            {
                if (cmbDiskStorage.SelectedItem == null || cmbDiskStorage.SelectedItem.ToString().Contains("Loading"))
                {
                    MessageBox.Show("Please select or wait for a storage pool selection.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else if (currentStep == 6) // Network
            {
                if (string.IsNullOrEmpty(txtBridge.Text.Trim()))
                {
                    MessageBox.Show("Please specify a network bridge (e.g. vmbr0).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (tabWizard.SelectedIndex > 0)
            {
                tabWizard.SelectedIndex--;
                UpdateWizardState();
            }
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            if (!ValidateCurrentStep()) return;

            if (tabWizard.SelectedIndex < 7)
            {
                tabWizard.SelectedIndex++;
                UpdateWizardState();
            }
            else
            {
                // Reached "Finish" on Confirm tab. Submit creation!
                await SubmitResourceCreationAsync();
            }
        }

        private async Task SubmitResourceCreationAsync()
        {
            btnNext.Enabled = false;
            btnBack.Enabled = false;
            btnNext.Text = "Deploying...";

            string node = cmbNode.SelectedItem.ToString();
            int vmid = (int)numVmId.Value;
            string name = txtName.Text.Trim();

            bool success;
            if (_type == "vm")
            {
                var parameters = new Dictionary<string, string>
                {
                    { "vmid", vmid.ToString() },
                    { "name", name },
                    { "cores", numCores.Value.ToString() },
                    { "sockets", numSockets.Value.ToString() },
                    { "memory", numMemory.Value.ToString() },
                    { "scsihw", ((KeyValuePair<string, string>)cmbScsiController.SelectedItem).Key },
                    { "ostype", ((KeyValuePair<string, string>)cmbOsType.SelectedItem).Key },
                    { "agent", chkQemuAgent.Checked ? "1" : "0" }
                };

                // Add ISO content if provided
                string iso = cmbIsoImage.Text.Trim();
                if (!string.IsNullOrEmpty(iso))
                {
                    parameters.Add("ide2", $"{iso},media=cdrom");
                }

                // Add secondary ISO for autounattend if checked
                if (chkAutounattend.Visible && chkAutounattend.Checked)
                {
                    string secIso = cmbAutounattendIso.Text.Trim();
                    if (!string.IsNullOrEmpty(secIso))
                    {
                        parameters.Add("sata4", $"{secIso},media=cdrom");
                    }
                }

                // Add Disk details
                string storage = cmbDiskStorage.SelectedItem.ToString();
                string format = ((KeyValuePair<string, string>)cmbDiskFormat.SelectedItem).Key;
                parameters.Add("scsi0", $"{storage}:{numDiskSize.Value},discard=on,format={format}");

                // Add Network parameters
                string netStr = $"virtio,bridge={txtBridge.Text.Trim()}";
                if (numVlan.Value > 0) netStr += $",tag={numVlan.Value}";
                if (chkFirewall.Checked) netStr += ",firewall=1";
                parameters.Add("net0", netStr);

                // Start after created
                if (chkStartAfterCreated.Checked) parameters.Add("start", "1");

                success = await _client.CreateVmAdvancedAsync(node, parameters);
            }
            else // LXC
            {
                var parameters = new Dictionary<string, string>
                {
                    { "vmid", vmid.ToString() },
                    { "hostname", name },
                    { "cores", numCores.Value.ToString() },
                    { "memory", numMemory.Value.ToString() },
                    { "ostemplate", cmbTemplatePath.Text.Trim() },
                    { "unprivileged", chkUnprivileged.Checked ? "1" : "0" }
                };

                // Add Storage Rootfs details
                string storage = cmbDiskStorage.SelectedItem.ToString();
                parameters.Add("rootfs", $"{storage}:{numDiskSize.Value}");

                // Add Network details
                string netStr = $"name=eth0,bridge={txtBridge.Text.Trim()},ip=dhcp";
                if (numVlan.Value > 0) netStr += $",tag={numVlan.Value}";
                if (chkFirewall.Checked) netStr += ",firewall=1";
                parameters.Add("net0", netStr);

                // Start after created
                if (chkStartAfterCreated.Checked) parameters.Add("start", "1");

                success = await _client.CreateLxcAdvancedAsync(node, parameters);
            }

            if (success)
            {
                MessageBox.Show($"Successfully queued creation of '{name}' (ID: {vmid}).", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await _parent.RefreshDataAsync();
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to create resource. Please review node task logs or details.", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnNext.Enabled = true;
                btnBack.Enabled = true;
                btnNext.Text = "Finish";
            }
        }

        private void panelDragDrop_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private async void panelDragDrop_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files == null || files.Length == 0) return;

            string filePath = files[0];
            string fileName = System.IO.Path.GetFileName(filePath);
            string ext = System.IO.Path.GetExtension(filePath).ToLower();

            // Determine if the file is suitable for the current resource type
            string contentType = "";
            if (_type == "vm")
            {
                if (ext != ".iso")
                {
                    MessageBox.Show("Please drop a valid .iso image file for VM deployment.", "Invalid File Type", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                contentType = "iso";
            }
            else // LXC
            {
                if (ext != ".zst" && ext != ".gz" && ext != ".xz" && ext != ".tgz" && ext != ".tar")
                {
                    MessageBox.Show("Please drop a valid container template (.tar.zst, .tar.gz, .tar.xz) for LXC deployment.", "Invalid File Type", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                contentType = "vztmpl";
            }

            // Target storage: typically "local" holds ISOs and container templates in Proxmox.
            // Let's verify if "local" is active, or use the first active storage.
            string targetStorage = "local";
            if (_cachedStorages != null && _cachedStorages.Count > 0)
            {
                // Find local or any active storage
                var localStore = _cachedStorages.FirstOrDefault(s => s.Storage == "local" && s.Active);
                if (localStore == null)
                {
                    // Fallback to first active storage
                    var activeStore = _cachedStorages.FirstOrDefault(s => s.Active);
                    if (activeStore != null)
                    {
                        targetStorage = activeStore.Storage;
                    }
                }
            }

            if (cmbNode.SelectedItem == null)
            {
                MessageBox.Show("Please select a target node first before uploading templates.", "No Node Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string node = cmbNode.SelectedItem.ToString();

            var confirm = MessageBox.Show(
                $"Do you want to upload '{fileName}' directly to storage '{targetStorage}' on node '{node}'?\n\nThis will transfer the file to the Proxmox server.",
                "Confirm File Upload",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            // Show progress bar and update text
            lblDragDropHint.Visible = false;
            lblUploadStatus.Visible = true;
            pbUploadProgress.Visible = true;
            lblUploadStatus.Text = $"Uploading {fileName}...";

            btnNext.Enabled = false;
            btnBack.Enabled = false;
            btnCancel.Enabled = false;

            try
            {
                bool success = await _client.UploadFileAsync(node, targetStorage, contentType, filePath);

                if (success)
                {
                    MessageBox.Show($"Successfully uploaded '{fileName}' to storage '{targetStorage}'.", "Upload Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Reload storage content to select the new image!
                    await LoadStorageContentsAsync();
                    
                    // Automatically select the uploaded file
                    string expectedVolId = $"{targetStorage}:{contentType}/{fileName}";
                    if (_type == "vm")
                    {
                        int idx = cmbIsoImage.FindStringExact(expectedVolId);
                        if (idx >= 0)
                        {
                            cmbIsoImage.SelectedIndex = idx;
                        }
                        else
                        {
                            cmbIsoImage.Text = expectedVolId;
                        }
                    }
                    else
                    {
                        int idx = cmbTemplatePath.FindStringExact(expectedVolId);
                        if (idx >= 0)
                        {
                            cmbTemplatePath.SelectedIndex = idx;
                        }
                        else
                        {
                            cmbTemplatePath.Text = expectedVolId;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Failed to upload file. Please check Proxmox connection, certificates, and storage capacity.", "Upload Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during upload: {ex.Message}", "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                lblDragDropHint.Visible = true;
                lblUploadStatus.Visible = false;
                pbUploadProgress.Visible = false;

                btnNext.Enabled = true;
                btnBack.Enabled = tabWizard.SelectedIndex > 0;
                btnCancel.Enabled = true;
            }
        }

        private void CmbOsType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAutounattendVisibility();
        }

        private void chkAutounattend_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAutounattendVisibility();
        }

        private void UpdateAutounattendVisibility()
        {
            if (_type != "vm") return;

            bool isWindows = false;
            if (cmbOsType.SelectedValue != null)
            {
                string osKey = cmbOsType.SelectedValue.ToString();
                isWindows = osKey.StartsWith("win");
            }
            else if (cmbOsType.SelectedItem is KeyValuePair<string, string> kvp)
            {
                isWindows = kvp.Key.StartsWith("win");
            }

            chkAutounattend.Visible = isWindows;

            if (isWindows && chkAutounattend.Checked)
            {
                lblAutounattendIso.Visible = true;
                cmbAutounattendIso.Visible = true;
                panelDragDrop.Location = new Point(33, 285);
                panelDragDrop.Height = 70;
                
                lblUploadStatus.Location = new Point(15, 20);
                pbUploadProgress.Location = new Point(15, 45);
            }
            else
            {
                lblAutounattendIso.Visible = false;
                cmbAutounattendIso.Visible = false;
                panelDragDrop.Location = new Point(33, 230);
                panelDragDrop.Height = 100;
                
                lblUploadStatus.Location = new Point(15, 40);
                pbUploadProgress.Location = new Point(15, 65);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
