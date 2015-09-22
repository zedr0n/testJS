using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataReader
{
    public class MetadataCustomAttribute
    {
        private IMetaDataImport import = null;
        private uint token = 0;

        private string _name = null;
        private MetadataMethod _method = null;

        public MetadataCustomAttribute() { }
        public MetadataCustomAttribute(IMetaDataImport import, uint token)
        {
            this.import = import;
            this.token = token;
        }

        public MetadataCustomAttribute(IMetaDataImport import, uint token, uint methodToken)
        {
            this.import = import;
            this.token = token;
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
            if (import == null || token == 0)
                return;

            IntPtr ppBlob = IntPtr.Zero;
            uint pcbSize = 0;
            uint objectToken = 0;
            uint ptkType = 0;

            import.GetCustomAttributeProps(token, out objectToken, out ptkType, out ppBlob, out pcbSize);
            _name = new MetadataMethod(import, ptkType).typeName.Replace("\0",string.Empty);
        }
    }
}
