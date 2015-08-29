using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;

namespace testJS
{
    public class App
    {
        private static UI ui = new UI();

        [Ready]
        public static void Main()
        {
            //var msg = jQuery.Select("#helloMsg");

            //UI ui = new UI();

            var helloBtn = jQuery.Select("#helloBtn");
            //helloBtn.On("click", () => Global.Alert("Button clicked"));
            helloBtn.On("click", () => 
                {
                    Console.Log("Button clicked");
                    //string msg = getUserInput();
                    //ui.content.setOutput(Script.Write<string>("jsObject.onClick(msg)"));
                    ui.content.setOutput(JSObject.onClick(getUserInput()));
                });

        }

        public static string getUserInput()
        {
            return ui.content.getUserInput();
        }
    }
}