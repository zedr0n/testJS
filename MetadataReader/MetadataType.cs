using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataReader
{
    class MetadataType
    {
        private IMetaDataImport import = null;
        private uint token = 0;

        private string _name = null;
        private List<MetadataMethod> _methods = null;

        public string name
        {
            get {
                if(_name == null)
                    getProps();
                return _name;
            }
            set { _name = value; }
        }
        public List<MetadataMethod> methods
        {
            get
            {
                if(_methods == null)
                    enumerateMethods();
                return _methods;
            }
        }

        public MetadataType() { }
        public MetadataType(IMetaDataImport import, uint token)
        {
            this.import = import;
            this.token = token;
        }

        public void enumerateMethods()
        {
            if(import == null || token == null)
                return;

            // Handle of enumeration
            uint enumHandle = 0;

            uint[] methods = new uint[10];

            // get the methods
            uint methodCount = 0;
            import.EnumMethods(ref enumHandle, token, methods, Convert.ToUInt32(methods.Length), out methodCount);

            _methods = new List<MetadataMethod>();

            for (uint methodIndex = 0; methodIndex < methodCount; ++methodIndex)
                _methods.Add(new MetadataMethod(import,methods[methodIndex]));

            import.CloseEnum(enumHandle);
        }

        private void getProps()
        {
            if (import == null || token == 0)
                return;

            //The TypeDef's name will be stored in this array. The 1024 is a "magical number", seems like a type's name can be maximum this long. The corhlpr.h also defines a suspicious constant like this: #define MAX_CLASSNAME_LENGTH 1024 
            char[] typeName = new char[1024];
            //Number of how many characters were filled in the typeName array. 
            uint nameLength;
            //TypeDef's flags. 
            uint typeDefFlags;
            //If the TypeDef is a derived type then the base type's token. 
            uint baseTypeToken;

            //Get the TypeDef's properties. 
            import.GetTypeDefProps(token, typeName, Convert.ToUInt32(typeName.Length), out nameLength, out typeDefFlags, out baseTypeToken);

            //Get the TypeDef's name. 
            name = new string(typeName, 0, Convert.ToInt32(nameLength)).Replace("\0",string.Empty);
        }
    }
}
