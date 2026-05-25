namespace ProxmoxVEGui
{
    partial class MainForm
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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblSidebarHeader = new System.Windows.Forms.Label();
            this.treeResources = new System.Windows.Forms.TreeView();
            this.panelContentContainer = new System.Windows.Forms.Panel();
            this.panelConsole = new System.Windows.Forms.Panel();
            this.lblConsoleWarning = new System.Windows.Forms.Label();
            this.webViewConsole = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.panelDashboard = new System.Windows.Forms.Panel();
            this.groupStats = new ProxmoxVEGui.GlassPanel();
            this.chartRam = new ProxmoxVEGui.UsageChart();
            this.chartCpu = new ProxmoxVEGui.UsageChart();
            this.panelSpecsCard = new ProxmoxVEGui.GlassPanel();
            this.lblDetailDisk = new System.Windows.Forms.Label();
            this.lblDetailIp = new System.Windows.Forms.Label();
            this.lblDetailHa = new System.Windows.Forms.Label();
            this.lblDetailNode = new System.Windows.Forms.Label();
            this.lblUptime = new System.Windows.Forms.Label();
            this.lblSpecsMemory = new System.Windows.Forms.Label();
            this.lblSpecsCores = new System.Windows.Forms.Label();
            this.lblResourceID = new System.Windows.Forms.Label();
            this.lblResourceType = new System.Windows.Forms.Label();
            this.lblSpecsTitle = new System.Windows.Forms.Label();
            this.panelTasksLog = new ProxmoxVEGui.GlassPanel();
            this.gridTasks = new System.Windows.Forms.DataGridView();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUser = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTasksHeader = new System.Windows.Forms.Label();
            this.panelSubNavigation = new ProxmoxVEGui.GlassPanel();
            this.btnRefresh = new RoundedButton();
            this.btnCreateLxc = new RoundedButton();
            this.btnCreateVm = new RoundedButton();
            this.btnTabConsole = new RoundedButton();
            this.btnTabConfig = new RoundedButton();
            this.btnTabDashboard = new RoundedButton();
            this.panelHeader = new ProxmoxVEGui.GlassPanel();
            this.flowLayoutActions = new System.Windows.Forms.FlowLayoutPanel();
            this.btnStart = new RoundedButton();
            this.btnStop = new RoundedButton();
            this.btnShutdown = new RoundedButton();
            this.btnReboot = new RoundedButton();
            this.btnDelete = new RoundedButton();
            this.btnLogout = new RoundedButton();
            this.lblResourceStatus = new System.Windows.Forms.Label();
            this.lblSelectedResource = new System.Windows.Forms.Label();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelContentContainer.SuspendLayout();
            this.panelConsole.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webViewConsole)).BeginInit();
            this.panelDashboard.SuspendLayout();
            this.groupStats.SuspendLayout();
            this.panelSpecsCard.SuspendLayout();
            this.panelTasksLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridTasks)).BeginInit();
            this.panelSubNavigation.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.flowLayoutActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Size = new System.Drawing.Size(1184, 681);
            this.splitContainer1.SplitterDistance = 260;
            this.splitContainer1.SplitterWidth = 4;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.splitContainer1.Panel1.Controls.Add(this.lblSidebarHeader);
            this.splitContainer1.Panel1.Controls.Add(this.treeResources);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));
            this.splitContainer1.Panel2.Controls.Add(this.panelContentContainer);
            this.splitContainer1.Panel2.Controls.Add(this.panelTasksLog);
            this.splitContainer1.Panel2.Controls.Add(this.panelSubNavigation);
            this.splitContainer1.Panel2.Controls.Add(this.panelHeader);
            // 
            // lblSidebarHeader
            // 
            this.lblSidebarHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSidebarHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSidebarHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.lblSidebarHeader.Location = new System.Drawing.Point(12, 18);
            this.lblSidebarHeader.Name = "lblSidebarHeader";
            this.lblSidebarHeader.Size = new System.Drawing.Size(236, 30);
            this.lblSidebarHeader.TabIndex = 0;
            this.lblSidebarHeader.Text = "DATACENTER TREE";
            this.lblSidebarHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // treeResources
            // 
            this.treeResources.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeResources.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(18)))), ((int)(((byte)(31)))));
            this.treeResources.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeResources.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeResources.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.treeResources.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.treeResources.Location = new System.Drawing.Point(12, 60);
            this.treeResources.Name = "treeResources";
            this.treeResources.Size = new System.Drawing.Size(236, 609);
            this.treeResources.TabIndex = 1;
            this.treeResources.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeResources_AfterSelect);
            // 
            // panelContentContainer
            // 
            this.panelContentContainer.Controls.Add(this.panelConsole);
            this.panelContentContainer.Controls.Add(this.panelDashboard);
            this.panelContentContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContentContainer.Location = new System.Drawing.Point(0, 115);
            this.panelContentContainer.Name = "panelContentContainer";
            this.panelContentContainer.Size = new System.Drawing.Size(924, 386);
            this.panelContentContainer.TabIndex = 2;
            // 
            // panelConsole
            // 
            this.panelConsole.Controls.Add(this.lblConsoleWarning);
            this.panelConsole.Controls.Add(this.webViewConsole);
            this.panelConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelConsole.Location = new System.Drawing.Point(0, 0);
            this.panelConsole.Name = "panelConsole";
            this.panelConsole.Size = new System.Drawing.Size(924, 386);
            this.panelConsole.TabIndex = 1;
            this.panelConsole.Visible = false;
            // 
            // lblConsoleWarning
            // 
            this.lblConsoleWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblConsoleWarning.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConsoleWarning.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblConsoleWarning.Location = new System.Drawing.Point(0, 0);
            this.lblConsoleWarning.Name = "lblConsoleWarning";
            this.lblConsoleWarning.Size = new System.Drawing.Size(924, 386);
            this.lblConsoleWarning.TabIndex = 1;
            this.lblConsoleWarning.Text = "Select a VM, Container or Node to view its interactive Console/Shell.";
            this.lblConsoleWarning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // webViewConsole
            // 
            this.webViewConsole.AllowExternalDrop = true;
            this.webViewConsole.CreationProperties = null;
            this.webViewConsole.DefaultBackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(18)))), ((int)(((byte)(31)))));
            this.webViewConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webViewConsole.Location = new System.Drawing.Point(0, 0);
            this.webViewConsole.Name = "webViewConsole";
            this.webViewConsole.Size = new System.Drawing.Size(924, 386);
            this.webViewConsole.TabIndex = 0;
            this.webViewConsole.ZoomFactor = 1D;
            // 
            // panelDashboard
            // 
            this.panelDashboard.Controls.Add(this.groupStats);
            this.panelDashboard.Controls.Add(this.panelSpecsCard);
            this.panelDashboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDashboard.Location = new System.Drawing.Point(0, 0);
            this.panelDashboard.Name = "panelDashboard";
            this.panelDashboard.Padding = new System.Windows.Forms.Padding(15);
            this.panelDashboard.Size = new System.Drawing.Size(924, 386);
            this.panelDashboard.TabIndex = 0;
            // 
            // groupStats
            // 
            this.groupStats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupStats.Controls.Add(this.chartRam);
            this.groupStats.Controls.Add(this.chartCpu);
            this.groupStats.Location = new System.Drawing.Point(15, 175);
            this.groupStats.Name = "groupStats";
            this.groupStats.Size = new System.Drawing.Size(894, 196);
            this.groupStats.TabIndex = 1;
            // 
            // chartRam
            // 
            this.chartRam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.chartRam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.chartRam.ChartColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.chartRam.Location = new System.Drawing.Point(455, 0);
            this.chartRam.Name = "chartRam";
            this.chartRam.Size = new System.Drawing.Size(439, 196);
            this.chartRam.Suffix = "%";
            this.chartRam.TabIndex = 1;
            this.chartRam.Title = "Memory Usage";
            // 
            // chartCpu
            // 
            this.chartCpu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.chartCpu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.chartCpu.ChartColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.chartCpu.Location = new System.Drawing.Point(0, 0);
            this.chartCpu.Name = "chartCpu";
            this.chartCpu.Size = new System.Drawing.Size(439, 196);
            this.chartCpu.Suffix = "%";
            this.chartCpu.TabIndex = 0;
            this.chartCpu.Title = "CPU Usage";
            // 
            // panelSpecsCard
            // 
            this.panelSpecsCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSpecsCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.panelSpecsCard.Controls.Add(this.lblDetailDisk);
            this.panelSpecsCard.Controls.Add(this.lblDetailIp);
            this.panelSpecsCard.Controls.Add(this.lblDetailHa);
            this.panelSpecsCard.Controls.Add(this.lblDetailNode);
            this.panelSpecsCard.Controls.Add(this.lblUptime);
            this.panelSpecsCard.Controls.Add(this.lblSpecsMemory);
            this.panelSpecsCard.Controls.Add(this.lblSpecsCores);
            this.panelSpecsCard.Controls.Add(this.lblResourceID);
            this.panelSpecsCard.Controls.Add(this.lblResourceType);
            this.panelSpecsCard.Controls.Add(this.lblSpecsTitle);
            this.panelSpecsCard.Location = new System.Drawing.Point(15, 10);
            this.panelSpecsCard.Name = "panelSpecsCard";
            this.panelSpecsCard.Padding = new System.Windows.Forms.Padding(15);
            this.panelSpecsCard.Size = new System.Drawing.Size(894, 150);
            this.panelSpecsCard.TabIndex = 0;
            // 
            // lblDetailDisk
            // 
            this.lblDetailDisk.AutoSize = true;
            this.lblDetailDisk.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetailDisk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.lblDetailDisk.Location = new System.Drawing.Point(620, 105);
            this.lblDetailDisk.Name = "lblDetailDisk";
            this.lblDetailDisk.Size = new System.Drawing.Size(95, 17);
            this.lblDetailDisk.TabIndex = 9;
            this.lblDetailDisk.Text = "Bootdisk size: -";
            // 
            // lblDetailIp
            // 
            this.lblDetailIp.AutoSize = true;
            this.lblDetailIp.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetailIp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.lblDetailIp.Location = new System.Drawing.Point(620, 75);
            this.lblDetailIp.Name = "lblDetailIp";
            this.lblDetailIp.Size = new System.Drawing.Size(89, 17);
            this.lblDetailIp.TabIndex = 8;
            this.lblDetailIp.Text = "IP Addresses: -";
            // 
            // lblDetailHa
            // 
            this.lblDetailHa.AutoSize = true;
            this.lblDetailHa.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetailHa.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.lblDetailHa.Location = new System.Drawing.Point(620, 45);
            this.lblDetailHa.Name = "lblDetailHa";
            this.lblDetailHa.Size = new System.Drawing.Size(73, 17);
            this.lblDetailHa.TabIndex = 7;
            this.lblDetailHa.Text = "HA State: -";
            // 
            // lblDetailNode
            // 
            this.lblDetailNode.AutoSize = true;
            this.lblDetailNode.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetailNode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.lblDetailNode.Location = new System.Drawing.Point(320, 105);
            this.lblDetailNode.Name = "lblDetailNode";
            this.lblDetailNode.Size = new System.Drawing.Size(81, 17);
            this.lblDetailNode.TabIndex = 6;
            this.lblDetailNode.Text = "Host Node: -";
            // 
            // lblUptime
            // 
            this.lblUptime.AutoSize = true;
            this.lblUptime.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUptime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.lblUptime.Location = new System.Drawing.Point(320, 75);
            this.lblUptime.Name = "lblUptime";
            this.lblUptime.Size = new System.Drawing.Size(59, 17);
            this.lblUptime.TabIndex = 5;
            this.lblUptime.Text = "Uptime: -";
            // 
            // lblSpecsMemory
            // 
            this.lblSpecsMemory.AutoSize = true;
            this.lblSpecsMemory.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpecsMemory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.lblSpecsMemory.Location = new System.Drawing.Point(320, 45);
            this.lblSpecsMemory.Name = "lblSpecsMemory";
            this.lblSpecsMemory.Size = new System.Drawing.Size(107, 17);
            this.lblSpecsMemory.TabIndex = 4;
            this.lblSpecsMemory.Text = "Allocated RAM: -";
            // 
            // lblSpecsCores
            // 
            this.lblSpecsCores.AutoSize = true;
            this.lblSpecsCores.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpecsCores.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.lblSpecsCores.Location = new System.Drawing.Point(20, 105);
            this.lblSpecsCores.Name = "lblSpecsCores";
            this.lblSpecsCores.Size = new System.Drawing.Size(79, 17);
            this.lblSpecsCores.TabIndex = 3;
            this.lblSpecsCores.Text = "CPU Cores: -";
            // 
            // lblResourceID
            // 
            this.lblResourceID.AutoSize = true;
            this.lblResourceID.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResourceID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.lblResourceID.Location = new System.Drawing.Point(20, 75);
            this.lblResourceID.Name = "lblResourceID";
            this.lblResourceID.Size = new System.Drawing.Size(88, 17);
            this.lblResourceID.TabIndex = 2;
            this.lblResourceID.Text = "Resource ID: -";
            // 
            // lblResourceType
            // 
            this.lblResourceType.AutoSize = true;
            this.lblResourceType.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResourceType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.lblResourceType.Location = new System.Drawing.Point(20, 45);
            this.lblResourceType.Name = "lblResourceType";
            this.lblResourceType.Size = new System.Drawing.Size(102, 17);
            this.lblResourceType.TabIndex = 1;
            this.lblResourceType.Text = "Resource Type: -";
            // 
            // lblSpecsTitle
            // 
            this.lblSpecsTitle.AutoSize = true;
            this.lblSpecsTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpecsTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.lblSpecsTitle.Location = new System.Drawing.Point(16, 12);
            this.lblSpecsTitle.Name = "lblSpecsTitle";
            this.lblSpecsTitle.Size = new System.Drawing.Size(162, 21);
            this.lblSpecsTitle.TabIndex = 0;
            this.lblSpecsTitle.Text = "System Specifications";
            // 
            // panelTasksLog
            // 
            this.panelTasksLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.panelTasksLog.Controls.Add(this.gridTasks);
            this.panelTasksLog.Controls.Add(this.lblTasksHeader);
            this.panelTasksLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelTasksLog.Location = new System.Drawing.Point(0, 501);
            this.panelTasksLog.Name = "panelTasksLog";
            this.panelTasksLog.Size = new System.Drawing.Size(924, 180);
            this.panelTasksLog.TabIndex = 3;
            // 
            // gridTasks
            // 
            this.gridTasks.AllowUserToAddRows = false;
            this.gridTasks.AllowUserToDeleteRows = false;
            this.gridTasks.AllowUserToResizeRows = false;
            this.gridTasks.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(18)))), ((int)(((byte)(31)))));
            this.gridTasks.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridTasks.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridTasks.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridTasks.ColumnHeadersHeight = 28;
            this.gridTasks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTime,
            this.colNode,
            this.colUser,
            this.colDescription,
            this.colStatus});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(18)))), ((int)(((byte)(31)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridTasks.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridTasks.EnableHeadersVisualStyles = false;
            this.gridTasks.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.gridTasks.Location = new System.Drawing.Point(0, 30);
            this.gridTasks.MultiSelect = false;
            this.gridTasks.Name = "gridTasks";
            this.gridTasks.ReadOnly = true;
            this.gridTasks.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridTasks.RowHeadersVisible = false;
            this.gridTasks.RowTemplate.Height = 24;
            this.gridTasks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridTasks.Size = new System.Drawing.Size(924, 150);
            this.gridTasks.TabIndex = 1;
            // 
            // colTime
            // 
            this.colTime.HeaderText = "Start Time";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            this.colTime.Width = 140;
            // 
            // colNode
            // 
            this.colNode.HeaderText = "Node";
            this.colNode.Name = "colNode";
            this.colNode.ReadOnly = true;
            this.colNode.Width = 100;
            // 
            // colUser
            // 
            this.colUser.HeaderText = "User name";
            this.colUser.Name = "colUser";
            this.colUser.ReadOnly = true;
            this.colUser.Width = 130;
            // 
            // colDescription
            // 
            this.colDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDescription.HeaderText = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 120;
            // 
            // lblTasksHeader
            // 
            this.lblTasksHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.lblTasksHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTasksHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTasksHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.lblTasksHeader.Location = new System.Drawing.Point(0, 0);
            this.lblTasksHeader.Name = "lblTasksHeader";
            this.lblTasksHeader.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblTasksHeader.Size = new System.Drawing.Size(924, 30);
            this.lblTasksHeader.TabIndex = 0;
            this.lblTasksHeader.Text = "Tasks / Cluster Log";
            this.lblTasksHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelSubNavigation
            // 
            this.panelSubNavigation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.panelSubNavigation.Controls.Add(this.btnRefresh);
            this.panelSubNavigation.Controls.Add(this.btnCreateLxc);
            this.panelSubNavigation.Controls.Add(this.btnCreateVm);
            this.panelSubNavigation.Controls.Add(this.btnTabConsole);
            this.panelSubNavigation.Controls.Add(this.btnTabConfig);
            this.panelSubNavigation.Controls.Add(this.btnTabDashboard);
            this.panelSubNavigation.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSubNavigation.Location = new System.Drawing.Point(0, 75);
            this.panelSubNavigation.Name = "panelSubNavigation";
            this.panelSubNavigation.Size = new System.Drawing.Size(924, 40);
            this.panelSubNavigation.TabIndex = 1;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(744, 5);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(160, 30);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "Refresh Tree / Data";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnCreateLxc
            // 
            this.btnCreateLxc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateLxc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(165)))), ((int)(((byte)(233)))));
            this.btnCreateLxc.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCreateLxc.FlatAppearance.BorderSize = 0;
            this.btnCreateLxc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateLxc.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreateLxc.ForeColor = System.Drawing.Color.White;
            this.btnCreateLxc.Location = new System.Drawing.Point(610, 5);
            this.btnCreateLxc.Name = "btnCreateLxc";
            this.btnCreateLxc.Size = new System.Drawing.Size(120, 30);
            this.btnCreateLxc.TabIndex = 3;
            this.btnCreateLxc.Text = "+ Create LXC";
            this.btnCreateLxc.UseVisualStyleBackColor = false;
            this.btnCreateLxc.Click += new System.EventHandler(this.btnCreateLxc_Click);
            // 
            // btnCreateVm
            // 
            this.btnCreateVm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateVm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnCreateVm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCreateVm.FlatAppearance.BorderSize = 0;
            this.btnCreateVm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateVm.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreateVm.ForeColor = System.Drawing.Color.White;
            this.btnCreateVm.Location = new System.Drawing.Point(480, 5);
            this.btnCreateVm.Name = "btnCreateVm";
            this.btnCreateVm.Size = new System.Drawing.Size(120, 30);
            this.btnCreateVm.TabIndex = 2;
            this.btnCreateVm.Text = "+ Create VM";
            this.btnCreateVm.UseVisualStyleBackColor = false;
            this.btnCreateVm.Click += new System.EventHandler(this.btnCreateVm_Click);
            // 
            // btnTabConsole
            // 
            this.btnTabConsole.BackColor = System.Drawing.Color.Transparent;
            this.btnTabConsole.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTabConsole.FlatAppearance.BorderSize = 0;
            this.btnTabConsole.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(18)))), ((int)(((byte)(31)))));
            this.btnTabConsole.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.btnTabConsole.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabConsole.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTabConsole.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnTabConsole.Location = new System.Drawing.Point(120, 0);
            this.btnTabConsole.Name = "btnTabConsole";
            this.btnTabConsole.Size = new System.Drawing.Size(150, 40);
            this.btnTabConsole.TabIndex = 1;
            this.btnTabConsole.Text = "Console / Shell";
            this.btnTabConsole.UseVisualStyleBackColor = false;
            this.btnTabConsole.Click += new System.EventHandler(this.btnTabConsole_Click);
            // 
            // btnTabConfig
            // 
            this.btnTabConfig.BackColor = System.Drawing.Color.Transparent;
            this.btnTabConfig.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTabConfig.FlatAppearance.BorderSize = 0;
            this.btnTabConfig.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(18)))), ((int)(((byte)(31)))));
            this.btnTabConfig.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.btnTabConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabConfig.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTabConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnTabConfig.Location = new System.Drawing.Point(270, 0);
            this.btnTabConfig.Name = "btnTabConfig";
            this.btnTabConfig.Size = new System.Drawing.Size(150, 40);
            this.btnTabConfig.TabIndex = 5;
            this.btnTabConfig.Text = "⚙ Configuration";
            this.btnTabConfig.UseVisualStyleBackColor = false;
            this.btnTabConfig.Click += new System.EventHandler(this.btnTabConfig_Click);
            // 
            // btnTabDashboard
            // 
            this.btnTabDashboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));
            this.btnTabDashboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTabDashboard.FlatAppearance.BorderSize = 0;
            this.btnTabDashboard.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(18)))), ((int)(((byte)(31)))));
            this.btnTabDashboard.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.btnTabDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabDashboard.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTabDashboard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(115)))), ((int)(((byte)(22)))));
            this.btnTabDashboard.Location = new System.Drawing.Point(0, 0);
            this.btnTabDashboard.Name = "btnTabDashboard";
            this.btnTabDashboard.Size = new System.Drawing.Size(120, 40);
            this.btnTabDashboard.TabIndex = 0;
            this.btnTabDashboard.Text = "Dashboard";
            this.btnTabDashboard.UseVisualStyleBackColor = false;
            this.btnTabDashboard.Click += new System.EventHandler(this.btnTabDashboard_Click);
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.panelHeader.Controls.Add(this.flowLayoutActions);
            this.panelHeader.Controls.Add(this.btnLogout);
            this.panelHeader.Controls.Add(this.lblResourceStatus);
            this.panelHeader.Controls.Add(this.lblSelectedResource);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(924, 75);
            this.panelHeader.TabIndex = 0;
            // 
            // flowLayoutActions
            // 
            this.flowLayoutActions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutActions.Controls.Add(this.btnStart);
            this.flowLayoutActions.Controls.Add(this.btnStop);
            this.flowLayoutActions.Controls.Add(this.btnShutdown);
            this.flowLayoutActions.Controls.Add(this.btnReboot);
            this.flowLayoutActions.Controls.Add(this.btnDelete);
            this.flowLayoutActions.Location = new System.Drawing.Point(344, 18);
            this.flowLayoutActions.Name = "flowLayoutActions";
            this.flowLayoutActions.Size = new System.Drawing.Size(460, 40);
            this.flowLayoutActions.TabIndex = 4;
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.btnStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStart.FlatAppearance.BorderSize = 0;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(3, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(85, 30);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnStop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Location = new System.Drawing.Point(94, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(85, 30);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnShutdown
            // 
            this.btnShutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.btnShutdown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShutdown.FlatAppearance.BorderSize = 0;
            this.btnShutdown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShutdown.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShutdown.ForeColor = System.Drawing.Color.White;
            this.btnShutdown.Location = new System.Drawing.Point(185, 3);
            this.btnShutdown.Name = "btnShutdown";
            this.btnShutdown.Size = new System.Drawing.Size(85, 30);
            this.btnShutdown.TabIndex = 2;
            this.btnShutdown.Text = "Shutdown";
            this.btnShutdown.UseVisualStyleBackColor = false;
            this.btnShutdown.Click += new System.EventHandler(this.btnShutdown_Click);
            // 
            // btnReboot
            // 
            this.btnReboot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnReboot.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReboot.FlatAppearance.BorderSize = 0;
            this.btnReboot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReboot.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReboot.ForeColor = System.Drawing.Color.White;
            this.btnReboot.Location = new System.Drawing.Point(276, 3);
            this.btnReboot.Name = "btnReboot";
            this.btnReboot.Size = new System.Drawing.Size(85, 30);
            this.btnReboot.TabIndex = 3;
            this.btnReboot.Text = "Reboot";
            this.btnReboot.UseVisualStyleBackColor = false;
            this.btnReboot.Click += new System.EventHandler(this.btnReboot_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(113)))), ((int)(((byte)(108)))));
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(367, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(85, 30);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(820, 21);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(85, 30);
            this.btnLogout.TabIndex = 3;
            this.btnLogout.Text = "Log Out";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // lblResourceStatus
            // 
            this.lblResourceStatus.AutoSize = true;
            this.lblResourceStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResourceStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblResourceStatus.Location = new System.Drawing.Point(21, 44);
            this.lblResourceStatus.Name = "lblResourceStatus";
            this.lblResourceStatus.Size = new System.Drawing.Size(127, 19);
            this.lblResourceStatus.TabIndex = 1;
            this.lblResourceStatus.Text = "Uptime / Status: -";
            // 
            // lblSelectedResource
            // 
            this.lblSelectedResource.AutoSize = true;
            this.lblSelectedResource.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedResource.ForeColor = System.Drawing.Color.White;
            this.lblSelectedResource.Location = new System.Drawing.Point(18, 9);
            this.lblSelectedResource.Name = "lblSelectedResource";
            this.lblSelectedResource.Size = new System.Drawing.Size(243, 32);
            this.lblSelectedResource.TabIndex = 0;
            this.lblSelectedResource.Text = "Select a resource...";
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 4000;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(15)))), ((int)(((byte)(25)))));
            this.ClientSize = new System.Drawing.Size(1184, 681);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Proxmox VE Manager - Console & Cluster Administration";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panelContentContainer.ResumeLayout(false);
            this.panelConsole.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webViewConsole)).EndInit();
            this.panelDashboard.ResumeLayout(false);
            this.groupStats.ResumeLayout(false);
            this.panelSpecsCard.ResumeLayout(false);
            this.panelSpecsCard.PerformLayout();
            this.panelTasksLog.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridTasks)).EndInit();
            this.panelSubNavigation.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.flowLayoutActions.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lblSidebarHeader;
        private System.Windows.Forms.TreeView treeResources;
        private ProxmoxVEGui.GlassPanel panelHeader;
        private System.Windows.Forms.Label lblSelectedResource;
        private System.Windows.Forms.Label lblResourceStatus;
        private RoundedButton btnLogout;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutActions;
        private RoundedButton btnStart;
        private RoundedButton btnStop;
        private RoundedButton btnShutdown;
        private RoundedButton btnReboot;
        private RoundedButton btnDelete;
        private ProxmoxVEGui.GlassPanel panelSubNavigation;
        private RoundedButton btnTabDashboard;
        private RoundedButton btnTabConsole;
        private RoundedButton btnTabConfig;
        private RoundedButton btnCreateVm;
        private RoundedButton btnCreateLxc;
        private RoundedButton btnRefresh;
        private System.Windows.Forms.Panel panelContentContainer;
        private System.Windows.Forms.Panel panelDashboard;
        private System.Windows.Forms.Panel panelConsole;
        private Microsoft.Web.WebView2.WinForms.WebView2 webViewConsole;
        private System.Windows.Forms.Label lblConsoleWarning;
        private ProxmoxVEGui.GlassPanel panelSpecsCard;
        private System.Windows.Forms.Label lblSpecsTitle;
        private System.Windows.Forms.Label lblResourceType;
        private System.Windows.Forms.Label lblResourceID;
        private System.Windows.Forms.Label lblSpecsCores;
        private System.Windows.Forms.Label lblSpecsMemory;
        private System.Windows.Forms.Label lblUptime;
        private ProxmoxVEGui.GlassPanel groupStats;
        private ProxmoxVEGui.UsageChart chartCpu;
        private ProxmoxVEGui.UsageChart chartRam;
        private System.Windows.Forms.Label lblDetailNode;
        private System.Windows.Forms.Label lblDetailHa;
        private System.Windows.Forms.Label lblDetailIp;
        private System.Windows.Forms.Label lblDetailDisk;
        private ProxmoxVEGui.GlassPanel panelTasksLog;
        private System.Windows.Forms.Label lblTasksHeader;
        private System.Windows.Forms.DataGridView gridTasks;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUser;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.Timer timerRefresh;
    }
}
