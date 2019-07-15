using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Web;


namespace ConneXion.Data.Import.Configuration
{

    public class Column : ConfigurationElement
    {
         // Constructor allowing name, url, and port to be specified.
        public Column(string name)
        {
            Name = name;

        }

        // Default constructor, will use default values as defined
        // below.
        public Column()
        {
        }

        [ConfigurationProperty("name", IsRequired = false, IsKey = true)]
        public string Name
        { 
         get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("displayName", IsRequired = true, IsKey = true)]
        public string DisplayName
        {
            get
            {
                return (string)this["displayName"];
            }
            set
            {
                this["displayName"] = value;
            }
        }

        public static explicit operator ColumnInfo(Column config)
        {
            return new ColumnInfo(config.Name, config.DisplayName);
        }


         
    }
}
