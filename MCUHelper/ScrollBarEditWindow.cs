using MassConnector.ElfParsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MassConnector
{
    public partial class ScrollBarEditWindow : Form
    {
        IValuesUpdater valuesUpdater;

        public ScrollBarEditWindow(IValuesUpdater valuesUpdater)
        {
            InitializeComponent();
            this.valuesUpdater = valuesUpdater;
        }

        Boolean stopUpdate = false;
        void StopUpdate()
        {
            stopUpdate = true;
        }

        void StartUpdate()
        {
            stopUpdate = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            StopUpdate();

            RadioButton rb = (sender as RadioButton);
            string[] range = rb.Text.Split(':');

            minValueEdit.Text = range[0];
            maxValueEdit.Text = range[1];
            trackBar1.Minimum = Int32.Parse(range[0]);
            trackBar1.Maximum = Int32.Parse(range[1]);
            StartUpdate();
        }

        public void SetMin(int min)
        {
            StopUpdate();
            trackBar1.Minimum = min;
            minValueEdit.Text = min.ToString();
            StartUpdate();
        }

        public void SetMax(int max)
        {
            StopUpdate();
            trackBar1.Maximum = max;
            maxValueEdit.Text = max.ToString();
            StartUpdate();
        }

        int MinValue
        {
            get
            {
                Int32 min;
                if (Int32.TryParse(minValueEdit.Text, out min) == true)
                {
                    return min;
                }
                else 
                {
                    minValueEdit.Text = (-1000).ToString();
                    return -1000;
                }
            }
        }
      // int GetMinRange()
      // {
      //     Int32 min;
      //     if (Int32.TryParse(minValueEdit.Text, out min) == true)
      //     {
      //         return val;
      //     }
      //     else 
      //     {
      //         min = -10;
      //     }
      //
      //     else
      //     {
      //         if (currentVariable != null)
      //         {
      //             return Math.Min(Math.Abs((int)currentVariable.GetValue()) * 2, - 100);
      //         } 
      //         else
      //         {
      //             return -100;
      //         }
      //     }
      // }
      //
      // int MaxRange
      // {
      //     get
      //     {
      //         Int32 val;
      //         if (Int32.TryParse(maxValueEdit.Text, out val) == true)
      //         {
      //             return val;
      //         }
      //         else
      //         {
      //             if (currentVariable != null)
      //             {
      //                 return (int)currentVariable.GetValue() * 2;
      //             }
      //             else
      //             {
      //                 return -100;
      //             }
      //         }
      //     }
      // }

        public void UpdateIndex(int index)
        {
            if ((index >= 0) && (index < allFields.Count))
            {
                currentVariable = allFields[index];
                StopUpdate();
                variableComboBox.SelectedIndex = index;

                int value = Convert.ToInt32(currentVariable.GetValue());

                trackBar1.Value = value;

                StartUpdate();
            }
        }

        MainVariablesList variablesList = null;
        ElfVariableList allFields;

        public MainVariablesList VariablesList
        {
            set
            {
                variablesList = value;
                allFields = variablesList.GetAllFields();
                UpdateComboBox(allFields);
            }
            get
            {
                return variablesList;
            }
        }

        ElfVariable currentVariable = null;
        void UpdateComboBox(ElfVariableList allFields)
        {
            variableComboBox.Items.Clear();
            foreach(ElfVariable field in allFields)
            {
                variableComboBox.Items.Add(field.FullName);
            }
            variableComboBox.SelectedIndex = -1;
            currentVariable = null;
        }

        private void variableComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (allFields == null)
            {
                return;
            }

            if ((variableComboBox.SelectedIndex >= 0 ) && (variableComboBox.SelectedIndex < allFields.Count))
            {
                currentVariable = allFields[variableComboBox.SelectedIndex];                
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (currentVariable != null)
            {
                if (stopUpdate == false)
                {
                    valuesUpdater.AddWriteCommand(currentVariable, trackBar1.Value.ToString());
                }
            }
            valueLabel.Text = "Value: " + trackBar1.Value;
        }
    }
}
