using System;
using System.Collections.Generic;
using System.Text;

namespace ConneXion.Data
{
    /// <summary>
    /// Attribut zur Umwandlung einer Property in einen DbParameter
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class DBPropertyAttribute : Attribute
    {
        /// <summary>
        /// Ausgabeparameter
        /// </summary>
        public bool IsOutputParameter { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public DBPropertyAttribute()
        {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="isOutputParameter">Ist Ausgabeparameter</param>
        public DBPropertyAttribute(bool isOutputParameter)
        {
            IsOutputParameter = isOutputParameter;
        }


    }
}
