using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;

namespace JS
{
    public class App
    {
        private static UI ui = new UI();

        public static void setText()
        {
            ui.output = "Test";
        }

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
                    ui.output = jsObject.onClick(ui.input);
                });

        }


    }
}