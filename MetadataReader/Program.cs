using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace MetadataReader
{
    public class COMReader
    {
        public static string ExportAttribute = "Export"; 

        IMetaDataImport import = null;

        private List<MetadataType> _types = null;

        public List<MetadataType> types
        {
            get
            {
                if (_types == null)
                    EnumerateTypeDefinitions();
                return _types;
            }
        }

        private COMReader() {}
        public COMReader(string assemblyPath)
        {
            IMetaDataDispenserEx dispenser = new MetaDataDispenserEx();
            object rawScope = null;
            //GUID of the IMetaDataImport interface. 
            Guid metaDataImportGuid = new Guid("7DAC8207-D3AE-4c75-9B67-92801A497D44");

            //Open the assembly. 
            dispenser.OpenScope(assemblyPath, 0, ref metaDataImportGuid, out rawScope);
            //The rawScope contains an IMetaDataImport interface. 
            this.import = (IMetaDataImport)rawScope;
        }
        public COMReader(IMetaDataImport import)
        {
            this.import = import;
        }

        public List<MetadataCustomAttribute> EnumerateCustomAttributes(string name)
        {
            List<MetadataCustomAttribute> list = new List<MetadataCustomAttribute>();

            foreach (MetadataType type in types)
                foreach (MetadataMethod method in type.methods)
                    list.AddRange(method.attributes.Where(attribute => attribute.name.Contains(name)));

            return list;
        }
        void EnumerateTypeDefinitions()
        {
            //Handle of the enumeration. 
            uint enumHandle = 0;
            //We will read maximum 10 TypeDefs at once which will be stored in this array. 
            uint[] typeDefs = new uint[10];
            //Number of read TypeDefs. 
            uint count = 0;

            import.EnumTypeDefs(ref enumHandle, typeDefs, Convert.ToUInt32(typeDefs.Length), out count);

            _types = new List<MetadataType>();
            //Continue reading TypeDef's while he typeDefs array contains any new TypeDef. 
            while (count > 0)
            {
                for (uint typeDefsIndex = 0; typeDefsIndex < count; typeDefsIndex++)
                    _types.Add(new MetadataType(import, typeDefs[typeDefsIndex]));

                import.EnumTypeDefs(ref enumHandle, typeDefs, Convert.ToUInt32(typeDefs.Length), out count);
            }

            import.CloseEnum(enumHandle);
        } 
    }

    public class Exports
    {
        public List<string> methods = new List<string>();

        public Exports() { }
        
        public void add(string method)
        {
            methods.Add(method);
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

        static void Main(string[] args)
        {

            if (args.Length < 1)
                return;

            string assemblyPath = args[0];
            string xmlPath = "";
            if(args.Length > 1)
                xmlPath = args[1] + "\\";

            if (!File.Exists(assemblyPath))
                return;

            COMReader comReader = new COMReader(assemblyPath);

            //reader.EnumerateTypeDefinitions(); 
            //reader.EnumerateCustomAttributes(0);

            StreamWriter file = new StreamWriter(xmlPath + "exports.xml");
            Exports methods = new Exports();

            foreach (MetadataCustomAttribute attribute in comReader.EnumerateCustomAttributes(COMReader.ExportAttribute))
                methods.add(attribute.method.className + "." + attribute.method.name);

            file.Write(methods.writeToXML());
            file.Close();
        }
    }
}
