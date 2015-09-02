using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

using Backend;

namespace xmlGen
{
    public static class Extensions
    {
        public static void test(this object obj)
        {

        }
    }
    
    class Program
    {
        private static string writeToXML(object obj)
        {
            XmlSerializer x = new XmlSerializer(obj.GetType());
            StringWriter sw = new StringWriter();
            XmlWriter xw = XmlWriter.Create(sw, new XmlWriterSettings
            {
                OmitXmlDeclaration = true
            });
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            x.Serialize(xw, obj, ns);
            return sw.ToString();
        }

        static void Main(string[] args)
        {
            string xmlPath = "";
            if (args.Length > 0)
            {
                xmlPath = args[0] + "\\"; 
            }

            JSHandler jsHandler = new JSHandler();

            string xml = writeToXML(jsHandler);

            StreamWriter file = new StreamWriter(xmlPath + "methods.xml");
            file.Write(xml);
            file.Close();
        }
    }
}
