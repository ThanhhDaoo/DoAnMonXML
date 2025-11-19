using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LibraryManagement.Helpers
{
    /// <summary>
    /// Helper class để tạo các UI components hiện đại
    /// </summary>
    public static class ModernUIHelper
    {
        // Màu sắc hiện đại
        public static class Colors
        {
            public static Color Primary = Color.FromArgb(52, 152, 219);
            public static Color PrimaryDark = Color.FromArgb(41, 128, 185);
            public static Color Success = Color.FromArgb(46, 204, 113);
            public static Color SuccessDark = Color.FromArgb(39, 174, 96);
            public static Color Danger = Color.FromArgb(231, 76, 60);
            public static Color DangerDark = Color.FromArgb(192, 57, 43);
            public static Color Warning = Color.FromArgb(230, 126, 34);
            public static Color WarningDark = Color.FromArgb(211, 84, 0);
            public static Color Info = Color.FromArgb(155, 89, 182);
            public static Color InfoDark = Color.FromArgb(142, 68, 173);
            public static Color Dark = Color.FromArgb(44, 62, 80);
            public static Color Light = Color.FromArgb(236, 240, 241);
            public static Color Gray = Color.FromArgb(149, 165, 166);
            public static Color White = Color.White;
        }

        /// <summary>
        /// Tạo TextBox hiện đại với rounded corners
        /// </summary>
        public static Panel CreateModernTextBox(string placeholder = "", int width = 280, int height = 40)
        {
            Panel panel = new Panel
            {
                Size = new Size(width, height),
                BackColor = Colors.Light
            };

            // Rounded corners
            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(panel.ClientRectangle, 8))
                {
                    panel.Region = new Region(path);
                    e.Graphics.FillPath(new SolidBrush(Colors.Light), path);
                }
            };

            TextBox textBox = new TextBox
            {
                Location = new Point(12, (height - 25) / 2),
                Size = new Size(width - 24, 25),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.None,
                BackColor = Colors.Light,
                Text = placeholder,
                ForeColor = Colors.Gray
            };

            // Placeholder behavior
            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Colors.Dark;
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Colors.Gray;
                }
            };

            panel.Controls.Add(textBox);
            panel.Tag = textBox; // Store reference to textbox
            return panel;
        }

        /// <summary>
        /// Tạo Button hiện đại với gradient và hiệu ứng
        /// </summary>
        public static Button CreateModernButton(string text, Color color, int width = 120, int height = 45)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(width, height),
                BackColor = color,
                ForeColor = Colors.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };

            btn.FlatAppearance.BorderSize = 0;

            // Rounded corners
            btn.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(btn.ClientRectangle, 10))
                {
                    btn.Region = new Region(path);
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        btn.ClientRectangle, color, ControlPaint.Dark(color, 0.1f), 45f))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }

                // Draw text
                TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, btn.ClientRectangle,
                    btn.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };

            // Hover effect
            Color originalColor = color;
            Color hoverColor = ControlPaint.Dark(color, 0.15f);

            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = hoverColor;
                btn.Invalidate();
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = originalColor;
                btn.Invalidate();
            };

            return btn;
        }

        /// <summary>
        /// Tạo Card Panel hiện đại
        /// </summary>
        public static Panel CreateCard(int width = 250, int height = 300, int cornerRadius = 15)
        {
            Panel card = new Panel
            {
                Size = new Size(width, height),
                BackColor = Colors.White,
                Cursor = Cursors.Hand
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Shadow effect
                using (GraphicsPath shadowPath = GetRoundedRectangle(
                    new Rectangle(3, 3, width - 6, height - 6), cornerRadius))
                {
                    using (PathGradientBrush shadowBrush = new PathGradientBrush(shadowPath))
                    {
                        shadowBrush.CenterColor = Color.FromArgb(50, 0, 0, 0);
                        shadowBrush.SurroundColors = new[] { Color.FromArgb(0, 0, 0, 0) };
                        e.Graphics.FillPath(shadowBrush, shadowPath);
                    }
                }

                // Card background
                using (GraphicsPath path = GetRoundedRectangle(card.ClientRectangle, cornerRadius))
                {
                    card.Region = new Region(path);
                    e.Graphics.FillPath(new SolidBrush(Colors.White), path);
                }
            };

            return card;
        }

        /// <summary>
        /// Tạo Header Panel với gradient
        /// </summary>
        public static Panel CreateGradientHeader(string title, string subtitle, Color color1, Color color2, int height = 100)
        {
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = height,
                BackColor = color1
            };

            header.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    header.ClientRectangle, color1, color2, LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, header.ClientRectangle);
                }
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Colors.White,
                Location = new Point(30, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            Label lblSubtitle = new Label
            {
                Text = subtitle,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(220, 220, 220),
                Location = new Point(30, 55),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            header.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });
            return header;
        }

        /// <summary>
        /// Style DataGridView hiện đại
        /// </summary>
        public static void StyleDataGridView(DataGridView dgv, Color headerColor)
        {
            dgv.BackgroundColor = Colors.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.Font = new Font("Segoe UI", 10);
            dgv.RowTemplate.Height = 40;
            dgv.EnableHeadersVisualStyles = false;

            // Header style
            dgv.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Colors.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgv.ColumnHeadersHeight = 45;

            // Row style
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.DefaultCellStyle.SelectionBackColor = headerColor;
            dgv.DefaultCellStyle.SelectionForeColor = Colors.White;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(224, 224, 224);
            dgv.DefaultCellStyle.Padding = new Padding(5);
        }

        /// <summary>
        /// Tạo Search Box với icon
        /// </summary>
        public static Panel CreateSearchBox(string placeholder = "Tìm kiếm...", int width = 350, int height = 45)
        {
            Panel searchPanel = new Panel
            {
                Size = new Size(width, height),
                BackColor = Colors.Light
            };

            searchPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectangle(searchPanel.ClientRectangle, 10))
                {
                    searchPanel.Region = new Region(path);
                }
            };

            Label lblIcon = new Label
            {
                Text = "🔍",
                Font = new Font("Segoe UI", 14),
                Location = new Point(12, 10),
                Size = new Size(30, 25),
                BackColor = Color.Transparent
            };

            TextBox txtSearch = new TextBox
            {
                Location = new Point(50, (height - 25) / 2),
                Size = new Size(width - 60, 25),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.None,
                BackColor = Colors.Light,
                Text = placeholder,
                ForeColor = Colors.Gray
            };

            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == placeholder)
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Colors.Dark;
                }
            };

            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = placeholder;
                    txtSearch.ForeColor = Colors.Gray;
                }
            };

            searchPanel.Controls.AddRange(new Control[] { lblIcon, txtSearch });
            searchPanel.Tag = txtSearch;
            return searchPanel;
        }

        /// <summary>
        /// Tạo Icon Button
        /// </summary>
        public static Button CreateIconButton(string icon, string text, Color color, int width = 110, int height = 45)
        {
            Button btn = CreateModernButton($"{icon} {text}", color, width, height);
            return btn;
        }

        /// <summary>
        /// Helper: Tạo rounded rectangle path
        /// </summary>
        private static GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Thêm shadow effect cho control
        /// </summary>
        public static void AddShadow(Control control, int shadowSize = 5)
        {
            control.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle shadowRect = new Rectangle(
                    shadowSize, shadowSize,
                    control.Width - shadowSize * 2,
                    control.Height - shadowSize * 2);

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(shadowRect);
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.CenterColor = Color.FromArgb(100, 0, 0, 0);
                        brush.SurroundColors = new[] { Color.FromArgb(0, 0, 0, 0) };
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };
        }

        /// <summary>
        /// Tạo Circular Avatar/Icon
        /// </summary>
        public static Panel CreateCircularIcon(string icon, Color backgroundColor, int size = 100)
        {
            Panel panel = new Panel
            {
                Size = new Size(size, size),
                BackColor = Color.Transparent
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(new SolidBrush(backgroundColor), 0, 0, size, size);
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", size / 2.5f),
                Size = new Size(size, size),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                ForeColor = Colors.White
            };

            panel.Controls.Add(lblIcon);
            return panel;
        }

        /// <summary>
        /// Làm cho form có thể kéo được
        /// </summary>
        public static void MakeFormDraggable(Form form, Control dragControl)
        {
            bool dragging = false;
            Point dragCursorPoint = Point.Empty;
            Point dragFormPoint = Point.Empty;

            dragControl.MouseDown += (s, e) =>
            {
                dragging = true;
                dragCursorPoint = Cursor.Position;
                dragFormPoint = form.Location;
            };

            dragControl.MouseMove += (s, e) =>
            {
                if (dragging)
                {
                    Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                    form.Location = Point.Add(dragFormPoint, new Size(diff));
                }
            };

            dragControl.MouseUp += (s, e) => { dragging = false; };
        }
    }
}
