using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Specialized;
using System.Collections.Generic;



namespace ConneXion.Data.Ole
{
	/// <summary>
	/// Zusammendfassende Beschreibung für DbConnection.
	/// </summary>
    public class OLEConnection : BaseDbConnection
	{
		/// <summary>
        /// Konstuktor
        /// </summary>
        /// <param name="ConneciontString">ConneciontString</param>
		public OLEConnection(string ConneciontString)
		{
			dbConnection = new OleDbConnection(ConneciontString);
		}

		/// <summary>
		/// Holen eines DataTables aus der Datenbank
		/// </summary>
		/// <param name="CommandText">SQL Abfragetext</param>
		/// <param name="Parameters">Abfrageparameter</param>
		/// <returns></returns>
		public override DataTable GetDataTable(string CommandText, params IDataParameter[] Parameters)
		{
			OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = CommandTimeout;
			cmd.Connection = (OleDbConnection)dbConnection;
			if(transaction != null)
                cmd.Transaction = (OleDbTransaction)transaction;
			cmd.CommandText = CommandText;
			
			foreach(OleDbParameter param in Parameters)
				cmd.Parameters.Add(param);

			OleDbDataAdapter adpt = new OleDbDataAdapter(cmd);
			//OleDbCommandBuilder cb = new OleDbCommandBuilder(adpt);
            new OleDbCommandBuilder(adpt);
            DataTable dt = new DataTable();
			adpt.Fill(dt);
			return dt;
		}



        /// <summary>
        /// NICHT FÜR OLE IMPLEMENTIERT!
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Automatic">Verbindungsmanagement</param>
        /// <param name="Parameters">Abfrageparameter</param>
        /// <returns>XmlDokument</returns>
        public override System.Xml.XmlDocument GetXmlDocument(string CommandText, bool Automatic, params IDataParameter[] Parameters)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// NICHT FÜR OLE IMPLEMENTIERT!
        /// </summary>
        public override IEnumerable<string> GetVarCharMax(string CommandText, bool Automatic, params IDataParameter[] Parameters)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Abfrage ohne Rückgabewert
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Automatic">Automatische Verbindungsverwaltung</param>
        /// <param name="Parameters">Abfrageparameter</param>
        /// <returns>Anzahl betroffener Datensätze</returns>
		public override int ExecuteNonQuery(string CommandText, bool Automatic, params IDataParameter[] Parameters)
		{
			int result = 0;
			OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = CommandTimeout;
			cmd.Connection = (OleDbConnection)dbConnection;
			if(transaction != null)
				cmd.Transaction = (OleDbTransaction)transaction;
			cmd.CommandText = CommandText;
			
			foreach(OleDbParameter param in Parameters)
				cmd.Parameters.Add(param);
			
			try 
			{
				if(Automatic) 
					OpenConnection();
				result = cmd.ExecuteNonQuery();
				return result;
			} 
			catch(Exception)
			{
				throw;
			}
			finally
			{
                cmd = null;
				if(Automatic) 
					CloseConnection();
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
			OleDbCommand cmd = new OleDbCommand();
            cmd.CommandTimeout = CommandTimeout;
			cmd.Connection = (OleDbConnection)dbConnection;
			if(transaction != null)
				cmd.Transaction = (OleDbTransaction)transaction;
			cmd.CommandText = CommandText;
			
			foreach(OleDbParameter param in Parameters)
				cmd.Parameters.Add(param);
			
			try 
			{
				if(Automatic)
					OpenConnection();
				result = cmd.ExecuteScalar();
				return result;
			} 
			catch(Exception)
			{
				throw;
			}
			finally
			{
                cmd = null;
				if(Automatic)
					CloseConnection();
			}
        }


        /// <summary>
        /// NICHT FÜR OLE IMPLEMENTIERT!
        /// </summary>
        /// <param name="CommandText">SQL Abfragetext</param>
        /// <param name="Parameters">Abfrageparameter</param>
        /// <param name="Automatic">Verbindungsmanagement</param>
        /// <returns></returns>
        public override Dictionary<string, object> GetDataRow(string CommandText, bool Automatic, params IDataParameter[] Parameters)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Erzeugen eines neuen OleDbParameters
        /// </summary>
        /// <param name="name">Parametername</param>
        /// <param name="value">Wert</param>
        /// <param name="isOutputParameter">Ausgabeparameter</param>
        /// <returns>Parameter mit Wert und Typ</returns>
        public override IDataParameter CreateParameter(string name, object value, bool isOutputParameter)
        {
            OleDbParameter param = new OleDbParameter(name, value);
            if (isOutputParameter)
                param.Direction = ParameterDirection.InputOutput;
            return param;
        }

        

       

       

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="CommandText"></param>
        /// <param name="Automatic"></param>
        /// <param name="Parameters"></param>
        public override List<T> GetDataList<T>(string CommandText, bool Automatic, params IDataParameter[] Parameters)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="CommandText"></param>
        /// <param name="Automatic"></param>
        /// <param name="Parameters"></param>
        public override List<I> GetDataList<I, T>(string CommandText, bool Automatic, params IDataParameter[] Parameters)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Nicht Implementiert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="CommandText"></param>
        /// <param name="Automatic"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [Obsolete("User method without ref!")]
        public override bool GetDataItem<T>(ref T item, string CommandText, bool Automatic, params IDataParameter[] Parameters)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Nicht Implementiert
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public override IDataParameter CreateParameter(string name, int type, int size)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Nicht Implementiert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CommandText"></param>
        /// <param name="Automatic"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public override T[] GetDataArray<T>(string CommandText, bool Automatic, params IDataParameter[] Parameters)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override T GetDataItem<T>(string commandText, bool automatic, params IDataParameter[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
