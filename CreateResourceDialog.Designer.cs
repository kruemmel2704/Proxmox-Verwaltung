namespace ProxmoxVEGui
{
    partial class CreateResourceDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblWizardSteps = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.btnBack = new RoundedButton();
            this.btnNext = new RoundedButton();
            this.btnCancel = new RoundedButton();
            this.tabWizard = new System.Windows.Forms.TabControl();
            
            // TabPages
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.tabOS = new System.Windows.Forms.TabPage();
            this.tabSystem = new System.Windows.Forms.TabPage();
            this.tabDisks = new System.Windows.Forms.TabPage();
            this.tabCPU = new System.Windows.Forms.TabPage();
            this.tabMemory = new System.Windows.Forms.TabPage();
            this.tabNetwork = new System.Windows.Forms.TabPage();
            this.tabConfirm = new System.Windows.Forms.TabPage();

            // Controls on TabGeneral
            this.lblNode = new System.Windows.Forms.Label();
            this.cmbNode = new System.Windows.Forms.ComboBox();
            this.lblVmId = new System.Windows.Forms.Label();
            this.numVmId = new System.Windows.Forms.NumericUpDown();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();

            // Controls on TabOS
            this.lblOSHeader = new System.Windows.Forms.Label();
            this.lblOsType = new System.Windows.Forms.Label();
            this.cmbOsType = new System.Windows.Forms.ComboBox();
            this.lblIsoImage = new System.Windows.Forms.Label();
            this.cmbIsoImage = new System.Windows.Forms.ComboBox();
            this.lblTemplatePath = new System.Windows.Forms.Label();
            this.cmbTemplatePath = new System.Windows.Forms.ComboBox();
            this.panelDragDrop = new System.Windows.Forms.Panel();
            this.lblDragDropHint = new System.Windows.Forms.Label();
            this.pbUploadProgress = new System.Windows.Forms.ProgressBar();
            this.lblUploadStatus = new System.Windows.Forms.Label();

            // Controls on TabSystem
            this.lblSystemHeader = new System.Windows.Forms.Label();
            this.lblScsiController = new System.Windows.Forms.Label();
            this.cmbScsiController = new System.Windows.Forms.ComboBox();
            this.chkQemuAgent = new System.Windows.Forms.CheckBox();
            this.chkUnprivileged = new System.Windows.Forms.CheckBox();

            // Controls on TabDisks
            this.lblDisksHeader = new System.Windows.Forms.Label();
            this.lblDiskStorage = new System.Windows.Forms.Label();
            this.cmbDiskStorage = new System.Windows.Forms.ComboBox();
            this.lblDiskSize = new System.Windows.Forms.Label();
            this.numDiskSize = new System.Windows.Forms.NumericUpDown();
            this.lblDiskFormat = new System.Windows.Forms.Label();
            this.cmbDiskFormat = new System.Windows.Forms.ComboBox();

            // Controls on TabCPU
            this.lblCpuHeader = new System.Windows.Forms.Label();
            this.lblSockets = new System.Windows.Forms.Label();
            this.numSockets = new System.Windows.Forms.NumericUpDown();
            this.lblCores = new System.Windows.Forms.Label();
            this.numCores = new System.Windows.Forms.NumericUpDown();
            this.lblCpuType = new System.Windows.Forms.Label();
            this.cmbCpuType = new System.Windows.Forms.ComboBox();

            // Controls on TabMemory
            this.lblMemoryHeader = new System.Windows.Forms.Label();
            this.lblMemory = new System.Windows.Forms.Label();
            this.numMemory = new System.Windows.Forms.NumericUpDown();
            this.chkBallooning = new System.Windows.Forms.CheckBox();

            // Controls on TabNetwork
            this.lblNetworkHeader = new System.Windows.Forms.Label();
            this.lblBridge = new System.Windows.Forms.Label();
            this.txtBridge = new System.Windows.Forms.TextBox();
            this.lblVlan = new System.Windows.Forms.Label();
            this.numVlan = new System.Windows.Forms.NumericUpDown();
            this.chkFirewall = new System.Windows.Forms.CheckBox();

            // Controls on TabConfirm
            this.lblConfirmHeader = new System.Windows.Forms.Label();
            this.txtSummary = new System.Windows.Forms.RichTextBox();
            this.chkStartAfterCreated = new System.Windows.Forms.CheckBox();

            this.panelHeader.SuspendLayout();
            this.panelFooter.SuspendLayout();
            this.tabWizard.SuspendLayout();

            this.tabGeneral.SuspendLayout();
            this.tabOS.SuspendLayout();
            this.tabSystem.SuspendLayout();
            this.tabDisks.SuspendLayout();
            this.tabCPU.SuspendLayout();
            this.tabMemory.SuspendLayout();
            this.tabNetwork.SuspendLayout();
            this.tabConfirm.SuspendLayout();

            ((System.ComponentModel.ISupportInitialize)(this.numVmId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDiskSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSockets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCores)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMemory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVlan)).BeginInit();
            this.SuspendLayout();

            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.panelHeader.Controls.Add(this.lblWizardSteps);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(520, 95);
            this.panelHeader.TabIndex = 0;
            // 
            // lblWizardSteps
            // 
            this.lblWizardSteps.AutoSize = true;
            this.lblWizardSteps.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWizardSteps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblWizardSteps.Location = new System.Drawing.Point(22, 60);
            this.lblWizardSteps.Name = "lblWizardSteps";
            this.lblWizardSteps.Size = new System.Drawing.Size(434, 14);
            this.lblWizardSteps.TabIndex = 1;
            this.lblWizardSteps.Text = "General > OS > System > Disks > CPU > Memory > Network > Confirm";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.lblTitle.Location = new System.Drawing.Point(18, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(262, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Create Virtual Machine";
            // 
            // panelFooter
            // 
            this.panelFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.panelFooter.Controls.Add(this.btnBack);
            this.panelFooter.Controls.Add(this.btnNext);
            this.panelFooter.Controls.Add(this.btnCancel);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 500);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(520, 65);
            this.panelFooter.TabIndex = 1;
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.ForeColor = System.Drawing.Color.White;
            this.btnBack.Location = new System.Drawing.Point(20, 15);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(90, 35);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "< Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.btnNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.ForeColor = System.Drawing.Color.White;
            this.btnNext.Location = new System.Drawing.Point(290, 15);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(100, 35);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Next >";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(405, 15);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(95, 35);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tabWizard
            // 
            this.tabWizard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabWizard.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabWizard.Controls.Add(this.tabGeneral);
            this.tabWizard.Controls.Add(this.tabOS);
            this.tabWizard.Controls.Add(this.tabSystem);
            this.tabWizard.Controls.Add(this.tabDisks);
            this.tabWizard.Controls.Add(this.tabCPU);
            this.tabWizard.Controls.Add(this.tabMemory);
            this.tabWizard.Controls.Add(this.tabNetwork);
            this.tabWizard.Controls.Add(this.tabConfirm);
            this.tabWizard.ItemSize = new System.Drawing.Size(0, 1);
            this.tabWizard.Location = new System.Drawing.Point(15, 110);
            this.tabWizard.Name = "tabWizard";
            this.tabWizard.SelectedIndex = 0;
            this.tabWizard.Size = new System.Drawing.Size(490, 375);
            this.tabWizard.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabWizard.TabIndex = 2;
            // 
            // tabGeneral
            // 
            this.tabGeneral.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.tabGeneral.Controls.Add(this.lblNode);
            this.tabGeneral.Controls.Add(this.cmbNode);
            this.tabGeneral.Controls.Add(this.lblVmId);
            this.tabGeneral.Controls.Add(this.numVmId);
            this.tabGeneral.Controls.Add(this.lblName);
            this.tabGeneral.Controls.Add(this.txtName);
            this.tabGeneral.Location = new System.Drawing.Point(4, 5);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Size = new System.Drawing.Size(482, 366);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            // 
            // lblNode
            // 
            this.lblNode.AutoSize = true;
            this.lblNode.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblNode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblNode.Location = new System.Drawing.Point(30, 30);
            this.lblNode.Name = "lblNode";
            this.lblNode.Size = new System.Drawing.Size(86, 19);
            this.lblNode.TabIndex = 0;
            this.lblNode.Text = "Target Node";
            // 
            // cmbNode
            // 
            this.cmbNode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.cmbNode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbNode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbNode.ForeColor = System.Drawing.Color.White;
            this.cmbNode.FormattingEnabled = true;
            this.cmbNode.Location = new System.Drawing.Point(33, 55);
            this.cmbNode.Name = "cmbNode";
            this.cmbNode.Size = new System.Drawing.Size(400, 25);
            this.cmbNode.TabIndex = 1;
            // 
            // lblVmId
            // 
            this.lblVmId.AutoSize = true;
            this.lblVmId.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblVmId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblVmId.Location = new System.Drawing.Point(30, 110);
            this.lblVmId.Name = "lblVmId";
            this.lblVmId.Size = new System.Drawing.Size(44, 19);
            this.lblVmId.TabIndex = 2;
            this.lblVmId.Text = "VMID";
            // 
            // numVmId
            // 
            this.numVmId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.numVmId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numVmId.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numVmId.ForeColor = System.Drawing.Color.White;
            this.numVmId.Location = new System.Drawing.Point(33, 135);
            this.numVmId.Maximum = new decimal(new int[] { 9999999, 0, 0, 0 });
            this.numVmId.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numVmId.Name = "numVmId";
            this.numVmId.Size = new System.Drawing.Size(180, 25);
            this.numVmId.TabIndex = 3;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblName.Location = new System.Drawing.Point(30, 190);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(46, 19);
            this.lblName.TabIndex = 4;
            this.lblName.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtName.ForeColor = System.Drawing.Color.White;
            this.txtName.Location = new System.Drawing.Point(33, 215);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(400, 25);
            this.txtName.TabIndex = 5;
            // 
            // tabOS
            // 
            this.tabOS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.tabOS.Controls.Add(this.lblOSHeader);
            this.tabOS.Controls.Add(this.lblOsType);
            this.tabOS.Controls.Add(this.cmbOsType);
            this.tabOS.Controls.Add(this.lblIsoImage);
            this.tabOS.Controls.Add(this.cmbIsoImage);
            this.tabOS.Controls.Add(this.lblTemplatePath);
            this.tabOS.Controls.Add(this.cmbTemplatePath);
            this.tabOS.Controls.Add(this.panelDragDrop);
            this.tabOS.Location = new System.Drawing.Point(4, 5);
            this.tabOS.Name = "tabOS";
            this.tabOS.Size = new System.Drawing.Size(482, 366);
            this.tabOS.TabIndex = 1;
            this.tabOS.Text = "OS / Template";
            // 
            // lblOSHeader
            // 
            this.lblOSHeader.AutoSize = true;
            this.lblOSHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblOSHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.lblOSHeader.Location = new System.Drawing.Point(30, 20);
            this.lblOSHeader.Name = "lblOSHeader";
            this.lblOSHeader.Size = new System.Drawing.Size(183, 21);
            this.lblOSHeader.TabIndex = 0;
            this.lblOSHeader.Text = "Operating System / Image";
            // 
            // lblOsType
            // 
            this.lblOsType.AutoSize = true;
            this.lblOsType.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblOsType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblOsType.Location = new System.Drawing.Point(30, 60);
            this.lblOsType.Name = "lblOsType";
            this.lblOsType.Size = new System.Drawing.Size(59, 19);
            this.lblOsType.TabIndex = 1;
            this.lblOsType.Text = "OS Type";
            // 
            // cmbOsType
            // 
            this.cmbOsType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.cmbOsType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOsType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbOsType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbOsType.ForeColor = System.Drawing.Color.White;
            this.cmbOsType.FormattingEnabled = true;
            this.cmbOsType.Location = new System.Drawing.Point(33, 85);
            this.cmbOsType.Name = "cmbOsType";
            this.cmbOsType.Size = new System.Drawing.Size(400, 25);
            this.cmbOsType.TabIndex = 2;
            // 
            // lblIsoImage
            // 
            this.lblIsoImage.AutoSize = true;
            this.lblIsoImage.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblIsoImage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblIsoImage.Location = new System.Drawing.Point(30, 140);
            this.lblIsoImage.Name = "lblIsoImage";
            this.lblIsoImage.Size = new System.Drawing.Size(73, 19);
            this.lblIsoImage.TabIndex = 3;
            this.lblIsoImage.Text = "ISO Image";
            // 
            // cmbIsoImage
            // 
            this.cmbIsoImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.cmbIsoImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbIsoImage.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbIsoImage.ForeColor = System.Drawing.Color.White;
            this.cmbIsoImage.Location = new System.Drawing.Point(33, 165);
            this.cmbIsoImage.Name = "cmbIsoImage";
            this.cmbIsoImage.Size = new System.Drawing.Size(400, 25);
            this.cmbIsoImage.TabIndex = 4;
            // 
            // lblTemplatePath
            // 
            this.lblTemplatePath.AutoSize = true;
            this.lblTemplatePath.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblTemplatePath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblTemplatePath.Location = new System.Drawing.Point(30, 60);
            this.lblTemplatePath.Name = "lblTemplatePath";
            this.lblTemplatePath.Size = new System.Drawing.Size(163, 19);
            this.lblTemplatePath.TabIndex = 5;
            this.lblTemplatePath.Text = "LXC Template (ostemplate)";
            // 
            // cmbTemplatePath
            // 
            this.cmbTemplatePath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.cmbTemplatePath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTemplatePath.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbTemplatePath.ForeColor = System.Drawing.Color.White;
            this.cmbTemplatePath.Location = new System.Drawing.Point(33, 85);
            this.cmbTemplatePath.Name = "cmbTemplatePath";
            this.cmbTemplatePath.Size = new System.Drawing.Size(400, 25);
            this.cmbTemplatePath.TabIndex = 6;
            // 
            // panelDragDrop
            // 
            this.panelDragDrop.AllowDrop = true;
            this.panelDragDrop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.panelDragDrop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDragDrop.Controls.Add(this.lblDragDropHint);
            this.panelDragDrop.Controls.Add(this.pbUploadProgress);
            this.panelDragDrop.Controls.Add(this.lblUploadStatus);
            this.panelDragDrop.Location = new System.Drawing.Point(33, 230);
            this.panelDragDrop.Name = "panelDragDrop";
            this.panelDragDrop.Size = new System.Drawing.Size(400, 100);
            this.panelDragDrop.TabIndex = 7;
            this.panelDragDrop.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelDragDrop_DragEnter);
            this.panelDragDrop.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelDragDrop_DragDrop);
            // 
            // lblDragDropHint
            // 
            this.lblDragDropHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDragDropHint.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Italic);
            this.lblDragDropHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblDragDropHint.Location = new System.Drawing.Point(0, 0);
            this.lblDragDropHint.Name = "lblDragDropHint";
            this.lblDragDropHint.Size = new System.Drawing.Size(398, 98);
            this.lblDragDropHint.TabIndex = 0;
            this.lblDragDropHint.Text = "📥 Drag & Drop local ISO or Container Template\nhere to upload directly to Proxmox storage";
            this.lblDragDropHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbUploadProgress
            // 
            this.pbUploadProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.pbUploadProgress.Location = new System.Drawing.Point(15, 65);
            this.pbUploadProgress.Name = "pbUploadProgress";
            this.pbUploadProgress.Size = new System.Drawing.Size(368, 12);
            this.pbUploadProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbUploadProgress.TabIndex = 1;
            this.pbUploadProgress.Visible = false;
            // 
            // lblUploadStatus
            // 
            this.lblUploadStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.lblUploadStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(230)))), ((int)(((byte)(53)))));
            this.lblUploadStatus.Location = new System.Drawing.Point(15, 40);
            this.lblUploadStatus.Name = "lblUploadStatus";
            this.lblUploadStatus.Size = new System.Drawing.Size(368, 20);
            this.lblUploadStatus.TabIndex = 2;
            this.lblUploadStatus.Text = "Uploading file...";
            this.lblUploadStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblUploadStatus.Visible = false;
            // 
            // tabSystem
            // 
            this.tabSystem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.tabSystem.Controls.Add(this.lblSystemHeader);
            this.tabSystem.Controls.Add(this.lblScsiController);
            this.tabSystem.Controls.Add(this.cmbScsiController);
            this.tabSystem.Controls.Add(this.chkQemuAgent);
            this.tabSystem.Controls.Add(this.chkUnprivileged);
            this.tabSystem.Location = new System.Drawing.Point(4, 5);
            this.tabSystem.Name = "tabSystem";
            this.tabSystem.Size = new System.Drawing.Size(482, 366);
            this.tabSystem.TabIndex = 2;
            this.tabSystem.Text = "System";
            // 
            // lblSystemHeader
            // 
            this.lblSystemHeader.AutoSize = true;
            this.lblSystemHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblSystemHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.lblSystemHeader.Location = new System.Drawing.Point(30, 20);
            this.lblSystemHeader.Name = "lblSystemHeader";
            this.lblSystemHeader.Size = new System.Drawing.Size(123, 21);
            this.lblSystemHeader.TabIndex = 0;
            this.lblSystemHeader.Text = "System Settings";
            // 
            // lblScsiController
            // 
            this.lblScsiController.AutoSize = true;
            this.lblScsiController.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblScsiController.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblScsiController.Location = new System.Drawing.Point(30, 60);
            this.lblScsiController.Name = "lblScsiController";
            this.lblScsiController.Size = new System.Drawing.Size(107, 19);
            this.lblScsiController.TabIndex = 1;
            this.lblScsiController.Text = "SCSI Controller";
            // 
            // cmbScsiController
            // 
            this.cmbScsiController.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.cmbScsiController.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScsiController.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbScsiController.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbScsiController.ForeColor = System.Drawing.Color.White;
            this.cmbScsiController.FormattingEnabled = true;
            this.cmbScsiController.Location = new System.Drawing.Point(33, 85);
            this.cmbScsiController.Name = "cmbScsiController";
            this.cmbScsiController.Size = new System.Drawing.Size(400, 25);
            this.cmbScsiController.TabIndex = 2;
            // 
            // chkQemuAgent
            // 
            this.chkQemuAgent.AutoSize = true;
            this.chkQemuAgent.Checked = true;
            this.chkQemuAgent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkQemuAgent.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.chkQemuAgent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.chkQemuAgent.Location = new System.Drawing.Point(33, 150);
            this.chkQemuAgent.Name = "chkQemuAgent";
            this.chkQemuAgent.Size = new System.Drawing.Size(203, 23);
            this.chkQemuAgent.TabIndex = 3;
            this.chkQemuAgent.Text = "Qemu Agent (vm-agent tag)";
            this.chkQemuAgent.UseVisualStyleBackColor = true;
            // 
            // chkUnprivileged
            // 
            this.chkUnprivileged.AutoSize = true;
            this.chkUnprivileged.Checked = true;
            this.chkUnprivileged.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnprivileged.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.chkUnprivileged.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.chkUnprivileged.Location = new System.Drawing.Point(33, 60);
            this.chkUnprivileged.Name = "chkUnprivileged";
            this.chkUnprivileged.Size = new System.Drawing.Size(176, 23);
            this.chkUnprivileged.TabIndex = 4;
            this.chkUnprivileged.Text = "Unprivileged Container";
            this.chkUnprivileged.UseVisualStyleBackColor = true;
            // 
            // tabDisks
            // 
            this.tabDisks.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.tabDisks.Controls.Add(this.lblDisksHeader);
            this.tabDisks.Controls.Add(this.lblDiskStorage);
            this.tabDisks.Controls.Add(this.cmbDiskStorage);
            this.tabDisks.Controls.Add(this.lblDiskSize);
            this.tabDisks.Controls.Add(this.numDiskSize);
            this.tabDisks.Controls.Add(this.lblDiskFormat);
            this.tabDisks.Controls.Add(this.cmbDiskFormat);
            this.tabDisks.Location = new System.Drawing.Point(4, 5);
            this.tabDisks.Name = "tabDisks";
            this.tabDisks.Size = new System.Drawing.Size(482, 366);
            this.tabDisks.TabIndex = 3;
            this.tabDisks.Text = "Disks";
            // 
            // lblDisksHeader
            // 
            this.lblDisksHeader.AutoSize = true;
            this.lblDisksHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblDisksHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.lblDisksHeader.Location = new System.Drawing.Point(30, 20);
            this.lblDisksHeader.Name = "lblDisksHeader";
            this.lblDisksHeader.Size = new System.Drawing.Size(149, 21);
            this.lblDisksHeader.TabIndex = 0;
            this.lblDisksHeader.Text = "Hard Disk / Rootfs";
            // 
            // lblDiskStorage
            // 
            this.lblDiskStorage.AutoSize = true;
            this.lblDiskStorage.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblDiskStorage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblDiskStorage.Location = new System.Drawing.Point(30, 60);
            this.lblDiskStorage.Name = "lblDiskStorage";
            this.lblDiskStorage.Size = new System.Drawing.Size(89, 19);
            this.lblDiskStorage.TabIndex = 1;
            this.lblDiskStorage.Text = "Disk Storage";
            // 
            // cmbDiskStorage
            // 
            this.cmbDiskStorage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.cmbDiskStorage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDiskStorage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDiskStorage.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbDiskStorage.ForeColor = System.Drawing.Color.White;
            this.cmbDiskStorage.FormattingEnabled = true;
            this.cmbDiskStorage.Location = new System.Drawing.Point(33, 85);
            this.cmbDiskStorage.Name = "cmbDiskStorage";
            this.cmbDiskStorage.Size = new System.Drawing.Size(400, 25);
            this.cmbDiskStorage.TabIndex = 2;
            // 
            // lblDiskSize
            // 
            this.lblDiskSize.AutoSize = true;
            this.lblDiskSize.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblDiskSize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblDiskSize.Location = new System.Drawing.Point(30, 140);
            this.lblDiskSize.Name = "lblDiskSize";
            this.lblDiskSize.Size = new System.Drawing.Size(98, 19);
            this.lblDiskSize.TabIndex = 3;
            this.lblDiskSize.Text = "Disk Size (GB)";
            // 
            // numDiskSize
            // 
            this.numDiskSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.numDiskSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numDiskSize.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numDiskSize.ForeColor = System.Drawing.Color.White;
            this.numDiskSize.Location = new System.Drawing.Point(33, 165);
            this.numDiskSize.Maximum = new decimal(new int[] { 99999, 0, 0, 0 });
            this.numDiskSize.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numDiskSize.Name = "numDiskSize";
            this.numDiskSize.Size = new System.Drawing.Size(180, 25);
            this.numDiskSize.TabIndex = 4;
            this.numDiskSize.Value = new decimal(new int[] { 32, 0, 0, 0 });
            // 
            // lblDiskFormat
            // 
            this.lblDiskFormat.AutoSize = true;
            this.lblDiskFormat.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblDiskFormat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblDiskFormat.Location = new System.Drawing.Point(30, 220);
            this.lblDiskFormat.Name = "lblDiskFormat";
            this.lblDiskFormat.Size = new System.Drawing.Size(83, 19);
            this.lblDiskFormat.TabIndex = 5;
            this.lblDiskFormat.Text = "Disk Format";
            // 
            // cmbDiskFormat
            // 
            this.cmbDiskFormat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.cmbDiskFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDiskFormat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDiskFormat.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbDiskFormat.ForeColor = System.Drawing.Color.White;
            this.cmbDiskFormat.FormattingEnabled = true;
            this.cmbDiskFormat.Location = new System.Drawing.Point(33, 245);
            this.cmbDiskFormat.Name = "cmbDiskFormat";
            this.cmbDiskFormat.Size = new System.Drawing.Size(400, 25);
            this.cmbDiskFormat.TabIndex = 6;
            // 
            // tabCPU
            // 
            this.tabCPU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.tabCPU.Controls.Add(this.lblCpuHeader);
            this.tabCPU.Controls.Add(this.lblSockets);
            this.tabCPU.Controls.Add(this.numSockets);
            this.tabCPU.Controls.Add(this.lblCores);
            this.tabCPU.Controls.Add(this.numCores);
            this.tabCPU.Controls.Add(this.lblCpuType);
            this.tabCPU.Controls.Add(this.cmbCpuType);
            this.tabCPU.Location = new System.Drawing.Point(4, 5);
            this.tabCPU.Name = "tabCPU";
            this.tabCPU.Size = new System.Drawing.Size(482, 366);
            this.tabCPU.TabIndex = 4;
            this.tabCPU.Text = "CPU";
            // 
            // lblCpuHeader
            // 
            this.lblCpuHeader.AutoSize = true;
            this.lblCpuHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblCpuHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.lblCpuHeader.Location = new System.Drawing.Point(30, 20);
            this.lblCpuHeader.Name = "lblCpuHeader";
            this.lblCpuHeader.Size = new System.Drawing.Size(107, 21);
            this.lblCpuHeader.TabIndex = 0;
            this.lblCpuHeader.Text = "CPU Settings";
            // 
            // lblSockets
            // 
            this.lblSockets.AutoSize = true;
            this.lblSockets.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblSockets.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblSockets.Location = new System.Drawing.Point(30, 60);
            this.lblSockets.Name = "lblSockets";
            this.lblSockets.Size = new System.Drawing.Size(56, 19);
            this.lblSockets.TabIndex = 1;
            this.lblSockets.Text = "Sockets";
            // 
            // numSockets
            // 
            this.numSockets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.numSockets.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numSockets.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numSockets.ForeColor = System.Drawing.Color.White;
            this.numSockets.Location = new System.Drawing.Point(33, 85);
            this.numSockets.Maximum = new decimal(new int[] { 8, 0, 0, 0 });
            this.numSockets.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numSockets.Name = "numSockets";
            this.numSockets.Size = new System.Drawing.Size(180, 25);
            this.numSockets.TabIndex = 2;
            this.numSockets.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblCores
            // 
            this.lblCores.AutoSize = true;
            this.lblCores.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblCores.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblCores.Location = new System.Drawing.Point(240, 60);
            this.lblCores.Name = "lblCores";
            this.lblCores.Size = new System.Drawing.Size(43, 19);
            this.lblCores.TabIndex = 3;
            this.lblCores.Text = "Cores";
            // 
            // numCores
            // 
            this.numCores.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.numCores.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numCores.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numCores.ForeColor = System.Drawing.Color.White;
            this.numCores.Location = new System.Drawing.Point(243, 85);
            this.numCores.Maximum = new decimal(new int[] { 128, 0, 0, 0 });
            this.numCores.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numCores.Name = "numCores";
            this.numCores.Size = new System.Drawing.Size(190, 25);
            this.numCores.TabIndex = 4;
            this.numCores.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblCpuType
            // 
            this.lblCpuType.AutoSize = true;
            this.lblCpuType.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblCpuType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblCpuType.Location = new System.Drawing.Point(30, 140);
            this.lblCpuType.Name = "lblCpuType";
            this.lblCpuType.Size = new System.Drawing.Size(68, 19);
            this.lblCpuType.TabIndex = 5;
            this.lblCpuType.Text = "CPU Type";
            // 
            // cmbCpuType
            // 
            this.cmbCpuType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.cmbCpuType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCpuType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCpuType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbCpuType.ForeColor = System.Drawing.Color.White;
            this.cmbCpuType.FormattingEnabled = true;
            this.cmbCpuType.Location = new System.Drawing.Point(33, 165);
            this.cmbCpuType.Name = "cmbCpuType";
            this.cmbCpuType.Size = new System.Drawing.Size(400, 25);
            this.cmbCpuType.TabIndex = 6;
            // 
            // tabMemory
            // 
            this.tabMemory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.tabMemory.Controls.Add(this.lblMemoryHeader);
            this.tabMemory.Controls.Add(this.lblMemory);
            this.tabMemory.Controls.Add(this.numMemory);
            this.tabMemory.Controls.Add(this.chkBallooning);
            this.tabMemory.Location = new System.Drawing.Point(4, 5);
            this.tabMemory.Name = "tabMemory";
            this.tabMemory.Size = new System.Drawing.Size(482, 366);
            this.tabMemory.TabIndex = 5;
            this.tabMemory.Text = "Memory";
            // 
            // lblMemoryHeader
            // 
            this.lblMemoryHeader.AutoSize = true;
            this.lblMemoryHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblMemoryHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.lblMemoryHeader.Location = new System.Drawing.Point(30, 20);
            this.lblMemoryHeader.Name = "lblMemoryHeader";
            this.lblMemoryHeader.Size = new System.Drawing.Size(138, 21);
            this.lblMemoryHeader.TabIndex = 0;
            this.lblMemoryHeader.Text = "Memory Settings";
            // 
            // lblMemory
            // 
            this.lblMemory.AutoSize = true;
            this.lblMemory.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblMemory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblMemory.Location = new System.Drawing.Point(30, 60);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(95, 19);
            this.lblMemory.TabIndex = 1;
            this.lblMemory.Text = "Memory (MB)";
            // 
            // numMemory
            // 
            this.numMemory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.numMemory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numMemory.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numMemory.ForeColor = System.Drawing.Color.White;
            this.numMemory.Increment = new decimal(new int[] { 512, 0, 0, 0 });
            this.numMemory.Location = new System.Drawing.Point(33, 85);
            this.numMemory.Maximum = new decimal(new int[] { 1048576, 0, 0, 0 });
            this.numMemory.Minimum = new decimal(new int[] { 256, 0, 0, 0 });
            this.numMemory.Name = "numMemory";
            this.numMemory.Size = new System.Drawing.Size(400, 25);
            this.numMemory.TabIndex = 2;
            this.numMemory.Value = new decimal(new int[] { 2048, 0, 0, 0 });
            // 
            // chkBallooning
            // 
            this.chkBallooning.AutoSize = true;
            this.chkBallooning.Checked = true;
            this.chkBallooning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBallooning.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.chkBallooning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.chkBallooning.Location = new System.Drawing.Point(33, 140);
            this.chkBallooning.Name = "chkBallooning";
            this.chkBallooning.Size = new System.Drawing.Size(206, 23);
            this.chkBallooning.TabIndex = 3;
            this.chkBallooning.Text = "Enable Ballooning Device (min)";
            this.chkBallooning.UseVisualStyleBackColor = true;
            // 
            // tabNetwork
            // 
            this.tabNetwork.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.tabNetwork.Controls.Add(this.lblNetworkHeader);
            this.tabNetwork.Controls.Add(this.lblBridge);
            this.tabNetwork.Controls.Add(this.txtBridge);
            this.tabNetwork.Controls.Add(this.lblVlan);
            this.tabNetwork.Controls.Add(this.numVlan);
            this.tabNetwork.Controls.Add(this.chkFirewall);
            this.tabNetwork.Location = new System.Drawing.Point(4, 5);
            this.tabNetwork.Name = "tabNetwork";
            this.tabNetwork.Size = new System.Drawing.Size(482, 366);
            this.tabNetwork.TabIndex = 6;
            this.tabNetwork.Text = "Network";
            // 
            // lblNetworkHeader
            // 
            this.lblNetworkHeader.AutoSize = true;
            this.lblNetworkHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblNetworkHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.lblNetworkHeader.Location = new System.Drawing.Point(30, 20);
            this.lblNetworkHeader.Name = "lblNetworkHeader";
            this.lblNetworkHeader.Size = new System.Drawing.Size(139, 21);
            this.lblNetworkHeader.TabIndex = 0;
            this.lblNetworkHeader.Text = "Network Settings";
            // 
            // lblBridge
            // 
            this.lblBridge.AutoSize = true;
            this.lblBridge.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblBridge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblBridge.Location = new System.Drawing.Point(30, 60);
            this.lblBridge.Name = "lblBridge";
            this.lblBridge.Size = new System.Drawing.Size(48, 19);
            this.lblBridge.TabIndex = 1;
            this.lblBridge.Text = "Bridge";
            // 
            // txtBridge
            // 
            this.txtBridge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtBridge.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBridge.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtBridge.ForeColor = System.Drawing.Color.White;
            this.txtBridge.Location = new System.Drawing.Point(33, 85);
            this.txtBridge.Name = "txtBridge";
            this.txtBridge.Size = new System.Drawing.Size(400, 25);
            this.txtBridge.TabIndex = 2;
            this.txtBridge.Text = "vmbr0";
            // 
            // lblVlan
            // 
            this.lblVlan.AutoSize = true;
            this.lblVlan.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblVlan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblVlan.Location = new System.Drawing.Point(30, 140);
            this.lblVlan.Name = "lblVlan";
            this.lblVlan.Size = new System.Drawing.Size(127, 19);
            this.lblVlan.TabIndex = 3;
            this.lblVlan.Text = "VLAN Tag (optional)";
            // 
            // numVlan
            // 
            this.numVlan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.numVlan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numVlan.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numVlan.ForeColor = System.Drawing.Color.White;
            this.numVlan.Location = new System.Drawing.Point(33, 165);
            this.numVlan.Maximum = new decimal(new int[] { 4094, 0, 0, 0 });
            this.numVlan.Name = "numVlan";
            this.numVlan.Size = new System.Drawing.Size(180, 25);
            this.numVlan.TabIndex = 4;
            // 
            // chkFirewall
            // 
            this.chkFirewall.AutoSize = true;
            this.chkFirewall.Checked = true;
            this.chkFirewall.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFirewall.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.chkFirewall.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.chkFirewall.Location = new System.Drawing.Point(33, 220);
            this.chkFirewall.Name = "chkFirewall";
            this.chkFirewall.Size = new System.Drawing.Size(128, 23);
            this.chkFirewall.TabIndex = 5;
            this.chkFirewall.Text = "Enable Firewall";
            this.chkFirewall.UseVisualStyleBackColor = true;
            // 
            // tabConfirm
            // 
            this.tabConfirm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.tabConfirm.Controls.Add(this.lblConfirmHeader);
            this.tabConfirm.Controls.Add(this.txtSummary);
            this.tabConfirm.Controls.Add(this.chkStartAfterCreated);
            this.tabConfirm.Location = new System.Drawing.Point(4, 5);
            this.tabConfirm.Name = "tabConfirm";
            this.tabConfirm.Size = new System.Drawing.Size(482, 366);
            this.tabConfirm.TabIndex = 7;
            this.tabConfirm.Text = "Confirm";
            // 
            // lblConfirmHeader
            // 
            this.lblConfirmHeader.AutoSize = true;
            this.lblConfirmHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblConfirmHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.lblConfirmHeader.Location = new System.Drawing.Point(30, 20);
            this.lblConfirmHeader.Name = "lblConfirmHeader";
            this.lblConfirmHeader.Size = new System.Drawing.Size(157, 21);
            this.lblConfirmHeader.TabIndex = 0;
            this.lblConfirmHeader.Text = "Confirm Parameters";
            // 
            // txtSummary
            // 
            this.txtSummary.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtSummary.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSummary.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(230)))), ((int)(((byte)(53)))));
            this.txtSummary.Location = new System.Drawing.Point(33, 50);
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.ReadOnly = true;
            this.txtSummary.Size = new System.Drawing.Size(415, 230);
            this.txtSummary.TabIndex = 1;
            this.txtSummary.Text = "Configuration Summary...";
            // 
            // chkStartAfterCreated
            // 
            this.chkStartAfterCreated.AutoSize = true;
            this.chkStartAfterCreated.Checked = true;
            this.chkStartAfterCreated.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStartAfterCreated.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.chkStartAfterCreated.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.chkStartAfterCreated.Location = new System.Drawing.Point(33, 295);
            this.chkStartAfterCreated.Name = "chkStartAfterCreated";
            this.chkStartAfterCreated.Size = new System.Drawing.Size(201, 23);
            this.chkStartAfterCreated.TabIndex = 2;
            this.chkStartAfterCreated.Text = "Start after created (boot)";
            this.chkStartAfterCreated.UseVisualStyleBackColor = true;
            // 
            // CreateResourceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(520, 565);
            this.Controls.Add(this.tabWizard);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateResourceDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Proxmox Resource Wizard";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelFooter.ResumeLayout(false);
            this.tabWizard.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tabOS.ResumeLayout(false);
            this.tabOS.PerformLayout();
            this.tabSystem.ResumeLayout(false);
            this.tabSystem.PerformLayout();
            this.tabDisks.ResumeLayout(false);
            this.tabDisks.PerformLayout();
            this.tabCPU.ResumeLayout(false);
            this.tabCPU.PerformLayout();
            this.tabMemory.ResumeLayout(false);
            this.tabMemory.PerformLayout();
            this.tabNetwork.ResumeLayout(false);
            this.tabNetwork.PerformLayout();
            this.tabConfirm.ResumeLayout(false);
            this.tabConfirm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVmId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDiskSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSockets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCores)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMemory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVlan)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblWizardSteps;
        private System.Windows.Forms.Panel panelFooter;
        private RoundedButton btnBack;
        private RoundedButton btnNext;
        private RoundedButton btnCancel;
        private System.Windows.Forms.TabControl tabWizard;
        
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabOS;
        private System.Windows.Forms.TabPage tabSystem;
        private System.Windows.Forms.TabPage tabDisks;
        private System.Windows.Forms.TabPage tabCPU;
        private System.Windows.Forms.TabPage tabMemory;
        private System.Windows.Forms.TabPage tabNetwork;
        private System.Windows.Forms.TabPage tabConfirm;

        // General Tab
        private System.Windows.Forms.Label lblNode;
        private System.Windows.Forms.ComboBox cmbNode;
        private System.Windows.Forms.Label lblVmId;
        private System.Windows.Forms.NumericUpDown numVmId;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;

        // OS / Template Tab
        private System.Windows.Forms.Label lblOSHeader;
        private System.Windows.Forms.Label lblOsType;
        private System.Windows.Forms.ComboBox cmbOsType;
        private System.Windows.Forms.Label lblIsoImage;
        private System.Windows.Forms.ComboBox cmbIsoImage;
        private System.Windows.Forms.Label lblTemplatePath;
        private System.Windows.Forms.ComboBox cmbTemplatePath;
        private System.Windows.Forms.Panel panelDragDrop;
        private System.Windows.Forms.Label lblDragDropHint;
        private System.Windows.Forms.ProgressBar pbUploadProgress;
        private System.Windows.Forms.Label lblUploadStatus;

        // System Tab
        private System.Windows.Forms.Label lblSystemHeader;
        private System.Windows.Forms.Label lblScsiController;
        private System.Windows.Forms.ComboBox cmbScsiController;
        private System.Windows.Forms.CheckBox chkQemuAgent;
        private System.Windows.Forms.CheckBox chkUnprivileged;

        // Disks Tab
        private System.Windows.Forms.Label lblDisksHeader;
        private System.Windows.Forms.Label lblDiskStorage;
        private System.Windows.Forms.ComboBox cmbDiskStorage;
        private System.Windows.Forms.Label lblDiskSize;
        private System.Windows.Forms.NumericUpDown numDiskSize;
        private System.Windows.Forms.Label lblDiskFormat;
        private System.Windows.Forms.ComboBox cmbDiskFormat;

        // CPU Tab
        private System.Windows.Forms.Label lblCpuHeader;
        private System.Windows.Forms.Label lblSockets;
        private System.Windows.Forms.NumericUpDown numSockets;
        private System.Windows.Forms.Label lblCores;
        private System.Windows.Forms.NumericUpDown numCores;
        private System.Windows.Forms.Label lblCpuType;
        private System.Windows.Forms.ComboBox cmbCpuType;

        // Memory Tab
        private System.Windows.Forms.Label lblMemoryHeader;
        private System.Windows.Forms.Label lblMemory;
        private System.Windows.Forms.NumericUpDown numMemory;
        private System.Windows.Forms.CheckBox chkBallooning;

        // Network Tab
        private System.Windows.Forms.Label lblNetworkHeader;
        private System.Windows.Forms.Label lblBridge;
        private System.Windows.Forms.TextBox txtBridge;
        private System.Windows.Forms.Label lblVlan;
        private System.Windows.Forms.NumericUpDown numVlan;
        private System.Windows.Forms.CheckBox chkFirewall;

        // Confirm Tab
        private System.Windows.Forms.Label lblConfirmHeader;
        private System.Windows.Forms.RichTextBox txtSummary;
        private System.Windows.Forms.CheckBox chkStartAfterCreated;
    }
}
