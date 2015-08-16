using Bridge;
using Bridge.Html5;

namespace testJS
{
    public class App
    {
        [Ready]
        public static void Main()
        {
            // Simple alert() to confirm it's working
            Global.Alert("Success");
        }
    }
}