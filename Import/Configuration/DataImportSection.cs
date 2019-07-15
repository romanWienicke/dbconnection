using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ConneXion.Data.Import.Configuration
{
    public class DataImportSection : ConfigurationSection
    {
        [ConfigurationProperty("name", DefaultValue = "DataImportConfigurationSection", IsRequired = false, IsKey = false)]
        [StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
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

        [ConfigurationProperty("mappings", IsDefaultCollection = false)]
        public DataImportCollection Mappings
        {
            get
            {
                return (DataImportCollection)base["mappings"];
            }
        }

        public List<ColumnInfo> ColumnInfos
        {
            get
            {
                List<ColumnInfo> ColumnInfos = new List<ColumnInfo>();
                DataImportSection config = (DataImportSection)ConfigurationManager.GetSection("dataImport");
                foreach (Column column in config.Mappings)
                {
                    ColumnInfos.Add((ColumnInfo)column);
                }
                return ColumnInfos;
            }
        }
    }
}
