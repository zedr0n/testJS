using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bridge.jQuery2;

namespace testJS
{
    public class Area
    {
        protected jQuery jquery;
        protected Margin mMargin;
        protected OuterHeight mOuterHeight;
        protected Height mHeight;
        protected bool mUpdateCss;

        // disable default constructor
        private Area() { }
        public Area(string selection)
        {
            jquery = jQuery.Select(selection);

            // create properties
            mMargin = new Margin(jquery);
            mHeight = new Height(jquery);
            mOuterHeight = new OuterHeight(jquery, mHeight);

            updateCss = true;
        }

        public int margin
        {
            get
            {
                return mMargin.get();
            }
            set
            {
                mMargin.set(value);
            }
        }
        public int outerHeight
        {
            get
            {
                return mOuterHeight.get();
            }
            set { mOuterHeight.set(value); }
        }
        public virtual int height
        {
            get
            {
                return mHeight.get();
            }
            set
            {
                mHeight.set(value);
            }
        }
        public int initialHeight
        {
            get
            {
                return mHeight.init;
            }
        }
        public int initialOuterHeight
        {
            get
            {
                return mOuterHeight.init;
            }
        }
        public virtual bool updateCss
        {
            get { return mUpdateCss;  }
            set {
                mUpdateCss = value;
                mHeight.updateCss = value;
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
            images.updateCss = updateCss;
        }

        public override bool updateCss
        {
            get
            {
                return base.updateCss;
            }
            set
            {
                base.updateCss = value;
                if(images != null)
                    images.updateCss = value;
            }
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

        public string getUserInput()
        {
            var msg = jquery.Find(".input-lg");
            return msg.Val();
        }

        public void setOutput(string output)
        {
            jquery.Find(".input-lg").Val(output);
        }
    }
    public class Footer : Area
    {
        public Footer() :
            base(".footer") { }
    }
}
