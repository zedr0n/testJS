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

            headerHeight = jQuery.Select(".header").Height();
            imageHeight = jQuery.Select("img").Height();
            inputMargin = jQuery.Select(".inputContainer").OuterHeight(true) - jQuery.Select(".inputContainer").OuterHeight();
            inputHeight = jQuery.Select(".inputContainer").Height();

            setContainer();
            disableScrollbar();
            Window.OnResize = setContainer;

            //var body = jQuery.Select("body");
            //jQuery.Select("body").On("resize", 


        }

        public static void setContainer(Event e = null)
        {
            var container = jQuery.Select(".inputContainer");

            var height = -1;
            while(height < inputHeight)
            {
                height = Window.InnerHeight;

                height -= jQuery.Select(".header").OuterHeight(true);
                height -= jQuery.Select(".footer").OuterHeight(true);
                
                height -= inputMargin;

                if (height > inputHeight)
                {
                    while (height > 20 + inputHeight && jQuery.Select(".header").Height() < headerHeight)
                    {
                        height += jQuery.Select(".header").OuterHeight(true);
                        jQuery.Select(".header").Height(jQuery.Select(".header").Height() + 20);
                        height -= jQuery.Select(".header").OuterHeight(true);
                    }
                }
                else
                {
                    jQuery.Select(".header").Height(jQuery.Select(".header").Height() - 20);
                }

            }
            jQuery.Select("img").Height(imageHeight - (headerHeight - jQuery.Select(".header").Height()));
            container.Height(height);
        }

        public static void disableScrollbar()
        {
            jQuery.Select("body").Css("overflow","hidden");
        }

        private static int headerHeight = 0;
        private static int imageHeight = 0;
        private static int inputHeight = 0;
        private static int inputMargin = 0;
    }
}