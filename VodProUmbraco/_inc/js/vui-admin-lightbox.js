
/* Additional Functions */
jQuery.fn.extend({
    unLazy: function () {
        this.on("click", function () {
            forceResize();
        })
    }
});


jQuery.fn.extend({
    vuiDeleteImage: function () {
        this.on("click", function (event) {
            event.preventDefault();
            var imageId = $(this).parents(".lightbox-item").attr("data-id");
            var el = $(this);
            var xmlActionsURL = '/vui/vui-xml-actions/';
            var DataObject = { "imageid": imageId };
            $.ajax({
                url: xmlActionsURL + "?a=deli",
                type: "POST",
                dataType: 'json',
                data: DataObject,
                success: function (data) {
                    //// console.log("Delete image from favourites: " + data.response + "; " + data.data);
                    // $("#favourite-message").text(data.response + " " + data.data);
                    el.parents(".lightbox-item").find(".lightbox-item-img .lightbox-item-img-inner img").addClass("deleted");
                    $("#lightbox-1 .lightbox-next").trigger("click");
                }
            });
        });

        return this.each(function () {
            ;
        });
    }
});


jQuery.fn.extend({
    GetScreenshots: function () {
        this.on("click", function (event) {
            event.preventDefault();
            var aid = $(this).attr("data-id");
            var sid = $(this).attr("data-service");
            var xmlActionsURL = '/vui/vui-xml-actions/';
            var DataObject = { "analysis": aid, "service": sid };
            $.ajax({
                url: xmlActionsURL + "?a=sii",
                type: "POST",
                dataType: 'json',
                data: DataObject,
                success: function (data) {
                    var outhtml = "";
                    if (data.response == "valid") {
                        if (data.data.ScreenshotsList.resultCount > 0) {
                            var screenshots = data.data.ScreenshotsList.screenshots
                            for (i = 0; i < screenshots.length; i++) {
                                outhtml += '<a href="' + screenshots[i].ImageURL_full + '"><img src="' + screenshots[i].ImageURL_th + '" /></a>';
                            }
                        }
                    }
                    $("#lightbox").append(outhtml);
                }
            });
        });
    }
});



jQuery.fn.extend({
    reSelecter: function () {
        this.selecter = null;
        this.unbind("change");
        this.removeClass("selecter-element");
        this.parent().find("div").remove();
    //    $(this).selecter({ cover: true });
        return this;
    }
});


jQuery.fn.extend({
    vuiLightboxChange: function () {
        this.on("change", function (event) {
            // // console.log("service: [" + $("#dd-lightbox-service").val() + "] device [" + $("#dd-lightbox-device").val() + "] function [" + $("#dd-lightbox-pagetype").val() + "]");
            var lbid = "lightbox-1"

            var xmlActionsURL = '/vui/vui-xml-actions/';
            var DataObject = { "service": $("#dd-lightbox-service").val(), "device": $("#dd-lightbox-device").val(), "pagetype": $("#dd-lightbox-pagetype").val() };
            $.ajax({
                url: xmlActionsURL + "?a=lis",
                type: "POST",
                dataType: 'json',
                data: DataObject,
                success: function (data) {

                    // Only build on a good response from the JSON handler
                    if (data.response == "valid") {
                        drawLightbox(data, lbid, { isloadingmore: false });
                    }
                }
            });
        });
    }
});


jQuery.fn.extend({
    vuiLoadMore: function (options) {
        var settings = $.extend({
            pageType: "",
            device: ""
        }, options);

        this.on("click", function (event) {
            event.preventDefault();

            var lbid = "lightbox-1"

            var startfrom = $("#" + lbid + " .lightbox-gallery .carousel ul li.lightbox-item").length + 1;

            var xmlActionsURL = '/vui/vui-xml-actions/';
            var DataObject = { "service": "", "device": settings.device, "pagetype": settings.pageType };
            $.ajax({
                url: xmlActionsURL + "?a=uci",
                type: "POST",
                dataType: 'json',
                data: DataObject,
                success: function (data) {
                    if (data.response == "valid") {
                        // Set DropDowns

                        console.log(settings.pageTypeSelector);

                        drawLightbox(data, lbid, { isloadingmore: false,
                            device: settings.device,
                            pageType: settings.pageType
                        });
                    }
                }
            });
        });
    }
});






jQuery.fn.extend({
    vuiLightbox: function (options) {

        var settings = $.extend({
            screenshots: "analysis", //Could be "favourites" or "search"
            pageTypeSelector: "",
            deviceSelector: ""
        }, options);

        this.on("click", function (event) {
            event.preventDefault();
            var lbid = "lightbox-1"// + aid.toString();

            var $outer = parent.document.getElementById("umbracoMainPageBody");
            $($outer).find("#treeToggle").trigger("click");
            $($outer).find("#topBar").hide();
            $($outer).find("#rightDIV").height($($outer).height());

            if (settings.screenshots === "favourites") {

                var xmlActionsURL = '/vui/vui-xml-actions/';
                var DataObject = { "analysis": analysisId, "service": serviceId };
                $.ajax({
                    url: xmlActionsURL + "?a=fc",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {

                        // Only build on a good response from the JSON handler
                        if (data.response == "valid") {

                            // Set DropDowns
                            drawLightbox(data, lbid, { isloadingmore: false, serviceName: serviceName,
                                device: device,
                                analysisTag: analysisTag,
                                serviceId: serviceId,
                                analysisId: analysisId,
                                screenshots: "favourites"
                            });
                        }
                    }
                });
            }
            else if (settings.screenshots == "uncategorised") {
                var xmlActionsURL = '/vui/vui-xml-actions/';
                var DataObject = { "service": "", "device": $(settings.deviceSelector).val(), "pagetype": $(settings.pageTypeSelector).val() };
                $.ajax({
                    url: xmlActionsURL + "?a=uci",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {
                        if (data.response == "valid") {
                            // Set DropDowns
                            drawLightbox(data, lbid, { isloadingmore: false,
                                device: $(settings.deviceSelector).val(),
                                pageType: $(settings.pageTypeSelector).val()
                            });
                        }
                    }
                });
            }
            else if (settings.screenshots == "search") {
                var xmlActionsURL = '/vui/vui-xml-actions/';
                var DataObject = { "service": "", "device": $(settings.deviceSelector).val(), "pagetype": $(settings.pageTypeSelector).val() };
                $.ajax({
                    url: xmlActionsURL + "?a=lis",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {
                        if (data.response == "valid") {
                            // Set DropDowns
                            drawLightbox(data, lbid, { isloadingmore: false,
                                device: $(settings.deviceSelector).val(),
                                pageType: $(settings.pageTypeSelector).val()
                            });
                        }
                    }
                });
            }
            else if (settings.screenshots == "analysis") {

                var serviceName = $(this).attr("data-service"),
                    analysisTag = $(this).attr("data-analysis"),
                    analysisId = $(this).attr("data-id"),
                    serviceId = $(this).attr("data-serviceid"),
                    device = $(this).attr("data-device");

                // Otherwise populate from JSON and build
                var xmlActionsURL = '/vui/vui-xml-actions/';
                var DataObject = { "analysis": analysisId, "service": serviceId };
                $.ajax({
                    url: xmlActionsURL + "?a=sii",
                    type: "POST",
                    dataType: 'json',
                    data: DataObject,
                    success: function (data) {

                        // Only build on a good response from the JSON handler
                        if (data.response == "valid") {

                            // Set DropDowns
                            drawLightbox(data, lbid, { isloadingmore: false, serviceName: serviceName,
                                device: device,
                                analysisTag: analysisTag,
                                serviceId: serviceId,
                                analysisId: analysisId
                            });
                        }
                    }
                });
            }
        });

        return this.each(function () {

        });
    }
});
function addCommas(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}



function openVUILightbox(lbid) {
    $("#" + lbid).fadeIn(700, function () {
        resizeLightboxInner();
    });
    $('html').addClass('lightbox-on');
}
function resizeLightboxInner() {
    var lightboxInner = $('.lightbox-item-img-inner'),
            lightboxInnerTop = lightboxInner.eq(0).offset().top,
            wh = $(window).height();
    lightboxInner.height(wh - lightboxInnerTop);
}


function closeVUILightbox(lbid) {
    $("#" + lbid).fadeOut(700);
    $('html').removeClass('lightbox-on');

    var $outer = parent.document.getElementById("umbracoMainPageBody");
    $($outer).find("#topBar").show();
    $($outer).find("#treeToggle").trigger("click");
    $($outer).find("#rightDIV").height($($outer).height());

    return false;
}
function nextVUISlide(lbid) {
    $("#" + lbid + " .lightbox-gallery .carousel").trigger('go', '+=1');
}
function prevVUISlide(lbid) {
    $("#" + lbid + " .lightbox-gallery .carousel").trigger('go', '-=1');
}

function forceResize() {
    setTimeout(function () {
        // $("body").scrollTop("1");
        $("#wrapper").resize();
    }, 100);
}

function lightboxHeight() {
    ;
}
function scrollItem(dest) {
    var scrollItem = $('.lightbox-item-img-inner', '.lightbox-item.active');
    scrollItem.scrollTop(scrollItem.scrollTop() + dest * 200)
}
function fullScreen(element) {
    if (element.requestFullScreen) {
        element.requestFullScreen();
    } else if (element.mozRequestFullScreen) {
        element.mozRequestFullScreen();
    } else if (element.webkitRequestFullScreen) {
        element.webkitRequestFullScreen();
    }
}
function cancelFullscreen() {
    if (document.cancelFullScreen) {
        document.cancelFullScreen();
    } else if (document.mozCancelFullScreen) {
        document.mozCancelFullScreen();
    } else if (document.webkitCancelFullScreen) {
        document.webkitCancelFullScreen();
    }
}


function drawLightbox(data, lbid, options) {

    var settings = $.extend({
        serviceName: "",
        device: "",
        pageType: "",
        analysisTag: "",
        serviceId: 0,
        analysisId: 0,
        isloadingmore: false,
        screenshots: "analysis" //Could be "favourites"
    }, options);

    var lb = null;
    var isNew = true;
    var selectedPosition = 0,
        startPosition = -1;

    if ($("#" + lbid).length > 0) {
        lb = $("#" + lbid);
        isNew = false;

        if (!settings.isloadingmore) {
            lb.find(".lightbox-main .lightbox-nav").empty();
            lb.find(".lightbox-gallery .carousel ul").empty();
        }
        else {
            $("#" + lbid + " .lightbox-main .lightbox-nav a.load-more").remove();
            selectedPosition = $("#lightbox-1 .lightbox-nav a.nav").index($("#lightbox-1 .lightbox-nav a.active"));
            startPosition = selectedPosition - 1;
        }
    }
    else {
        lb = $(".lightbox-template").clone();
        lb.attr("id", lbid);
        lb.removeClass("lightbox-template").addClass("lightbox");
    }

    var totalResults = data.data.ScreenshotsList.screenshots.length;
    if (data.data.totalResults > 0) {
        totalResults = data.data.totalResults;
    }
    var resultStart = 0;
    if (data.data.resultStart > 0) {
        resultStart = data.data.resultStart;
    }

    if (data.data.ScreenshotsList.resultCount > 0) {
        var screenshots = data.data.ScreenshotsList.screenshots;

        // Loop through the screenshots and build up the html objects out of the template elements
        for (i = 0; i < screenshots.length; i++) {

            // First build the left-hand nav items
            var navtemplate = $("#lightbox-templates .left-nav-template").clone();
            navtemplate
                    .find(".player").text(screenshots[i].ServiceName).end()
                    .find(".name").text(screenshots[i].PageType).end()
                    .find(".device").text(screenshots[i].Device).end()
                    .find(".date").text(screenshots[i].ImportTag).end()
                    .find("img")
                        .attr("src", screenshots[i].ImageURL_th)
                        .attr("alt", screenshots[i].PageType)
                        .end();
            if (i == 0) {
                navtemplate.find("a").addClass("vis");
            }
            // Append it to the lightbox
            lb.find(".lightbox-main .lightbox-nav")
                                    .append(navtemplate.find("a"));

            var itemtemplate = $("#lightbox-templates .main-image-template").clone();
            itemtemplate
                .find("li").attr("data-id", screenshots[i].Id).end()
                .find(".count").text((resultStart + i + 1) + " of " + totalResults).end()
                .find(".lightbox-item-title").text(screenshots[i].PageType).end()
                .find(".lightbox-item-img-inner")
                    .attr("id", "ui-lightbox-" + screenshots[i].Id)
                    .attr("data-id", screenshots[i].Id)
                    .end()
                .find(".lightbox-item-img img")
                    .attr("src", screenshots[i].ImageURL_full)
                    .attr("alt", screenshots[i].PageType)
                    .end()
                ;
            if (screenshots[i].IsFavourite) {
                itemtemplate
                    .find("a.clip").addClass("is-favourite");
            }

            if (settings.analysisTag != "") {
                itemtemplate.find(".lightbox-item-package").text(settings.analysisTag).end()
            } else {
                var txt = screenshots[i].ServiceName + ' | ' + screenshots[i].Device + ' | ' + screenshots[i].ImportTag
                itemtemplate.find(".lightbox-item-package").text(txt).end()
            }

            if (i == 0) {
                itemtemplate.find("li").addClass("active");
            }
            lb.find(".lightbox-gallery .carousel ul")
                                    .append(itemtemplate.find("li"));
        }

        if (resultStart + 1 + screenshots.length < totalResults) {
            var reloadtemplate = $("#lightbox-templates .left-nav-reload-template").clone();
            lb.find(".lightbox-main .lightbox-nav")
                                    .append(reloadtemplate.find("a"));
        }

        if (isNew) {
            lb.appendTo("#lightboxes");
        }
    }
    // No Screenhots
    else {
        window.alert("no screenshots found please widen your criteria");
    }


/*
    if (settings.serviceName != "") {
        $("#" + lbid + " #dd-lightbox-service").val(settings.serviceName);
    }
    if (settings.device != "") {
        $("#" + lbid + " #dd-lightbox-device").val(settings.device);
    }
    if (settings.pageType != "") {
        $("#" + lbid + " #dd-lightbox-pagetype").val(settings.pageType);
    }
    */

    if (settings.screenshots == "favourites") {
        $("#" + lbid).addClass("favourites");
        $("#" + lbid + " select").each(function () {
            this.selectedIndex = 0;
        });
    }
    else {
        $("#" + lbid).removeClass("favourites");
    }

    // Fix select boxes. This needs further work...
    // $("#" + lbid + " select").reSelecter().vuiLightboxChange();
    // $("#" + lbid + " .favourites").vuiLightbox({ "screenshots": "favourites" });


    /* Fancybox doesn't work - utnr it off */
    $("#" + lbid + " .ui-lightbox").click(function (e) { e.preventDefault(); });

    // Unbind jCarousel
    $("#" + lbid + " .lightbox-gallery .carousel").unbind('touchstart touchmove touchend startCarousel stopCarousel resumeCarousel refreshCarousel go pauseCarousel endCarousel mousewheel');
    $("#" + lbid + " .lightbox-gallery .carousel").jCarouselLite = null;
    $("#" + lbid + " .lightbox-next").unbind('click');
    $("#" + lbid + " .lightbox-prev").unbind('click');

    // Favourite / Print links
    $("#" + lbid + " a.clip").vuiDeleteImage();

    // Rebind the Ligthbox
    $("#" + lbid + " .lightbox-gallery .carousel").jCarouselLite({
        btnNext: "#" + lbid + " .lightbox-next",
        btnPrev: "#" + lbid + " .lightbox-prev",
        circular: false,
        visible: 1,
        speed: 400,
        btnGo: $("#" + lbid + " .lightbox-nav a.nav"),
        start: startPosition,
        swipe: false,
        afterEnd: function (a, direction) {
        /*    var nav = $('.lightbox-nav'),
              active = nav.find('.vis'),
              wh = $(window).height(),
              navHeight = wh - nav.offset().top,
              activeHeight = active.outerHeight(),
              activeTop = active.offset().top - nav.offset().top,
              scrollSpeed = 300;
            if (direction) {
                if (activeHeight + activeTop > navHeight) {
                    nav.animate({
                        scrollTop: '+=' + ((activeHeight + activeTop) - navHeight)
                    }, scrollSpeed)
                }
            } else {
                if (activeTop < 0) {
                    nav.animate({
                        scrollTop: '+=' + activeTop
                    }, scrollSpeed)
                }
            }*/
        }
    }).on('swipeleft', nextVUISlide(lbid))
      .on('swiperight', prevVUISlide(lbid));

    /* NOT USED SWIPE THRESHOLDS
    swipeThresholds: {
    x: 50,
    y: 120,
    time: 800
    },
    mouseWheel: true
    */


    // Assign key strokes
    if ($("#" + lbid + ".lightbox").length > 0) {

        $("#" + lbid + " .lightbox-close").unbind('click');
        $("#" + lbid + " .lightbox-refresh").unbind('click');

        $("#" + lbid + " .lightbox-refresh").vuiLoadMore({ device: settings.device, pageType: settings.pageType });

        $("#" + lbid + " .lightbox-close").on('click', function (event) {
            event.preventDefault();
            closeVUILightbox(lbid);
        //    UpdateFavouriteImages();
        });
        $(document)
            .bind('keydown.esc', function () {
                closeVUILightbox(lbid);
            //    UpdateFavouriteImages();
            })
            .bind('keydown.r', function () {
                vuiLoadMore(lbid);
                //    UpdateFavouriteImages();
            })
            .bind('keydown.right', function () { nextVUISlide(lbid); })
            .bind('keydown.left', function () { prevVUISlide(lbid); });
    }


    // Draggable
    $("#" + lbid + " .lightbox-leftside ul li").draggable({ revert: true });
    $("#" + lbid + " .lightbox-item-img-inner").droppable({
        drop: function (event, ui) {

            

            var el = event.target;
            var imageid = $(el).attr("data-id");
            var category = ui.draggable.attr("data-id");
            var catTest = ui.draggable.text();
            var xmlActionsURL = '/vui/vui-xml-actions/';
            var DataObject = { "imageid": imageid, "category": category };
            $(el).addClass("dropped");
            $.ajax({
                url: xmlActionsURL + "?a=cvi",
                type: "POST",
                dataType: 'json',
                data: DataObject,
                success: function (data) {
                    if (data.response == "valid") {
                        var title = $(el).parents("li").find(".lightbox-item-package");
                        $(title).text($(title).text() + " | " + catTest);
                    }
                }
            });
            $("#" + lbid + " .lightbox-next").trigger("click");
        }
    });


    // Open the lightbox
    openVUILightbox(lbid);
    if (!settings.isloadingmore) {
        $("#" + lbid + " .lightbox-nav").scrollTop("1");
    }
    $("#" + lbid + " .lightbox-next").removeClass("disabled");
}
