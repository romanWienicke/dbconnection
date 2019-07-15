using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.ComponentModel;
using System.Data;

namespace ConneXion.Data.Manager
{
    /// <summary>
    /// Basis für alle DataManager Klassen
    /// </summary>
    public interface IDataManager<T> 
    {

        /// <summary>
        /// Element Einfügen
        /// </summary>
        /// <param name="item"></param>
        [DataObjectMethodAttribute(DataObjectMethodType.Insert, true)]
        T Insert(T item);
        /// <summary>
        /// Ein Element holen
        /// </summary>
        /// <param name="id">Id des Elements</param>
        /// <returns></returns>
        [DataObjectMethodAttribute(DataObjectMethodType.Select, false)]
        T Get(long id);
        /// <summary>
        /// Liste aller Elemente Holen
        /// </summary>
        /// <returns></returns>
        [DataObjectMethodAttribute(DataObjectMethodType.Select, true)]
        List<T> GetList(params IDataParameter[] parameters);
        /// <summary>
        /// Liste aller Elemente von / bis Id holen
        /// </summary>
        /// <param name="id">von /bis Id</param>
        /// <returns></returns>
        [DataObjectMethodAttribute(DataObjectMethodType.Delete, true)]
        bool Delete(T item);
        /// <summary>
        /// Element löschen
        /// </summary>
        /// <param name="id">Id des Elements</param>
        /// <returns></returns>
        bool Delete(long id);

        /// <summary>
        /// Element Speichern / Ändern
        /// </summary>
        /// <param name="item">Element</param>
        /// <returns></returns>
        [DataObjectMethodAttribute(DataObjectMethodType.Update, true)]
        T Save(T item);

        



    }

}
