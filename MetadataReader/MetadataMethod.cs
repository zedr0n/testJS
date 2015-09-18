using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataReader
{
    public class MetadataMethod
    {
        class MethodProps
        {
            public string name
            {
                get
                {
                    return new string(szMethod, 0, Convert.ToInt32(pchMethod)).Replace("\0",string.Empty);
                }
            }

            public char[] szMethod = new char[1024];
            public uint pchMethod = 0;
            public uint pwdAttr = 0;
            public IntPtr ppvSigBlob = IntPtr.Zero;
            public uint pcbSigBlob = 0;
            public uint pulCodeRVA = 0;
            public uint pdwImplFlags = 0;

            public uint classToken = 0;
        }

        private IMetaDataImport import = null;
        private uint token = 0;

        private string _name = null;
        private string _className = null;
        private List<MetadataCustomAttribute> _attributes = null;

        private MethodProps methodProps = null;

        public string name
        {
            get {
                if(_name == null)
                    getProps();
                _name = methodProps.name;
                return _name;
            }
        }

        public string className
        {
            get
            {
                if (_className == null)
                    getProps();
                MetadataType type = new MetadataType(import, methodProps.classToken);
                return type.name;
            }
        }

        public List<MetadataCustomAttribute> attributes
        {
            get 
            {
                if (_attributes == null)
                    enumerateCustomAttributes();
                return _attributes;
            }
        }

        public MetadataMethod() { }
        public MetadataMethod(IMetaDataImport import, uint token)
        {
            this.import = import;
            this.token = token;
        }

        private void getProps()
        {
            if (import == null || token == 0)
                return;

            methodProps = new MethodProps();
            import.GetMethodProps(token, out methodProps.classToken, methodProps.szMethod, Convert.ToUInt32(methodProps.szMethod.Length), out methodProps.pchMethod, out methodProps.pwdAttr, out methodProps.ppvSigBlob, out methodProps.pcbSigBlob, out methodProps.pulCodeRVA, out methodProps.pdwImplFlags);
        }

        private void enumerateCustomAttributes()
        {
            // Handle of enumeration
            uint enumHandle = 0;

            uint tkType = 0;
            uint[] attributes = new uint[10];
            uint attributeCount = 0;

            import.EnumCustomAttributes(ref enumHandle, token, tkType, attributes, Convert.ToUInt32(attributes.Length), out attributeCount);

            _attributes = new List<MetadataCustomAttribute>();

            for (uint attributeIndex = 0; attributeIndex < attributeCount; ++attributeIndex)
                _attributes.Add(new MetadataCustomAttribute(import, attributes[attributeIndex], token));

            import.CloseEnum(enumHandle);
        }
    }
}
