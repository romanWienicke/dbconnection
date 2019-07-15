using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;

namespace ConneXion.Data.Manager
{
    public abstract class BaseManager : IDisposable
    {
        private BaseDbConnection _connection;
        protected BaseDbConnection Connection
        {
            get
            {
                if(_connection == null)
                    _connection = GetConnection(CommandType.StoredProcedure);
                return _connection;
            }
        }
        
        /// <summary>
        /// Datenbankverbindung
        /// </summary>
        public static BaseDbConnection GetConnection(CommandType commandType)
        {
            try
            {
                BaseDbConnection connection = ProviderFactory.GetConnection(
                            (DBProvider)Enum.Parse(typeof(DBProvider), ConfigurationManager.AppSettings["DbProvider"], true),
                            ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["ConnectionStringName"]].ConnectionString);
                connection.CommandType = commandType;
                return connection;

            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException("ConneXion.Data ConfigurationError", ex);
            }
        }
        
        /// <summary>
        /// Konstruktor
        /// Verbindungstyp: ConfigurationManager.AppSettings["DbProvider"] 
        /// ConnectionString: ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString
        /// </summary>
        protected BaseManager()
        {
        }

        #region IDisposable Member

        public void Dispose()
        {
            if (Connection != null)
                Connection.Dispose();
        }

        #endregion
    }
}
