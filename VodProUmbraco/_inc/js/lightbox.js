jQuery.event.special.swipe.settings = {
  threshold: 0.1,
  sensitivity: 9
};
$(document).ready(function(){
  $('.lightbox-gallery .carousel').jCarouselLite({
    btnNext: '.lightbox-next',
    btnPrev: '.lightbox-prev',
    circular: false,
    swipe: false,
    visible: 1,
    speed: 400,
    btnGo: $('.lightbox-nav a'),
    afterEnd: function(a, direction) {
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
            scrollTop: '+='+((activeHeight + activeTop) - navHeight)
          },scrollSpeed)
        }
      } else {
        if (activeTop < 0) {
          nav.animate({
            scrollTop: '+='+activeTop
          },scrollSpeed)
        }
      }
    }
  }).on('swipeleft',nextSlide)
    .on('swiperight',prevSlide);

  $('.lightbox-close').on('click',closeLightbox);
  $(document).bind('keydown.esc', closeLightbox)
             .bind('keydown.right', nextSlide)
             .bind('keydown.left', prevSlide);

  $('.lightbox-gallery .carousel').on('click','.lightbox-item img',function(){
    $('.lightbox-fullscreen-img-inner').empty().append($(this).clone());
    $('#lightbox-fullscreen').show();
    fullScreen($('#lightbox-fullscreen')[0]);
  })
  $('.lightbox-fullscreen-close').click(function(){
    cancelFullscreen();
    $('#lightbox-fullscreen').hide();
  })
})
function openLightbox() {
  $('.lightbox').fadeIn(700,function(){
    var lightboxInner = $('.lightbox-item-img-inner'),
        lightboxInnerTop = lightboxInner.eq(0).offset().top,
        wh = $(window).height();

    lightboxInner.height(wh - lightboxInnerTop);
  });
  $('html').addClass('lightbox-on');
  
}
function closeLightbox() {
  $('.lightbox').fadeOut(700);
  $('html').removeClass('lightbox-on');
}
function nextSlide() {
  $('.lightbox-gallery .carousel').trigger('go','+=1');
}
function prevSlide() {
  $('.lightbox-gallery .carousel').trigger('go','-=1');
}
function scrollItem(dest) {
  var scrollItem = $('.lightbox-item-img-inner','.lightbox-item.active');
  scrollItem.scrollTop(scrollItem.scrollTop() + dest*200)
}
function fullScreen(element) {
  if(element.requestFullScreen) {
    element.requestFullScreen();
  } else if(element.mozRequestFullScreen) {
    element.mozRequestFullScreen();
  } else if(element.webkitRequestFullScreen) {
    element.webkitRequestFullScreen();
  }
}
function cancelFullscreen() {
  if(document.cancelFullScreen) {
    document.cancelFullScreen();
  } else if(document.mozCancelFullScreen) {
    document.mozCancelFullScreen();
  } else if(document.webkitCancelFullScreen) {
    document.webkitCancelFullScreen();
  }
}