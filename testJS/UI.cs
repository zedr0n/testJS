﻿using System;
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
        private int maxHeight;

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

            var start = DateTime.Now;

            var d = jQuery.Deffered( null );
            int counter = 0;
            // test performance
            for (int i = 0; i < 2000; ++i  )
            {
                Window.SetTimeout(() =>
                    {
                        maxHeight = (int)(Math.Random() * Window.InnerHeight);
                        onResize();
                        ++counter;
                        if(counter == 1999)
                            d.Resolve();
                    }
                , 25);
            }

            d.Done(() =>
            {
                Console.Log("Time taken: " + (DateTime.Now - start).ToString());
            });
            
            // initialize resize handler
            onResize();
            jQuery.Window.Resize(() =>
                {
                    Window.ClearTimeout(timeout);
                    timeout = Window.SetTimeout(onResize,100);
                });
            //Window.OnPageShow = Window.OnResize = onResize;
            
           
        }

        public void onResize()
        {
            //maxHeight = Window.InnerHeight;
            header.updateCss = false;
            // shrink the header if not fitting into window
            while ( height > maxHeight && header.height > 0 )
                header.height -= 20;

            // increase the header if possible
            while (height < maxHeight - 20 && header.height < header.initialHeight)
                header.height += 20;

            header.updateCss = true;

            content.updateCss = header.updateCss;
            content.height = maxHeight - header.outerHeight - footer.outerHeight;
        }
    }
}
