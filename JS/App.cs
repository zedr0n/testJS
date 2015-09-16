using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;

[System.AttributeUsage(System.AttributeTargets.Method)]
public class Export : System.Attribute { }

namespace JS
{
    public class App
    {
        private static UI ui = new UI();

        [Export]
        public static void setText()
        {
            ui.output = "Test";
        }
        [Export]
        public static void doClick()
        {
            var helloBtn = jQuery.Select("#helloBtn");
            helloBtn.Click();
        }
        [Export]
        public static string getOutput()
        {
            return ui.output;
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