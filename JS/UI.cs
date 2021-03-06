﻿using System;
using Bridge.Html5;
using Bridge.jQuery2;

namespace JS
{
    public class UI
    {
        public readonly Header header = new Header();
        public readonly Footer footer = new Footer();
        public readonly Content content = new Content();

        public string input
        {
            get { return content.getUserInput(); }
        }

        public string output
        {
            set { content.setOutput(value); }
            get { return content.getUserInput(); }
        }

        private int timeout;
        private int maxHeight;
        private const int HEIGHT_REDRAW = 10;

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
                    if (Math.Abs(maxHeight - Window.InnerHeight) > HEIGHT_REDRAW)
                        onResize();
                    Window.ClearTimeout(timeout);
                    timeout = Window.SetTimeout(onResize,100);
                });
            
           
        }

        public void hideHeader()
        {
            header.hide();
            content.height = Window.InnerHeight - header.outerHeight - footer.outerHeight;
        }

        public void onResize()
        {
            maxHeight = Window.InnerHeight;

            // don't resize if hidden
            if (header.height == 0)
            {
                content.height = maxHeight - footer.outerHeight;
                return;
            }

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

        // tests
        public void testPerf()
        {
            var start = DateTime.Now;

            var d = jQuery.Deffered(null);
            int counter = 0;
            // test performance
            for (int i = 0; i < 2000; ++i)
            {
                Window.SetTimeout(() =>
                {
                    maxHeight = (int)(Math.Random() * Window.InnerHeight);
                    onResize();
                    ++counter;
                    if (counter == 1999)
                        d.Resolve();
                }
                , 25);
            }

            d.Done(() =>
            {
                Console.Log("Time taken: " + (DateTime.Now - start).ToString());
            });
        }
    }
}
