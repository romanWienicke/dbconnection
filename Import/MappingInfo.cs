using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConneXion.Data.Import
{
    public class MappingInfo
    {
        public string DisplayColumnName { get; set; }
        public string[] SourceColumns { get; set; }
        public string MappedColumnName { get; set; }
        public string ColumnName { get; set; }


        public MappingInfo()
        {

        }

        public MappingInfo(string displayColumnName, string mappedColumnName)
        {
            DisplayColumnName = displayColumnName;
            MappedColumnName = mappedColumnName;
        }

        public MappingInfo(string displayColumnName, string[] sourceColumns)
        {
            DisplayColumnName = displayColumnName;
            SourceColumns = sourceColumns;
        }

        public MappingInfo(string displayColumnName, string columnName, string[] sourceColumns)
        {
            DisplayColumnName = displayColumnName;
            SourceColumns = sourceColumns;
            ColumnName = columnName;
        }
    }
}
