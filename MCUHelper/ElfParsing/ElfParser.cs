using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCUHelper.ElfParsing
{
    public class ElfParser
    {
        Process p;
        StreamWriter writer;
        String fileName;
        System.Windows.Forms.Timer readTimer;

        Mutex readMutex = new Mutex();
        Semaphore readSemaphore = new Semaphore(0, 1);

        public ElfParser(String fileName)
        {
            readTimer = new System.Windows.Forms.Timer();
            readTimer.Enabled = false;
            //readTimer.Tick += readTimer_Tick;
            readTimer.Interval = 5;

            this.fileName = fileName;

            p = new Process();
            p.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + @"\Data\arm-none-eabi-gdb.exe";
            p.StartInfo.Arguments = "-q \"" + fileName + "\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            p.OutputDataReceived += p_OutputDataReceived;
            
            p.Start();

            writer = p.StandardInput;
            String welcomeMessage = ReadLines(5);
        }

        int prevMessagesCount = 0;
        int messagesCount = 0;
        int waitCount = 0;

        bool readTimer_Tick(object sender, EventArgs e)
        {
            if (messagesCount != 0)
            {
                if (prevMessagesCount == messagesCount)
                {
                    waitCount++;
                }
                else
                {
                    prevMessagesCount = messagesCount;
                }
            }

            if ((waitCount >= 3) && (messagesCount != 0))
            {
                //readSemaphore.Release();
                return true;
            }
            return false;
        }

        String receivedData = "";
        void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            messagesCount++;
            receivedData += e.Data;
        }


        ~ElfParser()
        {

        }

        String ReadLines(int waitPeriod)
        {
            waitCount = 0;
            prevMessagesCount = 0;
            messagesCount = 0;
            receivedData = "";
            readTimer.Enabled = true;
            p.BeginOutputReadLine();
            Boolean res = false;
            int ticks = 0;
            const int maxWait = 3000;
            do
            {
                
                res = readTimer_Tick(null, null);
                
                if (res != true)
                {
                    Thread.Sleep(waitPeriod);
                    ticks += waitPeriod;
                    if (ticks > maxWait)
                    {
                        p.CancelOutputRead();
                        return "";
                    }
                }
                else
                {
                    res = true;
                }
              //  String s = reader.ReadLine();
            } while (res != true);
            p.CancelOutputRead();
            
            return receivedData;
        }


        String SendCommand(String gdbCommand, int waitPeriod)
        {
            waitCount = 0;
            prevMessagesCount = 0;
            messagesCount = 0;
            receivedData = "";

            String ret = "";
            int repeats = 10;
            do
            {
                writer.WriteLine(gdbCommand);
                ret = ReadLines(waitPeriod);
            } while ((ret.Length == 0) && (repeats > 0));

            /* Remove repeated parts */
            if (ret.Length % 2 == 0)
            {
                Boolean similar = true;
                int startIndex = ret.Length / 2;
                for (int i = 0; i <ret.Length / 2; i++)
                {
                    if (ret[i] != ret[i + ret.Length / 2])
                    {
                        similar = false;
                    }
                }
                if (similar)
                {
                    ret = ret.Substring(0, ret.Length / 2);
                }
            }
            return ret;
        }

        public ElfVariableList GetAllVariables()
        {
            ElfVariableList variables = new ElfVariableList();
            String data = SendCommand("info variables\r\n", 50);
            if (data.IndexOf(";") == -1)
            {
                data = ReadLines(50);
            }
            string[] lines = data.Split(';');
            for (int i = 1; i < lines.Length; i++)
            {
                String line = lines[i];
                line = line.Replace('\t', ' ');
                //if (line.IndexOf(';') > 0)
                {
                    string[] parts = line.Split(' ');
                    if (parts.Length >= 2)
                    {
                        ElfVariable variable = new ElfVariable();
                        variable.Name = parts[parts.Length - 1];
                        //variable.DataType = parts[parts.Length - 2];
                        variables.Add(variable);
                    }
                }
            }
            return variables;
        }

        public String GetVariableInfo(String variableName)
        {
            String answer = SendCommand("ptype " + variableName +"\r\n", 1);
            
            return answer;
        }

        public String GetVariableAdress(String variableName)
        {
            String varName = variableName;
           // if (variableName.IndexOf("[") > 0)
            {
            //    varName = varName.Remove(variableName.IndexOf("["));
            }
            String answer = SendCommand("print &" + varName, 0);
            string[] words = answer.Split(' ');
            String address = "";
            foreach (String word in words)
            {
                if (word.IndexOf("0x") == 0)
                {
                    address = word;
                    break;
                }
            }
            return address;
        }
    }
}
