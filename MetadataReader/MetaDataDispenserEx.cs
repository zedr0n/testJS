﻿using System.Runtime.InteropServices; 

namespace MetadataReader
{
    [ComImport, GuidAttribute("31BCFCE2-DAFB-11D2-9F81-00C04F79A0A3"), CoClass(typeof(CorMetaDataDispenserExClass))]
    public interface MetaDataDispenserEx : IMetaDataDispenserEx
    {
    } 
}
