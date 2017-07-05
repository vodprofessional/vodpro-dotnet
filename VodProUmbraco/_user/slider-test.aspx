<%@ Master Language="C#" MasterPageFile="~/masterpages/vui3-master.master" AutoEventWireup="true" %>


<asp:Content ID="ContentHead" ContentPlaceHolderID="ContentPlaceHolderHead" runat="server">
    <title>VUIX Library - Feature Comparison</title>
    
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">
  <link rel="stylesheet" type="text/css" href="//cdn.jsdelivr.net/jquery.slick/1.6.0/slick.css"/>
  <link rel="stylesheet" type="text/css" href="/_inc/VP2014/css/jquery.fancybox.min.css"/>
  <style>
  html {
    font-size: 14px;
  }
  .title-col {
  
  }
  .title-row {
    height: 200px;
    float:left;
    clear:left;
  }
  .title-row:first-child {
    margin-top: 45px;
  }
  .title-row h2 {
    padding: 0 0 0 0;
    margin: 50px 0 0 0;
    text-align: right
    font-size: 1.5rem;
  }
  .slider-col {
  
  }
  .slide {
  }
  .slide-title {
  
  }
  .slide-title h2 {
    padding: 0 0 0 0;
    margin:  0 0 0 0;
    height:  45px;
    font-size: 1.5rem;
    text-align: center;
  }
  .slide-body {
    padding: 0 10px;
    text-align: center;
  }
  .slide-body a {
    display: block;
    height:  180px;
    width:   96%;
    margin:  0 0 20px 0;
    overflow:hidden;
    background-position: center center;
    background-size: cover;
  }
  .slide-body a img {
    display: none;
  }
  
  .slider-nav {
    position: absolute;
    top: 0;
  }
  .slider-nav-prev {
    left: -30px;
  }
  .slider-nav-next {
    right: -30px;
  }
  .slider-nav button {
    background: transparent;
    border: 0px none;
    font-size: 2rem;
    color: rgb(31, 166, 222);
  }
  @media only screen and (max-width : 1200px) {

  }

  /* Medium Devices, Desktops */
  @media only screen and (max-width : 992px) {

  }

  /* Small Devices, Tablets */
  @media only screen and (max-width : 768px) {

  }
  
  </style>
    
</asp:Content>

<asp:Content ID="ContentBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
<div class="container">
<div class="row">
<div class="col-xs-6 col-sm-3 title-col">
  <div class="title-row"><h2>Netflix</h2></div>
  <div class="title-row"><h2>Amazon Prime</h2></div>
  <div class="title-row"><h2>HBO</h2></div>
  <div class="title-row"><h2>iPlayer</h2></div>
  <div class="title-row"><h2>Another</h2></div>
</div>

<div class="slider-col col-xs-6 col-sm-9">

  <div class="slider">
    <div class="slide">
      <div class="slide-title">
        <h2>Featured Content</h2>
      </div>
      <div class="slide-body">
        <a href="http://placehold.it/600x400" data-fancybox="featured content" data-caption="Featured Content on Netflix">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="featured content" data-caption="Featured Content on Amazon">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="featured content" data-caption="Featured Content on HBO">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="featured content" data-caption="Featured Content on iPlayer">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="featured content" data-caption="Featured Content on Another">
          <img src="http://placehold.it/600x400" />
        </a>
      </div>
    </div>

    <div class="slide">
      <div class="slide-title">
      <h2>Search</h2>
      </div>
      <div class="slide-body">
        <a href="http://placehold.it/600x400" data-fancybox="Search" data-caption="Search on Netflix">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Search" data-caption="Search on Amazon">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Search" data-caption="Search on HBO">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Search" data-caption="Search on iPlayer">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Search" data-caption="Search on Another">
          <img src="http://placehold.it/600x400" />
        </a>
      </div>
    </div>


    <div class="slide">
      <div class="slide-title">
        <h2>Video Player</h2>
      </div>
      <div class="slide-body">
        <a href="http://placehold.it/600x400" data-fancybox="Video Player" data-caption="Video Player on Netflix">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Video Player" data-caption="Video Player on Amazon">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Video Player" data-caption="Video Player on HBO">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Video Player" data-caption="Video Player on iPlayer">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Video Player" data-caption="Video Player on Another">
          <img src="http://placehold.it/600x400" />
        </a>
      </div>
    </div>


    <div class="slide">
      <div class="slide-title">
      <h2>Content description</h2>
      </div>
      <div class="slide-body">
        <a href="http://placehold.it/600x400" data-fancybox="Recommended content" data-caption="Content description on Netflix">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Recommended content" data-caption="Content description on Amazon">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Recommended content" data-caption="Content description on HBO">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Recommended content" data-caption="Content description on iPlayer">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Recommended content" data-caption="Content description on Another">
          <img src="http://placehold.it/600x400" />
        </a>
      </div>
    </div>

    <div class="slide">
      <div class="slide-title">
      <h2>Content description</h2>
      </div>
      <div class="slide-body">
        <a href="http://placehold.it/600x400" data-fancybox="Recommended content" data-caption="Content description on Netflix">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Recommended content" data-caption="Content description on Amazon">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Recommended content" data-caption="Content description on HBO">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Recommended content" data-caption="Content description on iPlayer">
          <img src="http://placehold.it/600x400" />
        </a>
        <a href="http://placehold.it/600x400" data-fancybox="Recommended content" data-caption="Content description on Another">
          <img src="http://placehold.it/600x400" />
        </a>
      </div>
    </div>

  </div>
  
</div>
</div>
</div>

<div style="display:none">
  
</div>

<script type="text/javascript" src="/_inc/VP2014/js/jquery.min.js"></script>
<script type="text/javascript" src="//cdn.jsdelivr.net/jquery.slick/1.6.0/slick.min.js"></script>
<script type="text/javascript" src="/_inc/VP2014/js/jquery.fancybox.min.js"></script>

<script type="text/javascript">
  $(document).ready(function(){
    
    $('.slide-body a img').each(function (i, img) {
      var a = $(img).parent();
      $(a).css('background-image', 'url(' + $(img).attr('src') + ')');
    });
  
    $('.slider').slick({
      slidesToShow: 3,
      prevArrow: '<div class="slider-nav slider-nav-prev"><button type="button" class=""><i class="fa fa-chevron-circle-left"></i></button></div>',
      nextArrow: '<div class="slider-nav slider-nav-next"><button type="button" class=""><i class="fa fa-chevron-circle-right"></i></button></div>',
    });
    
    $(".slider a").fancybox({
		// Options will go here
	});
  });
</script>
</asp:Content>