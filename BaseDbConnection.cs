using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Xml;
using System.Threading;
using System.Collections;
using System.Text.RegularExpressions;

namespace ConneXion.Data
{
    /// <summary>
    /// Basisklasse für Datenbanklayer
    /// </summary>
    public abstract class BaseDbConnection : IDisposable
    {
        #region Member Variablen
        /// <summary>
        /// Datenbank Verbindung
        /// </summary>
        protected IDbConnection dbConnection;
        /// <summary>
        /// Transaktion
        /// </summary>
        protected IDbTransaction transaction;

        private Mutex mutex = new Mutex();

        /// <summary>
        /// Datenbank Name
        /// </summary>
        public string Database
        {
            get { return dbConnection.Database; }
        }


        private int commandTimeout = 30;
        /// <summary>
        /// Timeout für Command     
        /// </summary>
        public int CommandTimeout
        {
            get { return commandTimeout; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be greater than 0!");
                commandTimeout = value;
            }
        }

        /// <summary>
        /// CommandType
        /// </summary>
        public CommandType CommandType
        {
            get;
            set;
        }

        /// <summary>
        /// ConnecionString
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return dbConnection.ConnectionString;
            }

            set
            {
                dbConnection.ConnectionString = value;
            }
        }
        #endregion

        #region Konstruktoren
        /// <summary>
        /// Konstuktor
        /// CommandType = StoredProcedure
        /// </summary>
        protected BaseDbConnection()
        {
            CommandType = System.Data.CommandType.StoredProcedure;
        }

        /// <summary>
        /// Konstuktor    
        /// </summary>
        /// <param name="CommandType"></param>
        protected BaseDbConnection(CommandType CommandType)
        {
            this.CommandType = CommandType;
        }

        /// <summary>
        /// Dekonstruktor
        /// </summary>
        ~BaseDbConnection()
        {
            Dispose();
        }
        #endregion

        
        /// <summary>
        /// Holen eines DataTable aus der Datenbank
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Parameters">Abfrageparameter</param>
        /// <returns></returns>
        public abstract DataTable GetDataTable(string commandText, params IDataParameter[] parameters);

        /// <summary>
        /// Ausführen einer Abfrage mit einem Rückgabewert
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, params IDataParameter[] parameters)
        {
            return ExecuteScalar(commandText, true, parameters);
        }

        /// <summary>
        /// Ausführen einer Abfrage mit einem Rückgabewert
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Automatic">Automatisches Verbindungsmanagement</param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public abstract object ExecuteScalar(string commandText, bool automatic, params IDataParameter[] parameters);


        /// <summary>
        /// Ausführen einer Abfrage ohne Rückgabewert
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Automatic">Automatisches Verbindungsmanagement</param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public abstract int ExecuteNonQuery(string commandText, bool automatic, params IDataParameter[] parameters);

        /// <summary>
        /// Ausführen einer Abfrage ohne Rückgabewert
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText, params IDataParameter[] parameters)
        {
            return ExecuteNonQuery(commandText, true, parameters);
        }

        /// <summary>
        /// Ausführen einer Abfrage mit einem XmlDocument als Rückgabe
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public XmlDocument GetXmlDocument(string commandText, params IDataParameter[] parameters)
        {
            return GetXmlDocument(commandText, true, parameters);
        }

        /// <summary>
        /// Ausführen einer Abfrage mit einem XmlDocument als Rückgabe
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Automatic">Automatisches Verbindungsmanagement</param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public abstract XmlDocument GetXmlDocument(string commandText, bool automatic, params IDataParameter[] parameters);

        /// <summary>
        /// Daten einer Zeile als Objekt Array holen
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetDataRow(string commandText, params IDataParameter[] parameters)
        {
            return GetDataRow(commandText, true, parameters);
        }

        /// <summary>
        /// Daten einer Zeile als Objekt Array holen
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Automatic">Automatisches Verbindungsmanagement</param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public abstract Dictionary<string, object> GetDataRow(string commandText, bool automatic, params IDataParameter[] parameters);



        /// <summary>
        /// Liste holen
        /// </summary>
        /// <typeparam name="T">Typ T</typeparam>
        /// <param name="list">Liste</param>
        /// <param name="CommandText">SQL Abfrage</param>
        /// <param name="Automatic">Automatische Verbindungsverwaltung</param>
        /// <param name="Parameters">SQL Parameter</param>
        public abstract List<T> GetDataList<T>(string commandText, bool automatic, params IDataParameter[] parameters) where T : new();

        /// <summary>
        ///  Liste holen
        /// </summary>
        /// <typeparam name="I">Interface Liste</typeparam>
        /// <typeparam name="T">Typ T</typeparam>
        /// <param name="list">Liste</param>
        /// <param name="CommandText">SQL Abfrage</param>
        /// <param name="Automatic">Automatische Verbindungsverwaltung</param>
        /// <param name="Parameters">SQL Parameter</param>
        public abstract List<I> GetDataList<I, T>(string commandText, bool automatic, params IDataParameter[] parameters)
            where T : I, new()
            where I : IIdentifiable;
        

        /// <summary>
        /// Liste holen
        /// </summary>
        /// <typeparam name="T">Typ T</typeparam>
        /// <param name="list">Liste</param>
        /// <param name="CommandText">SQL Abfrage</param>
        /// <param name="Parameters">SQL Parameter</param>
        public List<T> GetDataList<T>(string commandText, params IDataParameter[] parameters) where T : new()
        {
            return GetDataList<T>(commandText, true, parameters);
        }

        /// <summary>
        ///  Liste holen
        /// </summary>
        /// <typeparam name="I">Interface Liste</typeparam>
        /// <typeparam name="T">Typ T</typeparam>
        /// <param name="list">Liste</param>
        /// <param name="CommandText">SQL Abfrage</param>
        /// <param name="Parameters">SQL Parameter</param>
        public List<I> GetDataList<I, T>(string commandText, params IDataParameter[] parameters) where T : I, new() where I: IIdentifiable
        {
            return GetDataList<I, T>(commandText, true, parameters);
        }


        

        /// <summary>
        /// Datenelement holen
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="CommandText"></param>
        /// <param name="Automatic"></param>
        /// <param name="Parameters"></param>
        [Obsolete("User method without ref!")]
        public abstract bool GetDataItem<T>(ref T item, string commandText, bool automatic, params IDataParameter[] parameters);


        /// <summary>
        /// Datenelement holen
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="item">zu befüllendes Element</param>
        /// <param name="CommandText">SQL Abfrage</param>
        /// <param name="Parameters">Parameter</param>
        [Obsolete("User method without ref!")]
        public bool GetDataItem<T>(ref T item, string commandText, params IDataParameter[] parameters)
        {
            return GetDataItem<T>(ref item, commandText, true, parameters);
        }

        /// <summary>
        /// Datenelement holen
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="CommandText"></param>
        /// <param name="Automatic"></param>
        /// <param name="Parameters"></param>
        public abstract T GetDataItem<T>(string commandText, bool automatic, params IDataParameter[] parameters) where T : new();

        /// <summary>
        /// Datenelement holen
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="CommandText">SQL Abfrage</param>
        /// <param name="Parameters">Parameter</param>
        public T GetDataItem<T>(string commandText, params IDataParameter[] parameters) where T : new()
        {
            return GetDataItem<T>(commandText, true, parameters);
        }

        /// <summary>
        /// Datenliste als Array holen
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="CommandText">SQL Abfrage</param>
        /// <param name="Automatic">Verbindungsmanagement</param>
        /// <param name="Parameters">SQL Parameter</param>
        /// <returns>Array aus T</returns>
        public abstract T[] GetDataArray<T>(string commandText, bool automatic, params IDataParameter[] parameters);

        /// <summary>
        /// Datenliste als Array holen
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="CommandText">SQL Abfrage</param>
        /// <param name="Parameters">SQL Parameter</param>
        /// <returns>befülltes T</returns>
        public T[] GetDataArray<T>(string commandText, params IDataParameter[] parameters)
        {
            return GetDataArray<T>(commandText, true, parameters);
        }

        /// <summary>
        /// Varchar Max Daten holen
        /// </summary>
        /// <param name="CommandText"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public IEnumerable<string> GetVarCharMax(string CommandText, params IDataParameter[] Parameters)
        {
            return GetVarCharMax(CommandText, true, Parameters);
        }

        public abstract IEnumerable<string> GetVarCharMax(string CommandText, bool Automatic, params IDataParameter[] Parameters);

        #region Verbindung & Transaktion
        /// <summary>
        /// Öffnen der Datenbankverbingung
        /// </summary>
        public void OpenConnection()
        {
            //mutex.WaitOne();
            if (dbConnection.State != ConnectionState.Open)
                dbConnection.Open();
            
        }

        /// <summary>
        /// Schließen der Datenbankverbingung
        /// </summary>
        public void CloseConnection()
        {
            if (dbConnection.State != ConnectionState.Closed)
                dbConnection.Close();
            //mutex.ReleaseMutex();
        }

        /// <summary>
        /// Starten einer Transaktion
        /// </summary>
        public void TransactionStart(IsolationLevel isolationLevel)
        {
            OpenConnection();
            transaction = dbConnection.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Commit einer Transaktion
        /// </summary>
        public void TransactionCommit()
        {
            transaction.Commit();
            CloseConnection();
        }

        /// <summary>
        /// Rollback einer Transaktion
        /// </summary>
        public void TransactionRollback()
        {
            transaction.Rollback();
            CloseConnection();
        }
        #endregion

        #region PropertiesToParameter
        /// <summary>
        /// Erzeugen von DbParametern aus dem Objekt "target"
        /// </summary>
        /// <param name="target">Instanz des Objekts</param>
        /// <returns>Array von Parametern</returns>
        public IDataParameter[] PropertiesToParameter(object target)
        {
            return PropertiesToParameter(target, null);
        }

        /// <summary>
        /// Erzeugen von DbParametern aus dem Objekt "target"
        /// </summary>
        /// <param name="target">Instanz des Objekts</param>
        /// <param name="defaultParams">Vorgegebene Parameter (output)</param>
        /// <returns>Array von Parametern</returns>
        public IDataParameter[] PropertiesToParameter(object target, params IDataParameter[] defaultParams)
        {
            List<IDataParameter> parameters;

            if (defaultParams == null)
                parameters = new List<IDataParameter>();
            else
                parameters = new List<IDataParameter>(defaultParams);

            Type type = target.GetType();
            PropertyInfo[] info = type.GetProperties();

            for (int i = 0; i < info.Length; i++)
            {
                DBPropertyAttribute attr = GetDbDataParameterValue(info[i]);

                if (attr == null) continue;
                // Wert holen und falls null auf dbnull stellen
                object value = info[i].GetValue(target, null);

                if (value == null)
                    value = DBNull.Value;

                parameters.Add(
                    CreateParameter(string.Format("@{0}", info[i].Name), value, attr.IsOutputParameter)
                    );
            }
            return parameters.ToArray();
        }

        /// <summary>
        /// Erzeugen von DbParametern aus dem Objekt "target"
        /// </summary>
        /// <param name="target">Instanz des Objekts</param>
        /// <param name="defaultParams">Vorgegebene Parameter (output)</param>
        /// <returns>Array von Parametern</returns>
        public IDataParameter[] DictionaryToParameter(IDictionary target, params IDataParameter[] defaultParams)
        {
            List<IDataParameter> parameters;

            if (defaultParams == null)
                parameters = new List<IDataParameter>();
            else
                parameters = new List<IDataParameter>(defaultParams);

            foreach (object key in target.Keys)
            {
                parameters.Add(
                    CreateParameter(string.Format("@{0}", key), target[key])
                    );
            }
            
            return parameters.ToArray();
        }

        /// <summary>
        /// Ausgabeparameter holen
        /// </summary>
        /// <param name="parameters">Parameter Array mit Ausgabeparametern</param>
        /// <returns>Dictionary mit allen Ausgabeparametern</returns>
        public static Dictionary<string, object> GetOutputParameterData(IDataParameter[] parameters)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            foreach (IDataParameter param in parameters)
            {
                if (param.Direction == ParameterDirection.Input) continue;
                data.Add(param.ParameterName, param.Value);
            }
            return data;
        }

        /// <summary>
        /// Objekt aus Parametern erzeugen
        /// </summary>
        /// <param name="parameters">Parameter Array </param>
        /// <returns>new T()</returns>
        public static T CreateInstance<T>(IDataParameter[] parameters) where T: new()
        {
            T item = new T();

            foreach (IDataParameter param in parameters)
            {
                SetPropertyValue<T>(ref item, param.ParameterName, param.Value);
            }
            return item;
        }

        /// <summary>
        /// Erzeugen eines Input Parameters
        /// </summary>
        /// <param name="name">Parametername</param>
        /// <param name="value">Value</param>
        /// <returns>Parameter</returns>
        public IDataParameter CreateParameter(string name, object value)
        {
            return CreateParameter(name, value, false);
        }

        /// <summary>
        /// Erzeugen eines Parameters
        /// </summary>
        /// <param name="name">Parametername</param>
        /// <param name="value">Value</param>
        /// <param name="isOutputParameter">true für einen Output Parameter</param>
        /// <returns>Parameter</returns>
        public abstract IDataParameter CreateParameter(string name, object value, bool isOutputParameter);

        /// <summary>
        /// Erzeugen eines Parameters
        /// </summary>
        /// <param name="name">Parametername</param>
        /// <param name="type">Sql Typ</param>
        /// <param name="size">Größe</param>
        /// <returns></returns>
        public abstract IDataParameter CreateParameter(string name, int type, int size);

        /// <summary>
        /// Überprüfen ob das Attribut DbPropertyAttribute verwendet wurde und wert auslesen.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static DBPropertyAttribute GetDbDataParameterValue(PropertyInfo info)
        {
            //foreach (object o in info.GetCustomAttributes(true))
            //{
            //    if (!(o is DBPropertyAttribute)) continue;
            //    return (o as DBPropertyAttribute);
            //}
            foreach (DBPropertyAttribute attr in info.GetCustomAttributes(typeof(DBPropertyAttribute), true))
                return attr;
            return null;
        }

        protected static void SetPropertyValue<T>(ref T o, string propertyName, object value)
        {
            PropertyInfo[] infos = o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    infos[i].SetValue(o, value, null);
                    return;
                }
            }
        }
        #endregion

        
        #region IDisposable Members

        public void Dispose()
        {
            if (transaction != null)
                transaction.Dispose();

            if (dbConnection != null)
                dbConnection.Dispose();
        }

        #endregion


        
    }
}
