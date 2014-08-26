/**
 * The jVODGallery is a custom built, pure HTML5 image gallery for vodprofessionals.com
 *
 * Usage examples:
 *
 * #1 Single image url
 * {code}
 * <script src="jvodgallery.js"></script>
 * <script type="text/javascript">
 *    $('any selector').jVODGallery({
 *      logoImg: 'logo url here to an image',
 *      imgSource: <url of single image>
 *    });
 * {code}
 *
 * #2 Single image jquery selector
 * {code}
 * <script src="jvodgallery.js"></script>
 * <script type="text/javascript">
 *    $('any selector').jVODGallery({
 *      logoImg: 'logo url here to an image',
 *      imgSource: <jquery selector>
 *    });
 * {code}
 *
 * #3 Images in object
 * {code}
 * <script src="jvodgallery.js"></script>
 * <script type="text/javascript">
 *    $('any selector').jVODGallery({
 *      logoImg: 'logo url here to an image',
 *      imgSource: {
 *          <uniqe ID #1>: 'url of image #1',
 *          <uniqe ID #2>: 'url of image #2',
 *          <uniqe ID #3>: 'url of image #3'
 *      },
 *      titles: {
 *          <uniqe ID #1>: 'title of image #1',
 *          <uniqe ID #2>: 'title of image #2',
 *          <uniqe ID #3>: 'title of image #3'
 *      },
 *      filters: {
 *          <filtername #1 (without spaces!)>: {
 *              '<filtervalue #1.1>': [<image unique id #1>, <image unique #3>],
 *              '<filtervalue #1.2>': [<image unique id #2>]
 *          },
 *          <filtername #2 (without spaces!)>: function() { return { <id>: 'imageid' }; },
 *          <filtername #3 (without spaces!)>: {
 *              '<filtervalue #3.1>': [<image unique id #1>, <image unique #2>],
 *          }
 *      }
 *    });
 * {code}
 *
 * IMPORTANT: If you wish to have the customized dropdowns, include the ddsclick.js script before the jvodgallery.js file
 * {code}
 *   <script src="jquery.ddslick.js"></script>
 * {code}
 */
;(function( $, window, document, undefined ){

    var pluginName = 'jVODGallery',
        events     = new Reactor(),
        defaults   = {
            logoImg     : "",
            imgSource   : "",   // If it's a string, it's a jquery selector. If it's an object, it's links to images
            titles      : {},
            filters     : {}
        };



    //
    // The plugin constructor
    //
    function JVODGallery( element, options ) {
        this.element   = $(element);
        this.id        = Utility.generateUID();
        this.options   = $.extend( {}, defaults, options ) ;
        this._defaults = defaults;

        this.init();
    }

    // The init method runs after construction
    // Contains all the logic to kick off the plugin behavior
    JVODGallery.prototype.init = function () {
        this.domElement = $(
            '<div id="jvodgallery-' + this.id + '" data-id="' + this.id + '" class="jvodgallery-main">' +
            '   <div class="jvodgallery-dim"><div id="jvodgallery-no-image">We found no images for those filters</div></div>' +
            '</div>').appendTo('body');
        Utility.addClickHandler(this.element, this.showGallery, this);

        events.registerEvent( 'gallery-show' );
        events.registerEvent( 'page-left' );
        events.registerEvent( 'page-right' );
        events.registerEvent( 'page-preview-left' );
        events.registerEvent( 'page-preview-right' );
        events.registerEvent( 'resize' );

        var imageCount = this.countImages();

        new CloseButton( this );
        this.imageContainer = new ImageContainer( this );
        if (imageCount > 1) {
            this.leftPager = new LeftPager( this, this.imageContainer );
            this.rightPager = new RightPager( this, this.imageContainer );
            this.previewContainer = new PreviewContainer( this );
            this.filterPanel = new FilterPanel( this );
        }

        new Resize(function () {
            var dim = Utility.getViewportDimensions();
            events.dispatchEvent('resize', [dim.width, dim.height]);
        });
    };

    // Get a plugin setting or the default value
    JVODGallery.prototype.getSetting = function ( setting ) {
        return typeof this.options[setting] == 'undefined' ? this._defaults[setting] : this.options[setting];
    };

    // Show the gallery
    JVODGallery.prototype.showGallery = function () {
        this.domElement.show();
        events.dispatchEvent( 'gallery-show' );
    };

    // Hide the gallery
    JVODGallery.prototype.hideGallery = function ( reset ) {
        if ( typeof reset == 'undefined' || reset === true ) {
            this.resetState();
        }

        this.domElement.hide();
    };

    // Reset state of the gallery
    JVODGallery.prototype.resetState = function () {
        this.imageContainer.resetState();
        if (this.previewContainer) {
            this.previewContainer.resetState();
        }
        if (this.filterPanel) {
            this.filterPanel.resetState();
        }
    };

    // Return the image DOM tags for the gallery to show
    JVODGallery.prototype.getImageTags = function () {
        var imgs = this.getSetting( "imgSource" );

        if ( typeof imgs == 'object' ) {
            // Handle shortcut where the parameter is an array of image urls
            var container = $(
                '<ul id="jvodgallery-shadow-img-container-'+ this.id + '" class="jvodgallery-shadow-img-container">' +
                '</ul>').appendTo('body');
            for (var i in imgs) {
                if (imgs.hasOwnProperty(i)) {
                    $('<img src="' + imgs[i] + '" data-id="' + i + '"/>')
                        .appendTo(container);
                }
            }
            imgs = container.find('img');
        }
        if ( typeof imgs == "string" ) {
            // Handle shortcut where the parameter is a selector string
            var container = $(
                    '<ul id="jvodgallery-shadow-img-container-'+ this.id + '" class="jvodgallery-shadow-img-container">' +
                    '</ul>').appendTo('body');
            try {
                $(imgs).each(function (index, value) {
                    $(value).clone().appendTo(container).data('id', 0);
                });
                imgs = container.find('img');
            } catch (err) {
                // Looks like it's not a selector string, handle it as a url
                $('<img src="' + imgs + '" data-id="0"/>')
                    .appendTo(container);
                imgs = container.find('img');
            }
        }

        // Now imgs is an array of <img> tags
        return imgs;
    };

    // Returns the count of images in this gallery
    JVODGallery.prototype.countImages = function () {
        var imgs = this.getSetting( "imgSource" );
        if ( typeof imgs == 'object' ) {
            return Object.keys(imgs).length;
        }
        if ( typeof imgs == "string" ) {
            return 1;
        }

        console.error('Wrong image gallery definition or no image in gallery');
        return -1;
    }



    //
    //
    //
    function CloseButton ( container ) {
        this.container = container;
        this.domElement = $('<span class="jvodgallery-close"><span>&times;</span></span>').appendTo(this.container.domElement);

        Utility.addClickHandler(this.domElement, this.container.hideGallery, this.container);
    }



    //
    //
    //
    function ImageContainer ( container ) {
        this.container = container;
        this.images = {};
        this.actualImage = 0;
        this.domElement = $('<div class="jvodgallery-img-container-outer"></div>').appendTo(this.container.domElement);

        var imgTags = this.container.getImageTags();
        imgTags.each($.proxy(function( index, img ) {
            var id = $(img).data('id');
            if (typeof id == 'undefined') {
                id = Utility.generateUID()
            }
            this.images[id] = new Image(this, img);
        }, this));

        events.addEventListener( 'resize', $.proxy(this.resize, this) );
    }

    //
    ImageContainer.prototype.resize = function ( w ) {
        this.domElement.css({
            'width': (this.images.length * w) + 'px',
            'margin-left': (this.actualImage * w * -1) + 'px'
        });
    };

    //
    ImageContainer.prototype.resetState = function ( ) {
        this.actualImage = 0;
        this.domElement.css( 'margin-left', 0 );
    };



    //
    //
    //
    function Image ( container, imageTag ) {
        this.container = container;
        this.imageTag = imageTag;
        this.domElement = $('<div class="jvodgallery-img-item"></div>').appendTo(this.container.domElement);
        this.hidden = false;

        $('<div class="jvodgallery-img-item-inner"></div>')
            .appendTo(this.domElement)
            .append(this.imageTag);
        this.resize( Utility.getViewportDimensions().width, Utility.getViewportDimensions().height );

        events.addEventListener( 'resize', $.proxy(this.resize, this) );
    }

    //
    Image.prototype.resize = function ( w, h ) {
        this.domElement.css({
            'width' : w  + 'px',
            'height': h + 'px'
        });
   };

    //
    Image.prototype.hide = function ( ) {
        this.domElement.hide();
        this.hidden = true;
    };

    //
    Image.prototype.show = function ( ) {
        this.domElement.show();
        this.hidden = false;
    };



    //
    //
    //
    function LeftPager ( container, imageContainer ) {
        this.container = container;
        this.imageContainer = imageContainer;
        this.domElement = $('<span class="jvodgallery-left"><span><</span></span>')
            .appendTo(this.container.domElement);

        if (this.imageContainer.actualImage == 0) {
            this.domElement.hide();
        }

        events.addEventListener( 'page-right', $.proxy( this.movedRight, this) );
        events.addEventListener( 'page-left', $.proxy( this.movedLeft, this) );

        Utility.addClickHandler(this.domElement, this.moveLeft, this);
    }

    LeftPager.prototype.movedRight = function () {
        this.domElement.show();
    };

    LeftPager.prototype.movedLeft = function () {
        if (this.imageContainer.actualImage == 0) {
            this.domElement.hide();  // No more paging left
        }
    };

    LeftPager.prototype.moveLeft = function ( howMany ) {
        var step = howMany;
        if ( typeof howMany == 'undefined' ) {
            step = 1;
        }

        this.imageContainer.actualImage -= step;
        this.imageContainer.domElement.animate({
            'margin-left': '+=' + (Utility.getViewportDimensions().width * step)
        });

        events.dispatchEvent( 'page-left', [howMany] );
    };



    //
    //
    //
    function RightPager ( container, imageContainer ) {
        this.container = container;
        this.imageContainer = imageContainer;
        this.domElement = $('<span class="jvodgallery-right"><span>></span></span>')
            .appendTo(this.container.domElement);

        if (this.imageContainer.actualImage == this.imageContainer.images.length - 1) {
            this.domElement.hide();
        }

        events.addEventListener( 'page-left', $.proxy( this.movedLeft, this) );
        events.addEventListener( 'page-right', $.proxy( this.movedRight, this) );

        Utility.addClickHandler(this.domElement, this.moveRight, this);
    }

    //
    RightPager.prototype.movedLeft = function () {
        this.domElement.show();
    };

    //
    RightPager.prototype.movedRight = function () {
        if (this.imageContainer.actualImage == Object.keys(this.imageContainer.images).length - 1) {
            this.domElement.hide();  // No more paging right
        }
    };

    //
    RightPager.prototype.moveRight = function ( howMany ) {
        var step = howMany;
        if ( typeof howMany == 'undefined' ) {
            step = 1;
        }

        this.imageContainer.actualImage += step;
        this.imageContainer.domElement.animate({
            'margin-left': '-=' + (Utility.getViewportDimensions().width * step)
        });

        events.dispatchEvent( 'page-right', [howMany] );
    };



    //
    //
    //
    function PreviewContainer ( container ) {
        this.container = container;
        this.domElement = $('<div class="jvodgallery-preview-container"></div>').appendTo(this.container.domElement);
        this.imageContainer = $('<div class="jvodgallery-preview-container-inner"></div>').appendTo(this.domElement);
        this.scrollContainer = $('<div class="jvodgallery-preview-container-scroll"></div>').appendTo(this.imageContainer);
        this.logoElement = $('<div class="jvodgallery-logo">' +
                '<img src="' + this.container.getSetting("logoImg") + '"/>' +
            '</div>').appendTo(this.domElement);
        this.previewImages = [];
        this.displacement = 0;
        this.switch = new PreviewContainerSwitch( this );
        this.previewRightPager = new PreviewRightPager( this );
        this.previewLeftPager = new PreviewLeftPager( this );

        events.addEventListener( 'resize', $.proxy( this.resize, this ) );
        events.addEventListener( 'page-left', $.proxy( this.viewerMovedLeft, this ) );
        events.addEventListener( 'page-right', $.proxy( this.viewerMovedRight, this ) );

        this.initState();
        this.previewLeftPager.initState();
        this.previewRightPager.initState();
        this.switch.initState();
    }

    //
    PreviewContainer.prototype.initState = function ( ) {
        var cnt = 0;
        for (var i in this.container.imageContainer.images) {
            if (this.container.imageContainer.images.hasOwnProperty(i)) {
                $.proxy(function ( item, idx ) {
                    var img = new PreviewContainerImage( this, $(item.imageTag).clone(), idx );
                    if (this.container.imageContainer.actualImage == idx) {
                        img.toggleActive();
                    }
                    this.previewImages.push(img);
                }, this)(this.container.imageContainer.images[i], cnt++);
            }
        }

        var scrollContainerWidth = this.scrollContainer.find('.jvodgallery-preview-image').toArray().map(
            function ( item ) {
                return $(item).outerWidth(true);
            }
        ).reduce(
            function ( sum, i ) {
                return sum + i;
            }
        ,0);

        this.scrollContainer.css({
            'width': scrollContainerWidth + 'px'
        });

        this.domElement.css({'margin-bottom': '-' + this.domElement.outerHeight() + 'px'});
    };

    //
    PreviewContainer.prototype.resetState = function ( ) {
        this.scrollContainer.css( 'margin-left', 0 );
        this.domElement.css({'margin-bottom': '-' + this.domElement.outerHeight() + 'px'});
        this.displacement = 0;
        this.domElement.find('.jvodgallery-preview-image').removeClass('jvodgallery-preview-image-active');
        if ('undefined' != typeof this.previewImages[0]) {
            this.previewImages[0].toggleActive();
        }

        this.switch.resetState();
    };

    //
    PreviewContainer.prototype.resize = function ( ) {
        this.scrollActiveImageToView();
    };

    //
    PreviewContainer.prototype.viewerMovedRight = function ( delta ) {
        if (typeof delta == 'undefined') {
            delta = 1;
        }

        // Set the viewer actual image active in the preview
        this.previewImages[this.container.imageContainer.actualImage - delta].toggleActive();
        this.previewImages[this.container.imageContainer.actualImage].toggleActive();

        this.scrollActiveImageToView();
    };

    //
    PreviewContainer.prototype.viewerMovedLeft = function ( delta ) {
        if (typeof delta == 'undefined') {
            delta = 1;
        }

        // Set the viewer actual image active in the preview
        this.previewImages[this.container.imageContainer.actualImage + delta].toggleActive();
        this.previewImages[this.container.imageContainer.actualImage].toggleActive();

        this.scrollActiveImageToView();
    };

    //
    PreviewContainer.prototype.scrollActiveImageToView = function ( ) {
        var img = this.previewImages[this.container.imageContainer.actualImage];
        if ('undefined' != typeof img) {
            var p = img.getPosition();
            if (p.left <= this.displacement) {
                this.previewLeftPager.moveLeft( this.displacement - p.left );
            }
            if (p.right > this.displacement + this.imageContainer.width()) {
                this.previewRightPager.moveRight( p.right - (this.displacement + this.imageContainer.width()) );
            }
        }
    };



    //
    //
    //
    function PreviewContainerSwitch ( container ) {
        this.container = container;
        this.isOpen = undefined;
        this.domElement = $('<div class="jvodgallery-preview-container-switch"><span></span></div>')
            .appendTo(this.container.domElement);

        Utility.addClickHandler(this.domElement, this.togglePreview, this);
    }

    //
    PreviewContainerSwitch.prototype.initState = function ( ) {
        this.resetState();
    };

    //
    PreviewContainerSwitch.prototype.resetState = function ( ) {
        var cookies = Utility.cookie();

        if ('undefined' != typeof cookies['previewcontainer'] && 'closed' == cookies['previewcontainer']) {
            // Close
            this.isOpen = false;
            this.container.domElement.css({'margin-bottom': '-' + this.container.domElement.outerHeight() + 'px'});
        } else {
            // Open
            this.isOpen = true;
            this.container.domElement.css({'margin-bottom': '0px'});
        }
    };

    //
    PreviewContainerSwitch.prototype.togglePreview = function ( ) {
        if (this.isOpen) {
            this.container.domElement.animate({
                'margin-bottom': '-=' + this.container.domElement.outerHeight()
            });
            this.isOpen = false;
            document.cookie = "previewcontainer=closed";
        }
        else {
            this.container.domElement.animate({
                'margin-bottom': '+=' + this.container.domElement.outerHeight()
            });
            this.isOpen = true;
            document.cookie = "previewcontainer=open";
        }
    };



    //
    //
    //
    function PreviewLeftPager ( container ) {
        this.container = container;
        this.domElement = $('<span class="jvodgallery-preview-left"><span>&laquo;</span></span>')
            .appendTo(this.container.domElement);

        events.addEventListener( 'resize', $.proxy( this.initState, this ) );
        events.addEventListener( 'page-preview-right', $.proxy( this.movedRight, this) );
        events.addEventListener( 'page-preview-left', $.proxy( this.movedLeft, this) );

        Utility.addClickHandler(this.domElement, this.moveLeft, this);
    }

    //
    PreviewLeftPager.prototype.initState = function ( ) {
        if (this.container.displacement == 0) {
            this.domElement.hide();
        }
    };

    //
    PreviewLeftPager.prototype.movedRight = function () {
        this.domElement.show();
    };

    //
    PreviewLeftPager.prototype.movedLeft = function () {
        if (this.container.displacement == 0) {
            this.domElement.hide();  // No more paging left
        }
    };

    //
    PreviewLeftPager.prototype.moveLeft = function ( scrollWidth ) {
        if (typeof scrollWidth == 'undefined') {
            scrollWidth = this.container.imageContainer.width() / 2;
        }

        if (this.container.displacement - scrollWidth < 0) {
            scrollWidth = this.container.displacement; // Displacement cannot be less than zero
        }

        this.container.displacement -= scrollWidth;
        this.container.scrollContainer.animate({
            'margin-left': '+=' + scrollWidth
        });

        events.dispatchEvent( 'page-preview-left', [scrollWidth] );
    };



    //
    //
    //
    function PreviewRightPager ( container ) {
        this.container = container;
        this.domElement = $('<span class="jvodgallery-preview-right"><span>&raquo;</span></span>')
            .appendTo(this.container.domElement);

        events.addEventListener( 'gallery-show', $.proxy( this.initState, this ) );
        events.addEventListener( 'resize', $.proxy( this.initState, this ) );
        events.addEventListener( 'page-preview-left', $.proxy( this.movedLeft, this) );
        events.addEventListener( 'page-preview-right', $.proxy( this.movedRight, this) );

        Utility.addClickHandler(this.domElement, this.moveRight, this);
    }

    //
    PreviewRightPager.prototype.initState = function ( ) {
        if (this.container.displacement + this.container.imageContainer.width() >= this.container.scrollContainer.width()) {
            this.domElement.hide();
        }
        else {
            this.domElement.show();
        }
    };

    //
    PreviewRightPager.prototype.movedLeft = function () {
        if (this.container.displacement + this.container.imageContainer.width() >= this.container.scrollContainer.width()) {
            this.domElement.hide();
        }
        else {
            this.domElement.show();
        }
    };

    //
    PreviewRightPager.prototype.movedRight = function () {
        if (this.container.displacement == this.container.scrollContainer.width() - this.container.imageContainer.width()) {
            this.domElement.hide();  // No more paging right
        }
    };

    //
    PreviewRightPager.prototype.moveRight = function ( scrollWidth ) {
        if (typeof scrollWidth == 'undefined') {
            scrollWidth = this.container.imageContainer.width() / 2;
        }

        if (this.container.scrollContainer.width() - (this.container.displacement + scrollWidth) < this.container.imageContainer.width()) {
            scrollWidth = this.container.scrollContainer.width() - this.container.imageContainer.width() - this.container.displacement;
        }

        this.container.displacement += scrollWidth;
        this.container.scrollContainer.animate({
            'margin-left': '-=' + scrollWidth
        });

        events.dispatchEvent( 'page-preview-right', [scrollWidth] );
    };



    //
    //
    //
    function PreviewContainerImage ( container, img, position ) {
        this.container = container;
        this.position = position;
        this.hidden = false;
        this.domElement = $('<div class="jvodgallery-preview-image"></div>').appendTo(this.container.scrollContainer);
        $('<div class="jvodgallery-preview-image-inner"></div>').appendTo(this.domElement).append( img );

        Utility.addClickHandler(this.domElement, this.pageToImage, this);
    }

    //
    PreviewContainerImage.prototype.pageToImage = function ( ) {
        var actualPosition = this.container.container.imageContainer.actualImage;
        if ( actualPosition < this.position ) {
            this.container.container.rightPager.moveRight(this.position - actualPosition);
        }
        else if (actualPosition > this.position) {
            this.container.container.leftPager.moveLeft(actualPosition - this.position);
        }
    };

    //
    PreviewContainerImage.prototype.toggleActive = function ( ) {
        this.domElement.toggleClass('jvodgallery-preview-image-active');
    };

    //
    PreviewContainerImage.prototype.getPosition = function ( ) {
        var p = this.domElement.position();
        p.right = p.left + this.domElement.outerWidth(true);
        return p;
    };

    //
    PreviewContainerImage.prototype.hide = function ( ) {
        this.domElement.hide();
        this.hidden = true;
    };

    //
    PreviewContainerImage.prototype.show = function ( ) {
        this.domElement.show();
        this.hidden = false;
    };



    //
    //
    //
    function FilterPanel ( container ) {
        this.container = container;
        this.domElement = $('<div class="jvodgallery-filter-panel"></div>').appendTo(this.container.domElement);
        this.filterElement = $('<div class="jvodgallery-filter-panel-container"></div>').appendTo(this.domElement);
        this.filterDomElements = {};
        this.filterSettings = this.container.getSetting( "filters" );
        this.activeFilters = {};
        this.hiddenImages = {};
        this.imageCache = {};
        this.imagePreviewCache = {};
        new PreviewTitlebar( this );
        new FilterPanelSwitch( this );


        for (var f in this.filterSettings) {
            if ('function' == typeof this.filterSettings[f]) {
                this.filterSettings[f] = this.filterSettings[f]();
            }
        }

        for (var imgId in this.container.imageContainer.images) {
            if (this.container.imageContainer.images.hasOwnProperty(imgId)) {
                this.imageCache[imgId] = this.container.imageContainer.images[imgId];
            }
        }

        for (var idx in this.container.previewContainer.previewImages) {
            if (this.container.previewContainer.previewImages.hasOwnProperty(idx)) {
                this.imagePreviewCache[idx] = this.container.previewContainer.previewImages[idx];
            }
        }

        for (var filter in this.filterSettings) {
            if (this.filterSettings.hasOwnProperty(filter)) {
                this.filterDomElements[filter] = $('<select id="filter-select-' + filter + '" data-filter="' + filter + '"></select>')
                    .appendTo($('<label></label>')
                        .appendTo(this.filterElement)
                );

                for (var value in this.filterSettings[filter]) {
                    if (this.filterSettings[filter].hasOwnProperty(value)) {
                        $('<option value="'+value+'">'+value+'</option>').appendTo(this.filterDomElements[filter]);

                        var obj = {};
                        for (var imgId1 in this.container.imageContainer.images) {
                            if (this.container.imageContainer.images.hasOwnProperty(imgId1)) {
                                this.filterSettings[filter][value] = this.filterSettings[filter][value]
                                    .map(function ( currVal ) { return currVal + '' });
                                if (this.filterSettings[filter][value].indexOf(imgId1) > -1) {
                                    obj[imgId1] = 1;
                                }
                                else {
                                    obj[imgId1] = -1;
                                }
                            }
                        }
                        this.filterSettings[filter][value] = obj;
                    }
                }

                this.filterSettings[filter]['All ' + filter] = {};
                for (var imgId2 in this.container.imageContainer.images) {
                    if (this.container.imageContainer.images.hasOwnProperty(imgId2)) {
                        this.filterSettings[filter]['All ' + filter][imgId2] = 0;
                    }
                }
                $('<option selected="selected" value="All '+filter+'">All '+filter+'</option>')
                    .prependTo(this.filterDomElements[filter]);

                if ('function' == typeof $()['ddslick']) {
                    this.filterDomElements[filter].ddslick({
                        width: '190px',
                        onSelected: $.proxy(function (selectedItem) {
                                        var fi = selectedItem.original[0].id.substring(14);
                                        this.filterDomElements[fi].val = function() {
                                            return selectedItem.selectedData.value;
                                        }
                                        this.filterChange(fi);
                                    }, this)
                        });
                }
                else {
                    Utility.addChangeHandler( this.filterDomElements[filter], this.filterChange, this, [filter] );
                }
            }
        }
    }

    //
    FilterPanel.prototype.resetState = function ( ) {
        this.filterElement.css({"margin-top": 0});

        this.activeFilters = {};
        for (var f in this.filterDomElements) {
            if (this.filterDomElements.hasOwnProperty(f)) {
                if ('function' == typeof $()['ddslick']) {
                    $('#filter-select-' + f).ddslick('select', { 'index': 0 });
                } else {
                    this.filterDomElements[f].val('All '+f);
                }
            }
        }

        this.hiddenImages = {};
        for (var imgId in this.imageCache) {
            if (this.imageCache.hasOwnProperty(imgId)) {
                this.hiddenImages[imgId] = 0;
            }
        }

        this.updateImageContainer();
        this.updatePreviewContainer();
    };

    //
    FilterPanel.prototype.filterChange = function ( filter ) {
        var value = this.filterDomElements[filter].val();

        this.activeFilters[filter] = this.filterSettings[filter][value];
        this.hiddenImages = {};
        for (var imgId in this.imageCache) {
            if (this.imageCache.hasOwnProperty(imgId)) {
                for (var f in this.activeFilters) {
                    if (this.activeFilters.hasOwnProperty(f)) {
                        if (this.activeFilters[f][imgId] == -1) {
                            this.hiddenImages[imgId] = -1;
                        }
                        else if (this.hiddenImages[imgId] != -1 && this.activeFilters[f][imgId] == 1) {
                            this.hiddenImages[imgId] = 1;
                        }
                        else if (this.hiddenImages[imgId] != 1 && this.hiddenImages[imgId] != -1) {
                            this.hiddenImages[imgId] = 0;
                        }
                    }
                }
            }
        }

        this.updateImageContainer();
        this.updatePreviewContainer();
    };

    //
    FilterPanel.prototype.updateImageContainer = function ( ) {
        var imgContainerEmpty = true;

        for (var imgId in this.hiddenImages) {
            if (this.hiddenImages.hasOwnProperty(imgId)) {
                if (this.hiddenImages[imgId] > -1) {
                    this.container.imageContainer.images[imgId] = this.imageCache[imgId];
                    this.imageCache[imgId].show();
                    imgContainerEmpty = false;
                }
                else {
                    this.imageCache[imgId].hide();
                    delete this.container.imageContainer.images[imgId];
                }
            }
        }

        if (imgContainerEmpty)
            $('#jvodgallery-no-image').show();
        else
            $('#jvodgallery-no-image').hide();
        this.container.imageContainer.actualImage = 0;
        this.container.imageContainer.resize(Utility.getViewportDimensions().width);
        this.container.leftPager.movedRight();
        this.container.leftPager.movedLeft();
        this.container.rightPager.movedLeft();
        this.container.rightPager.movedRight();
    };

    //
    FilterPanel.prototype.updatePreviewContainer = function ( ) {
        var cnt = 0, visibleCnt = 0;
        this.container.previewContainer.previewImages = [];
        for (var idx in this.hiddenImages) {
            if (this.hiddenImages.hasOwnProperty(idx)) {
                if (this.hiddenImages[idx] > -1) {
                    this.container.previewContainer.previewImages[visibleCnt] = this.imagePreviewCache[cnt];
                    this.container.previewContainer.previewImages[visibleCnt].position = visibleCnt;
                    this.imagePreviewCache[cnt].show();
                    visibleCnt++;
                }
                else {
                    this.imagePreviewCache[cnt].hide();
                }
            }
            cnt++;
        }

        this.container.previewContainer.displacement = 0;
        var scrollContainerWidth = this.container.previewContainer.previewImages.map(
            function ( item ) {
                return item.domElement.outerWidth(true);
            }
        ).reduce(
            function ( sum, i ) {
                return sum + i;
            }
            ,0);
        this.container.previewContainer.scrollContainer.css({
            'margin-left': 0,
            'width': scrollContainerWidth + 'px'
        });

        this.container.previewContainer.domElement.find('.jvodgallery-preview-image').removeClass('jvodgallery-preview-image-active');
        if ('undefined' != typeof this.container.previewContainer.previewImages[0]) {
            this.container.previewContainer.previewImages[0].toggleActive();
        }

        this.container.previewContainer.previewRightPager.movedLeft();
        this.container.previewContainer.previewLeftPager.movedRight();
        this.container.previewContainer.previewLeftPager.movedLeft();
    };





    //
    //
    //
    function FilterPanelSwitch ( container ) {
        this.isFilterElementClosed = true;
        this.container = container;
        this.isOpen = undefined;
        this.domElement = $('<div class="jvodgallery-filter-panel-switch"><span></span></div>')
            .appendTo(this.container.domElement);
        this.initState();

        Utility.addClickHandler(this.domElement, this.toggleFilterPanel, this);
    }

    //
    FilterPanelSwitch.prototype.initState = function ( ) {
        this.resetState();
    };

    //
    FilterPanelSwitch.prototype.resetState = function ( ) {
        var cookies = Utility.cookie();

        if ('undefined' != typeof cookies['filterpanel'] && 'closed' == cookies['filterpanel']) {
            // Close
            this.isFilterElementClosed = true;
            this.container.domElement.css({'margin-top': '-' + this.container.domElement.outerHeight() + 'px'});
        } else {
            // Open
            this.isFilterElementClosed = false;
            this.container.domElement.css({'margin-top': '0px'});
        }
    };

    //
    FilterPanelSwitch.prototype.toggleFilterPanel = function ( ) {
        if (this.isFilterElementClosed) {
            this.container.domElement.animate({
                'margin-top': '+=' + this.container.domElement.outerHeight()
            });
            this.isFilterElementClosed = false;
            document.cookie = "filterpanel=open";
        }
        else {
            this.container.domElement.animate({
                'margin-top': '-=' + this.container.domElement.outerHeight()
            });
            this.isFilterElementClosed = true;
            document.cookie = "filterpanel=closed";
        }
    };


    //
    //
    //
    function PreviewTitlebar ( container ) {
        this.container = container;
        this.titles = this.container.container.getSetting('titles');
        this.domElement = $('<div class="jvodgallery-preview-titlebar"></div>').appendTo(this.container.domElement);
        this.pageElement = $('<span class="jvodgallery-preview-titlebar-page"></span>').appendTo(this.domElement);
        this.titleElement = $('<span class="jvodgallery-preview-titlebar-title"></span>').appendTo(this.domElement);

        this.initState();

        events.addEventListener( 'page-left', $.proxy( this.updateTitlebar, this ) );
        events.addEventListener( 'page-right', $.proxy( this.updateTitlebar, this ) );
    }

    //
    PreviewTitlebar.prototype.initState = function ( ) {
        this.updateTitlebar();
    };

    //
    PreviewTitlebar.prototype.updateTitlebar = function ( ) {
        var imageIdxs = Object.keys(this.container.container.imageContainer.images);
        var titleText = this.titles[imageIdxs[this.container.container.imageContainer.actualImage]];
        if (typeof imageIdxs[this.container.container.imageContainer.actualImage] != 'undefined' &&
            this.titles[imageIdxs[this.container.container.imageContainer.actualImage]] != 'undefined'&&
            'string' == typeof titleText && titleText.length > 0) {
            this.titleElement.text(titleText);
        }
        else {
            this.titleElement.text('');
        }

        this.pageElement.text((this.container.container.imageContainer.actualImage + 1) + '/' + imageIdxs.length)
    };



    //
    // A utility static class
    //
    function Utility() { }

    // Return an Object of cookies
    Utility.cookie = function () {
        var tmp = document.cookie.split(';');
        var cookies = new Object();
        for (var i in tmp) {
            var z = tmp[i].split('=');
            if (z.length == 2) {
                cookies[z[0].trim()] = z[1].trim();
            }
        }

        return cookies;
    }

    // Generates a new unique ID
    Utility.generateUID = function () {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    };

    // Helper to simplify click handler setup
    Utility.addClickHandler = function ( elem, handler, context, args ) {
        return $(elem).click(function (e) {
            e.preventDefault();
            return handler.apply(
                typeof context == 'undefined' ? this : context,
                typeof args == 'undefined' ? [] : args
            );
        });
    };

    // Helper to simplify change handler setup
    Utility.addChangeHandler = function ( elem, handler, context, args ) {
        return $(elem).change(function (e) {
            e.preventDefault();
            return handler.apply(
                typeof context == 'undefined' ? this : context,
                typeof args == 'undefined' ? [] : args
            );
        });
    };

    // Return the current viewport dimensions
    Utility.getViewportDimensions = function () {
        var w = Math.min(document.documentElement.clientWidth, window.innerWidth || 100);
        var h = Math.min(document.documentElement.clientHeight, window.innerHeight || 100);

        return { width: w, height: h };
    };



    //
    //
    //
    function Reactor(){
        this.events = {};
    }

    Reactor.Event = function ( name ) {
        this.name = name;
        this.callbacks = [];
    };

    Reactor.Event.prototype.registerCallback = function ( callback ){
        this.callbacks.push( callback );
    };

    Reactor.prototype.registerEvent = function( eventName ){
        if (typeof this.events[eventName] == 'undefined') {
            this.events[eventName] = new Reactor.Event( eventName );
        }
    };

    Reactor.prototype.dispatchEvent = function ( eventName, eventArgs ) {
        this.events[eventName].callbacks.forEach(function ( callback ) {
            callback.apply( this, eventArgs );
        });
    };

    Reactor.prototype.addEventListener = function ( eventName, callback ) {
        this.events[eventName].registerCallback( callback );
    };




    //
    //
    //
    function Resize ( callback ) {
        this.callback = callback;
        this.rtime = new Date(1, 1, 2000, 12, 0, 0);
        this.timeout = false;
        this.delta = 200;

        $(window).resize($.proxy(function() {
            this.rtime = new Date();
            if (this.timeout === false) {
                this.timeout = true;
                setTimeout($.proxy(this.resizeEnd, this), this.delta);
            }
        }, this));
    }

    //
    Resize.prototype.resizeEnd = function ( ) {
        if (new Date() - this.rtime < this.delta) {
            setTimeout($.proxy(this.resizeEnd, this), this.delta);
        } else {
            this.timeout = false;
            this.callback();
        }
    };


    // Add the plugin to jQuery
    $.fn[pluginName] = function ( options ) {
        return this.each(function () {
            new JVODGallery( this, options );
        });
    };

})( jQuery, window, document );
