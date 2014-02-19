$(document).ready(function () {


    var href = location.href; // get the url
    var split = href.split("#"); // split the string; usually there'll be only one # in an url so there'll be only two parts after the splitting
    var hashtag;
    var hashdevice;
    if (split[1] != null) {
        hashtag = split[1];
        if (hashtag.length > 1) {
            var hashsplit = hashtag.split("!");
            if (hashsplit[1] != null) {
                hashdevice = hashsplit[1];
                hashtag = hashsplit[0];
            }
        }
    }

    if ($('.b-tabs').length > 0) {
        /*
        if ($('body').hasClass('service')) {
        initialIndex = 2;
        } else if ($('body').hasClass('benchmarking')) {
        initialIndex = 3;
        }
        */

        $('.b-tabs').each(function (index) {
            var initialIndex = 0;
            $(this).find("ul li").each(function (ind) {
                if ($(this).find("a").not("._device-disabled").length > 0) {
                    initialIndex = ind;
                    return false;
                }
            });
            $(this).find(".tabs").tabs("div.panes > div", {
                initialIndex: initialIndex
            });

            // Added to cater for Uber-tabs
            if (hashdevice) {
                if ($('a[data-device="' + hashdevice + '"]').not("._device-disabled").length > 0) {
                    $('a[data-device="' + hashdevice + '"]').not("._device-disabled").each(function (e) {
                        $(this).parents(".uber-tag").addClass("active");
                        $(this).trigger("click");
                    });
                }
            }
            if ($(".uber-tag.active").length == 0) {
                $(this).find(".tabs li a.current").parents(".uber-tab").addClass("active");
            }

        });
    }
    $("select").selecter({
        cover: true
    });
    $('.b-carousel.big .carousel').jCarouselLite({
        btnNext: '.big .next',
        btnPrev: '.big .prev',
        circular: false,
        visible: 3,
        scroll: 3
    });
    $('.b-carousel.small .carousel').jCarouselLite({
        btnNext: '.small .next',
        btnPrev: '.small .prev',
        circular: false,
        visible: 4,
        scroll: 4
    });
    $('.b-carousel.middle .carousel').jCarouselLite({
        btnNext: '.benchmarking .next',
        btnPrev: '.benchmarking .prev',
        circular: false,
        visible: 1
    });
    $('.b-carousel.middle2 .carousel').jCarouselLite({
        btnNext: '.benchmarking .next',
        btnPrev: '.benchmarking .prev',
        circular: false,
        visible: 1
    });
    $('.b-carousel.middle3 .carousel').jCarouselLite({
        btnNext: '.benchmarking .next',
        btnPrev: '.benchmarking .prev',
        circular: false,
        visible: 1
    });
    /*
    $('.screenshots-carousel .carousel').jCarouselLite({
    circular: true,
    visible: 6,
    auto: true,
    timeout: 4000,
    speed: 500
    });*/
    $('.home .iphone-gallery, .slide-gallery .carousel').jCarouselLite({
        circular: true,
        visible: 1,
        auto: true,
        timeout: 4000,
        speed: 500
    })
    $('.lightbox-gallery .carousel').jCarouselLite({
        btnNext: '.lightbox-next',
        btnPrev: '.lightbox-prev',
        circular: false,
        visible: 1,
        speed: 500,
        btnGo: $('.lightbox-nav a')
    })
    /*
    $('[data-panel]').on('click', function (e) {
    e.preventDefault();
    var panel = $('.b-rightside-panel');
    panel.find('.panel-content').load('/_inc/data/active-panel.html ' + $(this).attr('href'));
    panel.show();
    })
    */
    $('.search a').off('click').click(function (e) {
        e.preventDefault();
        openPopup('#search-popup');
    })
    /*
    $('.choose a').off('click').click(function () {
    $('body, html').scrollTop(0);
    $('.sub-menu').show();
    });
    */
    $('.close, .popup-close').click(function (e) {
        e.preventDefault();
        $('.b-rightside-panel, #popup-layout, .b-popup').hide("fade");
    })
    $('.b-service-finder a').click(function (e) {
        e.preventDefault();
        $('.sub-menu').show();
    })

    if ($('.lightbox').length > 0) {
        $('.lightbox-close').on('click', closeLightbox);
        $(document).bind('keydown.esc', closeLightbox)
               .bind('keydown.right', nextSlide)
               .bind('keydown.left', prevSlide);
    }

    var linkClicked = false;
    $('.b-nav-panel').on('click', 'a', function (e) {
        e.preventDefault();
        var link = $(this),
            slide = $(link.attr('href')),
            slideTop = slide.position().top;
        link.parents('ul').find('.active').removeClass('active');
        link.parent().addClass('active');

        linkClicked = true;

        $('html, body').animate({
            scrollTop: slideTop
        }, 300, function () {
            linkClicked = false;
        })
    })

    var $nav = $(".b-matrix-nav"),
      matrixTop;
    if ($nav.length > 0) {
        matrixTop = $nav.offset().top;
        $(window).scroll(function () {
            var scrollTop = $(window).scrollTop();
            if (scrollTop > matrixTop) {
                if (!$nav.hasClass("fixed")) {
                    $nav.addClass("fixed");
                }
            } else if ($nav.hasClass("fixed")) {
                $nav.removeClass("fixed");
            }
        })
    }


    if ($('body').hasClass('home')) {
        $(window).load(function () {
            var wh = $(window).height(),
          fh = $('footer').height(),
          lastSlide = $('.home-slide').last();

            if ((lastSlide.height() + fh) < wh) {
                lastSlide.css('height', (wh - fh) + 'px');
            }
        })

        var slidesCount = $('.home-slide').length;
        $(window).scroll(function () {
            var scrollTop = $(window).scrollTop();
            $('.home-slide').each(function (index) {
                var currentSlideTop = $(this).offset().top - 30 - scrollTop;

                if (currentSlideTop > 0 || (currentSlideTop <= 0 && (index == slidesCount - 1))) {
                    var indexLink = $('.b-nav-panel li').eq(currentSlideTop <= 0 ? index : index - 1);

                    if (!indexLink.hasClass('active') && !linkClicked) {
                        indexLink.parent().find('li').removeClass('active');
                        indexLink.addClass('active');
                    }
                    return false
                }
            })
        })
    }
    if ($('body').hasClass('service')) {
        var slidesCount = $('.service-slide').length;
        $(window).scroll(function () {
            var scrollTop = $(window).scrollTop();
            $('.service-slide').each(function (index) {
                var currentSlideTop = $(this).offset().top - 30 - scrollTop;

                if (currentSlideTop > 0 || (currentSlideTop <= 0 && (index == slidesCount - 1))) {
                    var indexLink = $('.b-nav-panel li').eq(currentSlideTop <= 0 ? index : index - 1);

                    if (!indexLink.hasClass('active') && !linkClicked) {
                        indexLink.parent().find('li').removeClass('active');
                        indexLink.addClass('active');
                    }
                    return false
                }
            })
        })
    }
    toggleW1200();

    // Finally, slide down the page to the selected point
    if ($('#' + hashtag).length > 0) {
        $("html, body").animate({ scrollTop: $('#' + hashtag).position().top }, 1000);
    }


}).click(function (e) {
    if ($(e.target).parents('.b-service-finder').length <= 0 && $(e.target).parents('.choose').length <= 0) {
        $('.sub-menu').hide();
    }
})  

$(window).resize(function(){
  toggleW1200();
}).scroll(function(){
  scrollW1200();
})

function toggleW1200() {
  if ($(this).width() < 1199) {
    $('body').addClass('w1200');
  } else {
    $('body').removeClass('w1200');
  }
  scrollW1200();
}

function scrollW1200() {
  if ($('body').hasClass('w1200')) {
    $('.b-right-nav, .b-nav-panel').css('top',$(window).scrollTop())
  } else {
    $('.b-right-nav, .b-nav-panel').removeAttr('style');
  }
}

function openPopup(elem) {
  var wh = $(window).height();
  var wtop = $(window).scrollTop();
  
  $('#popup-layout,'+elem).show();
  
  var popup_top = ~~((wh - $(elem).height())/2 + wtop);
  
  $(elem).css({
    top: (popup_top > 0 ? popup_top : 0)
  })
}

function openLightbox() {
  $('.lightbox').fadeIn(700);
  $('html').addClass('lightbox-on');
  return false;
}
function closeLightbox() {
  $('.lightbox').fadeOut(700);
  $('html').removeClass('lightbox-on');
  return false;
}
function nextSlide() {
  $('.lightbox-gallery .carousel').trigger('go','+=1');
}
function prevSlide() {
  $('.lightbox-gallery .carousel').trigger('go','-=1');
}