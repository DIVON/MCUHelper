using HighResTimer;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MassConnector.ElfParsing
{
    class WriteCommand
    {
        int newValue;
        ElfVariable variable;

        public WriteCommand(ElfVariable variable, int newValue)
        {
            this.variable = variable;
            this.newValue = newValue;
        }

        public void Send(SerialPort serialPort)
        {
            byte[] data = new byte[12];
            data[0] = 0xA5;
            data[1] = 0xC4;

            int address = Convert.ToInt32(variable.Address, 16);
            data[2] = (byte)(address & 0xFF);
            data[3] = (byte)((address >> 8) & 0xFF);
            data[4] = (byte)((address >> 16) & 0xFF);
            data[5] = (byte)((address >> 24) & 0xFF);

            data[6] = (Byte)(variable.Length & 0xFF);

            switch (variable.Length)
            {
                case 1:
                {
                    data[7] = (Byte)(newValue & 0xFF);
                    data[8] = 0;
                    data[9] = 0;
                    data[10] = 0;
                    break;
                }
                case 2:
                {
                    data[7] = (Byte)(newValue & 0xFF);
                    data[8] = (Byte)((newValue >> 8) & 0xFF);
                    data[9] = 0;
                    data[10] = 0;
                    break;
                }
                case 4:
                {
                    data[7] = (Byte)(newValue & 0xFF);
                    data[8] = (Byte)((newValue >> 8) & 0xFF);
                    data[9] = (Byte)((newValue >> 16) & 0xFF);
                    data[10] = (Byte)((newValue >> 24) & 0xFF);
                    break;
                }
                default:
                {
                    break;
                }
            }

            data[11] = 0x0D;
            if (serialPort.IsOpen)
            {
                serialPort.Write(data, 0, 12);
            }
        }
    }

    public class SerialPortUpdater : IValuesUpdater
    {
        MainVariablesList variables;

        public void SetVariablesList(MainVariablesList variablesList)
        {
            this.variables = variablesList;
        }

        ~SerialPortUpdater()
        {
            getValueThread.Abort();
        }

        SerialPort serialPort;
        public SerialPort Port
        {
            get
            {
                return serialPort;
            }
        }

        Queue<WriteCommand> writeCommands = new Queue<WriteCommand>();

        void IValuesUpdater.AddWriteCommand(ElfVariable var, String newValue)
        {
            int value = 0;

            if (var is ElfEnum)
            {
                ElfEnum elfEnum = (ElfEnum)var;
                value = elfEnum.GetIntValue(newValue);
            }
            else if (var is ElfFloat)
            {
                ElfFloat elffloat = (ElfFloat)var;
                value = ElfFloat.getBytes(newValue);
            }

            WriteCommand command = new WriteCommand(var, value);
            writeCommands.Enqueue(command);
            //writeCommands.Enqueue(command);
            //writeCommands.Enqueue(command);
            //writeCommands.Enqueue(command);
            //writeCommands.Enqueue(command);
            //writeCommands.Enqueue(command);
        }

        Thread getValueThread;
        AutoResetEvent getDataEvent = new AutoResetEvent(false);
        List<byte> receivedData = new List<byte>();

        public SerialPortUpdater()
        {
            serialPort = new SerialPort();
            serialPort.BaudRate = 115200;
            serialPort.DataReceived += serialPort_DataReceived;

            getValueThread = new Thread(new ThreadStart(UpdateValuesHandler));
            getValueThread.Priority = ThreadPriority.Highest;
            getValueThread.Start();

            _event.Reset();
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buf = new byte[serialPort.BytesToRead];

            serialPort.Read(buf, 0, buf.Length);
 
            receivedData.AddRange(buf);

            Boolean variableUpdated = UpdateVariableValue(receivedData);

            if (variableUpdated)
            {
                receivedData.Clear();
                getDataEvent.Set();
            }
        }

        public Boolean UpdateVariableValue(List<byte> data)
        {
            /* */
            Boolean updated = false;

            int maxTried = data.Count - (MaxVariableCount * 8 + 3);
            for (int i = 0; i <= maxTried; i++)
            {
                if ((data[i] == 0xA5) && (data[i + 1] == 0xB3) && (data[i + rxCount - 1] == 0x0D))
                {
                    for (int fieldIndex = 0; fieldIndex < MaxVariableCount; fieldIndex++)
                    {
                        int address = (data[i + fieldIndex * 8 + 5] << 24) | (data[i + fieldIndex * 8 + 4] << 16) | (data[i + fieldIndex * 8 + 3] << 8) | data[i + fieldIndex * 8 + 2];

                        ElfVariableList fields = variables.GetAllFields();
                        foreach (ElfVariable field in fields)
                        {
                            int varAddress = Convert.ToInt32(field.Address, 16);
                            if (varAddress == address)
                            {
                                byte[] valData = { data[i + fieldIndex * 8 + 6], data[i + fieldIndex * 8 + 7], data[i + fieldIndex * 8 + 8], data[i + fieldIndex * 8 + 9] };
                                field.UpdateValue(valData);
                                updated = true;
                            }
                        }
                    }
                }
            }
            return updated;
        }

        ManualResetEvent _event = new ManualResetEvent(true); 
        void UpdateValuesHandler()
        {
            while (true)
            {
                _event.WaitOne();
                if (variables != null)
                {
                    if (serialPort.IsOpen)
                    {
                        if (writeCommands.Count != 0)
                        {
                            WriteCommand nextWriteCommand = writeCommands.Dequeue();
                            nextWriteCommand.Send(serialPort);
                            //Thread.Sleep(5);
                        }
                        else
                        {
                            SendReadCommand();
                            getDataEvent.WaitOne(50);
                        }
                    }
                }
            }
        }

        const int MaxVariableCount = 20;
        const int rxCount = 2 + 1 + MaxVariableCount * 8;

        int prevLastIndex = 0;

        void SendReadCommand()
        {
            byte[] data = new byte[MaxVariableCount * 4 + 3];
            data[0] = (byte)0xA5;
            data[1] = (byte)0xB3;

            ElfVariableList fields = variables.GetAllFields();
            if (fields.Count <= MaxVariableCount)
            {
                for (int i = 0; i < fields.Count; i++)
                {
                    int address = Convert.ToInt32(fields[i].Address, 16);
                    data[i * 4 + 2] = (byte)(address & 0xFF);
                    data[i * 4 + 2 + 1] = (byte)((address >> 8) & 0xFF);
                    data[i * 4 + 2 + 2] = (byte)((address >> 16) & 0xFF);
                    data[i * 4 + 2 + 3] = (byte)((address >> 24) & 0xFF);
                }
                for (int i = fields.Count; i < MaxVariableCount; i++)
                {
                    //int address = Convert.ToInt32(fields[i].Address, 16);
                    data[i * 4 + 2] = (byte)(0);
                    data[i * 4 + 2 + 1] = (byte)(0);
                    data[i * 4 + 2 + 2] = (byte)(0);
                    data[i * 4 + 2 + 3] = (byte)(0);
                }
            }
            else
            {
                if (prevLastIndex + MaxVariableCount <= fields.Count)
                {
                    for (int i = prevLastIndex; i < prevLastIndex + MaxVariableCount; i++)
                    {
                        int address = Convert.ToInt32(fields[i].Address, 16);
                        data[(i % MaxVariableCount) * 4 + 2] = (byte)(address & 0xFF);
                        data[(i % MaxVariableCount) * 4 + 2 + 1] = (byte)((address >> 8) & 0xFF);
                        data[(i % MaxVariableCount) * 4 + 2 + 2] = (byte)((address >> 16) & 0xFF);
                        data[(i % MaxVariableCount) * 4 + 2 + 3] = (byte)((address >> 24) & 0xFF);
                    }
                    prevLastIndex = prevLastIndex + MaxVariableCount;
                    if (prevLastIndex == fields.Count)
                    {
                        prevLastIndex = 0;
                    }
                }
                else
                {
                    for (int i = prevLastIndex; i < fields.Count; i++)
                    {
                        int address = Convert.ToInt32(fields[i].Address, 16);
                        data[(i % MaxVariableCount) * 4 + 2] = (byte)(address & 0xFF);
                        data[(i % MaxVariableCount) * 4 + 2 + 1] = (byte)((address >> 8) & 0xFF);
                        data[(i % MaxVariableCount) * 4 + 2 + 2] = (byte)((address >> 16) & 0xFF);
                        data[(i % MaxVariableCount) * 4 + 2 + 3] = (byte)((address >> 24) & 0xFF);
                    }

                    int count = fields.Count - prevLastIndex;
                    for (int i = 0; i < MaxVariableCount - count; i++)
                    {
                        int address = Convert.ToInt32(fields[i].Address, 16);
                        data[(i % MaxVariableCount) * 4 + 2] = (byte)(address & 0xFF);
                        data[(i % MaxVariableCount) * 4 + 2 + 1] = (byte)((address >> 8) & 0xFF);
                        data[(i % MaxVariableCount) * 4 + 2 + 2] = (byte)((address >> 16) & 0xFF);
                        data[(i % MaxVariableCount) * 4 + 2 + 3] = (byte)((address >> 24) & 0xFF);
                    }

                    prevLastIndex = MaxVariableCount - count;
                }
            }

            data[MaxVariableCount * 4 + 2] = (byte)0x0D;

            if (serialPort.IsOpen)
            {
                serialPort.Write(data, 0, MaxVariableCount * 4 + 3);
            }
        }

        void IValuesUpdater.StartUpdate()
        {
            serialPort.Open();
            _event.Set();
        }

        void IValuesUpdater.StopUpdate()
        {
            _event.Reset();
            serialPort.Close();
        }


        public void FreeData()
        {
            getValueThread.Abort();
        }
    }
}
