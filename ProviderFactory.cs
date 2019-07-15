using System;
using System.Collections.Generic;
using System.Text;
using ConneXion.Data;


namespace ConneXion.Data
{
    /// <summary>
    /// Factory zur Erstellung der jeweiligen Verbindung
    /// </summary>
    public class ProviderFactory
    {
        /// <summary>
        /// Erstellt eine neue Datenbankverbindung
        /// </summary>
        /// <param name="provider">Datenbank Provider</param>
        /// <param name="connectionString">Verbindungszeichenfolge</param>
        /// <returns></returns>
        public static BaseDbConnection GetConnection(DBProvider provider, string connectionString)
        {
            switch (provider)
            {
                case DBProvider.SQLConnection:
                    return new Sql.SQLConnection(connectionString);
                case DBProvider.OLEConnection:
                    return new Ole.OLEConnection(connectionString);
                
            }
            return null;
        }
    }
}
