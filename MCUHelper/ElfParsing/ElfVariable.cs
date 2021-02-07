using AutosarGuiEditor.Source.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MCUHelper.ElfParsing
{
    public enum FieldDataType
    {
        fdtChar,
        fdtUnsignedChar,
        fdtShort,
        fdtUnsignedShort,
        fdtInteger,
        fdtUnsignedInteger,
        fdtLong,
        fdtUnsignedLong,
        fdtFloat,
        dftEnum,
        dftPointer,
        dftArray,
        dftStruct,
        dftUnknown
    }

    public class ElfVariable
    {
        public ElfObjectWithChildrens Parent;

        public String Name;
        public String Address;
        protected int length = 0;
        public int Length
        {
            get
            {
                return length;
            }
        }

        protected FieldDataType datatype;
        public FieldDataType DataType
        {
            get
            {
                return datatype;
            }
        }


        public virtual void UpdateValue(byte[] value)
        {

        }

        public virtual object GetValue()
        {
            return null;
        }

        public virtual String GetStrValue()
        {
            return null;
        }

        public virtual int GetBytes(String val)
        {
            return 0;
        }

        public String FullName
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.FullName + "." + Name;
                }
                else
                {
                    return Name;
                }
            }
        }

        public virtual void WriteToXml(XElement root, Boolean addToRoot = false)
        {
            XElement variableEl = new XElement("Variable");
            root.Add(variableEl);
            variableEl.Add(new XElement("Name", Name));
        }

        public virtual void LoadFromXml(XElement xml)
        {
            Name = XmlUtilits.GetFieldValue(xml, "Name", "ERROR");
            String datatypeStr = XmlUtilits.GetFieldValue(xml, "DataType", "ERROR");
            datatype = (FieldDataType)Enum.Parse(typeof(FieldDataType), datatypeStr);
        }

        /* Returns the root node of element */
        public virtual XElement WriteToXml(XElement root)
        {
            XElement varElement = new XElement("Variable");
            XAttribute nameAttrib = new XAttribute("Name", Name);
            varElement.Add(nameAttrib);
            root.Add(varElement);
            return varElement;
        }

        
    }

    public class ElfVariableList : List<ElfVariable>
    {
        public void WriteToXml(XElement root)
        {
            XElement dtList = new XElement("Fields");
            foreach (ElfVariable obj in this)
            {
                obj.WriteToXml(dtList);
            }
            root.Add(dtList);
        }

        public virtual void LoadFromXML(XElement xml, String NameId = "")
        {
            XElement xmlList = xml.Element("Fields");
            if (xmlList != null)
            {
                IEnumerable<XElement> elementsList = xmlList.Elements();
                if (elementsList != null)
                {
                    foreach (var element in elementsList)
                    {
                        ElfVariable newObj = new ElfVariable();
                        newObj.LoadFromXml(element);
                        base.Add(newObj);
                    }
                }
            }
        }
    }

    public class ElfChar : ElfVariable
    {
        public ElfChar()
        {
            base.datatype = FieldDataType.fdtChar;
            base.length = 1;
        }

        byte _value;
        public override void UpdateValue(byte[] value)
        {
            _value = Convert.ToByte(value[0]);
        }

        public override object GetValue()
        {
            return _value;
        }

        public override String GetStrValue()
        {
            return _value.ToString();
        }

        public override int GetBytes(String val)
        {
            val = val.Replace(".", ",");
            byte outVal = 0;
            byte.TryParse(val, out outVal);

            return outVal;
        }
    }

    public class ElfUnsignedChar : ElfVariable
    {
        public ElfUnsignedChar()
        {
            base.datatype = FieldDataType.fdtUnsignedChar;
            base.length = 1;
        }

        byte _value;
        public override void UpdateValue(byte[] value)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(value);

            _value = value[0];
        }

        public override object GetValue()
        {
            return _value;
        }

        public override String GetStrValue()
        {
            return _value.ToString();
        }

        public override int GetBytes(String val)
        {
            val = val.Replace(".", ",");
            byte outVal = 0;
            byte.TryParse(val, out outVal);

            return outVal;
        }
    }

    public class ElfShort : ElfVariable
    {
        public ElfShort()
        {
            base.datatype = FieldDataType.fdtShort;
            base.length = 2;
        }

        Int16 _value;
        public override void UpdateValue(byte[] value)
        {
            Int16 val = BitConverter.ToInt16(value, 0);
            _value = val;
        }

        public override object GetValue()
        {
            return _value;
        }

        public override String GetStrValue()
        {
            return _value.ToString();
        }

        public override int GetBytes(String val)
        {
            val = val.Replace(".", ",");
            Int16 outVal = 0;
            Int16.TryParse(val, out outVal);

            return outVal;
        }
    }

    public class ElfUnsignedShort : ElfVariable
    {
        public ElfUnsignedShort()
        {
            base.datatype = FieldDataType.fdtUnsignedShort;
            base.length = 2;
        }

        UInt16 _value;
        public override void UpdateValue(byte[] value)
        {
            var val = Convert.ToUInt16(value[1] * 256 + value[0]);
            _value = val;
        }

        public override object GetValue()
        {
            return _value;
        }

        public override String GetStrValue()
        {
            return _value.ToString();
        }

        public override int GetBytes(String val)
        {
            val = val.Replace(".", ",");
            UInt16 outVal = 0;
            UInt16.TryParse(val, out outVal);

            return outVal;
        }
    }

    public class ElfInt : ElfVariable
    {
        public ElfInt()
        {
            base.datatype = FieldDataType.fdtInteger;
            base.length = 4;
        }

        Int32 _value;
        public override void UpdateValue(byte[] value)
        {
            var val = Convert.ToInt32((value[3] << 24) + (value[2] << 16) + (value[1] << 8) + value[0]);
            _value = val;
        }

        public override object GetValue()
        {
            return _value;
        }

        public override String GetStrValue()
        {
            return _value.ToString();
        }

        public override int GetBytes(String val)
        {
            val = val.Replace(".", ",");
            int outVal = 0;
            int.TryParse(val, out outVal);

            return outVal;
        }
    }

    public class ElfUnsignedInt : ElfVariable
    {
        public ElfUnsignedInt()
        {
            base.datatype = FieldDataType.fdtUnsignedInteger;
            base.length = 4;
        }

        UInt32 _value;
        public override void UpdateValue(byte[] value)
        {
            var val = Convert.ToUInt32((value[3] << 24) + (value[2] << 16) + (value[1] << 8) + value[0]);
            _value = val;
        }

        public override object GetValue()
        {
            return _value;
        }

        public override String GetStrValue()
        {
            return _value.ToString();
        }

        public override int GetBytes(String val)
        {
            val = val.Replace(".", ",");
            UInt32 outVal = 0;
            UInt32.TryParse(val, out outVal);

            return unchecked((int)outVal);
        }
    }

    public class ElfLong : ElfVariable
    {
        public ElfLong()
        {
            base.datatype = FieldDataType.fdtLong;
            base.length = 8;
        }

        Int32 _value;
        public override void UpdateValue(byte[] value)
        {
            var val = Convert.ToInt32((value[3] << 24) + (value[2] << 16) + (value[1] << 8) + value[0]);
            _value = val;
        }

        public override object GetValue()
        {
            return _value;
        }

        public override String GetStrValue()
        {
            return _value.ToString();
        }

        public override int GetBytes(String val)
        {
            val = val.Replace(".", ",");
            Int32 outVal = 0;
            Int32.TryParse(val, out outVal);

            return outVal;
        }
    }

    public class ElfUnsignedLong : ElfVariable
    {
        public ElfUnsignedLong()
        {
            base.datatype = FieldDataType.fdtUnsignedLong;
            base.length = 8;
        }

        UInt32 _value;
        public override void UpdateValue(byte[] value)
        {
            var val = Convert.ToUInt32((value[3] *8 *8 *8) | (value[2] *8*8) | (value[1] *8* 8) | value[0]);
            _value = val;
        }

        public override object GetValue()
        {
            return _value;
        }

        public override String GetStrValue()
        {
            return _value.ToString();
        }

        public override int GetBytes(String val)
        {
            val = val.Replace(".", ",");
            UInt32 outVal = 0;
            UInt32.TryParse(val, out outVal);

            return unchecked((int)outVal);
        }
    }

    public class ElfFloat : ElfVariable
    {
        public ElfFloat()
        {
            base.datatype = FieldDataType.fdtFloat;
            base.length = 4;
        }

        float _value;
        public override void UpdateValue(byte[] value)
        {

            var val = System.BitConverter.ToSingle(value, 0); 
            _value = val;
        }

        public override object GetValue()
        {
            return _value;
        }

        public override String GetStrValue()
        {
            string addStr = "";
            if (_value >= 0)
            {
                addStr = " ";
            }
            return addStr + _value.ToString().Replace(",", ".");
        }

        public override int GetBytes(String val)
        {
            val = val.Replace(".", ",");
            float fValue;
            if (float.TryParse(val, out fValue))
            {
                byte[] vOut = BitConverter.GetBytes(fValue);
                return BitConverter.ToInt32(vOut, 0);
            }
            return 0;
        }
    }

    public class ElfPointer : ElfVariable
    {
        public ElfPointer()
        {
            base.datatype = FieldDataType.dftPointer;
            base.length = 4;
        }

        uint _value;
        public override void UpdateValue(byte[] value)
        {
            var val = Convert.ToUInt32((value[3] << 24) | (value[2] << 16) | (value[1] << 8) | value[0]);
            _value = val;
        }

        public override object GetValue()
        {
            return _value;
        }

        public override String GetStrValue()
        {
            return "0x" + _value.ToString("X");
        }

        public override int GetBytes(String val)
        {
            val = val.Replace(".", ",");
            int outVal = 0;
            int.TryParse(val, out outVal);

            return outVal;
        }
    }


    public class ElfObjectWithChildrens : ElfVariable
    {
        public ElfVariableList Childrens = new ElfVariableList();

        public override void WriteToXml(XElement root, Boolean addToRoot = false)
        {
            XElement objWithChildren = new XElement("Variable");
            root.Add(objWithChildren);
            base.WriteToXml(objWithChildren, true);

            XElement childrenXml = new XElement("Fields");
            foreach (ElfVariable var in Childrens)
            {
                var.WriteToXml(childrenXml);
            }
            objWithChildren.Add(childrenXml);
        }

        public override void LoadFromXml(XElement xml)
        {
            base.LoadFromXml(xml);
            Name = XmlUtilits.GetFieldValue(xml, "Name", "ERROR");
            String datatypeStr = XmlUtilits.GetFieldValue(xml, "DataType", "ERROR");
            datatype = (FieldDataType)Enum.Parse(typeof(FieldDataType), datatypeStr);
        }

        public override XElement WriteToXml(XElement root)
        {
            XElement elementNode = base.WriteToXml(root);
           // XAttribute attr = new XAttribute("hasChildren", true);

            XElement childrenNode = new XElement("Children");
            elementNode.Add(childrenNode);
            foreach(ElfVariable elfVar in this.Childrens)
            {
                elfVar.WriteToXml(childrenNode);
            }

            return elementNode;
        }
    }

    public class ElfStruct : ElfObjectWithChildrens
    {
        public ElfStruct()
        {
            base.datatype = FieldDataType.dftStruct;
            base.length = 0;
        }
    }

    public class ElfArray : ElfObjectWithChildrens
    {
        public ElfArray()
        {
            base.datatype = FieldDataType.dftArray;
            base.length = 0;
        }
    }


    public class ElfEnum : ElfVariable
    {
        public ElfEnum()
        {
            base.datatype = FieldDataType.dftEnum;
            base.length = 1;
        }

        public List<String> Values = new List<string>();
        public int Value;

        int _value;
        public override void UpdateValue(byte[] value)
        {
            var val = (int)(value[0]);
            _value = val;
        }

        public override object GetValue()
        {
            return _value.ToString();
        }

        public override String GetStrValue()
        {
            if (_value < Values.Count)
            {
                return Values[_value];
            }
            return _value.ToString();
        }

        public Byte GetIntValue(String value)
        {
            for (int i = 0; i < Values.Count; i++)
            {
                if (Values[i].Equals(value))
                {
                    return (Byte)(i & 0xFF);
                }
            }
            byte val;
            if (byte.TryParse(value, out val))
            {
                return val;
            }
            return 0;
        }
    }

    public static class ElfVariableFabric
    {
        public static ElfPointer CreatePointer(String Name, String address)
        {
            ElfPointer pointer = new ElfPointer();
            pointer.Name = Name;
            pointer.Address = address;
            return pointer;
        }

        public static ElfChar CreateChar(String Name, String address)
        {
            ElfChar variable = new ElfChar();
            variable.Name = Name;
            variable.Address = address;
            return variable;
        }

        public static ElfUnsignedChar CreateUnsignedChar(String Name, String address)
        {
            ElfUnsignedChar variable = new ElfUnsignedChar();
            variable.Name = Name;
            variable.Address = address;            
            return variable;
        }

        public static ElfShort CreateShort(String Name, String address)
        {
            ElfShort variable = new ElfShort();
            variable.Name = Name;
            variable.Address = address;            
            return variable;
        }

        public static ElfUnsignedShort CreateUnsignedShort(String Name, String address)
        {
            ElfUnsignedShort variable = new ElfUnsignedShort();
            variable.Name = Name;
            variable.Address = address;            
            return variable;
        }

        public static ElfInt CreateInt(String Name, String address)
        {
            ElfInt variable = new ElfInt();
            variable.Name = Name;
            variable.Address = address;
            return variable;
        }

        public static ElfUnsignedInt CreateUnsignedInt(String Name, String address)
        {
            ElfUnsignedInt variable = new ElfUnsignedInt();
            variable.Name = Name;
            variable.Address = address;            
            return variable;
        }

        public static ElfLong CreateLong(String Name, String address)
        {
            ElfLong variable = new ElfLong();
            variable.Name = Name;
            variable.Address = address;            
            return variable;
        }

        public static ElfUnsignedLong CreateUnsignedLong(String Name, String address)
        {
            ElfUnsignedLong variable = new ElfUnsignedLong();
            variable.Name = Name;
            variable.Address = address;
            return variable;
        }

        public static ElfEnum CreateEnum(String Name, String address)
        {
            ElfEnum variable = new ElfEnum();
            variable.Name = Name;
            variable.Address = address;
            return variable;
        }

        public static ElfFloat CreateFloat(String Name, String address)
        {
            ElfFloat variable = new ElfFloat();
            variable.Name = Name;
            variable.Address = address;
            return variable;
        }

        public static ElfStruct CreateStruct(String Name)
        {
            ElfStruct variable = new ElfStruct();
            variable.Name = Name;
            variable.Address = "";
            return variable;
        }

        public static ElfArray CreateArray(String Name)
        {
            ElfArray variable = new ElfArray();
            variable.Name = Name;
            variable.Address = "";
            return variable;
        }

        public static ElfVariable Create(FieldDataType datatype, String Name, String address)
        {
            switch (datatype)
            {
                case FieldDataType.dftEnum:
                {
                    return CreateEnum(Name, address);
                }
                case FieldDataType.dftPointer:
                {
                    return CreatePointer(Name, address);
                }
                case FieldDataType.dftStruct:
                {
                    return CreateStruct(Name);
                }
                case FieldDataType.dftUnknown:
                {
                    return null;
                }
                case FieldDataType.fdtChar:
                {
                    return CreateChar(Name, address);
                }
                case FieldDataType.fdtFloat:
                {
                    return CreateFloat(Name, address);
                }
                case FieldDataType.fdtInteger:
                {
                    return CreateInt(Name, address);
                }
                case FieldDataType.fdtLong:
                {
                    return CreateLong(Name, address);
                }
                case FieldDataType.fdtShort:
                {
                    return CreateShort(Name, address);
                }
                case FieldDataType.fdtUnsignedChar:
                {
                    return CreateUnsignedChar(Name, address);
                }
                case FieldDataType.fdtUnsignedInteger:
                {
                    return CreateUnsignedInt(Name, address);
                }
                case FieldDataType.fdtUnsignedLong:
                {
                    return CreateUnsignedLong(Name, address);
                }
                case FieldDataType.fdtUnsignedShort:
                {
                    return CreateUnsignedShort(Name, address);
                }
                case FieldDataType.dftArray:
                {
                    return CreateArray(Name);
                }
                default :
                {
                    return null;
                }
            }
        }
    }
}
