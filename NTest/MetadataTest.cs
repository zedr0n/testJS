using System.Collections.Generic;
using System.Linq;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

using NUnit.Framework;
using MetadataReader;
using xmlGen;
#pragma warning disable 618

namespace NTest
{
    [TestFixture]
    public class MetadataTest
    {
        ComReader _comReader;
        string _assemblyPath;
        
        
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

        private readonly string[] _expectedMethodNames = {"doClick", "doTest"};

        static string createAssembly()
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            var icc = codeProvider.CreateCompiler();
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateExecutable = false,
                OutputAssembly = "AutoGen.dll"
            };
            icc.CompileAssemblyFromSource(parameters, csCode);

            return parameters.OutputAssembly;
        }

        [TestFixtureSetUp]
        public void init()
        {
            _assemblyPath = createAssembly();
            _comReader = new ComReader(_assemblyPath);
        }

        [Test]
        public void testMetadataType()
        {
            HashSet<string> typeNames = new HashSet<string>();
            foreach(var type in _comReader.types)
                typeNames.Add(type.name);

            string[] expected = { "Export", "JS.App"};
            Assert.AreEqual(expected, typeNames.ToArray());
        }

        [Test]
        public void testCustomAttribute()
        {
            var methodNames = new HashSet<string>();
            var typeNames = new HashSet<string>();
            foreach(MetadataCustomAttribute attribute in _comReader.getCustomAttributesContaining(ComReader.exportAttribute))
            {
                methodNames.Add(attribute.method.name);
                typeNames.Add(attribute.method.typeName);
            }

            string[] expectedTypeNames = { "JS.App" };

            Assert.AreEqual(expectedTypeNames, typeNames.ToArray());
            Assert.AreEqual(_expectedMethodNames, methodNames.ToArray());
        }

        [Test]
        public void testMethodWithCustomAttribute()
        {
            HashSet<string> methodNames = new HashSet<string>();
            HashSet<string> typeNames = new HashSet<string>();
            foreach (MetadataMethod method in _comReader.getMethodsWithCustomAttribute(ComReader.exportAttribute))
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
        public void testXml()
        {
            MethodParser methodParser = new MethodParser(_assemblyPath);
            string expectedXML = "<MethodParser><methods><Method><name>doClick</name><parameters /><returnType>void</returnType><className>App</className><namespaceName>JS</namespaceName></Method><Method><name>doTest</name><parameters /><returnType>void</returnType><className>App</className><namespaceName>JS</namespaceName></Method></methods></MethodParser>";
            Assert.AreEqual(expectedXML, methodParser.writeToXml());
        }

        [Test]
        public void testNamespaces()
        {
            MethodParser methodParser = new MethodParser(_assemblyPath);
            string[] expectedNamespaces = { "JS" };
            Assert.AreEqual(expectedNamespaces, methodParser.namespaces.ToArray());

            foreach(string namespaceName in methodParser.namespaces)
                Assert.AreEqual(_expectedMethodNames,methodParser.getMethodsByNamespace(namespaceName).Select(method => method.name).ToArray());
        }

        [Test]
        public void testClassNames()
        {
            MethodParser methodParser = new MethodParser(_assemblyPath);
            string[] expectedClassNames = { "App" };
            Assert.AreEqual(expectedClassNames, methodParser.classNames.ToArray());

            foreach (string className in methodParser.classNames)
                Assert.AreEqual(_expectedMethodNames, methodParser.getMethodsByClassName(className).Select(method => method.name).ToArray());
        }
    }
}
