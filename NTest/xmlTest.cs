using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using xmlGen;
using JSHandlers;

namespace NTest
{
    [TestFixture]
    public class xmlTest
    {
        private static string xmlResult = "<MethodParser><methods><Method><name>onClick</name><parameters><Param><paramType>string</paramType><paramName>textInput</paramName></Param></parameters><returnType>string</returnType></Method><Method><name>onTest</name><parameters /><returnType>void</returnType></Method><Method><name>onTest2</name><parameters><Param><paramType>double</paramType><paramName>x</paramName></Param><Param><paramType>double</paramType><paramName>y</paramName></Param></parameters><returnType>double</returnType></Method></methods></MethodParser>";

        [Test]
        public void testButtonXML()
        {
            Assert.AreEqual(xmlResult, (new MethodParser(new ButtonHandler())).writeToXML());
        }

        [Test]
        public void testXML()
        {
            Assert.AreEqual(xmlResult, (new MethodParser()).writeToXML());
        }
    }
}
