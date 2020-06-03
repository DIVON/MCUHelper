using MCUHelper.ElfParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MCUHelper
{
    public class MainVariablesList
    {
        ElfParser parser;
        public MainVariablesList (ElfParser parser)
        {
            this.parser = parser;
        }

        public void AddVariable(String variableName)
        {
            Boolean find = false;
            foreach (String var in addedVariables)
            {
                if (var.Equals(variableName))
                {
                    find = true;
                    break;
                }
            }
            if (find == false)
            {
                addedVariables.Add(variableName);                
                UpdateVariables();
            }
        }

        private Boolean isItPointer(String str, FieldDataType baseDataType)
        {
            if ((baseDataType != FieldDataType.dftStruct) && (baseDataType != FieldDataType.dftArray))
            {
                if (str.Contains("*"))
                {
                    return true;
                }
            }

            /* it is a structure or array*/
            else if (str.Contains("} *") )
            {
                return true;
            }

            return false;
        }

        private int isItAnArray(String str, FieldDataType baseDataType)
        {
            int len = -1;
            if (baseDataType != FieldDataType.dftStruct)
            {
                int openBraceIndex = str.IndexOf("[");
                if (openBraceIndex > 0)
                {
                    int closeBraceIndex = str.IndexOf("]", openBraceIndex);
                    if (closeBraceIndex > openBraceIndex)
                    {
                        String strLen = str.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1);
                        int parsedLed;
                        if (int.TryParse(strLen, out parsedLed) == true)
                        {
                            len = parsedLed;
                        }
                    }
                }
            }
            /* it is a structure */
            else if (str.Contains("} ["))
            {
                int openBraceIndex = str.IndexOf("} [");
                if (openBraceIndex > 0)
                {
                    int closeBraceIndex = str.IndexOf("]", openBraceIndex);
                    if (closeBraceIndex > openBraceIndex)
                    {
                        String strLen = str.Substring(openBraceIndex + 3, closeBraceIndex - openBraceIndex - 3);
                        int parsedLed;
                        if (int.TryParse(strLen, out parsedLed) == true)
                        {
                            len = parsedLed;
                        }
                    }
                }
            }

            return len;
        }        

        FieldDataType GetVariableDataType(String datatypeString)
        {
            String[] splitted = datatypeString.Split(' ');
            int index = 3;
            if (splitted[index] == "volatile")
            {
                index++;
            }
            /* Check if it is unsigned */
            if (splitted[index] == "unsigned")
            {
                index++;
                switch (splitted[index])
                {
                    case "char":
                    {
                        return FieldDataType.fdtUnsignedChar;
                    }
                    case "short":
                    {
                        return FieldDataType.fdtUnsignedShort;
                    }
                    case "int":
                    {
                        return FieldDataType.fdtUnsignedInteger;
                    }
                    case "long":
                    {
                        return FieldDataType.fdtUnsignedLong;
                    }
                    default:
                    {
                        return FieldDataType.dftUnknown;
                    }
                }
            }
            else 
            {
                switch (splitted[index])
                {
                    case "struct":
                    {
                        if (datatypeString.IndexOf("} *") != -1)
                        {
                            return FieldDataType.dftPointer;
                        }
                        else if (datatypeString.IndexOf("} [") != -1)
                        {
                            return FieldDataType.dftStruct;
                        }
                        else
                        {
                            return FieldDataType.dftStruct;
                        }
                    }
                    case "char":
                    {
                        return FieldDataType.fdtChar;
                    }
                    case "short":
                    {
                        return FieldDataType.fdtShort;
                    }
                    case "int":
                    {
                        return FieldDataType.fdtInteger;
                    }
                    case "long":
                    {
                        return FieldDataType.fdtLong;
                    }
                    case "enum":
                    {
                        return FieldDataType.dftEnum;
                    }
                    case "float":
                    {
                        return FieldDataType.fdtFloat;
                    }
                    default:
                    {
                        return FieldDataType.dftUnknown;
                    }
                }
            }
        }
        
        void UpdateEnumValues(String enumStr, ElfEnum elfEnum)
        {
            int openBracer = enumStr.IndexOf("{");
            int closeBracer = enumStr.IndexOf("}");
            if ((closeBracer > openBracer) && (openBracer > 0))
            {
                String values = enumStr.Substring(openBracer + 1, closeBracer - openBracer - 1);
                String[] enumValues = values.Trim().Split(',');
                foreach (String str in enumValues)
                {
                    elfEnum.Values.Add(str.Trim());
                }
            }
        }

        public ElfVariable UpdateVariable(String varName, ElfObjectWithChildrens parent, String gdbName)
        {
            /* Get Variable fields*/
            String varInfo = parser.GetVariableInfo(gdbName);

            if (varInfo.IndexOf("No symbol") != -1)
            {
                /* variable not found */
                return null;
            }

            FieldDataType datatype = GetVariableDataType(varInfo);
            Boolean isPointer = isItPointer(varInfo, datatype);

            if (isPointer)
            {
                string address = parser.GetVariableAdress(gdbName);
                ElfPointer pointer = ElfVariableFabric.CreatePointer(varName, address);
                if (parent == null)
                {
                    variables.Add(pointer);
                }
                else
                {
                    parent.Childrens.Add(pointer);
                }
            }
            else 
            {
                /* Is it an array? */
                int arrayLen = isItAnArray(varInfo, datatype);
                
               

                if (arrayLen == -1)
                {
                    /* Is it a structure? */
                    if (datatype == FieldDataType.dftStruct)
                    {
                        ElfStruct variableStruct = ElfVariableFabric.CreateStruct(varName);
                        variableStruct.Parent = parent;
                        int openBraceIndex = varInfo.IndexOf('{');
                        int closeBraceIndex = varInfo.IndexOf('}');
                        String fieldString = varInfo.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 2);
                        String[] fields = fieldString.Split(';');
                        foreach (String field in fields)
                        {
                            /* extract field name */
                            string[] data = field.Trim().Split(' ');
                            if (data.Length >= 2)
                            {
                                String name = data[data.Length - 1];
                                name = name.Replace('*', ' ').Trim();
                                int indexOfBrace = name.IndexOf("[");
                                if (indexOfBrace > 0)
                                {
                                    name = name.Remove(indexOfBrace);
                                }

                                UpdateVariable(name, variableStruct, gdbName + "." + name);
                            }

                        }
                        if (parent == null)
                        {
                            variables.Add(variableStruct);
                        }
                        else
                        {
                            parent.Childrens.Add(variableStruct);
                        }
                    }
                    else if (datatype != FieldDataType.dftUnknown)
                    {
                        /* Get address of variable */
                        String address = parser.GetVariableAdress(gdbName);

                        ElfVariable variable = ElfVariableFabric.Create(datatype, varName, address);
                        if (parent != null)
                        {
                            if (variable != null)
                            {
                                parent.Childrens.Add(variable);
                                variable.Parent = parent;                                
                            }
                        }
                        else
                        {
                            variables.Add(variable);

                        }

                        if (datatype == FieldDataType.dftEnum)
                        {
                            if (variable is ElfEnum)
                            {
                                UpdateEnumValues(varInfo, (ElfEnum)variable);
                            }
                        }
                    }
                }
                else
                {
                    ElfArray newArray = ElfVariableFabric.CreateArray(varName);
                    newArray.Parent = parent;
                    if (parent == null)
                    {
                        variables.Add(newArray);
                    }
                    else
                    {
                        parent.Childrens.Add(newArray);
                    }

                    for (int i = 0; i < arrayLen; i++)
                    {
                        String elemName = varName + "[" + i + "]";
                        UpdateVariable(elemName, newArray, gdbName + "[" + i + "]");
                    }
                }
            }
            return null;
        }

        public void UpdateVariables()
        {
            foreach(String var in addedVariables)
            {
                UpdateVariable(var, null, var);
            }
        }

       



        public ElfVariable GetVariable(String variableName)
        {
            return null;
        }

        List<String> addedVariables = new List<string>();

        public ElfVariableList variables = new ElfVariableList();


        public void GetFields(ElfVariableList varList, ElfVariable variable)
        {
            if (variable is ElfObjectWithChildrens)
            {
                ElfObjectWithChildrens elfStruct = (ElfObjectWithChildrens)variable;
                foreach(ElfVariable child in elfStruct.Childrens)
                {
                    GetFields(varList, child);
                }
            }
            else
            {
                varList.Add(variable);
            }
        }


        public ElfVariableList GetAllFields()
        {
            ElfVariableList list = new ElfVariableList();
            for (int i = 0; i < variables.Count; i++)
            {
                GetFields(list, variables[i]);
            }
            return list;
        }

        public void WriteToXml(XElement root)
        {
            XElement variablesEl = new XElement("Variables");
            root.Add(variablesEl);
            variables.WriteToXml(variablesEl);
        }

        public void LoadFromXml(XElement xml)
        {

        }

        ElfVariable getVariableByIndex(ElfVariable variable, int currentIndex, int desiredIndex, out int newIndex)
        {
            if (currentIndex == desiredIndex)
            {
                newIndex = -1;
                return variable;
            }
            else
            {
                if (variable is ElfObjectWithChildrens)
                {
                    currentIndex++;
                    ElfVariable gotVar;
                    ElfObjectWithChildrens elfStruct = (ElfObjectWithChildrens)variable;
                    int nextIndex = currentIndex;
                    foreach (ElfVariable child in elfStruct.Childrens)
                    {
                        gotVar = getVariableByIndex(child, currentIndex, desiredIndex, out nextIndex);
                        
                        if (gotVar != null)
                        {
                            newIndex = -1;
                            return gotVar;
                        }
                        else
                        {
                            currentIndex = nextIndex;
                        }
                    }
                    newIndex = nextIndex;
                    return null;
                }
                else
                {
                    newIndex = currentIndex + 1;
                    return null;
                }
            }
            
        }

        public ElfVariable GetVariableByIndex(int desiredIndex)
        {
            int startIndex = 0;
            ElfVariable variable = null;
            for (int i = 0; i < variables.Count; i++)
            {
                variable = getVariableByIndex(variables[i], startIndex, desiredIndex, out startIndex);
                if (variable != null)
                {
                    return variable;
                }
            }
            return variable;
        }
    }
}
