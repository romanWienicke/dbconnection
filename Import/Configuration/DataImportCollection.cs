using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


namespace ConneXion.Data.Import.Configuration
{
    public class DataImportCollection : ConfigurationElementCollection
    {
        public DataImportCollection()
        {
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }
   
        protected override ConfigurationElement CreateNewElement()
        {
            return new Column();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            Column columnConfig = (Column)element;

            return columnConfig.Name;
        }


        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new Column(elementName);
        }


        public Column this[int index]
        {
            get
            {
                return (Column)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public Column this[string Name]
        {
            get
            {
                return (Column)BaseGet(Name);
            }
        }

        protected override string ElementName
        {
            get { return "column"; }
        }


        
    }
}
