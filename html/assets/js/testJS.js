/* global Bridge */

Bridge.define('testJS.App', {
    statics: {
        config: {
            init: function () {
                this.ui = new testJS.UI();
                Bridge.ready(this.main);
            }
        },
        main: function () {
            //var msg = jQuery.Select("#helloMsg");

            //UI ui = new UI();

            var helloBtn = $("#helloBtn");
            //helloBtn.On("click", () => Global.Alert("Button clicked"));
            helloBtn.on("click", function () {
                console.log("Button clicked");
                //string msg = getUserInput();
                //ui.content.setOutput(Script.Write<string>("jsObject.onClick(msg)"));
                testJS.App.ui.content.setOutput(jsObject.onClick(testJS.App.getUserInput()));
            });

        },
        getUserInput: function () {
            return testJS.App.ui.content.getUserInput();
        }
    }
});

Bridge.define('testJS.Area', {
    jquery: null,
    mMargin: null,
    mOuterHeight: null,
    mHeight: null,
    mUpdateCss: false,
    constructor: function () {
    },
    constructor$1: function (selection) {
        this.jquery = $(selection);

        // create properties
        this.mMargin = new testJS.Margin("constructor$1", this.jquery);
        this.mHeight = new testJS.Height("constructor$1", this.jquery);
        this.mOuterHeight = new testJS.OuterHeight("constructor$1", this.jquery, this.mHeight);

        this.setupdateCss(true);
    },
    getmargin: function () {
        return this.mMargin.get();
    },
    setmargin: function (value) {
        this.mMargin.set(value);
    },
    getouterHeight: function () {
        return this.mOuterHeight.get();
    },
    setouterHeight: function (value) {
        this.mOuterHeight.set(value);
    },
    getheight: function () {
        return this.mHeight.get();
    },
    setheight: function (value) {
        this.mHeight.set(value);
    },
    getinitialHeight: function () {
        return this.mHeight.init;
    },
    getinitialOuterHeight: function () {
        return this.mOuterHeight.init;
    },
    getupdateCss: function () {
        return this.mUpdateCss;
    },
    setupdateCss: function (value) {
        this.mUpdateCss = value;
        this.mHeight.setupdateCss(value);
    }
});

Bridge.define('testJS.Header', {
    inherits: [testJS.Area],
    images: null,
    constructor: function () {
        testJS.Area.prototype.constructor$1.call(this, ".header");

        this.images = new testJS.Area("constructor$1", "img");
        this.images.setupdateCss(this.getupdateCss());
    },
    getupdateCss: function () {
        return testJS.Area.prototype.getupdateCss.call(this);
    },
    setupdateCss: function (value) {
        testJS.Area.prototype.setupdateCss.call(this, value);
        if (this.images !== null)
            this.images.setupdateCss(value);
    },
    getheight: function () {
        return testJS.Area.prototype.getheight.call(this);
    },
    setheight: function (value) {
        testJS.Area.prototype.setheight.call(this, value);
        this.images.setheight(this.images.getinitialHeight() + (testJS.Area.prototype.getheight.call(this) - this.getinitialHeight()));
    }
});

Bridge.define('testJS.Footer', {
    inherits: [testJS.Area],
    constructor: function () {
        testJS.Area.prototype.constructor$1.call(this, ".footer");

    }
});

Bridge.define('testJS.Content', {
    inherits: [testJS.Area],
    constructor: function () {
        testJS.Area.prototype.constructor$1.call(this, ".content");

    },
    getUserInput: function () {
        var msg = this.jquery.find(".input-lg");
        return msg.val();
    },
    setOutput: function (output) {
        this.jquery.find(".input-lg").val(output);
    }
});

Bridge.define('testJS.ChangeEventArgs$1', function (T) { return {
    prev: null,
    next: null,
    constructor: function (prev, next) {
        this.prev = prev;
        this.next = next;
    }
}; });

Bridge.define('testJS.Property$1', function (T) { return {
    inherits: [Bridge.ICloneable],
    jquery: null,
    mUpdateCss: true,
    value: null,
    init: null,
    config: {
        events: {
            CssUpdate: null
        }
    },
    constructor: function () {
    },
    constructor$1: function (jquery) {
        this.jquery = jquery;
        this.read();
        this.init = this.value;
    },
    constructor$2: function (rhs) {
        this.value = rhs.value;
        this.init = rhs.init;
    },
    getupdateCss: function () {
        return this.mUpdateCss;
    },
    setupdateCss: function (value) {
        this.mUpdateCss = value;
        this.writeCSS();
    },
    clone: function () {
        return new testJS.Property$1(T)("constructor$2", this);
    },
    set: function (value) {
        this.value = value;
        this.writeCSS();
    },
    get: function () {
        return this.value;
    },
    writeCSS: function () {
        if (this.mUpdateCss) {
            this.write();
            this.onCssUpdate(Object.empty);
        }
    },
    onCssUpdate: function (e) {
        if (this.CssUpdate !== null)
            this.CssUpdate(this, e);
    },
    onCSSUpdate: function (sender, e) {
        this.read();
    },
    read: function () {
    },
    write: function () {
    }
}; });

Bridge.define('testJS.OuterHeight', {
    inherits: [testJS.Property$1(Bridge.Int)],
    constructor: function () {
        testJS.Property$1(Bridge.Int).prototype.$constructor.call(this);

    },
    constructor$1: function (jquery, height) {
        testJS.Property$1(Bridge.Int).prototype.constructor$1.call(this, jquery);

        height.addHeightChange(Bridge.fn.bind(this, this.onChange));
    },
    read: function () {
        this.value = this.jquery.outerHeight(true);
    },
    onChange: function (sender, e) {
        var heightChange = e.next.get() - e.prev.get();
        this.value += heightChange;
    }
});

Bridge.define('testJS.Margin', {
    inherits: [testJS.Property$1(Bridge.Int)],
    constructor: function () {
        testJS.Property$1(Bridge.Int).prototype.$constructor.call(this);

    },
    constructor$1: function (jquery) {
        testJS.Property$1(Bridge.Int).prototype.constructor$1.call(this, jquery);

    },
    get: function () {
        this.read();
        return this.value;
    },
    set: function (value) {
    },
    read: function () {
        this.value = this.jquery.outerHeight(true) - this.jquery.outerHeight(false);
    }
});

Bridge.define('testJS.Height', {
    inherits: [testJS.Property$1(Bridge.Int)],
    config: {
        events: {
            HeightChange: null
        }
    },
    constructor: function () {
        testJS.Property$1(Bridge.Int).prototype.$constructor.call(this);

    },
    constructor$1: function (jquery) {
        testJS.Property$1(Bridge.Int).prototype.constructor$1.call(this, jquery);

    },
    constructor$2: function (rhs) {
        testJS.Property$1(Bridge.Int).prototype.constructor$2.call(this, rhs);

    },
    clone: function () {
        return new testJS.Height("constructor$2", this);
    },
    onChange: function (e) {
        if (this.HeightChange !== null)
            this.HeightChange(this, e);
    },
    set: function (value) {
        var prev = Bridge.cast(this.clone(), testJS.Height);
        testJS.Property$1(Bridge.Int).prototype.set.call(this, value);
        this.onChange(new testJS.ChangeEventArgs$1(testJS.Height)(prev, this));
    },
    read: function () {
        this.value = this.jquery.height();
    },
    write: function () {
        this.jquery.height(this.value);
    }
});

Bridge.define('testJS.UI', {
    statics: {
        hEIGHT_REDRAW: 10
    },
    timeout: 0,
    maxHeight: 0,
    config: {
        init: function () {
            this.header = new testJS.Header();
            this.footer = new testJS.Footer();
            this.content = new testJS.Content();
        }
    },
    constructor: function () {
        // disable scrollbars
        document.body.style.overflow = "hidden";

        // initialize resize handler
        this.onResize();
        $(window).resize(Bridge.fn.bind(this, function () {
            if (Math.abs(this.maxHeight - window.innerHeight) > testJS.UI.hEIGHT_REDRAW)
                this.onResize();
            window.clearTimeout(this.timeout);
            this.timeout = window.setTimeout(Bridge.fn.bind(this, this.onResize), 100);
        }));


    },
    getheight: function () {
        return this.header.getouterHeight() + this.content.getinitialOuterHeight() + this.footer.getinitialOuterHeight();
    },
    onResize: function () {
        this.maxHeight = window.innerHeight;
        this.header.setupdateCss(false);
        // shrink the header if not fitting into window
        while (this.getheight() > this.maxHeight && this.header.getheight() > 0)
            this.header.setheight(this.header.getheight()-20);

        // increase the header if possible
        while (this.getheight() < this.maxHeight - 20 && this.header.getheight() < this.header.getinitialHeight())
            this.header.setheight(this.header.getheight()+20);

        this.header.setupdateCss(true);

        this.content.setupdateCss(this.header.getupdateCss());
        this.content.setheight(this.maxHeight - this.header.getouterHeight() - this.footer.getouterHeight());
    },
    testPerf: function () {
        var start = new Date();

        var d = $.Deferred(null);
        var counter = 0;
        // test performance
        for (var i = 0; i < 2000; ++i) 
            (function () {
                window.setTimeout(Bridge.fn.bind(this, function () {
                    this.maxHeight = Bridge.Int.trunc((Math.random() * window.innerHeight));
                    this.onResize();
                    ++counter;
                    if (counter === 1999)
                        d.resolve(null);
                }), 25);
            }).call(this);

        d.done(function () {
            console.log("Time taken: " + (new Bridge.TimeSpan((new Date() - start) * 10000)).toString());
        });
    }
});