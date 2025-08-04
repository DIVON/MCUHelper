using System.Windows.Forms;
namespace MCUHelper
{
    partial class VariableViewForm
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
            this.components = new System.ComponentModel.Container();
            this.connectButton = new System.Windows.Forms.Button();
            this.portsComboBox = new System.Windows.Forms.ComboBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.refreshButton = new System.Windows.Forms.Button();
            this.mainDisplayTimer = new System.Windows.Forms.Timer(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.newValueTextBox = new System.Windows.Forms.TextBox();
            this.variableTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.variablesContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.richTextBox1 = new System.Windows.Forms.ViewOnlyRichTextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.variablesContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(200, 12);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 0;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // portsComboBox
            // 
            this.portsComboBox.FormattingEnabled = true;
            this.portsComboBox.Location = new System.Drawing.Point(61, 14);
            this.portsComboBox.Name = "portsComboBox";
            this.portsComboBox.Size = new System.Drawing.Size(133, 21);
            this.portsComboBox.TabIndex = 1;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.ForeColor = System.Drawing.Color.Red;
            this.statusLabel.Location = new System.Drawing.Point(291, 17);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(107, 13);
            this.statusLabel.TabIndex = 2;
            this.statusLabel.Text = "Status: disconnected";
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(11, 12);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(44, 23);
            this.refreshButton.TabIndex = 6;
            this.refreshButton.Text = "R";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // mainDisplayTimer
            // 
            this.mainDisplayTimer.Interval = 1;
            this.mainDisplayTimer.Tick += new System.EventHandler(this.mainDisplayTimer_Tick);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(11, 57);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(1041, 23);
            this.textBox1.TabIndex = 14;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(426, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "Reload ELF";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 25;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // newValueTextBox
            // 
            this.newValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.newValueTextBox.Location = new System.Drawing.Point(923, 19);
            this.newValueTextBox.Name = "newValueTextBox";
            this.newValueTextBox.Size = new System.Drawing.Size(111, 20);
            this.newValueTextBox.TabIndex = 18;
            this.newValueTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.newValueTextBox_KeyDown);
            // 
            // variableTextBox
            // 
            this.variableTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.variableTextBox.Location = new System.Drawing.Point(6, 19);
            this.variableTextBox.Name = "variableTextBox";
            this.variableTextBox.ReadOnly = true;
            this.variableTextBox.Size = new System.Drawing.Size(911, 20);
            this.variableTextBox.TabIndex = 19;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.variableTextBox);
            this.groupBox1.Controls.Add(this.newValueTextBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 525);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1040, 48);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Variable update";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(533, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 21;
            this.button2.Text = "Clear";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // variablesContextMenu
            // 
            this.variablesContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem});
            this.variablesContextMenu.Name = "contextMenuStrip1";
            this.variablesContextMenu.Size = new System.Drawing.Size(174, 26);
            this.variablesContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.variablesContextMenu_Opening);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.removeToolStripMenuItem.Text = "Remove from view";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.AutoWordSelection = true;
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Menu;
            this.richTextBox1.DetectUrls = false;
            this.richTextBox1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.richTextBox1.Location = new System.Drawing.Point(11, 86);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(1041, 433);
            this.richTextBox1.TabIndex = 13;
            this.richTextBox1.Text = "";
            this.richTextBox1.WordWrap = false;
            this.richTextBox1.Click += new System.EventHandler(this.richTextBox1_Click);
            this.richTextBox1.ContextMenuStripChanged += new System.EventHandler(this.richTextBox1_ContextMenuStripChanged);
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            this.richTextBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.richTextBox1_MouseDown);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(650, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(160, 23);
            this.button3.TabIndex = 22;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // VariableViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 585);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.portsComboBox);
            this.Controls.Add(this.connectButton);
            this.DoubleBuffered = true;
            this.Name = "VariableViewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Variable View";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.variablesContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.ComboBox portsComboBox;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Timer mainDisplayTimer;
        private ViewOnlyRichTextBox richTextBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timer2;
        private TextBox newValueTextBox;
        private TextBox variableTextBox;
        private GroupBox groupBox1;
        private Button button2;
        private ContextMenuStrip variablesContextMenu;
        private ToolStripMenuItem removeToolStripMenuItem;
        private Button button3;
    }
}

