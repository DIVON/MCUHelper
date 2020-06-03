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
using MassConnector.ElfParsing;
using MassConnector.Timers;
using System.Runtime.InteropServices;
using HighResTimer;
using System.Reflection;
using System.Threading;

namespace MassConnector
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
            foreach(string port in availablePorts)
            {
                portsComboBox.Items.Add(port);
            }
            if (portsComboBox.Items.Count == 1u)
            {
                portsComboBox.SelectedIndex = 0;
                connectButton_Click(null, null);
            }
            portsComboBox.SelectedIndex = 1;

            connectButton_Click(null, null);
            //button1_Click_1(null, null);
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
                    valuesUpdater = new SerialPortUpdater();
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
            //parser = new ElfParser(@"D:\Projects\MASS\MCU\Debug\SuspensionStand.elf");

            variablesList = new MainVariablesList(parser);

            variablesList.UpdateVariable("cinSliderAngleProvider.Rte_WriteField_SliderPosition_Position", null, "cinSliderAngleProvider.Rte_WriteField_SliderPosition_Position");
            variablesList.UpdateVariable("cinHallFilterCh1.Rte_WriteField_FilteredData_qualifiedADC.adcValue", null, "cinHallFilterCh1.Rte_WriteField_FilteredData_qualifiedADC.adcValue");
            variablesList.UpdateVariable("cinHallFilterCh2.Rte_WriteField_FilteredData_qualifiedADC.adcValue", null, "cinHallFilterCh2.Rte_WriteField_FilteredData_qualifiedADC.adcValue");
            variablesList.UpdateVariable("cinHallFilterCh3.Rte_WriteField_FilteredData_qualifiedADC.adcValue", null, "cinHallFilterCh3.Rte_WriteField_FilteredData_qualifiedADC.adcValue");
            variablesList.UpdateVariable("cinBatteryVoltageFilter.Rte_WriteField_FilteredData_qualifiedADC.adcValue", null, "cinBatteryVoltageFilter.Rte_WriteField_FilteredData_qualifiedADC.adcValue");
            variablesList.UpdateVariable("cinBatteryVoltageProvider.Rte_WriteField_BatteryVoltage_Voltage", null, "cinBatteryVoltageProvider.Rte_WriteField_BatteryVoltage_Voltage");
            //variablesList.UpdateVariable("testStruct", null, "testStruct");
            //variablesList.UpdateVariable("testStruct[0]", null, "testStruct[0]");

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
                    if (!richTextBox1.SelectedText.Equals(text))
                    {
                        richTextBox1.Suspend();
                        richTextBox1.Text = text;
                        //richTextBox1.SelectAll();
                        //richTextBox1.SelectedText = text;
                        richTextBox1.Resume();
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

        ElfVariable clickedVar;
        private void richTextBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs mArg = (MouseEventArgs)e;
            int lineIndex = mArg.Y / richTextBox1.Font.Height;
            clickedVar = variablesList.GetVariableByIndex(lineIndex);
            if (clickedVar != null)
            {
                variableTextBox.Text = clickedVar.FullName;
                newValueTextBox.Enabled = !(clickedVar is ElfObjectWithChildrens);
                newValueTextBox.Text = clickedVar.GetStrValue();
            }
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

        

        
    }
}
