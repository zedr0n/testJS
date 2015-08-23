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
        public int windowHeight = Window.InnerHeight;

        public class Area
        {
            public int initialHeight;
            public int initialOuterHeight;
            protected jQuery jquery;

            // disable default constructor
            private Area() { }
            public Area(string selection)
            {
                jquery = jQuery.Select(selection);
                initialHeight = jquery.Height();
                initialOuterHeight = jquery.OuterHeight(true);
            }

            public int margin
            {
                get { return jquery.OuterHeight(true) - jquery.OuterHeight(false); }           
            }
            public int outerHeight
            {
                get { return jquery.OuterHeight(true);  }
            }
            public virtual int height
            {
                get { return jquery.Height(); }
                set { jquery.Height(value);  }
            }
        }
        public class Header : Area
        {
            private Area images;

            public Header() : 
                base(".header") 
            {
                images = new Area("img");
            }

            public override int height
            {
                get
                {
                    return base.height;
                }
                set
                {
                    base.height = value;
                    images.height = images.initialHeight + (base.height - base.initialHeight);
                }
            }
        }
        public class Content : Area
        {
            public Content() :
                base(".content") { }
        }
        public class Footer : Area
        {
            public Footer() :
                base(".footer") {}
        }

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
