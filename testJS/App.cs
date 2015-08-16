using Bridge;
using Bridge.Html5;

namespace testJS
{
    public class App
    {
        [Ready]
        public static void Main()
        {
            Main main = new Main();
            main.say("Success");

            main.say("Exiting");
        }
    }
}