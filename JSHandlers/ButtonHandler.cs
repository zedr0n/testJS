using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSHandlers
{
    public class ButtonHandler : JSHandler
    {
        // proper handlers
        public string onClick(string textInput)
        {
            if (!textInput.Contains("http"))
                return "http://" + textInput;
            
            return textInput;
        }
        public void onTest()
        {
            // do nothing
        }

        public ButtonHandler()
        {
            addHandler(new JSMethodHandler("onTest2", (Func<double, double,double>)((x,y) => x * y)));
        }
    }
}
