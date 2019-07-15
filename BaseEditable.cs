using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using ConneXion.Data.Manager;

namespace ConneXion.Data
{
    /// <summary>
    /// Basisklasse für direkt bearbeitbare Objekte
    /// </summary>
    public abstract class BaseEditable : IIdentifiable, IEditableObject, ICloneable
    {
        private IIdentifiable Backup { get; set; }
        protected IDataManager<IIdentifiable> DataManager { get; set; }
        
        #region ICloneable Member

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

        #region IBaseIdentifiable Member
        /// <summary>
        /// Datenbank Id
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        #endregion

        #region IEquatable<IBaseIdentifiable> Member

        public bool Equals(IIdentifiable other)
        {
            Type type = GetType();
            PropertyInfo[] info = type.GetProperties();

            for (int i = 0; i < info.Length; i++)
            {
                object original = info[i].GetValue(this, null);
                object copy = info[i].GetValue(other, null);

                if ((original == null && copy == null) || original.Equals(copy))
                    continue;
                else
                    return false;
            }
            return true;
        }

        #endregion

        #region IEditableObject Member

        public void BeginEdit()
        {
            Backup = (IIdentifiable)this.Clone();
        }

        public void CancelEdit()
        {
            if (Backup == null) return;

            PropertyInfo[] info = GetType().GetProperties();

            for (int i = 0; i < info.Length; i++)
            {
                info[i].SetValue(this, info[i].GetValue(Backup, null), null);
            }
        }

        public void EndEdit()
        {
            if (!this.Equals(Backup)) return;

            if (this.Id <= 0)
            {
                DataManager.Insert(this);
            }
            else
            {
                DataManager.Save(this);
            }
            Backup = (IIdentifiable)Clone();
        }

       
        #endregion


    }
}
