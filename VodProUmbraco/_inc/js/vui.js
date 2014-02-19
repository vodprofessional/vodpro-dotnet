var numResults = 100;



$(document).ready(function () {
    var xmlActionsURL = '/vui/vui-xml-actions/';
    var $root = $('html, body');
    $('.b-tiles-nav a').click(function (e) {
        e.preventDefault();
        $root.animate({
            scrollTop: $('[name="' + $.attr(this, 'href').substr(1) + '"]').offset().top
        }, 500);
        return false;
    });

    $(".home .b-logo .big-logo").fadeIn(1500);

    if ($('.b-service-finder').length > 0) {
        $('#headerwrap').addClass('loggedin');
    }

    // ServiceFavs
    $("a.add-favourite-service").AddServiceFavourite();
    $(".favourite-services a").DeleteServiceFavourite();

    $("img.lazy").lazyload();
    $("a.arrow").unLazy();

    // Uber Tabs
    $(".uber-tab").on("click", function (e) {
        if ($(this).find("a").not("._device-disabled").length > 0) {
            if (!$(this).hasClass("active")) {
                var currentdevice = $(this).parent("ul").find(".uber-tab.active").attr("data-devicegroup"),
                    newdevice = $(this).attr("data-devicegroup");
                // For the Screenshots Matrix
                if (currentdevice) {
                    if ($('li.device[data-devicegroup="' + currentdevice + '"]').length > 0) {
                        $('li.device[data-devicegroup="' + currentdevice + '"]').each(function () {
                            $(this).removeClass("visible");
                        });
                        $('li.overall[data-devicegroup="' + currentdevice + '"]').each(function () {
                            $(this).show();
                        });
                    }
                }
                if (newdevice) {
                    if ($('li.device[data-devicegroup="' + newdevice + '"]').length > 0) {
                        $('li.device[data-devicegroup="' + newdevice + '"]').each(function () {
                            $(this).addClass("visible");
                        });
                        $('li.overall[data-devicegroup="' + newdevice + '"]').each(function () {
                            $(this).hide();
                        });
                    }
                }

                // For Tab Syncing between Screenshots and Bechmarking...
                $("ul .uber-tab.active").removeClass("active").end();
                $('ul .uber-tab[data-devicegroup="' + newdevice + '"]').not("._platform-disabled").each(function (ind) {
                    $(this).addClass("active");
                    if ($(this).find("a").not("._device-disabled").length > 0) {
                        $(this).find("a").not("._device-disabled").first().trigger("click");
                    }
                });

                /*
                $(this).parent("ul").find(".uber-tab.active").removeClass("active").end();
                $(this).addClass("active");
                initialIndex = -1;
                $(this).find("li").each(function (ind) {
                if ($(this).find("a").not("._device-disabled").length > 0) {
                $(this).find("a").trigger("click");
                return false;
                }
                });
                if (initialIndex > -1) {
                $(this).find("a")[initialIndex];
                }
                */
            }
        }
    });
    $("li .-comp").on("click", function (e) {
        var device = $(this).parent("li").attr("data-devicegroup");
        console.log(device);
        $('.uber-tab[data-devicegroup="' + device + '"]').trigger("click");
    });
    $(".b-matrix-nav ul .uber-tab").first().trigger("click");

    $("#slide-screenshots .uber-tabs .tabs li a").on("click", function (e) {
        var lis = $("#slide-benchmarking").find('.uber-tab li');
        for (var i = 0; i < lis.length; i++) {
            var el = $(lis[i]).find("a");
            if ($(el).attr("data-device") == $(this).attr("data-device")) {
                var api = $("#slide-benchmarking .uber-tabs .tabs").data("tabs");
                api.click(i);
            }
        }
    });
    $("#slide-benchmarking .uber-tabs .tabs li a").on("click", function (e) {
        var lis = $("#slide-screenshots").find('.uber-tab li');
        for (var i = 0; i < lis.length; i++) {
            var el = $(lis[i]).find("a");
            if ($(el).attr("data-device") == $(this).attr("data-device")) {
                var api = $("#slide-screenshots .uber-tabs .tabs").data("tabs");
                api.click(i);
            }
        }
    });


    $(".tabs a._device-disabled , .tabs a.preview-link")
        .unbind("click")
        .on("click", function (e) {
            e.preventDefault();
        });

    $('.other-stories .carousel').jCarouselLite({
        btnNext: '.other-stories .next',
        btnPrev: '.other-stories .prev',
        circular: false,
        visible: 3,
        scroll: 3
    });

    $('.loggedin .iphone-gallery').jCarouselLite({
        circular: true,
        visible: 1,
        auto: true,
        timeout: 4000,
        speed: 500
    })

    $(".b-bencmarking img.device-coming-soon, .b-screenshots img.device-coming-soon, .b-matrix-nav img.device-coming-soon, .benchmarking img.device-coming-soon").after("<span class=\"device-coming-soon-sash\"></span>");

    screenshotCarousel();

    $('[data-panel]').ActionPanel();


    var href = location.href; // get the url
    var split = href.split("#"); // split the string; usually there'll be only one # in an url so there'll be only two parts after the splitting
    var hashtag;
    var hashtag2;
    if (split[1] != null) {
        hashtag = split[1];
        if (hashtag.length > 1 && hashtag.indexOf('login') == 0) {
            var hashsplit = hashtag.split("!");
            if (hashsplit[1] != null) {
                hashtag2 = decodeURIComponent(hashsplit[1]);
                hashtag = hashsplit[0];
                $('#username').val(hashtag2);
            }
            $('[data-panelregion="action-login-form"]').trigger("click");
        }
    }


    $('.service .b-bencmarking .item-block .b-content .t-block .title').hover(function (e) {
        $(this).find('.description').fadeIn(300);
    },
        function (e) {
            $(this).find('.description').fadeOut(200);
        }
    );

    $('.benchmarking .item-block .b-content .b-title li').hover(function (e) {
        $(this).find('.description').fadeIn(300);
    },
        function (e) {
            $(this).find('.description').fadeOut(200);
        }
    );



    $("a.preview-link")
        .attr("data-panel", "true").attr("data-panelregion", "action-contact-form")
        .ActionPanel();

    $("a.service-favourite").not(".favourite-disabled").ServiceFavourite();
    $("#btn-service-finder").ServiceFinder();


    $("a.open-lightbox").vuiLightbox();
    $(".b-right-nav .favourites").vuiLightbox({ "screenshots": $(".b-right-nav .favourites").attr("data-type") });

    $("#btn-image-search").vuiLightbox({ "screenshots": "search", "pageTypeSelector": "#dd-search-pagetype", "deviceSelector": "#dd-search-device" }).parents("#search-popup").hide();

    // Handle Tabbing lazyload
    var api = $(".b-tabs .tabs").data("tabs");
    if (api != null) {
        api.onClick(function (index) {
            forceResize();
        });
    }

    $(window).resize(function () {
        resizeLightboxInner();
    });

    $(".quiet-save").QuietFieldSubmit();
    $(".text-prefill").TextPrefill();
    $(".logout").Logout();

    if ($(".logged-wellcome-img .img img").hasClass("hidden")) {
        $(".logged-wellcome-img .img span").removeClass("hidden");
    }
    // FileUpload Test
    $('#fileupload').fileupload({
        url: xmlActionsURL + "?a=sui",
        dataType: 'json',
        done: function (e, data) {
            if (data.result.response === "valid") {
                d = new Date();
                $.each(data.result.files, function (index, file) {
                    $(".logged-wellcome-img .img span").addClass("hidden");
                    $(".logged-wellcome-img .img img").fadeOut(function () {
                        $(".logged-wellcome-img .img img").removeClass("hidden");
                        $(".logged-wellcome-img .img img").attr("src", file.name + "?" + d.getTime()).fadeIn();
                    });
                });
            }
            else {
                //
            }
        },
        progressall: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('#progress .progress-bar').css(
                'width',
                progress + '%'
            );
        }
    }).prop('disabled', !$.support.fileInput)
        .parent().addClass($.support.fileInput ? undefined : 'disabled');


    $("#regwall").wrap("<form method=post action=\"" + document.location.href + "\" id=\"frm\"></form>");
    $("h2.membercontent").text("Members Only Content");

    if ($(".b-info").innerHeight() - 10 < $(".b-info table").innerHeight()) { $(".b-info").css("height", $(".b-info table").innerHeight() + 10 + "px") }

    forceResize();
})

function screenshotUnCarousel() {
    $(".screenshots-carousel .carousel").unbind('touchstart touchmove touchend startCarousel stopCarousel resumeCarousel refreshCarousel go pauseCarousel endCarousel mousewheel jCarouselLite');
    $(".screenshots-carousel .carousel").jCarouselLite = null;
    $(".screenshots-carousel .carousel").remove();
                
}

function screenshotCarousel() {

    if ($('.screenshots-carousel .carousel ul li').length > 6) {
        $(".screenshots-carousel .favourite-images-message").hide();
        $('.screenshots-carousel .carousel').jCarouselLite({
            circular: true,
            visible: 6,
            auto: true,
            timeout: 2500,
            speed: 500
        });
    } else if ($('.screenshots-carousel .carousel ul li').length > 0) {
        $(".screenshots-carousel .favourite-images-message").hide();
        $('.screenshots-carousel .carousel').jCarouselLite({
            circular: false,
            visible: $('.screenshots-carousel .carousel ul li').length,
            auto: false
        });
    } else {
        $(".screenshots-carousel .favourite-images-message").show();
    }
    $('.screenshots-carousel .carousel ul li').css("overflow", "hidden").css("height", "177px");
    $(".b-logged-screenshots .screenshots-carousel .carousel ul li a").vuiLightbox({ "screenshots": "favourites" });
    
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


function isValidEmailAddress(emailAddress) {
    //var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
    var pattern = new RegExp(/([\.\-_a-zA-Z0-9])+@(([a-zA-Z0-9\-])+.)+([a-zA-Z0-9]{2,4})+$/i);
    return pattern.test(emailAddress.toLowerCase());
};

function closeVUILightbox(lbid) {
  $("#"+ lbid).fadeOut(700);
  $('html').removeClass('lightbox-on');
  return false;
}
function nextVUISlide(lbid) {
  $("#"+ lbid + " .lightbox-gallery .carousel").trigger('go','+=1');
}
function prevVUISlide(lbid) {
  $("#"+ lbid + " .lightbox-gallery .carousel").trigger('go','-=1');
}

function forceResize()
{
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
        isloadingmore : false,
        screenshots: "analysis" //Could be "favourites"
    }, options);

    var lb  = null;
    var isNew = true;
    var selectedPosition = 0,
        startPosition = -1;

    if($("#" + lbid).length > 0) {
        lb = $("#" + lbid);
        isNew = false;

        if (!settings.isloadingmore) {
            lb.find(".lightbox-main .lightbox-nav").empty();
            lb.find(".lightbox-gallery .carousel ul").empty();
        }
        else {
            $("#" + lbid + " .lightbox-main .lightbox-nav a.load-more").remove();
            selectedPosition = $("#lightbox-1 .lightbox-nav a.nav").index($("#lightbox-1 .lightbox-nav a.active"));
            startPosition = selectedPosition-1;
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

//            if (settings.analysisTag != "") {
//                var txt = screenshots[i].ServiceName + ' | ' + screenshots[i].Device + ' | ' + settings.analysisTag
//                itemtemplate.find(".lightbox-item-package").text().end()
//            } else {
                var txt = screenshots[i].ServiceName + ' | ' + screenshots[i].Device + ' | ' + screenshots[i].ImportTag
                itemtemplate.find(".lightbox-item-package").text(txt).end()
//            }

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



    if(settings.serviceName != "") {
        $("#" + lbid + " #dd-lightbox-service").val(settings.serviceName);
    }
    if (settings.device != "") {
        $("#" + lbid + " #dd-lightbox-device").val(settings.device);
    }
    if (settings.pageType != "") {
        $("#" + lbid + " #dd-lightbox-pagetype").val(settings.pageType);
    }


    if (settings.screenshots == "favourites") {
        $("#" + lbid).addClass("favourites");
        $("#" + lbid + " select").each(function() {
            this.selectedIndex = 0;
        });
    }
    else {
        $("#" + lbid).removeClass("favourites");
    }

    // Fix select boxes. This needs further work...
    $("#" + lbid + " select").reSelecter().vuiLightboxChange();
    $("#" + lbid + " .favourites").vuiLightbox({"screenshots":"favourites"});


    /* Fancybox doesn't work - utnr it off */
    $("#" + lbid + " .ui-lightbox").click(function(e) { e.preventDefault(); }) ;

    // Unbind jCarousel
    $("#" + lbid + " .lightbox-gallery .carousel").unbind('touchstart touchmove touchend startCarousel stopCarousel resumeCarousel refreshCarousel go pauseCarousel endCarousel mousewheel');
    $("#" + lbid + " .lightbox-gallery .carousel").jCarouselLite = null;
    $("#" + lbid + " .lightbox-next").unbind('click');
    $("#" + lbid + " .lightbox-prev").unbind('click');

    // Favourite / Print links
    $("#" + lbid + " a.clip").vuiFavourite();
    $("#" + lbid + " a.print").on("click", function (evt) {
        evt.preventDefault();
        window.print();
    });

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
            var nav = $('.lightbox-nav'),
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
            }
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

    // Assign the Load More Plugin to the load-more link
    $("#" + lbid + " .load-more").vuiLoadMore();

    // Assign key strokes
    if ($("#" + lbid + ".lightbox").length > 0) {
        $("#" + lbid + " .lightbox-close").on('click', function (event) {
            event.preventDefault();
            closeVUILightbox(lbid);
            UpdateFavouriteImages();
        });
        $(document)
            .bind('keydown.esc', function () {
                closeVUILightbox(lbid);
                UpdateFavouriteImages();
            })
            .bind('keydown.right', function () { nextVUISlide(lbid); })
            .bind('keydown.left', function () { prevVUISlide(lbid); });
    }

    // Open the lightbox
    openVUILightbox(lbid);
    if (!settings.isloadingmore) {
        $("#" + lbid + " .lightbox-nav").scrollTop("1");
    }
    $("#" + lbid + " .lightbox-next").removeClass("disabled");
}
