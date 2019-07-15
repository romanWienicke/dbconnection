using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ConneXion.Data.Import
{
    public class DataImportedEventArgs : EventArgs
    {
        public DataTable DtData { get; set; }
        public DataTable DtResult { get; set; }

        public DataImportedEventArgs()
        { }

        public DataImportedEventArgs(DataTable dtData)
        {
            DtData = dtData;
        }
    }
}
