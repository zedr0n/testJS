using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;

namespace testJS
{
    public class UI
    {
        public Header header = new Header();
        public Footer footer = new Footer();
        public Content content = new Content();

        private int timeout;

        public int height
        {
            get 
            {
                return header.outerHeight + content.initialOuterHeight + footer.initialOuterHeight;
            }
        }
        
        public UI()
        {
            // disable scrollbars
            Document.Body.Style.Overflow = Overflow.Hidden;

            // initialize resize handler
            onResize();
            jQuery.Window.Resize(() =>
                {
                    Window.ClearTimeout(timeout);
                    timeout = Window.SetTimeout(onResize,25);
                });
            //Window.OnPageShow = Window.OnResize = onResize;
           
        }

        public void onResize()
        {
            // shrink the header if not fitting into window
            while ( height > Window.InnerHeight && header.height > 0 )
                header.height -= 20;

            // increase the header if possible
            while (height < Window.InnerHeight - 20 && header.height < header.initialHeight)
                header.height += 20;

            content.height = Window.InnerHeight - header.outerHeight - footer.outerHeight;
        }
    }
}
