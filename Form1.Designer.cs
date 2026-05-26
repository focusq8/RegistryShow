namespace RegistryShow
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.pnlHeader   = new System.Windows.Forms.Panel();
            this.lblTitle    = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();

            this.pnlToolbar     = new System.Windows.Forms.Panel();
            this.btnRefresh     = new System.Windows.Forms.Button();
            this.btnDelete      = new System.Windows.Forms.Button();
            this.btnAutoRefresh = new System.Windows.Forms.Button();
            this.lblSearch      = new System.Windows.Forms.Label();
            this.txtSearch      = new System.Windows.Forms.TextBox();

            this.listView1   = new System.Windows.Forms.ListView();

            this.statusStrip1    = new System.Windows.Forms.StatusStrip();
            this.tsslItems       = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSep1        = new System.Windows.Forms.ToolStripSeparator();
            this.tsslLastScan    = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslSpring      = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslAutoRefresh = new System.Windows.Forms.ToolStripStatusLabel();

            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip1.AutoPopDelay = 1500;
            this.toolTip1.InitialDelay = 0;

            this.contextMenuStrip1           = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuSeparator             = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem     = new System.Windows.Forms.ToolStripMenuItem();

            // ── Header ──────────────────────────────────────────────────────
            this.pnlHeader.Dock    = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height  = 65;
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Controls.Add(this.lblTitle);

            this.lblTitle.Location = new System.Drawing.Point(16, 10);
            this.lblTitle.Size     = new System.Drawing.Size(700, 28);
            this.lblTitle.Text     = "Registry Startup Manager";
            this.lblTitle.Font     = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.AutoSize = false;

            this.lblSubtitle.Location = new System.Drawing.Point(18, 40);
            this.lblSubtitle.Size     = new System.Drawing.Size(600, 18);
            this.lblSubtitle.Text     = "Monitor and manage Windows startup entries";
            this.lblSubtitle.Font     = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSubtitle.AutoSize = false;

            // ── Toolbar ──────────────────────────────────────────────────────
            this.pnlToolbar.Dock   = System.Windows.Forms.DockStyle.Top;
            this.pnlToolbar.Height = 50;
            this.pnlToolbar.Controls.Add(this.lblSearch);
            this.pnlToolbar.Controls.Add(this.txtSearch);
            this.pnlToolbar.Controls.Add(this.btnAutoRefresh);
            this.pnlToolbar.Controls.Add(this.btnDelete);
            this.pnlToolbar.Controls.Add(this.btnRefresh);

            this.btnRefresh.Location  = new System.Drawing.Point(12, 10);
            this.btnRefresh.Size      = new System.Drawing.Size(100, 30);
            this.btnRefresh.Text      = "⟳  Refresh";
            this.btnRefresh.Font      = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.Click    += new System.EventHandler(this.btnRefresh_Click);

            this.btnDelete.Location  = new System.Drawing.Point(122, 10);
            this.btnDelete.Size      = new System.Drawing.Size(100, 30);
            this.btnDelete.Text      = "✕  Delete";
            this.btnDelete.Font      = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnDelete.Click    += new System.EventHandler(this.btnDelete_Click);

            this.btnAutoRefresh.Location  = new System.Drawing.Point(232, 10);
            this.btnAutoRefresh.Size      = new System.Drawing.Size(120, 30);
            this.btnAutoRefresh.Text      = "⏸  Auto: ON";
            this.btnAutoRefresh.Font      = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnAutoRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAutoRefresh.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnAutoRefresh.Click    += new System.EventHandler(this.btnAutoRefresh_Click);

            this.lblSearch.Location = new System.Drawing.Point(370, 16);
            this.lblSearch.Text     = "🔍";
            this.lblSearch.Font     = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSearch.AutoSize = true;

            this.txtSearch.Location     = new System.Drawing.Point(395, 11);
            this.txtSearch.Size         = new System.Drawing.Size(260, 28);
            this.txtSearch.Font         = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSearch.BorderStyle  = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);

            // ── ListView ─────────────────────────────────────────────────────
            this.listView1.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.listView1.View      = System.Windows.Forms.View.Details;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.OwnerDraw = true;
            this.listView1.Font      = new System.Drawing.Font("Segoe UI", 9.5F);
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.DrawItem        += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView1_DrawItem);
            this.listView1.DrawSubItem     += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView1_DrawSubItem);
            this.listView1.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView1_DrawColumnHeader);
            this.listView1.ColumnClick     += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);

            // ── StatusStrip ───────────────────────────────────────────────────
            this.tsslItems.Text    = "Items: 0";
            this.tsslItems.Font    = new System.Drawing.Font("Segoe UI", 9F);
            this.tsslLastScan.Text = "Last scan: —";
            this.tsslLastScan.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tsslSpring.Spring = true;
            this.tsslAutoRefresh.Text      = "● Auto-refresh: ON  ";
            this.tsslAutoRefresh.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tsslAutoRefresh.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.tsslItems, this.tsslSep1, this.tsslLastScan, this.tsslSpring, this.tsslAutoRefresh });
            this.statusStrip1.SizingGrip = false;

            // ── Context Menu ──────────────────────────────────────────────────
            this.copyPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRegeditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuSeparator2 = new System.Windows.Forms.ToolStripSeparator();

            this.copyPathToolStripMenuItem.Text   = "📋  Copy File Path";
            this.copyPathToolStripMenuItem.Font   = new System.Drawing.Font("Segoe UI", 9.5F);
            this.copyPathToolStripMenuItem.Click += new System.EventHandler(this.copyPath_Click);

            this.openRegeditToolStripMenuItem.Text   = "🔑  Open in Regedit";
            this.openRegeditToolStripMenuItem.Font   = new System.Drawing.Font("Segoe UI", 9.5F);
            this.openRegeditToolStripMenuItem.Click += new System.EventHandler(this.openRegedit_Click);

            this.openLocationToolStripMenuItem.Text   = "📂  Open File Location";
            this.openLocationToolStripMenuItem.Font   = new System.Drawing.Font("Segoe UI", 9.5F);
            this.openLocationToolStripMenuItem.Click += new System.EventHandler(this.openLocation_Click);

            this.deleteToolStripMenuItem.Text         = "✕  Delete Entry";
            this.deleteToolStripMenuItem.Font         = new System.Drawing.Font("Segoe UI", 9.5F);
            this.deleteToolStripMenuItem.Click       += new System.EventHandler(this.btnDelete_Click);

            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.copyPathToolStripMenuItem,
                this.openRegeditToolStripMenuItem,
                this.openLocationToolStripMenuItem,
                this.tsMenuSeparator,
                this.deleteToolStripMenuItem });
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;

            // ── Form ──────────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(1050, 600);
            this.MinimumSize         = new System.Drawing.Size(800, 500);
            this.Text                = "Registry Startup Manager";
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.Sizable;
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;

            this.Controls.Add(this.listView1);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.statusStrip1);

            this.Load        += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
        }

        #endregion

        private System.Windows.Forms.Panel   pnlHeader;
        private System.Windows.Forms.Label   lblTitle;
        private System.Windows.Forms.Label   lblSubtitle;
        private System.Windows.Forms.Panel   pnlToolbar;
        private System.Windows.Forms.Button  btnRefresh;
        private System.Windows.Forms.Button  btnDelete;
        private System.Windows.Forms.Button  btnAutoRefresh;
        private System.Windows.Forms.Label   lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ListView  listView1;
        private System.Windows.Forms.ToolTip   toolTip1;
        private System.Windows.Forms.StatusStrip        statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsslItems;
        private System.Windows.Forms.ToolStripSeparator   tsslSep1;
        private System.Windows.Forms.ToolStripStatusLabel tsslLastScan;
        private System.Windows.Forms.ToolStripStatusLabel tsslSpring;
        private System.Windows.Forms.ToolStripStatusLabel tsslAutoRefresh;
        private System.Windows.Forms.ContextMenuStrip    contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem   copyPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem   openRegeditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem   openLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator  tsMenuSeparator;
        private System.Windows.Forms.ToolStripSeparator  tsMenuSeparator2;
        private System.Windows.Forms.ToolStripMenuItem   deleteToolStripMenuItem;
    }
}
