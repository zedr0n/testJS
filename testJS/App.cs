using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;

namespace testJS
{
    public class App
    {
        [Ready]
        public static void Main()
        {
            //Main main = new Main();
            //main.say("Success");
            //main.say("Exiting");
            // Script.Write("debugger");
            var msg = jQuery.Select("#helloMsg");
            msg.Val("Success");

            if (msg == null)
                Console.Log("Couldn't find element");
            else
                Console.Log(msg.Val());

            var helloBtn = jQuery.Select("#helloBtn");

            helloBtn.On("click", () => Console.Log("Button clicked"));
        }
    }
}