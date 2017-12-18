using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

using GraphDB.Contract.Serial;
using GraphDB.Core;

using SerializableAttribute = GraphDB.Contract.Serial.SerializableAttribute;

namespace GraphDB.Utility
{
    public static class SerializableHelper
    {
        public static XmlElement Serialize(XmlDocument doc, object obj)
        {
            PropertyInfo[] pInfos = obj.GetType().GetProperties();

            XmlElement xmlNode = doc.CreateElement(GetBaseType(obj));
            xmlNode.SetAttribute( XmlNames.Class, obj.GetType().FullName);
            foreach (var curItem in pInfos)
            {
                if (!IsSerialableProperty(curItem))
                {
                    continue;
                }
                XmlElement tag = doc.CreateElement(curItem.Name);
                string txt = (string)curItem.GetValue(obj);
                XmlText value = doc.CreateTextNode(txt);
                tag.AppendChild(value);
                xmlNode.AppendChild(tag);
            }
            return xmlNode;
        }

        public static object Deserialize(XmlElement xmlNode)
        {
            object[] parameters = new object[1];
            parameters[0] = xmlNode;
            string className = xmlNode.GetAttribute(XmlNames.Class);
            Assembly asm = GetAssembly(className);
            return asm?.CreateInstance(className, true, BindingFlags.Default, null, parameters, null, null);
        }

        private static bool IsSerialableProperty(PropertyInfo pInfo)
        {
            if (pInfo.CustomAttributes.Any(x => x.AttributeType.Name == typeof(SerializableAttribute).Name))
            {
                return true;
            }
            return false;
        }

        private static string GetBaseType(object obj)
        {
            if( obj is Node )
            {
                return XmlNames.Node;
            }
            if (obj is Edge)
            {
                return XmlNames.Edge;
            }
            return "";
        }

        private static Assembly GetAssembly( string typeName )
        {
            string path = Assembly.GetExecutingAssembly().Location;
            string asmName = Path.GetFileName( path );
            foreach ( string curItem in Properties.Settings.Default.SerialAssemblyList )
            {
                Assembly asm = Assembly.LoadFile(path.Replace( asmName, curItem ));
                if( asm.ExportedTypes.Any( x => x.FullName == typeName ) )
                {
                    return asm;
                }
            }
            throw new FileLoadException("No valid assembly has been found.");
        }


    }
}