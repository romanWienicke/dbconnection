using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ConneXion.Data.Manager
{
    public class DataManagerExtended<I, T> : DataManager<T> where T : I, IIdentifiable, new() where I: IIdentifiable
    {
        public new List<I> GetList()
        {
            using (BaseDbConnection conn = GetConnection(CommandType.Text))
            {
                conn.CommandType = GetListCommandType;
                return conn.GetDataList<I, T>(GetListCommand);
            }
        }

    
    }
}
