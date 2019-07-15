using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Xml;
using System.Collections.Generic;
using System.Xml.XPath;

namespace ConneXion.Data.Sql
{
	/// <summary>
	/// Zusammendfassende Beschreibung für SqlCommand.
	/// </summary>
	public class SQLConnection : BaseDbConnection
	{
		/// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="ConnectionString">ConnectionString</param>
        public SQLConnection(string ConnectionString)
        {
            dbConnection = new SqlConnection(ConnectionString);
        }

		/// <summary>
		/// Holen eines DataTables aus der Datenbank
		/// </summary>
		/// <param name="CommandText">SQL Abfragetext</param>
		/// <param name="Parameters">Abfrageparameter</param>
		/// <returns></returns>
		public override DataTable GetDataTable(string CommandText, params IDataParameter[] Parameters)
		{
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandType = CommandType;
                cmd.Connection = (SqlConnection)dbConnection;
                if (transaction != null)
                    cmd.Transaction = (SqlTransaction)transaction;
                cmd.CommandText = CommandText;

                foreach (SqlParameter param in Parameters)
                    cmd.Parameters.Add(param);

                using (SqlDataAdapter adpt = new SqlDataAdapter(cmd))
                {
                    SqlCommandBuilder cb = new SqlCommandBuilder(adpt);
                    DataTable dt = new DataTable();
                    adpt.Fill(dt);
                    cb.Dispose();
                    return dt;
                }
                
            }
		}

        /// <summary>
        /// Abfrage ohne Rückgabewert
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Parameters">Abfrageparameter</param>
        /// <param name="Automatic">Verbindungsmanagement</param>
        /// <returns></returns>
		public override int ExecuteNonQuery(string CommandText, bool Automatic, params IDataParameter[] Parameters)
		{
			int result = 0;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandType = CommandType;
                cmd.Connection = (SqlConnection)dbConnection;
                if (transaction != null)
                    cmd.Transaction = (SqlTransaction)transaction;
                cmd.CommandText = CommandText;

                foreach (SqlParameter param in Parameters)
                    cmd.Parameters.Add(param);

                try
                {
                    if (Automatic)
                        OpenConnection();
                    result = cmd.ExecuteNonQuery();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (Automatic)
                        CloseConnection();
                }
            }
		}


        /// <summary>
        /// Abfrage mit einem XmlDocument als Rückgabe
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Parameters">Abfrageparameter</param>
        /// <param name="Automatic">Verbindungsmanagement</param>
        /// <returns></returns>
        public override XmlDocument GetXmlDocument(string CommandText, bool Automatic, params IDataParameter[] Parameters)
        {
            
            XmlDocument result = new XmlDocument();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandType = CommandType;
                cmd.Connection = (SqlConnection)dbConnection;
                if (transaction != null)
                    cmd.Transaction = (SqlTransaction)transaction;
                cmd.CommandText = CommandText;

                foreach (SqlParameter param in Parameters)
                    cmd.Parameters.Add(param);

                try
                {
                    if (Automatic)
                        OpenConnection();

                    result.Load(cmd.ExecuteXmlReader());

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (Automatic)
                        CloseConnection();
                }
            }
        }

        /// <summary>
        /// Abfrage mit einem Rückgabewert
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Parameters">Abfrageparameter</param>
        /// <param name="Automatic">Verbindungsmanagement</param>
        /// <returns></returns>
		public override object ExecuteScalar(string CommandText, bool Automatic, params IDataParameter[] Parameters)
		{
			object result = null;
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandType = CommandType;
                cmd.Connection = (SqlConnection)dbConnection;
                if (transaction != null)
                    cmd.Transaction = (SqlTransaction)transaction;
                cmd.CommandText = CommandText;

                foreach (SqlParameter param in Parameters)
                    cmd.Parameters.Add(param);

                try
                {
                    
                    if (Automatic)
                        OpenConnection();
                    result = cmd.ExecuteScalar();
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (Automatic)
                        CloseConnection();
                }
            }
		}

        /// <summary>
        /// Abfrage einer Zeile
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Parameters">Abfrageparameter</param>
        /// <param name="Automatic">Verbindungsmanagement</param>
        /// <returns>null wenn keine Daten vorhanden sind</returns>
        public override Dictionary<string, object> GetDataRow(string CommandText, bool Automatic, params IDataParameter[] Parameters)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandType = CommandType;
                cmd.Connection = (SqlConnection)dbConnection;
                if (transaction != null)
                    cmd.Transaction = (SqlTransaction)transaction;
                cmd.CommandText = CommandText;

                foreach (SqlParameter param in Parameters)
                    cmd.Parameters.Add(param);

                try
                {
                    if (Automatic)
                        OpenConnection();

                    SqlDataReader reader = cmd.ExecuteReader();
                    bool dataIsPresent = reader.Read();

                    if (dataIsPresent)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            result.Add(reader.GetName(i), reader[i]);
                        }
                    }
                    reader.Close();

                    if (dataIsPresent) return result;
                    return null;
                }
                catch 
                {
                    throw;
                }
                finally
                {
                    if (Automatic)
                        CloseConnection();
                }
            }
        }

        /// <summary>
        /// Liste von Objekten holen
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="CommandText"></param>
        /// <param name="Automatic"></param>
        /// <param name="Parameters"></param>
        public override List<T> GetDataList<T>(string CommandText, bool Automatic, params IDataParameter[] Parameters)
        {
            List<T> list = new List<T>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandType = CommandType;
                cmd.Connection = (SqlConnection)dbConnection;
                if (transaction != null)
                    cmd.Transaction = (SqlTransaction)transaction;
                cmd.CommandText = CommandText;

                foreach (SqlParameter param in Parameters)
                    cmd.Parameters.Add(param);

                try
                {
                    if (Automatic)
                        OpenConnection();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T t = new T();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader[i] == DBNull.Value)
                                    continue;
                                SetPropertyValue<T>(ref t, reader.GetName(i), reader[i]);
                            }
                            list.Add(t);
                            t = default(T);
                        }

                        reader.Close();
                    }
                }
                catch 
                {
                    throw;
                }
                finally
                {
                    if (Automatic)
                        CloseConnection();
                }
            }
            return list;
        }

        /// <summary>
        /// Liste von Objekten holen
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="CommandText"></param>
        /// <param name="Automatic"></param>
        /// <param name="Parameters"></param>
        public override List<I> GetDataList<I, T>(string CommandText, bool Automatic, params IDataParameter[] Parameters)
        {
            List<I> list = new List<I>();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandType = CommandType;
                cmd.Connection = (SqlConnection)dbConnection;
                if (transaction != null)
                    cmd.Transaction = (SqlTransaction)transaction;
                cmd.CommandText = CommandText;

                foreach (SqlParameter param in Parameters)
                    cmd.Parameters.Add(param);

                try
                {
                    if (Automatic)
                        OpenConnection();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            T t = new T();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader[i] == DBNull.Value)
                                    continue;
                                SetPropertyValue<T>(ref t, reader.GetName(i), reader[i]);
                            }
                            list.Add(t);
                            t = default(T);
                        }

                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (Automatic)
                        CloseConnection();
                }
            }
            return list;
        }


        /// <summary>
        /// Datenelement holen
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="item">Element von T</param>
        /// <param name="CommandText">Sql Abfrage</param>
        /// <param name="Automatic">Verbindungsmanagement</param>
        /// <param name="Parameters">Parameter</param>
        /// <returns>Element mit Daten</returns>
        [Obsolete("User method without ref!")]
        public override bool GetDataItem<T>(ref T item, string CommandText, bool Automatic, params IDataParameter[] Parameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandType = CommandType;
                cmd.Connection = (SqlConnection)dbConnection;
                if (transaction != null)
                    cmd.Transaction = (SqlTransaction)transaction;
                cmd.CommandText = CommandText;

                foreach (SqlParameter param in Parameters)
                    cmd.Parameters.Add(param);

                try
                {
                    if (Automatic)
                        OpenConnection();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        bool dataIsPresent = reader.Read();

                        if (dataIsPresent)
                        {

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader[i] == DBNull.Value)
                                    continue;
                                SetPropertyValue<T>(ref item, reader.GetName(i), reader[i]);
                            }
                        }


                        reader.Close();
                        return dataIsPresent;
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (Automatic)
                        CloseConnection();
                }
            }
        }

        public override T GetDataItem<T>(string commandText, bool automatic, params IDataParameter[] parameters)
        {
            T item = new T();
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandType = CommandType;
                cmd.Connection = (SqlConnection)dbConnection;
                if (transaction != null)
                    cmd.Transaction = (SqlTransaction)transaction;
                cmd.CommandText = commandText;

                foreach (SqlParameter param in parameters)
                    cmd.Parameters.Add(param);

                try
                {
                    if (automatic)
                        OpenConnection();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        bool dataIsPresent = reader.Read();

                        if (dataIsPresent)
                        {

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader[i] == DBNull.Value)
                                    continue;
                                SetPropertyValue<T>(ref item, reader.GetName(i), reader[i]);
                            }
                        }


                        reader.Close();
                        return item;
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (automatic)
                        CloseConnection();
                }
            }
        }

        /// <summary>
        /// Array aus T Holen
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="CommandText">SQL Abfrage</param>
        /// <param name="Automatic">Verbindungsmanagement</param>
        /// <param name="Parameters">Parameter</param>
        /// <returns>T[]</returns>
        public override T[] GetDataArray<T>(string CommandText, bool Automatic, params IDataParameter[] Parameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandType = CommandType;
                cmd.Connection = (SqlConnection)dbConnection;
                if (transaction != null)
                    cmd.Transaction = (SqlTransaction)transaction;
                cmd.CommandText = CommandText;

                foreach (SqlParameter param in Parameters)
                    cmd.Parameters.Add(param);

                List<T> list = new List<T>();

                try
                {
                    if (Automatic)
                        OpenConnection();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            list.Add((T)reader[0]);
                        }

                        reader.Close();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (Automatic)
                        CloseConnection();
                }
                return list.ToArray();
            }
        }

        public override IEnumerable<string> GetVarCharMax(string CommandText, bool Automatic, params IDataParameter[] Parameters)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandTimeout = CommandTimeout;
                cmd.CommandType = CommandType;
                cmd.Connection = (SqlConnection)dbConnection;
                if (transaction != null)
                    cmd.Transaction = (SqlTransaction)transaction;
                cmd.CommandText = CommandText;

                foreach (SqlParameter param in Parameters)
                    cmd.Parameters.Add(param);

                try{
                    if (Automatic)
                        OpenConnection();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                             yield return new string(reader.GetSqlChars(0).Value);         
                        }
                    }
                }
               
                finally
                {
                    if (Automatic)
                        CloseConnection();
                }

            }
        }

        /// <summary>
        /// Erzeugen eines SqlParameters
        /// </summary>
        /// <param name="name">Parametername</param>
        /// <param name="value">Wert</param>
        /// <param name="isOutputParameter">Ausgabeparameter</param>
        /// <returns>Parameter mit Daten</returns>
        public override IDataParameter CreateParameter(string name, object value, bool isOutputParameter)
        {
            if (value == null) value = DBNull.Value;
            SqlParameter param = new SqlParameter(name, value);
            if (isOutputParameter)
                param.Direction = ParameterDirection.InputOutput;
            return param;
        }

        /// <summary>
        /// Erzeugen eines Output Parameters
        /// </summary>
        /// <param name="name">Parametername</param>
        /// <param name="size">Größe</param>
        /// <param name="type">SQL Datentyp</param>
        /// <returns>Parameter</returns>
        public override IDataParameter CreateParameter(string name, int type, int size)
        {
            SqlParameter param = new SqlParameter(name, (SqlDbType)type, size);
            param.Direction = ParameterDirection.Output;
            return param;
        }
    }
}
