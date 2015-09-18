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

namespace NTest
{
    [TestFixture]
    public class MetadataTest
    {
        COMReader comReader = null;

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
            comReader = new COMReader(createAssembly());
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
            HashSet<string> classNames = new HashSet<string>();
            foreach(MetadataCustomAttribute attribute in comReader.getCustomAttributesContaining("Export"))
            {
                methodNames.Add(attribute.method.name);
                classNames.Add(attribute.method.className);
            }

            string[] expectedMethodNames = { "doClick", "doTest"};
            string[] expectedClassNames = { "JS.App" };

            Assert.AreEqual(expectedClassNames, classNames.ToArray());
            Assert.AreEqual(expectedMethodNames, methodNames.ToArray());
        }
    }
}
