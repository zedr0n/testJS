using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

using Backend;

using Awesomium.Core;

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

        public Method(MethodInfo methodInfo)
        {
            name = methodInfo.Name;
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
            methods = jsHandler.parseMethods();
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
    public static class Extensions
    {
        public static List<Method> parseMethods(this JSHandler jsHandler)
        {
            List<string> methodNames = new List<string>();
            foreach (MethodInfo methodInfo in jsHandler.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (jsHandler.getDelegate<JavascriptMethodHandler>(methodInfo) != null)
                    methodNames.Add(methodInfo.Name);
            }

            List<Method> methods = new List<Method>();
            foreach (MethodInfo methodInfo in jsHandler.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (methodNames.Contains(methodInfo.Name) && jsHandler.getDelegate<JavascriptMethodHandler>(methodInfo) == null)
                {
                    Method method = new Method(methodInfo);
                    methods.Add(method);
                }
            }
            return methods;

        }
    }
    
    class Program
    {

        static void Main(string[] args)
        {
            string xmlPath = "";
            if (args.Length > 0)
            {
                xmlPath = args[0] + "\\"; 
            }

            string xml = (new jsObject(new JSHandler())).writeToXML();

            StreamWriter file = new StreamWriter(xmlPath + "methods.xml");
            file.Write(xml);
            file.Close();
        }
    }
}
