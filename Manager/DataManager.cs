using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ConneXion.Data.Manager
{
    public abstract class DataManager<T> : BaseManager, IDataManager<T> where T : IIdentifiable, new()
    {
        protected CommandType InsertCommandType { get; set; }
        protected string InsertCommand { get; set; }
        protected CommandType GetCommandType { get; set; }
        protected string GetCommand { get; set; }
        protected CommandType GetListCommandType { get; set; }
        protected string GetListCommand { get; set; }
        protected CommandType DeleteCommandType { get; set; }
        protected string DeleteCommand { get; set; }
        protected CommandType SaveCommandType { get; set; }
        protected string SaveCommand { get; set; }


        /// <summary>
        /// DatenElement Einfügen
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sqlString">muss @id zurückgeben!</param>
        [System.ComponentModel.DataObjectMethodAttribute
            (System.ComponentModel.DataObjectMethodType.Insert, true)]
        public T Insert(T item)
        {
            using (BaseDbConnection conn = GetConnection(CommandType.Text))
            {
                conn.CommandType = InsertCommandType;
                IDataParameter[] parameters = conn.PropertiesToParameter(item);
                conn.ExecuteNonQuery(InsertCommand, parameters);
                item.Id = Convert.ToInt32(BaseDbConnection.GetOutputParameterData(parameters)["@Id"]);
                return item;
            }
        }

        public T Insert(params IDataParameter[] parameters)
        {
            using (BaseDbConnection conn = GetConnection(CommandType.Text))
            {
                conn.CommandType = InsertCommandType;
                T item = BaseDbConnection.CreateInstance<T>(parameters);
                conn.ExecuteNonQuery(InsertCommand, parameters);
                item.Id = Convert.ToInt32(BaseDbConnection.GetOutputParameterData(parameters)["@Id"]);
                return item;
            }
        }

        [System.ComponentModel.DataObjectMethodAttribute
            (System.ComponentModel.DataObjectMethodType.Select, false)]
        public T Get(long id)
        {
            using (BaseDbConnection conn = GetConnection(CommandType.Text))
            {
                conn.CommandType = GetCommandType;
                return conn.GetDataItem<T>(GetCommand, conn.CreateParameter("id", id));
                
            }
        }

        public T Get(params IDataParameter[] parameters)
        {
            using (BaseDbConnection conn = GetConnection(CommandType.Text))
            {
                conn.CommandType = GetCommandType;
                return conn.GetDataItem<T>(GetCommand, parameters);
                
            }
        }

        [System.ComponentModel.DataObjectMethodAttribute
            (System.ComponentModel.DataObjectMethodType.Select, false)]
        public List<T> GetList(params IDataParameter[] parameters)
        {
            using (BaseDbConnection conn = GetConnection(CommandType.Text))
            {
                conn.CommandType = GetListCommandType;
                return conn.GetDataList<T>(GetListCommand, parameters);
            }
        }

        [System.ComponentModel.DataObjectMethodAttribute
            (System.ComponentModel.DataObjectMethodType.Delete, true)]
        public bool Delete(T item)
        {
            return Delete(item.Id);
        }

        public bool Delete(long id)
        {
            using (BaseDbConnection conn = GetConnection(CommandType.Text))
            {
                conn.CommandType = DeleteCommandType;
                return conn.ExecuteNonQuery(DeleteCommand, conn.CreateParameter("id", id)) > 0;
            }
        }

        public bool Delete(params IDataParameter[] parameters)
        {
            using (BaseDbConnection conn = GetConnection(CommandType.Text))
            {
                conn.CommandType = DeleteCommandType;
                return conn.ExecuteNonQuery(DeleteCommand, parameters) > 0;
            }
        }

        [System.ComponentModel.DataObjectMethodAttribute
            (System.ComponentModel.DataObjectMethodType.Update, true)]
        public T Save(T item)
        {
            using (BaseDbConnection conn = GetConnection(CommandType.Text))
            {
                conn.CommandType = SaveCommandType;
                conn.ExecuteNonQuery(SaveCommand, conn.PropertiesToParameter(item));
                return item;
            }
        }

        public T Save(params IDataParameter[] parameters)
        {
            using (BaseDbConnection conn = GetConnection(CommandType.Text))
            {
                conn.CommandType = SaveCommandType;
                conn.ExecuteNonQuery(SaveCommand, parameters);
                return BaseDbConnection.CreateInstance<T>(parameters);
            }
        }
        
        [System.ComponentModel.DataObjectMethodAttribute
           (System.ComponentModel.DataObjectMethodType.Select, true)]
        public List<T> GetList()
        {
            using (BaseDbConnection conn = GetConnection(CommandType.Text))
            {
                conn.CommandType = GetListCommandType;
                return conn.GetDataList<T>(GetListCommand);
            }
        }

       
    }
}
