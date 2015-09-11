using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

using Awesomium.Core;
using JSHandlers;

namespace xmlGen
{
    public class Param
    {
        public string paramType;
        public string paramName;

        public Param() { }
        public Param(string paramType, string paramName)
        {
            this.paramType = paramType;
            this.paramName = paramName;
        }
    }
    public class Method
    {
        public string name;
        public List<Param> parameters = new List<Param>();
        public string returnType;

        public Method() { }

        public Method(MethodInfo methodInfo) : this(methodInfo.Name,methodInfo)
        {
        }

        public Method(string name, MethodInfo methodInfo)
        {
            this.name = name;
            returnType = methodInfo.ReturnType.Name.ToLower();
            foreach (ParameterInfo paramInfo in methodInfo.GetParameters())
            {
                parameters.Add(new Param(paramInfo.ParameterType.Name.ToLower(), paramInfo.Name));
            }

        }
    }

    public class jsObject
    {
        public List<Method> methods = new List<Method>();

        public jsObject() { }
        public jsObject(JSHandler jsHandler)
        {
            methods = new List<Method>();
            foreach(JSMethodHandler method in jsHandler.handlers)
                methods.Add(new Method(method.name,method.methodInfo));
        }

        public string writeToXML()
        {
            XmlSerializer x = new XmlSerializer(GetType());
            StringWriter sw = new StringWriter();
            XmlWriter xw = XmlWriter.Create(sw, new XmlWriterSettings
            {
                OmitXmlDeclaration = true
            });
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            x.Serialize(xw, this, ns);
            return sw.ToString();
        }
    }
    
    class Program
    {

        public static string getXML()
        {
            return (new jsObject(new ButtonHandler())).writeToXML();
        }

        static void Main(string[] args)
        {
            string xmlPath = "";
            if (args.Length > 0)
            {
                xmlPath = args[0] + "\\"; 
            }

            StreamWriter file = new StreamWriter(xmlPath + "methods.xml");
            file.Write(getXML());
            file.Close();
        }
    }
}
