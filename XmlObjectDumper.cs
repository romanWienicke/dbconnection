using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;

namespace ConneXion.Data
{
    /// <summary>
    /// Class for serializing objects to an XDocument
    /// </summary>
    public class XmlObjectDumper
    {
        /// <summary>
        /// Entry point 
        /// </summary>
        /// <param name="element">any object</param>
        /// <returns>the serialized data</returns>
        public static XDocument CreateXml(object element)
        {
            Type type = element.GetType();
            return new XDocument(CreateElementXml(element));  
        }

        /// <summary>
        /// Creates an XElement 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static XElement CreateElementXml(object element)
        {
            

            Type type = element.GetType();
            string typeName = Regex.Replace(type.Name, @"\W", "");
            
            if (type.IsArray)
            {
                XElement arrElement = new XElement(string.Format("{0}Array", typeName));
                
                foreach (object item in (Array)element)
                {
                    arrElement.Add(CreateElementXml(item));
                }
                return arrElement;
            }

            XElement xElement = new XElement(typeName);
            MemberInfo[] members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);

            foreach (MemberInfo m in members)
            {
                FieldInfo f = m as FieldInfo;
                PropertyInfo p = m as PropertyInfo;

                if (f != null || p != null)
                {
                    Type t = f != null ? f.FieldType : p.PropertyType;
                    if (t.IsValueType || t == typeof(string))
                    {
                        xElement.Add(
                            CreateMemberData(m.Name, f != null ? f.GetValue(element) : p.GetValue(element, null))
                        );
                    }
                    else
                    {
                        object elementValue = f != null ? f.GetValue(element) : p.GetValue(element, null);
                        if(elementValue != null)
                            xElement.Add(CreateElementXml(elementValue));
                    }

                }
            }
            return xElement;
        }

        /// <summary>
        /// Writes memberdate as XAttribures
        /// </summary>
        /// <param name="name">member name</param>
        /// <param name="value">member vlaue</param>
        /// <returns>member info as XAttribute</returns>
        private static XAttribute CreateMemberData(string name, object value)
        {
            if (value == null)
                value = string.Empty;

            return new XAttribute(name, value);
        }

       
    }
}
