using System;

namespace MetadataReader
{
    public class MetadataCustomAttribute
    {
        private readonly IMetaDataImport _import;
        private readonly uint _token;

        private string _name;
        private readonly MetadataMethod _method;

        public MetadataCustomAttribute() { }
/*
        public MetadataCustomAttribute(IMetaDataImport import, uint token)
        {
            this._import = import;
            this._token = token;
        }
*/
        public MetadataCustomAttribute(IMetaDataImport import, uint token, uint methodToken)
        {
            _import = import;
            _token = token;
            if(methodToken != 0)
                _method = new MetadataMethod(import, methodToken);
        }

        public string name
        {
            get
            {
                if (_name == null)
                    getProps();
                return _name;
            }
        }

        public MetadataMethod method
        {
            get
            {
                if (_method == null)
                    getProps();
                return _method;
            }
        }

        private void getProps()
        {
            if (_import == null || _token == 0)
                return;

            IntPtr ppBlob;
            uint pcbSize;
            uint objectToken;
            uint ptkType;

            _import.GetCustomAttributeProps(_token, out objectToken, out ptkType, out ppBlob, out pcbSize);
            _name = new MetadataMethod(_import, ptkType).typeName.Replace("\0",string.Empty);
        }
    }
}
