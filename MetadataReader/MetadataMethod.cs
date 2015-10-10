using System;
using System.Collections.Generic;

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
            public uint pulCodeRva = 0;
            public uint pdwImplFlags = 0;

            public uint classToken = 0;
        }

        private readonly IMetaDataImport _import = null;
        private readonly uint _token = 0;

        private string _name = null;
        private readonly string _typeName = null;
        private List<MetadataCustomAttribute> _attributes = null;

        private MethodProps _methodProps;

        public string name
        {
            get {
                if(_name == null)
                    getProps();
                _name = _methodProps.name;
                return _name;
            }
        }

        public string typeName
        {
            get
            {
                if (_typeName == null)
                    getProps();
                var type = new MetadataType(_import, _methodProps.classToken);
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
            this._import = import;
            this._token = token;
        }

        private void getProps()
        {
            if (_import == null || _token == 0)
                return;

            _methodProps = new MethodProps();
            _import.GetMethodProps(_token, out _methodProps.classToken, _methodProps.szMethod, Convert.ToUInt32(_methodProps.szMethod.Length), out _methodProps.pchMethod, out _methodProps.pwdAttr, out _methodProps.ppvSigBlob, out _methodProps.pcbSigBlob, out _methodProps.pulCodeRva, out _methodProps.pdwImplFlags);
        }

        private void enumerateCustomAttributes()
        {
            // Handle of enumeration
            uint enumHandle = 0;

            uint tkType = 0;
            uint[] customAttributes = new uint[10];
            uint attributeCount = 0;

            _import.EnumCustomAttributes(ref enumHandle, _token, tkType, customAttributes, Convert.ToUInt32(customAttributes.Length), out attributeCount);

            _attributes = new List<MetadataCustomAttribute>();

            for (uint attributeIndex = 0; attributeIndex < attributeCount; ++attributeIndex)
                _attributes.Add(new MetadataCustomAttribute(_import, customAttributes[attributeIndex], _token));

            _import.CloseEnum(enumHandle);
        }
    }
}
