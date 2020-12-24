using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using MCUHelper.ElfParsing;
using MCUHelper.Timers;
using System.Runtime.InteropServices;
using HighResTimer;
using System.Reflection;
using System.Threading;

namespace MCUHelper
{
 

    public partial class VariableViewForm : Form
    {       
        Mutex updateDataMutex = new Mutex();
        HighResolutionTimer displayTimer;
        public IValuesUpdater valuesUpdater;

        public VariableViewForm()
        {
            InitializeComponent();
            RefreshComPorts();

            valuesUpdater = new SerialPortUpdater();

            displayTimer = new HighResolutionTimer(100.0f);
            displayTimer.UseHighPriorityThread = false;
            displayTimer.Elapsed += displayTimer_Elapsed;

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
        }

        void displayTimer_Elapsed(object sender, HighResolutionTimerElapsedEventArgs e)
        {
            PrintVariables();
        }

        Semaphore dataReceivedSem = new Semaphore(0, 1);
        List<byte> receivedData = new List<byte>();


        Mutex dataMutex = new Mutex();
        AutoResetEvent getDataEvent = new AutoResetEvent(false);



        

        delegate void TimeProc(int id, int msg, int user, int param1, int param2);

        bool connected = false;

        ElfParser parser;
        MainVariablesList variablesList;

        public MainVariablesList VariablesList
        {
            get
            {
                return variablesList;
            }
        }

        void RefreshComPorts()
        {
            string[] availablePorts = SerialPort.GetPortNames();
            portsComboBox.Items.Clear();
            foreach(string port in availablePorts)
            {
                portsComboBox.Items.Add(port);
            }

            if (portsComboBox.Items.Count > 0)
            {
                portsComboBox.SelectedIndex = 0;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        void UpdateControls(bool isConnected)
        {
            portsComboBox.Enabled = !isConnected;
            refreshButton.Enabled = !isConnected;
            
            if (isConnected)
            {
                statusLabel.Text = "Status: connected";
                statusLabel.ForeColor = Color.Green;
                connectButton.Text = "Disconnect";
            }
            else
            {
                statusLabel.Text = "Status: disconnected";
                statusLabel.ForeColor = Color.Red;
                connectButton.Text = "Connect";
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (connected == false)
            {
                if (portsComboBox.SelectedIndex != -1)
                {                    
                    ((SerialPortUpdater)valuesUpdater).Port.PortName = portsComboBox.SelectedItem as String;
                    valuesUpdater.SetVariablesList(variablesList);
                    valuesUpdater.StartUpdate();
                    
                    mainDisplayTimer.Enabled = true;
                    connected = true;
                }
            }
            else
            {
                valuesUpdater.StopUpdate();
                connected = false;
                mainDisplayTimer.Enabled = false;
            }
            UpdateControls(connected);
        }




        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connected)
            {
                displayTimer.Stop();
                valuesUpdater.StopUpdate();
                valuesUpdater.FreeData();
            }
        }

        String PrintVariable(ElfVariable variable, int indent)
        {
            String str = "";
            /* Create a tab*/
            for (int i = 0; i < indent  * 2; i++)
            {
                str += ' ';
            }
            if (!(variable is ElfObjectWithChildrens))
            {
                if (variable is ElfPointer)
                {
                    str += str + "*" + variable.Name + " = " + variable.GetStrValue() + Environment.NewLine;
                }
                else
                {
                    str += str + /*"[" + variable.Address+"]" + */ variable.Name + " = " + variable.GetStrValue() + Environment.NewLine;
                }
            }
            else
            {                
                str += str + variable.Name + Environment.NewLine;
                ElfObjectWithChildrens elfStruct = (ElfObjectWithChildrens)variable;
                foreach (ElfVariable child in elfStruct.Childrens)
                {
                    str += PrintVariable(child, indent + 1);
                }
            }

            return str;
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            parser = new ElfParser(@"D:\Projects\MASS\MCU\Debug\SuspensionStand.elf");

            variablesList = new MainVariablesList(parser);

            variablesList.UpdateVariable("cinADCDriver", null, "cinADCDriver");
            variablesList.UpdateVariable("cinBatteryVoltageProvider", null, "cinBatteryVoltageProvider");

            valuesUpdater.SetVariablesList(variablesList);
            PrintVariables();
        }

        public void LoadElf(String elfFileName)
        {
            parser = new ElfParser(elfFileName);

            variablesList = new MainVariablesList(parser);

            valuesUpdater.SetVariablesList(variablesList);
            PrintVariables();

            this.Text = elfFileName;
        }

        

        void PrintVariables()
        {
            try
            {
                if (variablesList != null)
                {
                    String text = "";
                    foreach (ElfVariable variable in variablesList.variables)
                    {
                        text += PrintVariable(variable, 0);
                    }
                   // if (!richTextBox1.SelectedText.Equals(text))
                    {
                        //richTextBox1.Suspend();
                        richTextBox1.Text = text;
                        //richTextBox1.SelectAll();
                        //richTextBox1.SelectedText = text;
                        //richTextBox1.Resume();
                    }
                }
            }
            catch
            {

            }
        }
        

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                /* Add variable to view list */
                if (textBox1.Text.Contains('=') == false)
                {
                    variablesList.UpdateVariable(textBox1.Text, null, textBox1.Text);
                    PrintVariable(variablesList.variables[variablesList.variables.Count - 1], 0);

                    textBox1.Text = "";
                }
                else
                {
                    
                }
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshComPorts();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            
        }

        public static void SetDoubleBuffered(Control control)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;
                return handleParam;
            }
        }

        private void mainDisplayTimer_Tick(object sender, EventArgs e)
        {
            PrintVariables();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private int GetCharacterIndexOfSelection()
        {
            var wordWrappedIndex = richTextBox1.SelectionStart;

            RichTextBox scratch = new RichTextBox();
            scratch.Lines = richTextBox1.Lines;
            scratch.SelectionStart = wordWrappedIndex;
            scratch.SelectionLength = 1;
            scratch.WordWrap = false;
            return scratch.SelectionStart;
        }

        private int GetLineNumberOfSelection()
        {
            var selectionStartIndex = GetCharacterIndexOfSelection();

            RichTextBox scratch = new RichTextBox();
            scratch.Lines = richTextBox1.Lines;
            scratch.SelectionStart = selectionStartIndex;
            scratch.SelectionLength = 1;
            scratch.WordWrap = false;
            return scratch.GetLineFromCharIndex(selectionStartIndex);
        }

        ElfVariable clickedVar;
        private void richTextBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void newValueTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (clickedVar != null)
                {
                    valuesUpdater.AddWriteCommand(clickedVar, newValueTextBox.Text);                    
                }
            }
        }

        private void openElfFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
           

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                int lineIndex = GetLineNumberOfSelection();
                clickedVar = variablesList.GetVariableByIndex(lineIndex);

                if (clickedVar != null)
                {
                    variableTextBox.Text = clickedVar.FullName;
                    newValueTextBox.Enabled = !(clickedVar is ElfObjectWithChildrens);
                    newValueTextBox.Text = clickedVar.GetStrValue();
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                int index = richTextBox1.GetCharIndexFromPosition(e.Location);
                richTextBox1.Select(index, 0);

                int lineIndex = GetLineNumberOfSelection();
                clickedVar = variablesList.GetVariableByIndex(lineIndex);

                if (clickedVar != null)
                {
                    variablesContextMenu.Items[0].Text = "Remove " + clickedVar.Name;
                    variablesContextMenu.Items[0].Tag = clickedVar;
                    variablesContextMenu.Show(richTextBox1, e.Location);
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            variablesList.variables.Clear();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (variablesContextMenu.Items[0].Tag is  ElfVariable)
            {
                ElfVariable elfVar = variablesContextMenu.Items[0].Tag  as ElfVariable;
                variablesList.RemoveVariable(elfVar);
            }
        }

        private void richTextBox1_ContextMenuStripChanged(object sender, EventArgs e)
        {

        }

        private void variablesContextMenu_Opening(object sender, CancelEventArgs e)
        {
            
        }
    }
}
