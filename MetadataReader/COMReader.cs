using System;
using System.Collections.Generic;
using System.Linq;

namespace MetadataReader
{
    public class ComReader
    {
        public static string exportAttribute = "Export";

        readonly IMetaDataImport _import;

        private List<MetadataType> _types;

        public List<MetadataType> types
        {
            get
            {
                if (_types == null)
                    enumerateTypeDefinitions();
                return _types;
            }
        }

        private ComReader() { }
        public ComReader(string assemblyPath)
        {
            IMetaDataDispenserEx dispenser = new MetaDataDispenserEx();
            object rawScope;
            //GUID of the IMetaDataImport interface. 
            var metaDataImportGuid = new Guid("7DAC8207-D3AE-4c75-9B67-92801A497D44");

            //Open the assembly. 
            dispenser.OpenScope(assemblyPath, 0, ref metaDataImportGuid, out rawScope);
            //The rawScope contains an IMetaDataImport interface. 
            _import = (IMetaDataImport)rawScope;
        }
        public ComReader(IMetaDataImport import)
        {
            _import = import;
        }

        public List<MetadataCustomAttribute> getCustomAttributesContaining(string name)
        {
            List<MetadataCustomAttribute> list = new List<MetadataCustomAttribute>();

            foreach (MetadataType type in types)
                foreach (MetadataMethod method in type.methods)
                    list.AddRange(method.attributes.Where(attribute => attribute.name.Contains(name)));

            return list;
        }
        public List<MetadataMethod> getMethodsWithCustomAttribute(string attributeName)
        {
            List<MetadataMethod> list = new List<MetadataMethod>();

            foreach (MetadataType type in types)
                foreach (MetadataMethod method in type.methods)
                    list.AddRange(method.attributes
                                    .Where(attribute => attribute.name.Contains(attributeName))
                                    .Select(attribute => attribute.method)
                                    );

            return list;
        }

        void enumerateTypeDefinitions()
        {
            //Handle of the enumeration. 
            uint enumHandle = 0;
            //We will read maximum 10 TypeDefs at once which will be stored in this array. 
            var typeDefs = new uint[10];
            //Number of read TypeDefs. 
            uint count;

            _import.EnumTypeDefs(ref enumHandle, typeDefs, Convert.ToUInt32(typeDefs.Length), out count);

            _types = new List<MetadataType>();
            //Continue reading TypeDef's while he typeDefs array contains any new TypeDef. 
            while (count > 0)
            {
                for (uint typeDefsIndex = 0; typeDefsIndex < count; typeDefsIndex++)
                    _types.Add(new MetadataType(_import, typeDefs[typeDefsIndex]));

                _import.EnumTypeDefs(ref enumHandle, typeDefs, Convert.ToUInt32(typeDefs.Length), out count);
            }

            _import.CloseEnum(enumHandle);
        }
    }
}
