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
        public class Area
        {
            public int initialHeight;
            public int initialOuterHeight;
            protected jQuery jquery;
            protected Margin _margin;
            protected OuterHeight _outerHeight;
            protected Height _height;

            protected List<Property<int>> properties;

            protected bool cssUpdated = false;

            // disable default constructor
            private Area() { }
            public Area(string selection)
            {
                jquery = jQuery.Select(selection);
                initialHeight = jquery.Height();
                initialOuterHeight = jquery.OuterHeight(true);

                // create properties
                _margin = new Margin(jquery);
                _outerHeight = new OuterHeight(jquery);
                _height = new Height(jquery);

                // update properties list
                properties.Add(_margin);
                properties.Add(_outerHeight);
                properties.Add(_height);
            }

            public class Property<T>
            {
                protected jQuery jquery;

                public bool syncFromCSS = false;
                public bool toUpdateCSS = true;
                
                protected T value;
                public T init;

                // property node links
                List<Property<T>> in;
                List<Property<T>> out;

                protected Property() { }
                public Property(jQuery jquery)
                {
                    this.jquery = jquery;
                    read();
                    init = value;
                }

                public bool set(T value)
                {
                    this.value = value;
                    if (toUpdateCSS)
                    {
                        write();
                        toUpdateCSS = false;
                        syncFromCSS = true;
                    }

                    return syncFromCSS;
                }

                public T get()
                {
                    if (syncFromCSS)
                    {
                        read();
                        syncFromCSS = false;
                        return value;
                    }
                    return value;
                }

                protected virtual void read() { }
                protected virtual void write()
                {
                }

            }
            public class Margin : Property<int>
            {
                protected Margin() {}
                public Margin(jQuery jquery) : base(jquery) { }

                protected override void read()
                {
                    value = jquery.OuterHeight(true) - jquery.OuterHeight(false);
                }
            }
            public class OuterHeight : Property<int>
            {
                private Height height;
                protected OuterHeight() { }
                public OuterHeight(jQuery jquery) : base(jquery) { }
                public OuterHeight(jQuery jquery, Height height) : 
                    base(jquery)
                {
                    this.height = height;
                }

                protected override void read()
                {
                    value = jquery.OuterHeight(true);
                }
            }
            public class Height : Property<int>
            {
                protected Height() { }
                public Height(jQuery jquery) : base(jquery) { }

                protected override void read()
                {
                    value = jquery.Height();
                }

                protected override void write()
                {
                    jquery.Height(value);
                }

            }

            public int margin
            {
                get {
                    return _margin.get();
                }
                set {
                    _margin.set(value);
                }
            }
            public int outerHeight
            {
                get {
                    return _outerHeight.get();
                }
                set { _outerHeight.set(value); }
            }
            public virtual int height
            {
                get {
                    return _height.get();
                }
                set {
                    cssUpdated = _height.set(value);

                    // notify properties that CSS changed
                    foreach(Property<int> prop in properties)
                        prop.syncFromCSS = cssUpdated;
                }
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
