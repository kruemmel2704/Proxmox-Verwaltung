using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProxmoxVEGui
{
    public partial class MainForm : Form
    {
        private readonly ProxmoxClient _client;
        private List<PveNode> _cachedNodes = new List<PveNode>();
        private bool _webViewInitialized = false;
        private string _lastSelectedKey = "";
        private ConfigPanel _configPanel;

        private readonly Color _treeBackColor = Color.FromArgb(15, 23, 42);
        private readonly Color _treeTextColor = Color.FromArgb(226, 232, 240);
        private readonly Color _treeMutedTextColor = Color.FromArgb(148, 163, 184);
        private readonly Color _treeGroupTextColor = Color.FromArgb(203, 213, 225);
        private readonly Color _treeAccentColor = Color.FromArgb(249, 115, 22);
        private readonly Color _treeLineColor = Color.FromArgb(71, 85, 105);
        private readonly Color _treeSelectedBackColor = Color.FromArgb(249, 115, 22);
        private readonly Color _treeSelectedTextColor = Color.White;

        private readonly Color _statusRunningColor = Color.FromArgb(34, 197, 94);
        private readonly Color _statusStoppedColor = Color.FromArgb(239, 68, 68);

        // Resource context menu (right click on VM/LXC)
        private ContextMenuStrip _resourceContextMenu;
        private ToolStripMenuItem _contextStartItem;
        private ToolStripMenuItem _contextStopItem;

        // TreeView scrollbar
        private ModernScrollbarPart _treeScrollTrack;
        private ModernScrollbarPart _treeScrollThumb;
        private TreeViewNativeScrollbarHider _treeScrollbarHider;

        private bool _treeScrollDragging = false;
        private int _treeScrollDragOffsetY = 0;

        private const int TreeScrollbarWidth = 8;
        private const int TreeScrollbarMargin = 5;
        private const int TreeScrollbarMinThumbHeight = 42;

        // DataGridView scrollbar
        private ModernScrollbarPart _gridScrollTrack;
        private ModernScrollbarPart _gridScrollThumb;
        private DataGridViewNativeScrollbarHider _gridScrollbarHider;

        private bool _gridScrollDragging = false;
        private int _gridScrollDragOffsetY = 0;

        private const int GridScrollbarWidth = 8;
        private const int GridScrollbarMargin = 5;
        private const int GridScrollbarMinThumbHeight = 42;

        public MainForm(ProxmoxClient client)
        {
            _client = client;
            InitializeComponent();
            ApplyApplicationIcon();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            lblVersion.Text = "v" + typeof(Program).Assembly.GetName().Version.ToString(3);
            btnCheckUpdate.BorderRadius = 4;

            ApplyTreeViewDesign();
            ApplyGridScrollbar();

            _configPanel = new ConfigPanel(_client);
            panelContentContainer.Controls.Add(_configPanel);

            await RefreshDataAsync();
            SelectDatacenterNode();

            timerRefresh.Start();

            // Run background update check silently without blocking startup load
            _ = Task.Run(() => Updater.CheckForUpdatesAsync(this, silent: true));
        }

        // ─────────────────────────────────────────────────────────────────────
        // TREEVIEW SCROLLBAR
        // ─────────────────────────────────────────────────────────────────────

        private void ApplyTreeViewDesign()
        {
            treeResources.BackColor = _treeBackColor;
            treeResources.ForeColor = _treeTextColor;
            treeResources.LineColor = _treeLineColor;
            treeResources.BorderStyle = BorderStyle.None;

            treeResources.HideSelection = false;
            treeResources.HotTracking = false;
            treeResources.FullRowSelect = false;

            treeResources.ShowLines = true;
            treeResources.ShowRootLines = true;
            treeResources.ShowPlusMinus = true;
            treeResources.ShowNodeToolTips = true;

            treeResources.ItemHeight = 23;
            treeResources.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            treeResources.Scrollable = true;

            treeResources.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeResources.DrawNode -= treeResources_DrawNode;
            treeResources.DrawNode += treeResources_DrawNode;

            treeResources.MouseWheel -= treeResources_MouseWheel;
            treeResources.MouseWheel += treeResources_MouseWheel;

            treeResources.AfterExpand -= treeResources_AfterExpandCollapse;
            treeResources.AfterExpand += treeResources_AfterExpandCollapse;

            treeResources.AfterCollapse -= treeResources_AfterExpandCollapse;
            treeResources.AfterCollapse += treeResources_AfterExpandCollapse;

            treeResources.AfterSelect -= treeResources_AfterSelect;
            treeResources.AfterSelect += treeResources_AfterSelect;

            treeResources.KeyDown -= treeResources_KeyDown;
            treeResources.KeyDown += treeResources_KeyDown;

            treeResources.NodeMouseClick -= treeResources_NodeMouseClick;
            treeResources.NodeMouseClick += treeResources_NodeMouseClick;

            treeResources.Resize -= treeResources_ResizeOrMove;
            treeResources.Resize += treeResources_ResizeOrMove;

            treeResources.LocationChanged -= treeResources_ResizeOrMove;
            treeResources.LocationChanged += treeResources_ResizeOrMove;

            _treeScrollbarHider = new TreeViewNativeScrollbarHider();
            _treeScrollbarHider.Attach(treeResources);

            InitializeResourceContextMenu();
            CreateTreeScrollbar();
            PositionTreeScrollbar();
            HideNativeTreeScrollbars();
            UpdateTreeScrollbar();
        }

        private void CreateTreeScrollbar()
        {
            if (_treeScrollTrack == null)
            {
                _treeScrollTrack = new ModernScrollbarPart
                {
                    FillColor = Color.FromArgb(30, 41, 59),
                    HoverColor = Color.FromArgb(30, 41, 59),
                    Radius = 4,
                    Cursor = Cursors.Hand,
                    Visible = true
                };

                _treeScrollTrack.MouseDown += TreeScrollbarTrack_MouseDown;
                _treeScrollTrack.MouseMove += TreeScrollbar_MouseMove;
                _treeScrollTrack.MouseUp += TreeScrollbar_MouseUp;
                _treeScrollTrack.MouseWheel += TreeScrollbar_MouseWheel;
            }

            if (_treeScrollTrack.Parent != treeResources)
            {
                _treeScrollTrack.Parent?.Controls.Remove(_treeScrollTrack);
                treeResources.Controls.Add(_treeScrollTrack);
            }

            if (_treeScrollThumb == null)
            {
                _treeScrollThumb = new ModernScrollbarPart
                {
                    FillColor = Color.FromArgb(249, 115, 22),
                    HoverColor = Color.FromArgb(251, 146, 60),
                    Radius = 4,
                    Cursor = Cursors.Hand,
                    Visible = true
                };

                _treeScrollThumb.MouseDown += TreeScrollbarThumb_MouseDown;
                _treeScrollThumb.MouseMove += TreeScrollbar_MouseMove;
                _treeScrollThumb.MouseUp += TreeScrollbar_MouseUp;
                _treeScrollThumb.MouseWheel += TreeScrollbar_MouseWheel;
            }

            if (_treeScrollThumb.Parent != _treeScrollTrack)
            {
                _treeScrollThumb.Parent?.Controls.Remove(_treeScrollThumb);
                _treeScrollTrack.Controls.Add(_treeScrollThumb);
            }

            _treeScrollTrack.BringToFront();
            _treeScrollThumb.BringToFront();
            treeResources.Controls.SetChildIndex(_treeScrollTrack, 0);
        }

        private void PositionTreeScrollbar()
        {
            if (_treeScrollTrack == null) return;

            int x = treeResources.ClientSize.Width - TreeScrollbarWidth - TreeScrollbarMargin;
            int y = TreeScrollbarMargin;
            int height = treeResources.ClientSize.Height - (TreeScrollbarMargin * 2);

            if (height < 30 || x < 0)
            {
                _treeScrollTrack.Visible = false;
                return;
            }

            _treeScrollTrack.Bounds = new Rectangle(x, y, TreeScrollbarWidth, height);
            _treeScrollTrack.Visible = true;
            _treeScrollTrack.BringToFront();

            if (_treeScrollTrack.Parent == treeResources)
            {
                treeResources.Controls.SetChildIndex(_treeScrollTrack, 0);
            }
        }

        private void HideNativeTreeScrollbars()
        {
            _treeScrollbarHider?.HideNow();
        }

        private void ApplyApplicationIcon()
        {
            try
            {
                string[] possibleIconPaths =
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "img", "logo.ico"),
                    Path.Combine(Application.StartupPath, "assets", "img", "logo.ico"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.ico")
                };

                foreach (string iconPath in possibleIconPaths)
                {
                    if (File.Exists(iconPath))
                    {
                        this.Icon = new Icon(iconPath);
                        return;
                    }
                }
            }
            catch
            {
                // Icon ist optional. Die App soll trotzdem starten.
            }
        }

        private void UpdateTreeScrollbar()
        {
            if (_treeScrollTrack == null || _treeScrollThumb == null) return;

            HideNativeTreeScrollbars();

            List<TreeNode> visibleNodes = GetVisibleTreeNodes();
            int totalVisibleNodes = visibleNodes.Count;
            int visibleCapacity = GetTreeVisibleCapacity();

            if (totalVisibleNodes <= 0)
            {
                _treeScrollTrack.Visible = false;
                return;
            }

            bool needsScrollbar = totalVisibleNodes > visibleCapacity;

            _treeScrollTrack.Visible = true;
            _treeScrollTrack.Enabled = true;
            _treeScrollTrack.BringToFront();

            if (_treeScrollTrack.Parent == treeResources)
            {
                treeResources.Controls.SetChildIndex(_treeScrollTrack, 0);
            }

            int trackHeight = Math.Max(1, _treeScrollTrack.Height);

            if (!needsScrollbar)
            {
                _treeScrollThumb.FillColor = Color.FromArgb(71, 85, 105);
                _treeScrollThumb.HoverColor = Color.FromArgb(100, 116, 139);
                _treeScrollThumb.Bounds = new Rectangle(0, 0, TreeScrollbarWidth, trackHeight);
                _treeScrollThumb.Visible = true;
                _treeScrollThumb.BringToFront();
                _treeScrollThumb.Invalidate();
                _treeScrollTrack.Invalidate();
                return;
            }

            _treeScrollThumb.FillColor = Color.FromArgb(249, 115, 22);
            _treeScrollThumb.HoverColor = Color.FromArgb(251, 146, 60);

            int thumbHeight = Math.Max(
                TreeScrollbarMinThumbHeight,
                (int)Math.Round((double)visibleCapacity / totalVisibleNodes * trackHeight)
            );

            thumbHeight = Math.Min(trackHeight, thumbHeight);

            int maxThumbTop = Math.Max(0, trackHeight - thumbHeight);
            int maxTopIndex = Math.Max(1, totalVisibleNodes - visibleCapacity);

            int topIndex = 0;

            if (treeResources.TopNode != null)
            {
                topIndex = visibleNodes.IndexOf(treeResources.TopNode);
                if (topIndex < 0) topIndex = 0;
            }

            topIndex = ClampInt(topIndex, 0, maxTopIndex);

            int thumbTop = maxTopIndex > 0
                ? (int)Math.Round((double)topIndex / maxTopIndex * maxThumbTop)
                : 0;

            _treeScrollThumb.Bounds = new Rectangle(0, thumbTop, TreeScrollbarWidth, thumbHeight);
            _treeScrollThumb.Visible = true;
            _treeScrollThumb.BringToFront();

            _treeScrollThumb.Invalidate();
            _treeScrollTrack.Invalidate();
        }

        private List<TreeNode> GetVisibleTreeNodes()
        {
            var nodes = new List<TreeNode>();

            foreach (TreeNode node in treeResources.Nodes)
            {
                AddVisibleTreeNode(node, nodes);
            }

            return nodes;
        }

        private void AddVisibleTreeNode(TreeNode node, List<TreeNode> nodes)
        {
            if (node == null) return;

            nodes.Add(node);

            if (!node.IsExpanded) return;

            foreach (TreeNode child in node.Nodes)
            {
                AddVisibleTreeNode(child, nodes);
            }
        }

        private int GetTreeVisibleCapacity()
        {
            int itemHeight = Math.Max(1, treeResources.ItemHeight);
            int usableHeight = Math.Max(1, treeResources.ClientSize.Height - (TreeScrollbarMargin * 2));

            return Math.Max(1, usableHeight / itemHeight);
        }

        private void ScrollTreeBy(int delta)
        {
            List<TreeNode> visibleNodes = GetVisibleTreeNodes();
            if (visibleNodes.Count == 0) return;

            int visibleCapacity = GetTreeVisibleCapacity();
            int maxTopIndex = Math.Max(0, visibleNodes.Count - visibleCapacity);

            int currentIndex = 0;
            if (treeResources.TopNode != null)
            {
                currentIndex = visibleNodes.IndexOf(treeResources.TopNode);
                if (currentIndex < 0) currentIndex = 0;
            }

            int newIndex = ClampInt(currentIndex + delta, 0, maxTopIndex);

            if (newIndex >= 0 && newIndex < visibleNodes.Count)
            {
                treeResources.TopNode = visibleNodes[newIndex];
            }

            HideNativeTreeScrollbars();
            UpdateTreeScrollbar();
        }

        private void ScrollTreeToThumbTop(int thumbTop)
        {
            if (_treeScrollTrack == null || _treeScrollThumb == null) return;

            List<TreeNode> visibleNodes = GetVisibleTreeNodes();
            if (visibleNodes.Count == 0) return;

            int visibleCapacity = GetTreeVisibleCapacity();
            int maxTopIndex = Math.Max(0, visibleNodes.Count - visibleCapacity);

            int maxThumbTop = Math.Max(1, _treeScrollTrack.Height - _treeScrollThumb.Height);
            int cleanThumbTop = ClampInt(thumbTop, 0, maxThumbTop);

            double ratio = (double)cleanThumbTop / maxThumbTop;
            int targetIndex = ClampInt((int)Math.Round(ratio * maxTopIndex), 0, maxTopIndex);

            if (targetIndex >= 0 && targetIndex < visibleNodes.Count)
            {
                treeResources.TopNode = visibleNodes[targetIndex];
            }

            HideNativeTreeScrollbars();
            UpdateTreeScrollbar();
        }

        private void treeResources_MouseWheel(object sender, MouseEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                PositionTreeScrollbar();
                HideNativeTreeScrollbars();
                UpdateTreeScrollbar();
            }));
        }

        private void TreeScrollbar_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0) ScrollTreeBy(3);
            else if (e.Delta > 0) ScrollTreeBy(-3);
        }

        private void treeResources_KeyDown(object sender, KeyEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                PositionTreeScrollbar();
                HideNativeTreeScrollbars();
                UpdateTreeScrollbar();
            }));
        }

        private void treeResources_AfterExpandCollapse(object sender, TreeViewEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                PositionTreeScrollbar();
                HideNativeTreeScrollbars();
                UpdateTreeScrollbar();
            }));
        }

        private void treeResources_ResizeOrMove(object sender, EventArgs e)
        {
            PositionTreeScrollbar();
            HideNativeTreeScrollbars();
            UpdateTreeScrollbar();
        }

        private void TreeScrollbarTrack_MouseDown(object sender, MouseEventArgs e)
        {
            if (_treeScrollThumb == null) return;

            int visibleCapacity = GetTreeVisibleCapacity();

            if (e.Y < _treeScrollThumb.Top) ScrollTreeBy(-visibleCapacity);
            else if (e.Y > _treeScrollThumb.Bottom) ScrollTreeBy(visibleCapacity);
        }

        private void TreeScrollbarThumb_MouseDown(object sender, MouseEventArgs e)
        {
            if (_treeScrollThumb == null) return;

            _treeScrollDragging = true;
            _treeScrollDragOffsetY = e.Y;
            _treeScrollThumb.Capture = true;
            _treeScrollThumb.FillColor = Color.FromArgb(234, 88, 12);
            _treeScrollThumb.Invalidate();
        }

        private void TreeScrollbar_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_treeScrollDragging || _treeScrollTrack == null || _treeScrollThumb == null) return;

            Point mouseInTrack;

            if (sender == _treeScrollThumb)
                mouseInTrack = _treeScrollTrack.PointToClient(_treeScrollThumb.PointToScreen(e.Location));
            else
                mouseInTrack = e.Location;

            int newThumbTop = mouseInTrack.Y - _treeScrollDragOffsetY;
            ScrollTreeToThumbTop(newThumbTop);
        }

        private void TreeScrollbar_MouseUp(object sender, MouseEventArgs e)
        {
            if (_treeScrollThumb == null) return;

            _treeScrollDragging = false;
            _treeScrollThumb.Capture = false;

            UpdateTreeScrollbar();
        }

        private void treeResources_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node == null) return;

            bool isSelected = (e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected;
            var tag = e.Node.Tag as ResourceTag;

            Color backColor = isSelected ? _treeSelectedBackColor : _treeBackColor;
            Color textColor;

            if (isSelected)
            {
                textColor = _treeSelectedTextColor;
            }
            else if (e.Node.Level == 0)
            {
                textColor = _treeAccentColor;
            }
            else if (e.Node.Nodes.Count > 0)
            {
                textColor = _treeGroupTextColor;
            }
            else
            {
                textColor = _treeMutedTextColor;
            }

            int rightPadding = TreeScrollbarWidth + TreeScrollbarMargin + 10;

            Rectangle backgroundBounds = new Rectangle(
                e.Bounds.Left - 4,
                e.Bounds.Top,
                Math.Max(1, treeResources.ClientSize.Width - e.Bounds.Left - rightPadding + 4),
                e.Bounds.Height
            );

            using (SolidBrush backgroundBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backgroundBrush, backgroundBounds);
            }

            bool isVmOrLxc = tag != null && (tag.Type == "vm" || tag.Type == "lxc");

            int dotSize = 10;
            int dotSpacing = 7;
            int textLeft = e.Bounds.Left;

            if (isVmOrLxc)
            {
                bool isRunning = false;

                if (tag.Type == "vm" && tag.Data is PveVm vm)
                    isRunning = string.Equals(vm.Status, "running", StringComparison.OrdinalIgnoreCase);

                if (tag.Type == "lxc" && tag.Data is PveLxc lxc)
                    isRunning = string.Equals(lxc.Status, "running", StringComparison.OrdinalIgnoreCase);

                Color dotColor = isRunning ? _statusRunningColor : _statusStoppedColor;

                int dotX = e.Bounds.Left;
                int dotY = e.Bounds.Top + ((e.Bounds.Height - dotSize) / 2);

                using (SolidBrush dotBrush = new SolidBrush(dotColor))
                using (Pen dotBorderPen = new Pen(Color.FromArgb(15, 23, 42), 1))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillEllipse(dotBrush, dotX, dotY, dotSize, dotSize);
                    e.Graphics.DrawEllipse(dotBorderPen, dotX, dotY, dotSize, dotSize);
                    e.Graphics.SmoothingMode = SmoothingMode.None;
                }

                textLeft = e.Bounds.Left + dotSize + dotSpacing;
            }

            Rectangle textBounds = new Rectangle(
                textLeft,
                e.Bounds.Top,
                Math.Max(1, treeResources.ClientSize.Width - textLeft - rightPadding),
                e.Bounds.Height
            );

            TextRenderer.DrawText(
                e.Graphics,
                e.Node.Text,
                treeResources.Font,
                textBounds,
                textColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis
            );
        }

        private void SelectDatacenterNode()
        {
            if (treeResources.Nodes.Count > 0)
            {
                treeResources.SelectedNode = treeResources.Nodes[0];

                BeginInvoke(new Action(() =>
                {
                    PositionTreeScrollbar();
                    HideNativeTreeScrollbars();
                    UpdateTreeScrollbar();
                }));
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // RESOURCE CONTEXT MENU (RIGHT CLICK VM/LXC)
        // ─────────────────────────────────────────────────────────────────────

        private void InitializeResourceContextMenu()
        {
            if (_resourceContextMenu != null) return;

            _resourceContextMenu = new ContextMenuStrip
            {
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.FromArgb(226, 232, 240),
                ShowImageMargin = true,
                ShowCheckMargin = false,
                Renderer = new ModernContextMenuRenderer()
            };

            _contextStartItem = new ToolStripMenuItem("▶ Starten")
            {
                ForeColor = Color.FromArgb(226, 232, 240)
            };
            _contextStartItem.Click += (s, e) => PerformPowerAction("start");

            _contextStopItem = new ToolStripMenuItem("■ Stoppen")
            {
                ForeColor = Color.FromArgb(226, 232, 240)
            };
            _contextStopItem.Click += (s, e) => PerformPowerAction("stop");

            _resourceContextMenu.Items.Add(_contextStartItem);
            _resourceContextMenu.Items.Add(_contextStopItem);
        }

        private void treeResources_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.Node == null) return;

            var tag = e.Node.Tag as ResourceTag;
            if (tag == null || (tag.Type != "vm" && tag.Type != "lxc")) return;

            treeResources.SelectedNode = e.Node;
            UpdateUIForSelectedResource(tag);
            UpdateResourceContextMenuState(tag);

            _resourceContextMenu.Show(treeResources, e.Location);
        }

        private void UpdateResourceContextMenuState(ResourceTag tag)
        {
            if (_contextStartItem == null || _contextStopItem == null) return;

            bool isVmOrLxc = tag != null && (tag.Type == "vm" || tag.Type == "lxc");
            string status = GetResourceStatus(tag);
            bool isRunning = string.Equals(status, "running", StringComparison.OrdinalIgnoreCase);

            _contextStartItem.Enabled = isVmOrLxc && !isRunning;
            _contextStopItem.Enabled = isVmOrLxc && isRunning;

            if (tag != null)
            {
                string title = tag.Type == "vm" ? "VM" : "LXC";
                _contextStartItem.Text = $"▶ {title} starten";
                _contextStopItem.Text = $"■ {title} stoppen";
            }
        }

        private string GetResourceStatus(ResourceTag tag)
        {
            if (tag == null) return "";

            if (tag.Type == "vm" && tag.Data is PveVm vm)
                return vm.Status ?? "";

            if (tag.Type == "lxc" && tag.Data is PveLxc lxc)
                return lxc.Status ?? "";

            return "";
        }

        // ─────────────────────────────────────────────────────────────────────
        // DATAGRIDVIEW SCROLLBAR
        // ─────────────────────────────────────────────────────────────────────

        private void ApplyGridScrollbar()
        {
            // Disable ALL native scrollbars – we draw our own overlay instead
            gridTasks.ScrollBars = ScrollBars.None;
            gridTasks.Scroll += (s, e) => BeginInvoke(new Action(UpdateGridScrollbar));
            gridTasks.MouseWheel += (s, e) => ScrollGridBy(e.Delta < 0 ? 3 : -3);

            _gridScrollbarHider = new DataGridViewNativeScrollbarHider();
            _gridScrollbarHider.Attach(gridTasks);

            _gridScrollTrack = new ModernScrollbarPart
            {
                FillColor = Color.FromArgb(30, 41, 59),
                HoverColor = Color.FromArgb(30, 41, 59),
                Radius = 4,
                Cursor = Cursors.Hand,
                Visible = true
            };
            _gridScrollTrack.MouseDown += GridScrollbarTrack_MouseDown;
            _gridScrollTrack.MouseMove += GridScrollbar_MouseMove;
            _gridScrollTrack.MouseUp += GridScrollbar_MouseUp;
            _gridScrollTrack.MouseWheel += GridScrollbar_MouseWheel;

            _gridScrollThumb = new ModernScrollbarPart
            {
                FillColor = Color.FromArgb(249, 115, 22),
                HoverColor = Color.FromArgb(251, 146, 60),
                Radius = 4,
                Cursor = Cursors.Hand,
                Visible = true
            };
            _gridScrollThumb.MouseDown += GridScrollbarThumb_MouseDown;
            _gridScrollThumb.MouseMove += GridScrollbar_MouseMove;
            _gridScrollThumb.MouseUp += GridScrollbar_MouseUp;
            _gridScrollThumb.MouseWheel += GridScrollbar_MouseWheel;

            _gridScrollTrack.Controls.Add(_gridScrollThumb);
            gridTasks.Controls.Add(_gridScrollTrack);
            _gridScrollTrack.BringToFront();

            gridTasks.Resize += (s, e) => { PositionGridScrollbar(); UpdateGridScrollbar(); };

            PositionGridScrollbar();
            UpdateGridScrollbar();
        }

        private void PositionGridScrollbar()
        {
            if (_gridScrollTrack == null) return;

            int x = gridTasks.ClientSize.Width - GridScrollbarWidth - GridScrollbarMargin;
            int y = GridScrollbarMargin;
            int height = gridTasks.ClientSize.Height - (GridScrollbarMargin * 2);

            if (height < 30 || x < 0)
            {
                _gridScrollTrack.Visible = false;
                return;
            }

            _gridScrollTrack.Bounds = new Rectangle(x, y, GridScrollbarWidth, height);
            _gridScrollTrack.Visible = true;
            _gridScrollTrack.BringToFront();
        }

        private void UpdateGridScrollbar()
        {
            if (_gridScrollTrack == null || _gridScrollThumb == null) return;

            _gridScrollbarHider?.HideNow();

            int totalRows = gridTasks.Rows.Count;
            if (totalRows == 0)
            {
                _gridScrollTrack.Visible = false;
                return;
            }

            int visibleRows = GetGridVisibleRowCount();
            _gridScrollTrack.Visible = true;
            _gridScrollTrack.BringToFront();

            int trackHeight = Math.Max(1, _gridScrollTrack.Height);
            bool needsScrollbar = totalRows > visibleRows;

            if (!needsScrollbar)
            {
                _gridScrollThumb.FillColor = Color.FromArgb(71, 85, 105);
                _gridScrollThumb.HoverColor = Color.FromArgb(100, 116, 139);
                _gridScrollThumb.Bounds = new Rectangle(0, 0, GridScrollbarWidth, trackHeight);
                _gridScrollThumb.Visible = true;
                _gridScrollThumb.Invalidate();
                _gridScrollTrack.Invalidate();
                return;
            }

            _gridScrollThumb.FillColor = Color.FromArgb(249, 115, 22);
            _gridScrollThumb.HoverColor = Color.FromArgb(251, 146, 60);

            int thumbHeight = Math.Max(
                GridScrollbarMinThumbHeight,
                (int)Math.Round((double)visibleRows / totalRows * trackHeight)
            );
            thumbHeight = Math.Min(trackHeight, thumbHeight);

            int maxThumbTop = Math.Max(0, trackHeight - thumbHeight);
            int maxScroll = Math.Max(1, totalRows - visibleRows);

            int firstVisible = gridTasks.FirstDisplayedScrollingRowIndex >= 0
                ? gridTasks.FirstDisplayedScrollingRowIndex
                : 0;

            int thumbTop = maxScroll > 0
                ? (int)Math.Round((double)ClampInt(firstVisible, 0, maxScroll) / maxScroll * maxThumbTop)
                : 0;

            _gridScrollThumb.Bounds = new Rectangle(0, thumbTop, GridScrollbarWidth, thumbHeight);
            _gridScrollThumb.Visible = true;
            _gridScrollThumb.Invalidate();
            _gridScrollTrack.Invalidate();
        }

        private int GetGridVisibleRowCount()
        {
            int rowHeight = gridTasks.RowTemplate.Height > 0 ? gridTasks.RowTemplate.Height : 23;
            rowHeight = Math.Max(1, rowHeight);
            int usable = Math.Max(1, gridTasks.ClientSize.Height - gridTasks.ColumnHeadersHeight);
            return Math.Max(1, usable / rowHeight);
        }

        private void ScrollGridBy(int delta)
        {
            int totalRows = gridTasks.Rows.Count;
            if (totalRows == 0) return;

            int current = gridTasks.FirstDisplayedScrollingRowIndex >= 0
                ? gridTasks.FirstDisplayedScrollingRowIndex
                : 0;

            int visibleRows = GetGridVisibleRowCount();
            int newIndex = ClampInt(current + delta, 0, Math.Max(0, totalRows - visibleRows));

            gridTasks.FirstDisplayedScrollingRowIndex = newIndex;
            _gridScrollbarHider?.HideNow();
            UpdateGridScrollbar();
        }

        private void GridScrollbarTrack_MouseDown(object sender, MouseEventArgs e)
        {
            if (_gridScrollThumb == null) return;

            int visibleRows = GetGridVisibleRowCount();
            if (e.Y < _gridScrollThumb.Top) ScrollGridBy(-visibleRows);
            else if (e.Y > _gridScrollThumb.Bottom) ScrollGridBy(visibleRows);
        }

        private void GridScrollbarThumb_MouseDown(object sender, MouseEventArgs e)
        {
            _gridScrollDragging = true;
            _gridScrollDragOffsetY = e.Y;
            _gridScrollThumb.Capture = true;
            _gridScrollThumb.FillColor = Color.FromArgb(234, 88, 12);
            _gridScrollThumb.Invalidate();
        }

        private void GridScrollbar_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_gridScrollDragging || _gridScrollTrack == null || _gridScrollThumb == null) return;

            Point mouseInTrack = sender == _gridScrollThumb
                ? _gridScrollTrack.PointToClient(_gridScrollThumb.PointToScreen(e.Location))
                : e.Location;

            int newThumbTop = ClampInt(
                mouseInTrack.Y - _gridScrollDragOffsetY,
                0,
                Math.Max(0, _gridScrollTrack.Height - _gridScrollThumb.Height)
            );

            int maxThumbTop = Math.Max(1, _gridScrollTrack.Height - _gridScrollThumb.Height);
            int totalRows = gridTasks.Rows.Count;
            int visibleRows = GetGridVisibleRowCount();
            int maxScroll = Math.Max(1, totalRows - visibleRows);

            int targetRow = ClampInt(
                (int)Math.Round((double)newThumbTop / maxThumbTop * maxScroll),
                0,
                Math.Max(0, totalRows - 1)
            );

            if (totalRows > 0)
                gridTasks.FirstDisplayedScrollingRowIndex = targetRow;

            _gridScrollbarHider?.HideNow();
            UpdateGridScrollbar();
        }

        private void GridScrollbar_MouseUp(object sender, MouseEventArgs e)
        {
            _gridScrollDragging = false;
            if (_gridScrollThumb != null) _gridScrollThumb.Capture = false;
            UpdateGridScrollbar();
        }

        private void GridScrollbar_MouseWheel(object sender, MouseEventArgs e)
        {
            ScrollGridBy(e.Delta < 0 ? 3 : -3);
        }

        // ─────────────────────────────────────────────────────────────────────
        // DATA REFRESH
        // ─────────────────────────────────────────────────────────────────────

        public async Task RefreshDataAsync()
        {
            lblSelectedResource.Text = "Loading cluster information...";
            treeResources.BeginUpdate();

            treeResources.Nodes.Clear();

            var datacenterNode = new TreeNode("🌐 Datacenter")
            {
                Tag = new ResourceTag { Type = "datacenter", Name = "Datacenter" },
                ToolTipText = "Datacenter"
            };
            treeResources.Nodes.Add(datacenterNode);

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
                    Tag = new ResourceTag { Type = "node", NodeName = node.Node, Name = node.Node, Data = node },
                    ToolTipText = nodeDisplayName
                };
                datacenterNode.Nodes.Add(nodeTreeNode);

                if (isOnline)
                {
                    var vms = await _client.GetVmsAsync(node.Node);
                    var vmsGroupNode = new TreeNode("🖥️ Virtual Machines")
                    {
                        Tag = new ResourceTag { Type = "group_vm", NodeName = node.Node },
                        ToolTipText = "Virtual Machines"
                    };
                    nodeTreeNode.Nodes.Add(vmsGroupNode);

                    foreach (var vm in vms)
                    {
                        if (vm.IsTemplate) continue;

                        string vmDisplay = $"🖥️ [{vm.VmId}] {vm.Name}";
                        vmsGroupNode.Nodes.Add(new TreeNode(vmDisplay)
                        {
                            Tag = new ResourceTag { Type = "vm", NodeName = node.Node, VmId = vm.VmId, Name = vm.Name, Data = vm },
                            ToolTipText = vmDisplay
                        });
                    }

                    var lxcs = await _client.GetLxcsAsync(node.Node);
                    var lxcsGroupNode = new TreeNode("📦 Containers (LXC)")
                    {
                        Tag = new ResourceTag { Type = "group_lxc", NodeName = node.Node },
                        ToolTipText = "Containers (LXC)"
                    };
                    nodeTreeNode.Nodes.Add(lxcsGroupNode);

                    foreach (var lxc in lxcs)
                    {
                        string lxcDisplay = $"📦 [{lxc.VmId}] {lxc.Name}";
                        lxcsGroupNode.Nodes.Add(new TreeNode(lxcDisplay)
                        {
                            Tag = new ResourceTag { Type = "lxc", NodeName = node.Node, VmId = lxc.VmId, Name = lxc.Name, Data = lxc },
                            ToolTipText = lxcDisplay
                        });
                    }

                    var storages = await _client.GetStorageAsync(node.Node);
                    var storageGroupNode = new TreeNode("💾 Storage Pools")
                    {
                        Tag = new ResourceTag { Type = "group_storage", NodeName = node.Node },
                        ToolTipText = "Storage Pools"
                    };
                    nodeTreeNode.Nodes.Add(storageGroupNode);

                    foreach (var store in storages)
                    {
                        string storeStatusDot = store.Active ? "🟢" : "🔴";
                        string storeDisplay = $"{storeStatusDot} 💾 {store.Storage} ({store.Type})";
                        storageGroupNode.Nodes.Add(new TreeNode(storeDisplay)
                        {
                            Tag = new ResourceTag { Type = "storage", NodeName = node.Node, Name = store.Storage, Data = store },
                            ToolTipText = storeDisplay
                        });
                    }
                }
            }

            treeResources.ExpandAll();
            treeResources.EndUpdate();

            BeginInvoke(new Action(() =>
            {
                PositionTreeScrollbar();
                HideNativeTreeScrollbars();
                UpdateTreeScrollbar();
                treeResources.Invalidate();
            }));

            lblSelectedResource.Text = "Cluster ready.";

            await RefreshTasksLogAsync();

            if (treeResources.SelectedNode != null)
            {
                var tag = treeResources.SelectedNode.Tag as ResourceTag;
                UpdateUIForSelectedResource(tag);
            }

            BeginInvoke(new Action(() =>
            {
                PositionTreeScrollbar();
                HideNativeTreeScrollbars();
                UpdateTreeScrollbar();
                treeResources.Invalidate();
            }));
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

                if (task.Status == "OK")
                    row.Cells[4].Style.ForeColor = Color.FromArgb(34, 197, 94);
                else if (task.Status == "RUNNING")
                    row.Cells[4].Style.ForeColor = Color.FromArgb(59, 130, 246);
                else
                    row.Cells[4].Style.ForeColor = Color.FromArgb(239, 68, 68);
            }

            BeginInvoke(new Action(() =>
            {
                PositionGridScrollbar();
                _gridScrollbarHider?.HideNow();
                UpdateGridScrollbar();
            }));
        }

        // ─────────────────────────────────────────────────────────────────────
        // SELECTION & UI UPDATE
        // ─────────────────────────────────────────────────────────────────────

        private void treeResources_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var tag = e.Node.Tag as ResourceTag;
            if (tag == null) return;

            _lastSelectedKey = $"{tag.Type}_{tag.NodeName}_{tag.VmId}";
            UpdateUIForSelectedResource(tag);

            BeginInvoke(new Action(() =>
            {
                PositionTreeScrollbar();
                HideNativeTreeScrollbars();
                UpdateTreeScrollbar();
                treeResources.Invalidate();
            }));
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

            if (panelConsole.Visible)
            {
                LoadConsoleForSelectedResource();
            }
        }

        private async void FetchVmIpAddressAsync(string node, int vmid)
        {
            string ip = await _client.GetVmIpAsync(node, vmid);

            if (treeResources.SelectedNode != null)
            {
                var tag = treeResources.SelectedNode.Tag as ResourceTag;
                if (tag != null && tag.Type == "vm" && tag.VmId == vmid)
                    lblDetailIp.Text = "IP Address: " + ip;
            }
        }

        private async void FetchLxcIpAddressAsync(string node, int vmid)
        {
            string ip = await _client.GetLxcIpAsync(node, vmid);

            if (treeResources.SelectedNode != null)
            {
                var tag = treeResources.SelectedNode.Tag as ResourceTag;
                if (tag != null && tag.Type == "lxc" && tag.VmId == vmid)
                    lblDetailIp.Text = "IP Address: " + ip;
            }
        }

        private void UpdateActionButtonsState(ResourceTag tag)
        {
            if (tag == null || tag.Type == "datacenter" || tag.Type == "group_vm" ||
                tag.Type == "group_lxc" || tag.Type == "group_storage" || tag.Type == "storage")
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

        // ─────────────────────────────────────────────────────────────────────
        // TAB SWITCHING & CONSOLE
        // ─────────────────────────────────────────────────────────────────────

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

            BeginInvoke(new Action(() =>
            {
                PositionTreeScrollbar();
                HideNativeTreeScrollbars();
                UpdateTreeScrollbar();
                treeResources.Invalidate();
            }));
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
            if (tag == null || tag.Type == "datacenter" || tag.Type == "group_vm" ||
                tag.Type == "group_lxc" || tag.Type == "group_storage" || tag.Type == "storage")
            {
                webViewConsole.Visible = false;
                lblConsoleWarning.Visible = true;
                lblConsoleWarning.Text = "Select a VM, Container or Node to view its interactive Console/Shell.";
                return;
            }

            string consoleUrl = "";
            if (tag.Type == "node")
                consoleUrl = $"https://{_client.Host}:{_client.Port}/?console=shell&novnc=1&node={tag.NodeName}";
            else if (tag.Type == "vm")
                consoleUrl = $"https://{_client.Host}:{_client.Port}/?console=kvm&novnc=1&vmid={tag.VmId}&node={tag.NodeName}";
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

        // ─────────────────────────────────────────────────────────────────────
        // BUTTON EVENTS
        // ─────────────────────────────────────────────────────────────────────

        private void btnTabDashboard_Click(object sender, EventArgs e) => SwitchToTab("dashboard");
        private void btnTabConsole_Click(object sender, EventArgs e) => SwitchToTab("console");
        private void btnTabConfig_Click(object sender, EventArgs e) => SwitchToTab("config");

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await RefreshDataAsync();
        }

        private async void btnCheckUpdate_Click(object sender, EventArgs e)
        {
            btnCheckUpdate.Enabled = false;
            btnCheckUpdate.Text = "Checking...";
            await Updater.CheckForUpdatesAsync(this, silent: false);
            btnCheckUpdate.Enabled = true;
            btnCheckUpdate.Text = "Check Update";
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

            var confirm = MessageBox.Show(
                $"Are you sure you want to permanently delete [{tag.VmId}] {tag.Name}?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

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

        private void btnCreateVm_Click(object sender, EventArgs e) => OpenCreateDialog("vm");
        private void btnCreateLxc_Click(object sender, EventArgs e) => OpenCreateDialog("lxc");

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
                            allResourceIds.Add(tag.VmId);
                    }
                }
            }

            if (allResourceIds.Count > 0)
                nextId = allResourceIds.Max() + 1;

            var dialog = new CreateResourceDialog(this, _client, type, onlineNodes, nextId);
            dialog.ShowDialog();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TIMER REFRESH
        // ─────────────────────────────────────────────────────────────────────

        private async void timerRefresh_Tick(object sender, EventArgs e)
        {
            try
            {
                await RefreshTasksLogAsync();

                var apiNodes = await _client.GetNodesAsync();
                _cachedNodes = apiNodes;

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
                            treeResources.Invalidate();
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
                            treeResources.Invalidate();
                        }
                    }
                }

                BeginInvoke(new Action(() =>
                {
                    PositionTreeScrollbar();
                    HideNativeTreeScrollbars();
                    UpdateTreeScrollbar();
                    treeResources.Invalidate();
                }));
            }
            catch
            {
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // HELPERS
        // ─────────────────────────────────────────────────────────────────────

        private static int ClampInt(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;

            while (Math.Round(number / 1024) >= 1 && counter < suffixes.Length - 1)
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

    // ─────────────────────────────────────────────────────────────────────────
    // SUPPORT CLASSES
    // ─────────────────────────────────────────────────────────────────────────

    public class ResourceTag
    {
        public string Type { get; set; }
        public string NodeName { get; set; }
        public int VmId { get; set; }
        public string Name { get; set; }
        public object Data { get; set; }
    }

    public class ModernScrollbarPart : Panel
    {
        private bool _hover;

        public Color FillColor { get; set; } = Color.FromArgb(249, 115, 22);
        public Color HoverColor { get; set; } = Color.FromArgb(251, 146, 60);
        public int Radius { get; set; } = 4;

        public ModernScrollbarPart()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true
            );

            BackColor = Color.Transparent;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hover = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hover = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            if (rect.Width <= 0 || rect.Height <= 0) return;

            using (GraphicsPath path = CreateRoundedRectanglePath(rect, Radius))
            using (SolidBrush brush = new SolidBrush(_hover ? HoverColor : FillColor))
            {
                e.Graphics.FillPath(brush, path);
            }
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            int diameter = Math.Max(1, radius * 2);
            GraphicsPath path = new GraphicsPath();

            path.AddArc(rect.Left, rect.Top, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Top, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }

    public class ModernContextMenuRenderer : ToolStripProfessionalRenderer
    {
        public ModernContextMenuRenderer() : base(new ModernContextMenuColors())
        {
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);

            if (e.Item.Selected && e.Item.Enabled)
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(249, 115, 22)))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(15, 23, 42)))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            }
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(15, 23, 42)))
            {
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
            using (Pen pen = new Pen(Color.FromArgb(51, 65, 85)))
            {
                e.Graphics.DrawRectangle(pen, rect);
            }
        }
    }

    public class ModernContextMenuColors : ProfessionalColorTable
    {
        public override Color ToolStripDropDownBackground => Color.FromArgb(15, 23, 42);
        public override Color ImageMarginGradientBegin => Color.FromArgb(15, 23, 42);
        public override Color ImageMarginGradientMiddle => Color.FromArgb(15, 23, 42);
        public override Color ImageMarginGradientEnd => Color.FromArgb(15, 23, 42);
        public override Color MenuItemSelected => Color.FromArgb(249, 115, 22);
        public override Color MenuItemBorder => Color.FromArgb(249, 115, 22);
        public override Color MenuBorder => Color.FromArgb(51, 65, 85);
        public override Color SeparatorDark => Color.FromArgb(51, 65, 85);
        public override Color SeparatorLight => Color.FromArgb(51, 65, 85);
    }

    public class TreeViewNativeScrollbarHider : NativeWindow
    {
        private TreeView _treeView;

        private const int SB_BOTH = 3;
        private const int WM_PAINT = 0x000F;
        private const int WM_SIZE = 0x0005;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_VSCROLL = 0x0115;
        private const int WM_HSCROLL = 0x0114;
        private const int WM_MOUSEWHEEL = 0x020A;

        [DllImport("user32.dll")]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        public void Attach(TreeView treeView)
        {
            _treeView = treeView;
            if (_treeView == null) return;

            _treeView.HandleCreated += TreeView_HandleCreated;
            _treeView.HandleDestroyed += TreeView_HandleDestroyed;

            if (_treeView.IsHandleCreated)
            {
                AssignHandle(_treeView.Handle);
                HideNow();
            }
        }

        public void HideNow()
        {
            if (_treeView == null || _treeView.IsDisposed || !_treeView.IsHandleCreated) return;
            ShowScrollBar(_treeView.Handle, SB_BOTH, false);
        }

        private void TreeView_HandleCreated(object sender, EventArgs e)
        {
            if (_treeView == null) return;
            AssignHandle(_treeView.Handle);
            HideNow();
        }

        private void TreeView_HandleDestroyed(object sender, EventArgs e)
        {
            ReleaseHandle();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT || m.Msg == WM_SIZE || m.Msg == WM_NCPAINT ||
                m.Msg == WM_VSCROLL || m.Msg == WM_HSCROLL || m.Msg == WM_MOUSEWHEEL)
            {
                HideNow();
            }
        }
    }

    public class DataGridViewNativeScrollbarHider : NativeWindow
    {
        private DataGridView _grid;

        private const int SB_VERT = 1;
        private const int SB_BOTH = 3;
        private const int WM_PAINT = 0x000F;
        private const int WM_SIZE = 0x0005;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_NCCALCSIZE = 0x0083;
        private const int WM_VSCROLL = 0x0115;
        private const int WM_HSCROLL = 0x0114;
        private const int WM_MOUSEWHEEL = 0x020A;

        // Style bits
        private const int GWL_STYLE = -16;
        private const int WS_VSCROLL = 0x00200000;
        private const int WS_HSCROLL = 0x00100000;

        [DllImport("user32.dll")]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public void Attach(DataGridView grid)
        {
            _grid = grid;
            if (_grid == null) return;

            _grid.HandleCreated += (s, e) => { AssignHandle(_grid.Handle); HideNow(); };
            _grid.HandleDestroyed += (s, e) => ReleaseHandle();

            if (_grid.IsHandleCreated)
            {
                AssignHandle(_grid.Handle);
                HideNow();
            }
        }

        public void HideNow()
        {
            if (_grid == null || _grid.IsDisposed || !_grid.IsHandleCreated) return;

            // Remove scroll style bits so Windows stops reserving NC space for them
            int style = GetWindowLong(_grid.Handle, GWL_STYLE);
            int newStyle = style & ~WS_VSCROLL & ~WS_HSCROLL;
            if (style != newStyle)
                SetWindowLong(_grid.Handle, GWL_STYLE, newStyle);

            ShowScrollBar(_grid.Handle, SB_BOTH, false);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT || m.Msg == WM_SIZE ||
                m.Msg == WM_NCPAINT || m.Msg == WM_NCCALCSIZE ||
                m.Msg == WM_VSCROLL || m.Msg == WM_HSCROLL ||
                m.Msg == WM_MOUSEWHEEL)
            {
                HideNow();
            }
        }
    }
}