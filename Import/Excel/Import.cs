using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Data.OleDb;

namespace ConneXion.Data.Import.Excel
{

    /// <summary>
    /// Import Klasse
    /// </summary>
    public class Import : ExcelBase
    {
        #region Static query procedures
        /// <summary>
        /// Imports the first worksheet of the specified file
        /// </summary>
        /// <param name="File"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "0#")]
        public static DataTable Query(string File)
        {
            return Query(File, null);
        }
        /// <summary>
        /// Imports the specified sheet in the specified file
        /// </summary>
        /// <param name="File"></param>
        /// <param name="Range">The worksheet or excel range to query</param>
        /// <returns></returns>
        public static DataTable Query(string File, string Range)
        {
            return new Import(File, Range).Query();
        }

        /// <summary>
        /// Daten Auswählen
        /// </summary>
        /// <param name="File">Dateiname</param>
        /// <param name="Sql">Sql</param>
        /// <returns>Datatable</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "0#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "1#")]
        public static DataTable Select(string File, string Sql)
        {
            Import i = new Import(File);
            i.SQL = Sql;
            return i.Query();
        }
        #endregion


        #region Constructors

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Import() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="WorkBook">Arbeitsblatt</param>
        public Import(string WorkBook) : base(WorkBook) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="WorkBook">Arbeitsblatt</param>
        /// <param name="Range">bereich</param>
        public Import(string WorkBook, string Range)
            : this(WorkBook)
        {
            this.Range = Range;
        }
        #endregion


        #region SQL Query
        private string fields = "*";
        /// <summary>
        /// The fields which should be returned (default all fields with data: "*")
        /// </summary>
        [DefaultValue("*")]
        public string Fields
        {
            get { return fields; }
            set { fields = value; }
        }
        void ResetFields()
        {
            fields = "*";
        }
        private string where;
        /// <summary>
        /// An optional where clause. Works pretty much the same as 'normal' SQL. (Default=null)
        /// </summary>
        [DefaultValue(null)]
        public string Where
        {
            get { return where; }
            set { where = value; }
        }
        /// <summary>
        /// The sql to perform. If this value is filled
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        public string SQL;

        /// <summary>
        /// SQL Abfrage hoelen
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected string GetSelectSQL()
        {
            if (SQL != null) return SQL;
            // if no sql was provided, construct from worksheet and where
            string sql = string.Format("select {0} from {1}", fields, GetRange());
            if (where != null)
                sql += " WHERE " + where;
            return sql;
        }
        /// <summary>
        /// Performs the query with the specifed settings
        /// </summary>
        /// <returns></returns>
        public DataTable Query()
        {
            return Query((DataTable)null);
        }
        /// <summary>
        /// Same as <see cref="Query()"/>, but an existing datatable is used and filled
        /// (it will be your own responsibility to format the datatable correctly)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1306:SetLocaleForDataTypes")]
        public DataTable Query(DataTable dt)
        {
            CheckWorkbook();
            try
            {
                OpenConnection(true);
                if (dt == null)
                    dt = new DataTable();
                new OleDbDataAdapter(GetSelectSQL(), Connection).Fill(dt);
                return dt;
            }
            finally
            {
                CloseConnection(true);
            }
        }
        /// <summary>
        /// Fills the datatable with the results of the query
        /// (wrapper around <see cref="Query(DataTable)"/>)
        /// </summary>
        /// <param name="dt"></param>
        public void Fill(DataTable dt)
        {
            Query(dt);
        }
        #endregion




    }
}
