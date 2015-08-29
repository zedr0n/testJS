using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bridge;

namespace testJS
{
    [Ignore]
    public static class JSObject
    {
        [Template("jsObject.onClick({message})")]
        public static string onClick(string message)
        {
            return message;
        }
    }
}
