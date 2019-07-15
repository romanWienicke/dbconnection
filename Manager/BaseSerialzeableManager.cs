using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ConneXion.Data.Manager
{
    /// <summary>
    /// Datamanager /w Serialisierung
    /// </summary>
    public class BaseSerialzeableManager : BaseManager
    {
        /// <summary>
        /// Serialisierung um Speichern von Objekten
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static byte[] BinarySerialize(object item)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            bf.Serialize(stream, item);
            stream.Dispose();
            return (stream.ToArray());
        }

        /// <summary>
        /// Deserialisierung um Laden von Objekten
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static object BinaryDeserialize(object data)
        {
            MemoryStream stream = new MemoryStream((byte[])data);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(stream);
            }
            finally
            {
                stream.Dispose();
            }
        }
    }
}
