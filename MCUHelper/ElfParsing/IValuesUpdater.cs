using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCUHelper.ElfParsing
{
    public interface IValuesUpdater
    {
        void SetVariablesList(MainVariablesList variables);
        void AddWriteCommand(ElfVariable var, String newValue);
        void StartUpdate();
        void StopUpdate();
        void FreeData();
    }
}
