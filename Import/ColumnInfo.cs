using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConneXion.Data.Import
{
    public class ColumnInfo
    {
        public string ColumnName { get; set; }
        public string DisplayName { get; set; }
        public string SelectedName { get; set; }
        public List<SelectionItem> Selections{ get; set; }
        
        public ColumnInfo()
        { }

        public ColumnInfo(string columnName, string displayName, string selectedName) : this(columnName, displayName)
        {
            SelectedName = selectedName;
        }

        public ColumnInfo(string columnName, string displayName)
        {
            ColumnName = columnName;
            DisplayName = displayName;
        }

        public ColumnInfo(string columnName, string displayName, List<SelectionItem> selections)
            :this(columnName, displayName)
        {
            Selections = selections;
        }
    }

    public struct SelectionItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }



        public SelectionItem(string name, string value, bool selected): this()
        {
            Name = name;
            Value = value;
            Selected = selected;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
