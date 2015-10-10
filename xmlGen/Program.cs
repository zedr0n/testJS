using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using JSHandlers;
using MetadataReader;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace xmlGen
{
    public class MethodParser
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
            public readonly List<Param> parameters = new List<Param>();
            public string returnType;
            public string className;
            public string namespaceName;

            public bool shouldSerializeclassName() { return className != null; }
            public bool shouldSerializenamespaceName() { return namespaceName != null; }

            public Method() { }
            public Method(MethodInfo methodInfo)
                : this(methodInfo.Name, methodInfo, null, null) { }
            public Method(MethodInfo methodInfo, string className)
                : this(methodInfo.Name, methodInfo, null, className) { }
            public Method(string name, MethodInfo methodInfo)
                : this(name, methodInfo, null, null) { }
            public Method(string name, MethodInfo methodInfo,string namespaceName, string className)
            {
                this.name = name;
                this.returnType = methodInfo.ReturnType.Name.ToLower();
                this.namespaceName = namespaceName;
                this.className = className;
                foreach (ParameterInfo paramInfo in methodInfo.GetParameters())
                    parameters.Add(new Param(paramInfo.ParameterType.Name.ToLower(), paramInfo.Name));
            }
            // default void method with no parameters
            public Method(string name)
                : this(name, null, null) { }
            public Method(string name, string namespaceName, string className)
            {
                this.name = name;
                this.returnType = "void";
                this.namespaceName = namespaceName;
                this.className = className;
            }
            // explicit method constructor
            public Method(string name, string namespaceName, string className, string returnType, string[] parameterTypes, string[] parameterNames)
            {
                this.name = name;
                this.returnType = returnType;
                for(int i = 0; i < parameterNames.Length; ++i)
                    parameters.Add(new Param(parameterTypes[i], parameterNames[i]));
            }
        }

        public List<Method> methods = new List<Method>();
        [XmlIgnore]
        public List<string> namespaces
        {
            get { return methods.Select(method => method.namespaceName).Distinct().ToList(); }
        }
        [XmlIgnore]
        public List<string> classNames
        {
            get { return methods.Select(method => method.className).Distinct().ToList(); }
        }

        public List<Method> getMethodsByNamespace(string namespaceName)
        {
            return methods.Where(method => method.namespaceName == namespaceName).ToList();
        }

        public List<Method> getMethodsByClassName(string className)
        {
            return methods.Where(method => method.className == className).ToList();
        }

        private MethodParser() { }
        // automatically parse all the derived handlers from the JSHandlers assembly
        public MethodParser(Type T)
        {
            if(T != typeof(JSHandler))
                return;

            foreach (var jsHandler in from type in T.Assembly.GetTypes() where type.IsSubclassOf(T) 
                                      let constructorInfo = type.GetConstructor(new Type[] { }) 
                                      where constructorInfo != null select (JSHandler)constructorInfo.Invoke(new object[] { }))
            {
                addMethods(jsHandler);
            }
        }

        public MethodParser(JSHandler jsHandler)
        {
            addMethods(jsHandler);
        }
        // parse the assembly definitions using metadata reader
        public MethodParser(string assemblyPath)
        {
            var comReader = new ComReader(assemblyPath);

            foreach (MetadataMethod method in comReader.getMethodsWithCustomAttribute(ComReader.exportAttribute))
            {
                var tokens = method.typeName.Split('.');
                if(tokens.Length > 1)
                    methods.Add(new Method(method.name,tokens[0],tokens[1]));
            }
        }

        public void addMethods(JSHandler jsHandler)
        {
            foreach (JSMethodHandler method in jsHandler.handlers)
                methods.Add(new Method(method.name, method.methodInfo));
        }
        public string writeToXml()
        {
            var x = new XmlSerializer(GetType());
            var sw = new StringWriter();
            var xw = XmlWriter.Create(sw, new XmlWriterSettings
            {
                OmitXmlDeclaration = true
            });
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            x.Serialize(xw, this, ns);
            return sw.ToString();
        }
    }

    static class Program
    {
        // ReSharper disable once InconsistentNaming
        static void Main(string[] args)
        {
            string xmlPath = "";
            string assemblyPath = "";
            if (args.Length > 0)
            {
                if(args[0].Last() != '\\')
                    xmlPath = args[0] + "\\";
            }
            if (args.Length > 1)
                assemblyPath = args[1];

            StreamWriter file = new StreamWriter(xmlPath + "methods.xml");
            MethodParser methodParser = assemblyPath == "" ? new MethodParser(typeof(JSHandler)) : new MethodParser(assemblyPath);
            file.Write(methodParser.writeToXml());
            file.Close();
        }
    }
}
