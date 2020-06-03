using MCUHelper.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCUHelper
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public VariableViewForm variablesForm;


        private void elfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (elfFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                variablesForm = new VariableViewForm();
                variablesForm.LoadElf(elfFileDialog.FileName);
                variablesForm.MdiParent = this;
                variablesForm.Show();

            }
        }

        private void saveViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (variablesForm != null)
            {
                if (saveViewDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ViewerSettings settingsSaver = new ViewerSettings();
                    settingsSaver.SaveToFile(saveViewDialog.FileName, variablesForm.VariablesList, this, variablesForm, scrollBarWindows);
                }
            }
            else
            {
                MessageBox.Show("Elf is not loaded, nothing to save");
            }
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (variablesForm != null)
            {
                if (openViewDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ViewerSettings settingsSaver = new ViewerSettings();
                    settingsSaver.LoadFromFile(openViewDialog.FileName, variablesForm.VariablesList, this, variablesForm, scrollBarWindows);
                }
            }
            else
            {
                MessageBox.Show("Elf is not loaded, nothing to save");
            }
        }

        List<ScrollBarEditWindow> scrollBarWindows = new List<ScrollBarEditWindow>();

        private void scrollBarEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (variablesForm != null)
            {
                ScrollBarEditWindow scrollBarWindow = new ScrollBarEditWindow(variablesForm.valuesUpdater);
                scrollBarWindow.MdiParent = this;
                scrollBarWindow.VariablesList = variablesForm.VariablesList;
                scrollBarWindow.Show();
                scrollBarWindow.FormClosed += scrollBarWindow_FormClosed;
                scrollBarWindows.Add(scrollBarWindow);
            }
        }

        void scrollBarWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            scrollBarWindows.Remove(sender as ScrollBarEditWindow);
        }
    }
}
