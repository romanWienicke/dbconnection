using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Data;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace ConneXion.Data
{
    public static class ExtensionMethods
    {
        public static XDocument AsXDocument(this DataTable table)
        {
            XElement root = new XElement("root");

            int colCount = table.Columns.Count;

            string[] columns = new string[colCount];

            for (int i = 0; i < colCount; i++)
            {
                columns[i] = Regex.Replace(table.Columns[i].ColumnName, @"\W", "_");
            }

            foreach (DataRow row in table.Rows)
            {
                XElement xRow = new XElement("row");
                for (int i = 0; i < colCount; i++)
                {

                    xRow.Add(new XElement(columns[i], new XAttribute("value", row[i])));

                }
                root.Add(xRow);
            }
            return new XDocument(root);
        }
    }
}
