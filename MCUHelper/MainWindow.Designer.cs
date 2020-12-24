namespace MCUHelper
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openElfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.lastElfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scrollBarEditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elfFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.openViewDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveViewDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 572);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(843, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(843, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openElfToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveViewToolStripMenuItem,
            this.toolStripSeparator2,
            this.lastElfToolStripMenuItem,
            this.lastViewToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openElfToolStripMenuItem
            // 
            this.openElfToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.elfToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.openElfToolStripMenuItem.Name = "openElfToolStripMenuItem";
            this.openElfToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.openElfToolStripMenuItem.Text = "Open";
            // 
            // elfToolStripMenuItem
            // 
            this.elfToolStripMenuItem.Name = "elfToolStripMenuItem";
            this.elfToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.elfToolStripMenuItem.Text = "Elf";
            this.elfToolStripMenuItem.Click += new System.EventHandler(this.elfToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.viewToolStripMenuItem.Text = "View";
            this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(122, 6);
            // 
            // saveViewToolStripMenuItem
            // 
            this.saveViewToolStripMenuItem.Name = "saveViewToolStripMenuItem";
            this.saveViewToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.saveViewToolStripMenuItem.Text = "Save view";
            this.saveViewToolStripMenuItem.Click += new System.EventHandler(this.saveViewToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(122, 6);
            // 
            // lastElfToolStripMenuItem
            // 
            this.lastElfToolStripMenuItem.Name = "lastElfToolStripMenuItem";
            this.lastElfToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.lastElfToolStripMenuItem.Text = "LastElf";
            this.lastElfToolStripMenuItem.Click += new System.EventHandler(this.lastElfToolStripMenuItem_Click);
            // 
            // lastViewToolStripMenuItem
            // 
            this.lastViewToolStripMenuItem.Name = "lastViewToolStripMenuItem";
            this.lastViewToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.lastViewToolStripMenuItem.Text = "LastView";
            this.lastViewToolStripMenuItem.Click += new System.EventHandler(this.lastViewToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(122, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scrollBarEditToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // scrollBarEditToolStripMenuItem
            // 
            this.scrollBarEditToolStripMenuItem.Name = "scrollBarEditToolStripMenuItem";
            this.scrollBarEditToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.scrollBarEditToolStripMenuItem.Text = "ScrollBar edit";
            this.scrollBarEditToolStripMenuItem.Click += new System.EventHandler(this.scrollBarEditToolStripMenuItem_Click);
            // 
            // elfFileDialog
            // 
            this.elfFileDialog.Filter = "Elf files|*.elf|All files|*.*";
            // 
            // openViewDialog
            // 
            this.openViewDialog.Filter = "Vmm files|*.vmm|All files|*.*";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 594);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "MCU Helper";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openElfToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem elfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog elfFileDialog;
        private System.Windows.Forms.OpenFileDialog openViewDialog;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scrollBarEditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastElfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.SaveFileDialog saveViewDialog;
    }
}