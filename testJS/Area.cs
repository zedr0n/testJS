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
        protected Margin _margin;
        protected OuterHeight _outerHeight;
        protected Height _height;

        protected bool cssUpdated = false;

        // disable default constructor
        private Area() { }
        public Area(string selection)
        {
            jquery = jQuery.Select(selection);

            // create properties
            _margin = new Margin(jquery);
            _height = new Height(jquery);
            _outerHeight = new OuterHeight(jquery, _height);

        }

        public int margin
        {
            get
            {
                return _margin.get();
            }
            set
            {
                _margin.set(value);
            }
        }
        public int outerHeight
        {
            get
            {
                return _outerHeight.get();
            }
            set { _outerHeight.set(value); }
        }
        public virtual int height
        {
            get
            {
                return _height.get();
            }
            set
            {
                _height.set(value);
            }
        }
        public int initialHeight
        {
            get
            {
                return _height.init;
            }
        }
        public int initialOuterHeight
        {
            get
            {
                return _outerHeight.init;
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
            base(".footer") { }
    }
}
