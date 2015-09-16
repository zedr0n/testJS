using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using JS;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MetadataReader
{
    class Program
    {
        private static string attributeName = "Export"; 

        private static void EnumerateCustomAttributes(IMetaDataImport import, uint scope)
        {
            // Handle of enumeration
            uint enumHandle = 0;

            uint tkType = 0;
            uint[] attributes = new uint[10];
            uint attributeCount = 0;
            IntPtr ppBlob = IntPtr.Zero;
            uint pcbSize = 0;

            import.EnumCustomAttributes(ref enumHandle, scope, tkType, attributes, Convert.ToUInt32(attributes.Length), out attributeCount);

            for(uint attributeIndex = 0; attributeIndex < attributeCount; ++attributeIndex)
            {
                uint attribute = attributes[attributeIndex];

                uint objectToken = 0;
                uint ptkType = 0;


                import.GetCustomAttributeProps(attribute, out objectToken, out ptkType, out ppBlob, out pcbSize);

                string attrName = new MetadataMethod(import, ptkType).className;
                if (attrName.Contains(attributeName))
                    Console.WriteLine(new MetadataMethod(import,scope).name + " : " + attrName);
            }
        }
        private static void EnumerateMethods(IMetaDataImport import, uint typeDef)
        {
            // Handle of enumeration
            uint enumHandle = 0;

            uint[] methods = new uint[10];

            // get the methods
            uint methodCount = 0;
            import.EnumMethods(ref enumHandle, typeDef, methods, Convert.ToUInt32(methods.Length), out methodCount);

            for (uint methodIndex = 0; methodIndex < methodCount; ++methodIndex)
            {
                uint token = methods[methodIndex];

                MetadataMethod method = new MetadataMethod(import, token);

                EnumerateCustomAttributes(import, token);
            }
                
        }
        private static void EnumerateTypeDefinitions(IMetaDataImport import)
        {
            //Handle of the enumeration. 
            uint enumHandle = 0;
            //We will read maximum 10 TypeDefs at once which will be stored in this array. 
            uint[] typeDefs = new uint[10];
            //Number of read TypeDefs. 
            uint count = 0;

            import.EnumTypeDefs(ref enumHandle, typeDefs, Convert.ToUInt32(typeDefs.Length), out count);

            //Continue reading TypeDef's while he typeDefs array contains any new TypeDef. 
            while (count > 0)
            {
                for (uint typeDefsIndex = 0; typeDefsIndex < count; typeDefsIndex++)
                {
                    uint token = typeDefs[typeDefsIndex];

                    MetadataType type = new MetadataType(import, token);
                    //Write the TypeDef's name to the console. 
                    //Console.WriteLine(type.name);

                    EnumerateMethods(import, token);


                }

                import.EnumTypeDefs(ref enumHandle, typeDefs, Convert.ToUInt32(typeDefs.Length), out count);
            }

            import.CloseEnum(enumHandle);
        } 

        static string getAssemblyPath(string assemblyName)
        {
            Assembly jsAssembly = Assembly.Load("JS");
            UriBuilder uri = new UriBuilder(jsAssembly.CodeBase);
            return Uri.UnescapeDataString(uri.Path);
        }

        static void Main(string[] args)
        {

            string assemblyPath = getAssemblyPath("JS");

            IMetaDataDispenserEx dispenser = new MetaDataDispenserEx();
            IMetaDataImport import = null;
            object rawScope = null;
            //GUID of the IMetaDataImport interface. 
            Guid metaDataImportGuid = new Guid("7DAC8207-D3AE-4c75-9B67-92801A497D44");

            //Open the assembly. 
            dispenser.OpenScope(assemblyPath, 0, ref metaDataImportGuid, out rawScope);
            //The rawScope contains an IMetaDataImport interface. 
            import = (IMetaDataImport)rawScope;

            //Write to the console the TypeDefs by calling a method. 
            EnumerateTypeDefinitions(import); 
        }
    }
}
