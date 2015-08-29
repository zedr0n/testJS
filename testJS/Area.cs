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
        //public int initialHeight;
        //public int initialOuterHeight;
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
            //initialHeight = jquery.Height();
            //initialOuterHeight = jquery.OuterHeight(true);

            // create properties
            _margin = new Margin(jquery,this);
            _outerHeight = new OuterHeight(jquery,this);
            _height = new Height(jquery,this);

            // update properties list
            properties.Add(_margin);
            properties.Add(_outerHeight);
            properties.Add(_height);
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
                cssUpdated = _height.set(value);

                // notify properties that CSS changed
                foreach (Property<int> prop in properties)
                    prop.syncFromCSS = cssUpdated;
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
