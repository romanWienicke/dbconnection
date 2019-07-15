using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.ComponentModel;




namespace ConneXion.Data.Import.Excel
{
    /// <summary>
    /// Excel Basisklasse
    /// The classes in this file use oledb to query excel worksheets and thus can be
    /// used without the interop assemblies.
    /// In other words, you can simply copy , paste and use :D
    /// </summary>
    public class ExcelBase : Component, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Konstruktior
        /// </summary>
        public ExcelBase()
        {
            UseFinalizer = false;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="WorkBook">Name des Arbeitsblatts</param>
        public ExcelBase(string WorkBook)
            : this()
        {
            this.WorkBook = WorkBook;
        }
        #endregion
        #region Workbook/range settings
        string workbook;
        /// <summary>
        /// The workbook (file) name to query
        /// </summary>
        [DefaultValue(null)]
        public string WorkBook
        {
            get { return workbook; }
            set
            {
                CloseConnection();
                workbook = value;
                determinedrange = null;
            }
        }
        /// <summary>
        /// The Range which to query. This can be any Excel range (eg "A1:B5") or
        /// just a worksheet name.
        /// If this value is null, the first sheet of the <see cref="WorkBook"/> is used
        /// </summary>
        private string range;

        /// <summary>
        /// Bereich
        /// </summary>
        [DefaultValue(null)]
        public string Range
        {
            get { return range; }
            set
            {
                range = value;
                determinedrange = null;
            }
        }
        private int worksheetindex = 0;
        /// <summary>
        /// The 0 based INDEX of the worksheet to query. 
        /// If you want to set the name of the worksheet, use <see cref="Range"/> instead.
        /// NB: if <see cref="Range"/> is set, this property is ignored
        /// </summary>
        [DefaultValue(0)]
        public int WorkSheetIndex
        {
            get { return worksheetindex; }
            set
            {
                worksheetindex = value;
                determinedrange = null;
            }
        }
        #region Range formatting
        /// <summary>
        /// If a range was determined in a previous step, keep it buffered here
        /// </summary>
        string determinedrange;
        /// <summary>
        /// Gets the properly formatted sheet name
        /// if no worksheet was provided, read out sheet information and select everything
        /// from the first sheet
        /// </summary>
        public string GetRange()
        {
            if (determinedrange == null)
            {
                string range = Range;
                if (range == null)
                    range = DetermineRange();
                if (range.IndexOf(':') == -1 && !range.EndsWith("$"))
                    range += "$"; //sheetname has to be appended with a $
                determinedrange = "[" + range + "]";
            }
            return determinedrange;
        }

        /// <summary>
        /// See <see cref="AutoDetermineRange"/> property for more info
        /// </summary>
        /// <returns></returns>
        string DetermineRange()
        {
            string sheet = GetSheetName(worksheetindex);
            if (!autodeterminerange) return sheet;
            return new RangeFinder(this, sheet).ToString();
        }
        #region RangeFinder
        class RangeFinder
        {
            OleDbDataAdapter da;
            DataTable dtSchema;
            ExcelDataRange rng = new ExcelDataRange();
            Import eb;
            int cols;
            /// <summary>
            /// minimum amount of columns that need to be filled 
            /// <seealso cref="minfilled"/>
            /// </summary>
            int min;
            public RangeFinder(ExcelBase Owner, string sheet)
            {
                this.eb = new Import(Owner.WorkBook);
                eb.Range = sheet;
                eb.UseHeaders = false;
                eb.InterMixedAsText = true;
                //DataTable dt = eb.Query();
                try
                {
                    eb.OpenConnection();
                    //get the number of rows and columns
                    da = new OleDbDataAdapter(
                       "select * from [" + sheet + "]", eb.Connection);
                    dtSchema = new DataTable();
                    da.FillSchema(dtSchema, SchemaType.Source);
                    cols = dtSchema.Columns.Count;
                    int rows = (int)ExecuteScalar("select count(*) from [" + sheet + "]");
                    //fill the range object               
                    rng.From.Row = rng.From.Column = 1;
                    rng.To.Row = rows;
                    rng.To.Column = cols;

                    min = (int)(cols * minfilled);
                    //now rng contains the complete square range of data containing cells
                    //try to narrow it by getting as much hits as possible
                    DecreaseRange();
                }
                finally
                {
                    indexReader.Close();
                    eb.CloseConnection();
                }
            }
            object ExecuteScalar(string sql)
            {
                return new OleDbCommand(sql, da.SelectCommand.Connection).ExecuteScalar();
            }

            string indexquery;
            string GetIndexQuery()
            {
                if (indexquery == null)
                {
                    StringBuilder sql = new StringBuilder("select 0");
                    foreach (DataRow dr in dtSchema.Rows)
                    {
                        string colname = "[" + dr["column_name"].ToString() + "]";
                        sql.Append("+iif(").Append(colname).Append(" is null,0,1)");
                    }
                    sql.Append(" as ind from ");
                    indexquery = sql.ToString();
                }
                return indexquery;
            }
            //ExcelDataRange indexRange;
            DataTable indexTable = new DataTable();
            OleDbDataReader indexReader;
            int GetIndex()
            {
                if (!Forward)
                {
                    indexReader.Close();
                    indexReader = null;
                    da.SelectCommand.CommandText = string.Format(" select * from {0}:{0}"
                    , rng.To.Row);
                }
                if (indexReader == null)
                    indexReader = da.SelectCommand.ExecuteReader();
                int cnt = 0;
                if (!indexReader.Read()) return -1;
                for (int i = 0; i < indexReader.FieldCount; i++)
                {
                    if (!indexReader.IsDBNull(i)) cnt++;
                }
                return cnt;
                //da.TableMappings.Clear();


                //da = new OleDbDataAdapter(da.SelectCommand.CommandText, eb.conn);
                //indexTable = new DataTable();
                ////da.FillSchema(indexTable, SchemaType.Source);
                //da.Fill(indexTable);
                //return indexTable.Columns.Count;
            }
            /// <summary>
            /// minimum percentage that needs to be filled to count as a datarow
            /// </summary>            
            const double minfilled = .75;
            /// <summary>
            /// The amount of subsequent (or preceding) rows that need to be filled a <see cref="minfilled"/> percentage
            /// for it to count as a datarow
            /// </summary>
            const int CheckRows = 3;
            /// <summary>
            /// Decrease the range step by step
            /// The problem is that when obtaining all, a lot more nulls are returned
            /// than you would visibly see. That makes most algorithms to get the
            /// block useless.
            /// this is also why just obtaining the datatable complete and removing the
            /// rows will not suffice: the proper field data types will not have been set
            /// Best way I could figure without using interop was to increase the start
            /// range to see if the avarage filled values increase.
            /// </summary>
            void DecreaseRange()
            {
                for (; ; )
                {
                    if (GetIndex() >= min)
                    {
                        int i = 0;
                        for (; i < CheckRows; i++)
                        {
                            AlterRange(1);
                            if (GetIndex() < min)
                            {
                                break;
                            }
                        }
                        if (i == CheckRows)
                        {
                            AlterRange(-i);
                            if (Forward)
                                Forward = false;
                            else
                                break;
                        }
                    }
                    if (rng.From.Row > rng.To.Row)
                        throw new Exception("Could not determine data range");
                    AlterRange(1);
                }
            }
            bool Forward = true;
            void AlterRange(int i)
            {
                if (Forward)
                    rng.From.Row += i;
                else
                    rng.To.Row -= i;
            }


            public override string ToString()
            {
                return rng.ToString();
            }
            struct ExcelRange
            {
                public int Row, Column;
                public ExcelRange(int Col, int Row)
                {
                    this.Column = Col;
                    this.Row = Row;
                }
                public override string ToString()
                {
                    //return string.Format("R{0}C{1}", Row, Column);
                    string res = Row.ToString();
                    int col = Column;
                    while (col > 0)
                    {
                        int cc = col % 26;
                        char c = (char)('A' + cc - 1);
                        res = c.ToString() + res;
                        col /= 26;
                    }
                    return res;
                }
            }
            struct ExcelDataRange
            {
                public ExcelRange
                From, To;
                public override string ToString()
                {
                    return GetRange(From, To);
                }
                static string GetRange(ExcelRange from, ExcelRange to)
                {
                    return from.ToString() + ":" + to.ToString();
                }
                public string TopRow()
                {
                    return GetRange(From, new ExcelRange(To.Column, From.Row));
                }
                public string BottomRow()
                {
                    return GetRange(new ExcelRange(From.Column, To.Row), To);
                }
            }
        }
        #endregion
        #endregion


        /// <summary>
        /// Checks if the <see cref="WorkBook"/> exists
        /// </summary>
        public bool WorkBookExists
        {
            get { return System.IO.File.Exists(WorkBook); }
        }
        /// <summary>
        /// Checks if the workbook exists and throws an exception if it doesn't
        /// <seealso cref="WorkBookExists"/>
        /// </summary>
        protected void CheckWorkbook()
        {
            if (!WorkBookExists) throw new System.IO.FileNotFoundException("Workbook not found", WorkBook);
        }
        #endregion
        #region Connection
        /// <summary>
        /// Creates  a NEW connection. If this method is called directly, this
        /// class will not check if it is closed.
        /// To get a handled connection, use the <see cref="Connection"/> property.
        /// </summary>
        /// <returns></returns>
        public OleDbConnection CreateConnection()
        {
            CheckWorkbook();
            return new OleDbConnection(
            string.Format("Provider=Microsoft.Jet.OLEDB.4.0;" +
            "Data Source={0};Extended Properties='Excel 8.0;HDR={1};Imex={2}'",
            WorkBook, useheaders ? "Yes" : "No",
            imex ? "1" : "0")
            );
        }
        private bool useheaders = true;
        /// <summary>
        /// Determines if the first row in the specified <see cref="Range"/> contains the headers
        /// </summary>
        [DefaultValue(true)]
        public bool UseHeaders
        {
            get { return useheaders; }
            set
            {
                if (useheaders != value)
                {
                    CloseConnection();
                    useheaders = value;
                }
            }
        }
        private bool imex;
        /// <summary>
        /// if this value is <c>true</c>, 'intermixed' data columns are handled as text (otherwise Excel tries to make a calcuated guess on what the datatype should be)
        /// </summary>
        [DefaultValue(false)]
        public bool InterMixedAsText
        {
            get { return imex; }
            set
            {
                if (imex != value)
                {
                    CloseConnection();
                    imex = value;
                }
            }
        }
        private bool autodeterminerange;
        /// <summary>
        /// Tries to obtain the range automatically by looking for a large chunk of data. Use this value if there's a lot of
        /// static around the actual data.
        /// Beware though: this takes some additional steps and can cause performance loss
        /// when querying larger files.
        /// automatically determening the range is not fullproof. Be sure to check the results
        /// on first time use.
        /// NB: if the <see cref="Range"/> is set, this property is ignored.
        /// </summary>
        [DefaultValue(false)]
        public bool AutoDetermineRange
        {
            get { return autodeterminerange; }
            set
            {
                if (autodeterminerange != value)
                {
                    autodeterminerange = value;
                    determinedrange = null;
                }
            }
        }
        OleDbConnection conn;
        /// <summary>
        /// Gets a connection to the current <see cref="WorkBook"/>
        /// When called for the first time (or after changing the workbook)
        /// a new connection is created.
        /// </summary>
        public OleDbConnection Connection
        {
            get
            {
                if (conn == null)
                {
                    conn = CreateConnection();
                    UseFinalizer = true;
                }
                return conn;
            }
        }
        /// <summary>
        /// Closes the connection (if open)
        /// </summary>
        public void CloseConnection()
        {
            if (conn != null && ConnectionIsOpen)
                conn.Dispose();
            conn = null;
            UseFinalizer = false;
        }

        /// <summary>
        /// Verbindung schließen
        /// </summary>
        /// <param name="OnlyIfNoneOpen"></param>
        protected void CloseConnection(bool OnlyIfNoneOpen)
        {
            if (OnlyIfNoneOpen)
            {
                if (--opencount > 0 || wasopenbeforerememberstate) return;
            }
            CloseConnection();
        }
        /// <summary>
        /// Opens the <see cref="Connection"/>
        /// </summary>
        public void OpenConnection()
        {
            OpenConnection(false);
        }
        int opencount;
        bool wasopenbeforerememberstate;
        
        /// <summary>
        /// Verbindugn öffnen
        /// </summary>
        /// <param name="RememberState"></param>
        protected void OpenConnection(bool RememberState)
        {
            if (RememberState && opencount++ == 0) wasopenbeforerememberstate = ConnectionIsOpen;
            if (!ConnectionIsOpen)
                Connection.Open();
        }

        /// <summary>
        /// Anzeige ob offene Verbindung
        /// </summary>
        public bool ConnectionIsOpen
        {
            get { return conn != null && conn.State != ConnectionState.Closed; }
        }

        #endregion
        #region IDisposable Members
        /// <summary>
        /// Dispose
        /// </summary>
        public new void Dispose()
        {
            CloseConnection();
        }

        /// <summary>
        /// Dekonstruktor
        /// </summary>
        ~ExcelBase()
        {
            Dispose();
        }

        private bool usefinalizer;
        bool UseFinalizer
        {
            get { return usefinalizer; }
            set
            {
                if (usefinalizer == value) return;
                usefinalizer = value;
                if (value)
                    GC.ReRegisterForFinalize(this);
                else
                    GC.SuppressFinalize(this);
            }
        }
        #endregion
        #region Helper functions
        /// <summary>
        /// queries the connection for the sheetnames and returns them
        /// </summary>
        /// <returns></returns>
        public string[] GetSheetNames()
        {
            OpenConnection(true);
            try
            {
                // Read out sheet information
                DataTable dt = Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null || dt.Rows.Count == 0)
                {
                    throw new Exception("Could not get sheet names");
                }

                string[] res = new string[dt.Rows.Count];
                for (int i = 0; i < res.Length; i++)
                {
                    string name = dt.Rows[i]["TABLE_NAME"].ToString();

                    if (name[0] == '\'')
                    {
                        //numeric sheetnames get single quotes around them in the schema.
                        //remove them here
                        if (System.Text.RegularExpressions.Regex.IsMatch(
                        name, @"^'\d\w+\$'$"))
                            name = name.Substring(1, name.Length - 2);
                    }
                    res[i] = name;
                }
                return res;
            }
            finally
            {
                CloseConnection(true);
            }
        }
        /// <summary>
        /// Gets the name of the first sheet
        /// (this is also the default range used, when no <see cref="Range"/> is specified)
        /// </summary>
        /// <returns></returns>
        public string GetFirstSheet()
        {
            return GetSheetName(0);
        }

        /// <summary>
        /// Workbook holen
        /// </summary>
        /// <param name="index">Indes des Arbeitsblatts</param>
        /// <returns>Name des Arbeitsblatts</returns>
        public string GetSheetName(int index)
        {
            string[] sheets = GetSheetNames();
            if (index < 0 || index >= sheets.Length)
                throw new IndexOutOfRangeException("No worksheet exists at the specified index");
            return sheets[index];
        }
        #endregion

        public static List<string> GetExcelDataSheets(string fileName)
        {
            List<string> list = new List<string>();

            Excel.Import i = new Excel.Import(fileName);
            try
            {
                list.AddRange(i.GetSheetNames());
            }
            catch (Exception e)
            {
                throw e;
            }


            return list;
        }


        public static DataTable GetExcelDataFromFile(string fileName, string sheet)
        {
            Excel.Import i = new Excel.Import(fileName, sheet.Replace("'", ""));
            return i.Query();
        }
    }

    

}