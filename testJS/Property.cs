using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bridge.jQuery2;

namespace testJS
{
    public delegate void cssUpdateEventHandler(object sender,EventArgs e);
    public delegate void changeEventHandler(object sender, EventArgs e);

    public class Property<T>
    {
        protected jQuery jquery;
        protected Area area;

        public event cssUpdateEventHandler cssUpdate;
        public event changeEventHandler change;

        public bool syncFromCSS = false;
        public bool toUpdateCSS = true;

        protected T value;
        public T init;

        protected Property() { }
        public Property(jQuery jquery, Area area)
        {
            this.jquery = jquery;
            this.area = area;
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
            return value;
        }

        protected virtual void onCssUpdate(EventArgs e)
        {
            if (cssUpdate != null)
                cssUpdate(this, e);
        }

        protected virtual void onChange(EventArgs e)
        {
            if (change != null)
                change(this, e);
        }

        protected virtual void read() { }
        protected virtual void write()
        {
        }

    }
    public class Margin : Property<int>
    {
        protected Margin() { }
        public Margin(jQuery jquery, Area area) : base(jquery,area) { }

        protected override void read()
        {
            value = jquery.OuterHeight(true) - jquery.OuterHeight(false);
        }
    }
    public class OuterHeight : Property<int>
    {
        protected OuterHeight() { }
        public OuterHeight(jQuery jquery,Area area) : base(jquery,area) { }

        protected override void read()
        {
            value = jquery.OuterHeight(true);
        }
    }
    public class Height : Property<int>
    {
        protected Height() { }
        public Height(jQuery jquery, Area area) : base(jquery,area) { }

        protected override void read()
        {
            value = jquery.Height();
        }

        protected override void write()
        {
            jquery.Height(value);
        }

    }

}
