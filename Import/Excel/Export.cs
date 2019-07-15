using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace ConneXion.Data.Import.Excel
{
    /// <summary>
    /// Excel Export Klasse
    /// </summary>
    public class Export
    {
        StreamWriter writer;
        private int sheetCount;
        
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Ziel Dateiname</param>
        private Export(string fileName)
        {
            writer = new StreamWriter(fileName);
        }
        
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dataTable">Daten</param>
        /// <param name="fileName">Ziel Dateiname</param>
        public Export(DataTable dataTable, string fileName) : this(fileName)
        {
            writeStart();
            writeTable(dataTable);
            writeEnd();
            writer.Close();
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dataSet">Dataset</param>
        /// <param name="fileName">Ziel Dateiname</param>
        public Export(DataSet dataSet, string fileName) : this(fileName)
        {
            writeStart();
            foreach(DataTable dataTable in dataSet.Tables)
                writeTable(dataTable);
            writeEnd();
            writer.Close();
        }

        #region Constant Strings
        private void writeStart()
        { 
            writer.Write("<xml version>\r\n" +
                "<Workbook " +
                "xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" +
                "xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n " +
                "xmlns:x=\"urn:schemas-    microsoft-com:office:" +
                "excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:" +
                "office:spreadsheet\">\r\n <Styles>\r\n " +
                "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n " +
                "<Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>" +
                "\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>" +
                "\r\n <Protection/>\r\n </Style>\r\n " +
                "<Style ss:ID=\"BoldColumn\">\r\n <Font " +
                "x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n </Style>\r\n " +
                "<Style     ss:ID=\"StringLiteral\">\r\n <NumberFormat" +
                " ss:Format=\"@\"/>\r\n </Style>\r\n <Style " +
                "ss:ID=\"Decimal\">\r\n <NumberFormat " +
                "ss:Format=\"0.0000\"/>\r\n </Style>\r\n " +
                "<Style ss:ID=\"Integer\">\r\n <NumberFormat " +
                "ss:Format=\"0\"/>\r\n </Style>\r\n <Style " +
                "ss:ID=\"DateLiteral\">\r\n <NumberFormat " +
                "ss:Format=\"mm/dd/yyyy;@\"/>\r\n </Style>\r\n " +
                "</Styles>\r\n ");
        }

        private void writeEnd()
        {
            writer.Write("</Workbook>");
        }
        #endregion


        private void writeTable(DataTable dataTable)
        {
            if (string.IsNullOrEmpty(dataTable.TableName))
                writer.Write("<Worksheet ss:Name=\"Sheet" + ++sheetCount + "\">");
            else
                writer.Write("<Worksheet ss:Name=\"" + dataTable.TableName + "\">");

            writer.Write("<Table>");
            writer.Write("<Row>");
            
            foreach(DataColumn col in dataTable.Columns)
            {
                writer.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
                writer.Write(col.ColumnName);
                writer.Write("</Data></Cell>");
            }
            writer.Write("</Row>");
        

            foreach(DataRow row in dataTable.Rows)
            {
                string data = string.Empty;
                writer.Write("<Row>"); 
 
                for(int i = 0; i < dataTable.Columns.Count; i++)
                {
                    switch (row[i].GetType().ToString())
                    {
                        //case "System.String":
                        //case "System.Boolean":
                        //case "System.DBNull":
                        //    data = row[i].ToString().Trim();
                        //    writer.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                        //                    "<Data ss:Type=\"String\">");
                        //    break;

                        //case "System.DateTime":
                        //    data = ((DateTime)row[i]).ToString("yyyy-MM-dd THH:mm:ss:lll");
                        //    writer.Write("<Cell ss:StyleID=\"DateLiteral\">" +
                        //                 "<Data ss:Type=\"DateTime\">");
                        //    break;

                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            data = row[i].ToString();
                            writer.Write("<Cell ss:StyleID=\"Integer\">" +
                                    "<Data ss:Type=\"Number\">");
                            break;

                        case "System.Decimal":
                        case "System.Double":
                            data = row[i].ToString();
                            writer.Write("<Cell ss:StyleID=\"Decimal\">" +
                                  "<Data ss:Type=\"Number\">");
                            break;

                        default:
                            data = row[i].ToString().Trim();
                            writer.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                            "<Data ss:Type=\"String\">");
                            break;
                            //throw (new Exception(row[i].GetType().ToString() + " not handled."));
                    }
                    writer.Write(data);
                    writer.Write("</Data></Cell>");
                    
                }
                writer.Write("</Row>");
            }
            writer.Write("</Table>");
            writer.Write(" </Worksheet>");        
        }
    }
}
