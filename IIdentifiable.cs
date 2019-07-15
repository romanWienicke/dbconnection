using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConneXion.Data
{
    /// <summary>
    /// Interface für Datenbank Objekte
    /// </summary>
    public interface IIdentifiable : IEquatable<IIdentifiable>
    {
        /// <summary>
        /// Datenbank Id
        /// </summary>
        [DBProperty(true)]
        int Id
        {
            get;
            set;
        }
    }
}
