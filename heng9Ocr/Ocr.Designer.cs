namespace heng9Ocr
{
    partial class Ocr
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeCam = new System.Windows.Forms.ToolStripMenuItem();
            this.工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearText = new System.Windows.Forms.ToolStripMenuItem();
            this.分辨率ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Max = new System.Windows.Forms.ToolStripMenuItem();
            this.largish = new System.Windows.Forms.ToolStripMenuItem();
            this.medel = new System.Windows.Forms.ToolStripMenuItem();
            this.aLittle = new System.Windows.Forms.ToolStripMenuItem();
            this.small = new System.Windows.Forms.ToolStripMenuItem();
            this.PicShow = new System.Windows.Forms.PictureBox();
            this.TxtShow = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.PB = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.MsgTxt = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.x = new System.Windows.Forms.TextBox();
            this.y = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.w = new System.Windows.Forms.TextBox();
            this.h = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.reset = new System.Windows.Forms.Button();
            this.reReader = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.resultCount = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicShow)).BeginInit();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.工具ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(861, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeCam});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // closeCam
            // 
            this.closeCam.Name = "closeCam";
            this.closeCam.Size = new System.Drawing.Size(152, 22);
            this.closeCam.Text = "退出";
            this.closeCam.Click += new System.EventHandler(this.closeCam_Click);
            // 
            // 工具ToolStripMenuItem
            // 
            this.工具ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearText,
            this.分辨率ToolStripMenuItem});
            this.工具ToolStripMenuItem.Name = "工具ToolStripMenuItem";
            this.工具ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.工具ToolStripMenuItem.Text = "工具";
            // 
            // clearText
            // 
            this.clearText.Name = "clearText";
            this.clearText.Size = new System.Drawing.Size(152, 22);
            this.clearText.Text = "清屏";
            this.clearText.Click += new System.EventHandler(this.clearText_Click);
            // 
            // 分辨率ToolStripMenuItem
            // 
            this.分辨率ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Max,
            this.largish,
            this.medel,
            this.aLittle,
            this.small});
            this.分辨率ToolStripMenuItem.Name = "分辨率ToolStripMenuItem";
            this.分辨率ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.分辨率ToolStripMenuItem.Text = "分辨率";
            // 
            // Max
            // 
            this.Max.Name = "Max";
            this.Max.Size = new System.Drawing.Size(132, 22);
            this.Max.Text = "最大";
            this.Max.Click += new System.EventHandler(this.Max_Click);
            // 
            // largish
            // 
            this.largish.Name = "largish";
            this.largish.Size = new System.Drawing.Size(132, 22);
            this.largish.Text = "稍大";
            this.largish.Click += new System.EventHandler(this.largish_Click);
            // 
            // medel
            // 
            this.medel.Name = "medel";
            this.medel.Size = new System.Drawing.Size(132, 22);
            this.medel.Text = "中等(默认)";
            this.medel.Click += new System.EventHandler(this.medel_Click);
            // 
            // aLittle
            // 
            this.aLittle.Name = "aLittle";
            this.aLittle.Size = new System.Drawing.Size(132, 22);
            this.aLittle.Text = "稍小";
            this.aLittle.Click += new System.EventHandler(this.aLittle_Click);
            // 
            // small
            // 
            this.small.Name = "small";
            this.small.Size = new System.Drawing.Size(132, 22);
            this.small.Text = "小";
            this.small.Click += new System.EventHandler(this.small_Click);
            // 
            // PicShow
            // 
            this.PicShow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PicShow.Location = new System.Drawing.Point(4, 4);
            this.PicShow.Name = "PicShow";
            this.PicShow.Size = new System.Drawing.Size(668, 489);
            this.PicShow.TabIndex = 1;
            this.PicShow.TabStop = false;
            this.PicShow.SizeChanged += new System.EventHandler(this.PicShow_SizeChanged);
            // 
            // TxtShow
            // 
            this.TxtShow.Location = new System.Drawing.Point(2, 4);
            this.TxtShow.Multiline = true;
            this.TxtShow.Name = "TxtShow";
            this.TxtShow.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TxtShow.Size = new System.Drawing.Size(180, 516);
            this.TxtShow.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.PB);
            this.panel1.Controls.Add(this.TxtShow);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(676, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(185, 524);
            this.panel1.TabIndex = 4;
            this.panel1.SizeChanged += new System.EventHandler(this.panel1_SizeChanged);
            // 
            // PB
            // 
            this.PB.Dock = System.Windows.Forms.DockStyle.Top;
            this.PB.Location = new System.Drawing.Point(0, 0);
            this.PB.Maximum = 360;
            this.PB.Name = "PB";
            this.PB.Size = new System.Drawing.Size(185, 2);
            this.PB.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.PicShow, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(676, 524);
            this.tableLayoutPanel1.TabIndex = 5;
            this.tableLayoutPanel1.SizeChanged += new System.EventHandler(this.tableLayoutPanel1_SizeChanged);
            // 
            // MsgTxt
            // 
            this.MsgTxt.Location = new System.Drawing.Point(4, 4);
            this.MsgTxt.Name = "MsgTxt";
            this.MsgTxt.Size = new System.Drawing.Size(668, 21);
            this.MsgTxt.TabIndex = 3;
            this.MsgTxt.Text = "welcome";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.MsgTxt, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 520);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(676, 29);
            this.tableLayoutPanel2.TabIndex = 6;
            this.tableLayoutPanel2.SizeChanged += new System.EventHandler(this.tableLayoutPanel2_SizeChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(221, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "X :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(288, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "Y :";
            // 
            // x
            // 
            this.x.Location = new System.Drawing.Point(243, 3);
            this.x.Name = "x";
            this.x.Size = new System.Drawing.Size(42, 21);
            this.x.TabIndex = 9;
            // 
            // y
            // 
            this.y.Location = new System.Drawing.Point(312, 3);
            this.y.Name = "y";
            this.y.Size = new System.Drawing.Size(42, 21);
            this.y.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.MenuBar;
            this.label3.Location = new System.Drawing.Point(360, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "宽";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.Menu;
            this.label4.Location = new System.Drawing.Point(425, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "高";
            // 
            // w
            // 
            this.w.Location = new System.Drawing.Point(378, 3);
            this.w.Name = "w";
            this.w.Size = new System.Drawing.Size(42, 21);
            this.w.TabIndex = 13;
            // 
            // h
            // 
            this.h.Location = new System.Drawing.Point(446, 3);
            this.h.Name = "h";
            this.h.Size = new System.Drawing.Size(42, 21);
            this.h.TabIndex = 14;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(493, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(53, 22);
            this.button1.TabIndex = 15;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // reset
            // 
            this.reset.Location = new System.Drawing.Point(547, 3);
            this.reset.Name = "reset";
            this.reset.Size = new System.Drawing.Size(51, 22);
            this.reset.TabIndex = 16;
            this.reset.Text = "重置";
            this.reset.UseVisualStyleBackColor = true;
            this.reset.Click += new System.EventHandler(this.reset_Click);
            // 
            // reReader
            // 
            this.reReader.Location = new System.Drawing.Point(600, 3);
            this.reReader.Name = "reReader";
            this.reReader.Size = new System.Drawing.Size(62, 22);
            this.reReader.TabIndex = 17;
            this.reReader.Text = "清空缓存";
            this.reReader.UseVisualStyleBackColor = true;
            this.reReader.Click += new System.EventHandler(this.reReader_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.SystemColors.Menu;
            this.label5.Location = new System.Drawing.Point(663, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 18;
            this.label5.Text = "结果数";
            // 
            // resultCount
            // 
            this.resultCount.Location = new System.Drawing.Point(705, 3);
            this.resultCount.Name = "resultCount";
            this.resultCount.Size = new System.Drawing.Size(42, 21);
            this.resultCount.TabIndex = 19;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(171, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 20;
            this.label6.Text = "识别区域";
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(759, 8);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(29, 12);
            this.labelTime.TabIndex = 21;
            this.labelTime.Text = "用时";
            // 
            // Ocr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(861, 549);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.resultCount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.reReader);
            this.Controls.Add(this.reset);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.h);
            this.Controls.Add(this.w);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.y);
            this.Controls.Add(this.x);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Ocr";
            this.RightToLeftLayout = true;
            this.Text = "OCR";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Ocr_FormClosed);
            this.Load += new System.EventHandler(this.Ocr_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicShow)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 工具ToolStripMenuItem;
        private System.Windows.Forms.PictureBox PicShow;
        private System.Windows.Forms.TextBox TxtShow;
        private System.Windows.Forms.ToolStripMenuItem closeCam;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox MsgTxt;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ToolStripMenuItem clearText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox x;
        private System.Windows.Forms.TextBox y;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox w;
        private System.Windows.Forms.TextBox h;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar PB;
        private System.Windows.Forms.Button reset;
        private System.Windows.Forms.Button reReader;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox resultCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.ToolStripMenuItem 分辨率ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Max;
        private System.Windows.Forms.ToolStripMenuItem largish;
        private System.Windows.Forms.ToolStripMenuItem medel;
        private System.Windows.Forms.ToolStripMenuItem aLittle;
        private System.Windows.Forms.ToolStripMenuItem small;
    }
}

