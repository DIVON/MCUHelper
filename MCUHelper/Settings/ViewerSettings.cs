using AutosarGuiEditor.Source.Utility;
using MCUHelper.ElfParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MCUHelper.Settings
{
    public class WindowPosition
    {
        public int Top;
        public int Left;
        public int Width;
        public int Height;

        public void WriteToXml(XElement root)
        {
            root.Add(new XElement("Top", Top.ToString()));
            root.Add(new XElement("Left", Left.ToString()));
            root.Add(new XElement("Width", Width.ToString()));
            root.Add(new XElement("Height", Height.ToString()));
        }

        public void LoadFromXml(XElement xml)
        {
            int value;
            int.TryParse(xml.Element("Top").Value, out value);
            Top = value;

            int.TryParse(xml.Element("Left").Value, out value);
            Left = value;

            int.TryParse(xml.Element("Width").Value, out value);
            Width = value;

            int.TryParse(xml.Element("Height").Value, out value);
            Height = value;
        }

        public static WindowPosition GetPositionFromForm(Form form)
        {
            WindowPosition pos = new WindowPosition();
            pos.Left = form.Left;
            pos.Top = form.Top;
            pos.Height = form.Height;
            pos.Width = form.Width;
            return pos;
        }

        public void UpdateForm(Form form)
        {
            form.Left = Left;
            form.Top = Top;
            form.Width = Width;
            form.Height = Height;
        }
    }
    
    public class ViewerSettings
    {
        public void LoadFromFile(String fileName, MainVariablesList variablesList, MainWindow mainWindow, Form viewerWindow,  List<ScrollBarEditWindow> scrollBarWindows)
        {
            XElement xroot = XElement.Load(fileName);

            XElement mwEl = xroot.Element("MainWindow");
            WindowPosition mwPos = new WindowPosition();
            mwPos.LoadFromXml(mwEl);
            mwPos.UpdateForm(mainWindow);

            XElement vvEl = xroot.Element("ViewerWindow");
            WindowPosition vvPos = new WindowPosition();
            vvPos.LoadFromXml(vvEl);
            vvPos.UpdateForm(viewerWindow);

            LoadVariables(variablesList, xroot);

            XElement scrollBarsEl = xroot.Element("ScrollBars");
            LoadScrollBarWindows(scrollBarsEl, scrollBarWindows, variablesList, mainWindow);
        }

        public void SaveToFile(String fileName, MainVariablesList variablesList, MainWindow mainWindow, Form viewerWindow, List<ScrollBarEditWindow> scrollBarWindows)
        {
            XDocument xdoc = new XDocument();
            
            XElement root = new XElement("Settings");
            xdoc.Add(root);

            XElement mainWindowEl = new XElement("MainWindow");            
            WindowPosition mwPos = WindowPosition.GetPositionFromForm(mainWindow);
            mwPos.WriteToXml(mainWindowEl);
            root.Add(mainWindowEl);

            XElement viewerWindowEl = new XElement("ViewerWindow");            
            WindowPosition vwPos = WindowPosition.GetPositionFromForm(viewerWindow);
            vwPos.WriteToXml(viewerWindowEl);
            root.Add(viewerWindowEl);

            SaveVariableNames(variablesList.variables, root);

            XElement scrollBarWindowsEl = new XElement("ScrollBars");
            SaveScrollBarWindows(scrollBarWindowsEl, scrollBarWindows);
            root.Add(scrollBarWindowsEl);

            xdoc.Save(fileName);
        }

        void SaveScrollBarWindows(XElement root, List<ScrollBarEditWindow> scrollBarWindows)
        {
            foreach (ScrollBarEditWindow window in scrollBarWindows)
            {
                XElement scrollWindowEl = new XElement("ScrollBarWindow");
                WindowPosition mwPos = WindowPosition.GetPositionFromForm(window);
                mwPos.WriteToXml(scrollWindowEl);

                XElement minEl = new XElement("Min", window.minValueEdit.Text);
                XElement maxEl = new XElement("Max", window.maxValueEdit.Text);
                XElement indexEl = new XElement("Index", window.variableComboBox.SelectedIndex.ToString());
                scrollWindowEl.Add(minEl);
                scrollWindowEl.Add(maxEl);
                scrollWindowEl.Add(indexEl);

                root.Add(scrollWindowEl);
            }
        }

        void LoadScrollBarWindows(XElement root, List<ScrollBarEditWindow> scrollBarWindows, MainVariablesList variablesList, MainWindow mainWindow)
        {
            IEnumerable<XElement> forms = root.Elements();
            for (int i = 0; i < forms.Count(); i++)
            {
                ScrollBarEditWindow window = new ScrollBarEditWindow(mainWindow.variablesForm.valuesUpdater);
                WindowPosition vvPos = new WindowPosition();
                vvPos.LoadFromXml(forms.ElementAt(i));
                

                window.VariablesList = variablesList;
                //window.SetMax(Int32.Parse(forms.ElementAt(i).Element("Max").Value));
                //window.SetMin(Int32.Parse(forms.ElementAt(i).Element("Min").Value));
                window.UpdateIndex(Int32.Parse(forms.ElementAt(i).Element("Index").Value));
                
                window.MdiParent = mainWindow;
                window.Show();
                vvPos.UpdateForm(window);

                scrollBarWindows.Add(window);
            }   
        }

        void LoadVariables(MainVariablesList mainVariablesList, XElement root)
        {
            mainVariablesList.variables.Clear();
            XElement mvEl = root.Element("AllVariables");

            IEnumerable<XElement> names = mvEl.Elements();
            for (int i = 0; i < names.Count(); i++)
            {                
                String value = names.ElementAt(i).Attribute("Name").Value;
                mainVariablesList.UpdateVariable(value, null, value);
            }      
        }

        void RestoreElement(MainVariablesList mainVariablesList, XElement variableElement)
        {
            String name = variableElement.Attribute("Name").Value;

            XElement childElem = variableElement.Element("Children");
            if (childElem == null)
            {
                mainVariablesList.UpdateVariable(name, null, name);
            }
            else
            {
                for(int i = 0; i < childElem.Elements().Count(); i++)
                {
                    //childElem.Elements()
                }
            }
        }

        void SaveVariableNames(ElfVariableList variables, XElement root)
        {
            XElement allVariablesElement = new XElement("AllVariables");
            foreach(ElfVariable variable in variables)
            {
                variable.WriteToXml(allVariablesElement);
            }
            root.Add(allVariablesElement);
        }        
    }
}
