using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Diagnostics;
using Microsoft.CSharp;

using NUnit.Framework;
using MetadataReader;
using xmlGen;

namespace NTest
{
    [TestFixture]
    public class MetadataTest
    {
        COMReader comReader = null;
        MethodParser methodParser = null;
        string assemblyPath = null;
        
        
        static string csCode = @"
            [System.AttributeUsage(System.AttributeTargets.Method)]
            public class Export : System.Attribute { }

            namespace JS
            {
                public class App
                {
                    [Export]
                    public static void doClick() {}
                    [Export]
                    public static void doTest() {}
                }
            }
            ";

        string[] expectedMethodNames = { "doClick", "doTest" };

        string createAssembly()
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            ICodeCompiler icc = codeProvider.CreateCompiler();
            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.OutputAssembly = "AutoGen.dll";
            CompilerResults results = icc.CompileAssemblyFromSource(parameters, csCode);

            return parameters.OutputAssembly;
        }

        [TestFixtureSetUp]
        public void Init()
        {
            assemblyPath = createAssembly();
            comReader = new COMReader(assemblyPath);
        }

        [Test]
        public void testMetadataType()
        {
            HashSet<string> typeNames = new HashSet<string>();
            foreach(MetadataType type in comReader.types)
                typeNames.Add(type.name);

            string[] expected = { "Export", "JS.App"};
            Assert.AreEqual(expected, typeNames.ToArray());
        }

        [Test]
        public void testCustomAttribute()
        {
            HashSet<string> methodNames = new HashSet<string>();
            HashSet<string> typeNames = new HashSet<string>();
            foreach(MetadataCustomAttribute attribute in comReader.getCustomAttributesContaining(COMReader.ExportAttribute))
            {
                methodNames.Add(attribute.method.name);
                typeNames.Add(attribute.method.typeName);
            }

            string[] expectedTypeNames = { "JS.App" };

            Assert.AreEqual(expectedTypeNames, typeNames.ToArray());
            Assert.AreEqual(expectedMethodNames, methodNames.ToArray());
        }

        [Test]
        public void testMethodWithCustomAttribute()
        {
            HashSet<string> methodNames = new HashSet<string>();
            HashSet<string> typeNames = new HashSet<string>();
            foreach (MetadataMethod method in comReader.getMethodsWithCustomAttribute(COMReader.ExportAttribute))
            {
                methodNames.Add(method.name);
                typeNames.Add(method.typeName);
            }

            string[] expectedMethodNames = { "doClick", "doTest" };
            string[] expectedTypeNames = { "JS.App" };

            Assert.AreEqual(expectedTypeNames, typeNames.ToArray());
            Assert.AreEqual(expectedMethodNames, methodNames.ToArray());
        }

        [Test]
        public void testXML()
        {
            MethodParser methodParser = new MethodParser(assemblyPath);
            string expectedXML = "<MethodParser><methods><Method><name>doClick</name><parameters /><returnType>void</returnType><className>App</className><namespaceName>JS</namespaceName></Method><Method><name>doTest</name><parameters /><returnType>void</returnType><className>App</className><namespaceName>JS</namespaceName></Method></methods><namespaces><string>JS</string></namespaces><classNames><string>App</string></classNames></MethodParser>";
            Assert.AreEqual(expectedXML, methodParser.writeToXML());
        }

        [Test]
        public void testNamespaces()
        {
            MethodParser methodParser = new MethodParser(assemblyPath);
            string[] expectedNamespaces = { "JS" };
            Assert.AreEqual(expectedNamespaces, methodParser.namespaces.ToArray());

            foreach(string namespaceName in methodParser.namespaces)
                Assert.AreEqual(expectedMethodNames,methodParser.getMethodsByNamespace(namespaceName).Select(method => method.name).ToArray());
        }

        [Test]
        public void testClassNames()
        {
            MethodParser methodParser = new MethodParser(assemblyPath);
            string[] expectedClassNames = { "App" };
            Assert.AreEqual(expectedClassNames, methodParser.classNames.ToArray());

            foreach (string className in methodParser.classNames)
                Assert.AreEqual(expectedMethodNames, methodParser.getMethodsByClassName(className).Select(method => method.name).ToArray());
        }
    }
}
