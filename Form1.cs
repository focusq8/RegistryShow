using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegistryShow
{
    public partial class Form1 : Form
    {
        // ── Theme colors ────────────────────────────────────────────────────
        private static readonly Color BgDark      = Color.FromArgb(18,  18,  30);
        private static readonly Color BgPanel     = Color.FromArgb(28,  28,  45);
        private static readonly Color BgRow       = Color.FromArgb(24,  24,  38);
        private static readonly Color BgRowAlt    = Color.FromArgb(30,  30,  48);
        private static readonly Color BgRowSel    = Color.FromArgb(50,  90, 160);
        private static readonly Color BgRowWarn   = Color.FromArgb(80,  20,  20);
        private static readonly Color BgHeader    = Color.FromArgb(35,  35,  55);
        private static readonly Color TxtPrimary  = Color.FromArgb(205, 214, 244);
        private static readonly Color TxtSecond   = Color.FromArgb(166, 173, 200);
        private static readonly Color AccentBlue  = Color.FromArgb(137, 180, 250);
        private static readonly Color AccentRed   = Color.FromArgb(243, 139, 168);
        private static readonly Color AccentGreen = Color.FromArgb(166, 227, 161);
        private static readonly Color AccentOrange= Color.FromArgb(250, 179, 135);
        private static readonly Color BorderColor = Color.FromArgb(50,  50,  80);

        // ── Registry paths ──────────────────────────────────────────────────
        private static readonly string[] RegistryPaths =
        {
            @"Software\Microsoft\Windows\CurrentVersion\Run",
            @"Software\Microsoft\Windows\CurrentVersion\RunOnce",
            @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Run",
            @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\RunOnce"
        };
        private static readonly string[] UserRegistryPaths =
        {
            @"Software\Microsoft\Windows\CurrentVersion\Run",
            @"Software\Microsoft\Windows\CurrentVersion\RunOnce"
        };
        private static readonly string[] StartupFolders =
        {
            Environment.GetFolderPath(Environment.SpecialFolder.Startup),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup)
        };

        // ── Winlogon expected values ─────────────────────────────────────────
        private static readonly Dictionary<string, string> WinlogonExpected =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Shell",    "explorer.exe" },
            { "Userinit", @"C:\WINDOWS\system32\userinit.exe," }
        };
        private const string WinlogonPath = @"Software\Microsoft\Windows NT\CurrentVersion\Winlogon";

        // ── State ────────────────────────────────────────────────────────────
        private readonly List<RegistryKey> registryKeys = new List<RegistryKey>();
        private List<Tuple<string, string, string, string, bool>> allItems =
            new List<Tuple<string, string, string, string, bool>>();
        private CancellationTokenSource tokenSource;
        private ImageList imageList          = new ImageList();
        private int       sortColumn         = -1;
        private SortOrder sortOrder          = SortOrder.None;
        private bool      _autoRefreshEnabled = true;

        // ─────────────────────────────────────────────────────────────────────
        public Form1()
        {
            InitializeComponent();
            ApplyTheme();
            LoadAppIcon();
            InitializeRegistryKeys();
            listView1.SmallImageList = imageList;
            imageList.ImageSize      = new Size(20, 20);
        }

        private void LoadAppIcon()
        {
            try
            {
                string p = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
                if (File.Exists(p)) this.Icon = new Icon(p);
            }
            catch { }
        }

        // ── Theme ─────────────────────────────────────────────────────────────
        private void ApplyTheme()
        {
            this.BackColor = BgDark;

            pnlHeader.BackColor   = BgPanel;
            lblTitle.ForeColor    = AccentBlue;
            lblSubtitle.ForeColor = TxtSecond;
            pnlHeader.Paint += (s, e) =>
            {
                using (var pen = new Pen(BorderColor))
                    e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
            };

            pnlToolbar.BackColor = BgPanel;
            pnlToolbar.Paint += (s, e) =>
            {
                using (var pen = new Pen(BorderColor))
                    e.Graphics.DrawLine(pen, 0, pnlToolbar.Height - 1, pnlToolbar.Width, pnlToolbar.Height - 1);
            };

            StyleButton(btnRefresh,     AccentBlue,  Color.FromArgb(160, 200, 255));
            StyleButton(btnDelete,      AccentRed,   Color.FromArgb(255, 160, 180));
            StyleButton(btnAutoRefresh, AccentGreen, Color.FromArgb(180, 240, 170));
            btnAutoRefresh.ForeColor = BgDark;

            lblSearch.ForeColor  = TxtSecond;
            txtSearch.BackColor  = Color.FromArgb(35, 35, 55);
            txtSearch.ForeColor  = TxtSecond;
            txtSearch.Text       = "Search...";
            txtSearch.GotFocus  += (s, e) => { if (txtSearch.Text == "Search...") { txtSearch.Text = ""; txtSearch.ForeColor = TxtPrimary; } };
            txtSearch.LostFocus += (s, e) => { if (string.IsNullOrEmpty(txtSearch.Text)) { txtSearch.Text = "Search..."; txtSearch.ForeColor = TxtSecond; } };

            listView1.BackColor   = BgRow;
            listView1.ForeColor   = TxtPrimary;
            listView1.BorderStyle = BorderStyle.None;

            statusStrip1.BackColor    = BgPanel;
            tsslItems.ForeColor       = TxtSecond;
            tsslLastScan.ForeColor    = TxtSecond;
            tsslAutoRefresh.ForeColor = AccentGreen;
        }

        private void StyleButton(Button btn, Color back, Color hover)
        {
            btn.BackColor = back;
            btn.ForeColor = BgDark;
            btn.FlatAppearance.BorderSize         = 0;
            btn.FlatAppearance.MouseOverBackColor = hover;
        }

        // ── Registry init ────────────────────────────────────────────────────
        private void InitializeRegistryKeys()
        {
            foreach (var path in RegistryPaths)
                registryKeys.Add(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(path, true));
            foreach (var path in UserRegistryPaths)
                registryKeys.Add(RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(path, true));
        }

        // ── Data fetching ─────────────────────────────────────────────────────
        private List<Tuple<string, string, string, string, bool>> GetStartupItems()
        {
            var items = new List<Tuple<string, string, string, string, bool>>();

            Parallel.ForEach(registryKeys, key =>
            {
                if (key == null) return;
                lock (items)
                    foreach (var valueName in key.GetValueNames())
                    {
                        var valueData = key.GetValue(valueName, "")?.ToString() ?? "";
                        string fp     = ExtractFilePath(valueData);
                        items.Add(Tuple.Create(valueName, key.Name, valueData, fp, false));
                    }
            });

            foreach (var folder in StartupFolders)
            {
                if (!Directory.Exists(folder)) continue;
                foreach (var fp in Directory.GetFiles(folder))
                {
                    var fn = Path.GetFileName(fp);
                    if (!fn.Equals("desktop.ini", StringComparison.OrdinalIgnoreCase))
                        items.Add(Tuple.Create(fn, "Startup Folder", fp, fp, false));
                }
            }

            CheckWinlogon(items);
            return items;
        }

        private void CheckWinlogon(List<Tuple<string, string, string, string, bool>> items)
        {
            try
            {
                var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                                     .OpenSubKey(WinlogonPath);
                if (key == null) return;
                foreach (var entry in WinlogonExpected)
                {
                    var actual = key.GetValue(entry.Key, "")?.ToString() ?? "";
                    if (!string.Equals(actual.Trim(), entry.Value.Trim(), StringComparison.OrdinalIgnoreCase))
                        items.Add(Tuple.Create(entry.Key,
                            @"HKEY_LOCAL_MACHINE\" + WinlogonPath,
                            actual, ExtractFilePath(actual), true));
                }
            }
            catch { }
        }

        private string ExtractFilePath(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            value = value.Trim();

            // Quoted path: "C:\path\file.exe" args
            if (value.StartsWith("\""))
            {
                int end = value.IndexOf("\"", 1);
                return end > 1 ? value.Substring(1, end - 1) : value;
            }

            // Unquoted: find the executable extension and cut there
            foreach (var ext in new[] { ".exe", ".bat", ".cmd", ".com", ".dll" })
            {
                int idx = value.IndexOf(ext, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                    return value.Substring(0, idx + ext.Length);
            }

            return value.Split(' ')[0];
        }

        private Icon ExtractIconFromPath(string filePath)
        {
            try { if (File.Exists(filePath)) return Icon.ExtractAssociatedIcon(filePath); }
            catch { }
            return SystemIcons.Application;
        }

        // ── UI update ─────────────────────────────────────────────────────────
        private async Task RefreshStartupItemsAsync()
        {
            try
            {
                var fetched = await Task.Run(() => GetStartupItems());
                BeginInvoke((Action)(() =>
                {
                    allItems = fetched;
                    ApplyFilter();
                    tsslLastScan.Text = "Last scan: " + DateTime.Now.ToString("HH:mm:ss");
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilter()
        {
            string filter    = txtSearch.Text.Trim();
            bool   hasFilter = filter.Length > 0 && filter != "Search...";

            var filtered = hasFilter
                ? allItems.Where(i =>
                    i.Item1.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    i.Item2.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    i.Item3.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                  .ToList()
                : allItems;

            listView1.BeginUpdate();
            listView1.Items.Clear();
            imageList.Images.Clear();

            foreach (var item in filtered)
            {
                var icon = ExtractIconFromPath(item.Item4);
                if (!imageList.Images.ContainsKey(item.Item1))
                    imageList.Images.Add(item.Item1, icon);

                var lvi = new ListViewItem(item.Item1, imageList.Images.IndexOfKey(item.Item1))
                {
                    SubItems = { item.Item2, item.Item4 },
                    Tag      = item.Item5
                };
                listView1.Items.Add(lvi);
            }

            listView1.EndUpdate();
            FitLastColumn();
            tsslItems.Text = $"Items: {listView1.Items.Count}" + (hasFilter ? $"  (filtered from {allItems.Count})" : "");
        }

        // ── Auto-scan loop ────────────────────────────────────────────────────
        private async Task ScannerTaskAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (_autoRefreshEnabled) await RefreshStartupItemsAsync();
                await Task.Delay(10000, ct).ContinueWith(_ => { });
            }
        }

        // ── ListView owner-draw ───────────────────────────────────────────────
        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (var bg = new SolidBrush(BgHeader))
                e.Graphics.FillRectangle(bg, e.Bounds);
            using (var pen = new Pen(BorderColor))
                e.Graphics.DrawRectangle(pen, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
            using (var sf  = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
            using (var br  = new SolidBrush(AccentBlue))
                e.Graphics.DrawString(e.Header.Text,
                    new Font("Segoe UI", 9.5f, FontStyle.Bold), br,
                    new RectangleF(e.Bounds.X + 6, e.Bounds.Y, e.Bounds.Width - 6, e.Bounds.Height), sf);
        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e) => e.DrawDefault = false;

        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            bool isModified = e.Item.Tag is bool b && b;
            bool selected   = e.Item.Selected;
            bool isAlt      = e.Item.Index % 2 == 1;

            Color bg, fg;
            if (selected)        { bg = BgRowSel;  fg = Color.White; }
            else if (isModified) { bg = BgRowWarn; fg = AccentOrange; }
            else                 { bg = isAlt ? BgRowAlt : BgRow; fg = TxtPrimary; }

            using (var bgBr = new SolidBrush(bg))
                e.Graphics.FillRectangle(bgBr, e.Bounds);
            using (var pen = new Pen(BorderColor))
                e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);

            var rect = new RectangleF(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height);
            using (var sf  = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter })
            using (var fgBr = new SolidBrush(fg))
            {
                if (e.ColumnIndex == 0 && listView1.SmallImageList != null)
                {
                    int idx = e.Item.ImageIndex;
                    if (idx >= 0 && idx < listView1.SmallImageList.Images.Count)
                    {
                        var img = listView1.SmallImageList.Images[idx];
                        e.Graphics.DrawImage(img, e.Bounds.X + 4, e.Bounds.Y + (e.Bounds.Height - 16) / 2, 16, 16);
                    }
                    rect = new RectangleF(e.Bounds.X + 24, e.Bounds.Y, e.Bounds.Width - 28, e.Bounds.Height);
                    if (isModified)
                        using (var wf = new Font("Segoe UI", 9f, FontStyle.Bold))
                            e.Graphics.DrawString("⚠", wf, fgBr,
                                new RectangleF(e.Bounds.Right - 22, e.Bounds.Y, 20, e.Bounds.Height),
                                new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
                }
                e.Graphics.DrawString(e.SubItem.Text, listView1.Font, fgBr, rect, sf);
            }
        }

        // ── Sorting ───────────────────────────────────────────────────────────
        private void copyPath_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            string path = listView1.SelectedItems[0].SubItems[2].Text;
            if (!string.IsNullOrEmpty(path))
                Clipboard.SetText(path);
        }

        private void openRegedit_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            OpenInRegedit(listView1.SelectedItems[0].SubItems[1].Text);
        }

        private void OpenInRegedit(string fullPath)
        {
            try
            {
                // Regedit expects: Computer\HKEY_LOCAL_MACHINE\...
                string path = fullPath;
                if (!path.StartsWith("Computer\\", StringComparison.OrdinalIgnoreCase))
                    path = "Computer\\" + path;

                // Replace short hive names with full names if needed
                path = path.Replace("HKEY_LOCAL_MACHINE", "HKEY_LOCAL_MACHINE")
                           .Replace("HKEY_CURRENT_USER",  "HKEY_CURRENT_USER");

                Microsoft.Win32.Registry.SetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit",
                    "LastKey", path);

                System.Diagnostics.Process.Start("regedit.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot open regedit: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == sortColumn)
                sortOrder = sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            else { sortColumn = e.Column; sortOrder = SortOrder.Ascending; }
            listView1.ListViewItemSorter = new ListViewItemComparer(sortColumn, sortOrder);
        }

        // ── Ghost-column fix ──────────────────────────────────────────────────
        [DllImport("user32.dll")] private static extern IntPtr SendMessage(IntPtr h, int msg, IntPtr w, IntPtr l);
        [DllImport("user32.dll")] private static extern bool   GetClientRect(IntPtr h, out RECT r);
        [StructLayout(LayoutKind.Sequential)] private struct RECT { public int L, T, R, B; }
        private const int LVM_GETHEADER = 0x101F;

        private class HeaderPainter : NativeWindow
        {
            private const int WM_ERASEBKGND = 0x0014;
            private readonly Color _bg;
            public HeaderPainter(IntPtr h, Color bg) { _bg = bg; AssignHandle(h); }
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_ERASEBKGND)
                {
                    RECT r; GetClientRect(Handle, out r);
                    using (var g = Graphics.FromHdc(m.WParam))
                    using (var b = new SolidBrush(_bg))
                        g.FillRectangle(b, r.L, r.T, r.R - r.L, r.B - r.T);
                    m.Result = (IntPtr)1; return;
                }
                base.WndProc(ref m);
            }
        }

        // ── FitLastColumn ─────────────────────────────────────────────────────
        private void FitLastColumn()
        {
            if (listView1.Columns.Count == 0) return;
            int used = 0;
            for (int i = 0; i < listView1.Columns.Count - 1; i++)
                used += listView1.Columns[i].Width;
            int rem = listView1.ClientSize.Width - used;
            if (rem > 50) listView1.Columns[listView1.Columns.Count - 1].Width = rem;
        }

        // ── Event handlers ────────────────────────────────────────────────────
        private async void Form1_Load(object sender, EventArgs e)
        {
            listView1.Columns.Add("Name",          180);
            listView1.Columns.Add("Registry Path", 380);
            listView1.Columns.Add("File Path",     380);

            listView1.Resize += (s, ev) => FitLastColumn();
            this.Shown += (s, ev) =>
            {
                FitLastColumn();
                var hdr = SendMessage(listView1.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
                if (hdr != IntPtr.Zero) new HeaderPainter(hdr, BgHeader);
            };

            tokenSource = new CancellationTokenSource();
            _ = ScannerTaskAsync(tokenSource.Token);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            registryKeys.ForEach(k => k?.Close());
        }

        private async void btnRefresh_Click(object sender, EventArgs e) => await RefreshStartupItemsAsync();

        private void btnAutoRefresh_Click(object sender, EventArgs e)
        {
            _autoRefreshEnabled = !_autoRefreshEnabled;
            if (_autoRefreshEnabled)
            {
                btnAutoRefresh.Text      = "⏸  Auto: ON";
                btnAutoRefresh.BackColor = AccentGreen;
                btnAutoRefresh.ForeColor = BgDark;
                tsslAutoRefresh.Text     = "● Auto-refresh: ON  ";
                tsslAutoRefresh.ForeColor = AccentGreen;
            }
            else
            {
                btnAutoRefresh.Text      = "▶  Auto: OFF";
                btnAutoRefresh.BackColor = Color.FromArgb(80, 80, 100);
                btnAutoRefresh.ForeColor = TxtPrimary;
                tsslAutoRefresh.Text     = "● Auto-refresh: OFF  ";
                tsslAutoRefresh.ForeColor = AccentRed;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text != "Search...") ApplyFilter();
        }

        private void openLocation_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            string filePath = listView1.SelectedItems[0].SubItems[2].Text;
            if (string.IsNullOrEmpty(filePath)) { MessageBox.Show("No file path.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            if (File.Exists(filePath))
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            else
            {
                string folder = Path.GetDirectoryName(filePath);
                if (Directory.Exists(folder)) System.Diagnostics.Process.Start("explorer.exe", folder);
                else MessageBox.Show("Not found:\n" + filePath, "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            var    sel    = listView1.SelectedItems[0];
            string name   = sel.Text;
            string source = sel.SubItems[1].Text;
            string path   = sel.SubItems[2].Text;

            var confirm = MessageBox.Show(
                $"Delete \"{name}\" from startup?\n\nThis will remove the registry entry and the file if it exists.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                var key = registryKeys.FirstOrDefault(k => k?.Name == source);
                key?.DeleteValue(name, false);
                if (File.Exists(path)) File.Delete(path);
                MessageBox.Show("Done.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await RefreshStartupItemsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // ── ListView sorter ───────────────────────────────────────────────────────
    internal class ListViewItemComparer : System.Collections.IComparer
    {
        private readonly int col; private readonly SortOrder order;
        public ListViewItemComparer(int c, SortOrder o) { col = c; order = o; }
        public int Compare(object x, object y)
        {
            var lx = (ListViewItem)x; var ly = (ListViewItem)y;
            string tx = col < lx.SubItems.Count ? lx.SubItems[col].Text : "";
            string ty = col < ly.SubItems.Count ? ly.SubItems[col].Text : "";
            int r = string.Compare(tx, ty, StringComparison.OrdinalIgnoreCase);
            return order == SortOrder.Descending ? -r : r;
        }
    }
}
