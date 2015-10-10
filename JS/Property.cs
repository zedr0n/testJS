using System;
using Bridge.jQuery2;

namespace JS
{
    public class ChangeEventArgs<T> : EventArgs
    {
        public readonly T prev;
        public readonly T next;

        public ChangeEventArgs(T prev, T next)
        {
            this.prev = prev;
            this.next = next;
        }
    }

    public delegate void EventHandler<in TChangeEventArgs>(object sender, TChangeEventArgs e);

    public class Property<T> : ICloneable
    {
        protected readonly jQuery jquery;
        protected bool mUpdateCss = true;

        public bool updateCss
        {
            get { return mUpdateCss; }
            set {     
                mUpdateCss = value;
                writeCss();
            }
        }

        protected T value;
        public readonly T init;

        protected Property() { }

        protected Property(jQuery jquery)
        {
            this.jquery = jquery;
            read();
            init = value;
        }
        protected Property(Property<T> rhs)
        {
            value = rhs.value;
            init = rhs.init;
        }
        public virtual object Clone()
        {
            return new Property<T>(this);
        }

        public virtual void set(T value)
        {
            this.value = value;
            writeCss();
        }
        public virtual T get()
        {
            return value;
        }

        // event handling
        public event EventHandler CssUpdate;

        private void writeCss()
        {
            if (!mUpdateCss) return;
            write();
            onCssUpdate(EventArgs.Empty);
        }

        protected virtual void onCssUpdate(EventArgs e)
        {
            if (CssUpdate != null)
                CssUpdate(this, e);
        }
        protected virtual void onCSSUpdate(object sender, EventArgs e)
        {
            read();
        }

        // CSS interaction
        protected virtual void read() { }
        protected virtual void write() { }

    }
    public class Margin : Property<int>
    {
        protected Margin() { }
        public Margin(jQuery jquery) : base(jquery) { }

        // margin can only be read from CSS
        public override int get()
        {
            read();
            return value;
        }
        public override void set(int value) { }     

        protected override void read()
        {
            value = jquery.OuterHeight(true) - jquery.OuterHeight(false);
        }
    }
    public class OuterHeight : Property<int>
    {
        protected OuterHeight() { }
        public OuterHeight(jQuery jquery, Height height) : base(jquery) 
        {
            height.heightChange += onChange;
        }

        protected override void read()
        {
            value = jquery.OuterHeight(true);
        }

        protected void onChange(object sender, ChangeEventArgs<Height> e) 
        {
            int heightChange = e.next.get() - e.prev.get();
            value += heightChange;
        }
    }
    public class Height : Property<int>
    {
        protected Height() { }
        public Height(jQuery jquery) : base(jquery) { }
        protected Height(Height rhs) : base(rhs) { }

        public override object Clone()
        {
            return new Height(this);
        }

        public event EventHandler<ChangeEventArgs<Height>> heightChange;


        protected void onChange(ChangeEventArgs<Height> e)
        {
            if (heightChange != null)
                heightChange(this, e);
        }

        public override void set(int value)
        {
            var prev = (Height) Clone();
            base.set(value);
            onChange(new ChangeEventArgs<Height>(prev,this));
        }

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
