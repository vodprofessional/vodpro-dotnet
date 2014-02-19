<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="vui-categorise-images.ascx.cs" Inherits="VUI.usercontrols.vui_categorise_images" %>


<input type="hidden" id="lb-pagetype" value="NONE" />
<input type="hidden" id="lb-device" value="" />
<input type="hidden" id="lb-service" value="" />

<button type="button" class="open-lightbox">
    <span>Open the Lightbox of Uncategorised Images</span>
</button>





  <span id="lightboxes"></span>    
  
  
    
  <div id="lightbox-templates">

  <div class="lightbox-template" data-analysis="" data-service="">
    <a class="lightbox-refresh icon-refresh" href="#" title="Load Next 100"></a>
    <a class="lightbox-close icon-remove" href="#" title="Close"></a>
    <div class="lightbox-main">
	  <!-- Left Nav scrolling -->
      <div class="lightbox-leftside">
          <ul>
            <li data-id="101">Splash page	</li>
            <li data-id="102">Homepage	</li>
            <li data-id="103">Category homepage	</li>
            <li data-id="104">Categorisation	</li>
            <li data-id="105">Video description	</li>
            <li data-id="106">Branded video player	</li>
            <li data-id="107">Native video player	</li>
            <li data-id="108">YouTube video player	</li>
            <li data-id="109">Content recommendation	</li>
            <li data-id="110">Most popular	</li>
            <li data-id="111">Recently added	</li>
            <li data-id="112">Favourites	</li>
            <li data-id="113">Viewing history	</li>
            <li data-id="114">EPG	</li>
            <li data-id="115">Help	</li>
            <li data-id="116">Watch on other platforms	</li>
            <li data-id="117">Parental controls	</li>
            <li data-id="118">Contact	</li>
            <li data-id="119">About	</li>
            <li data-id="120">Search	</li>
            <li data-id="121">Predictive search	</li>
            <li data-id="122">A-Z page	</li>
            <li data-id="123">Playlists	</li>
            <li data-id="124">Sign in / Register	</li>
            <li data-id="131">Social sign-on	</li>
            <li data-id="125">Social sharing (out)	</li>
            <li data-id="126">Social sharing (in-service)	</li>
            <li data-id="127">Buy / Subscribe	</li>
            <li data-id="128">Special functionality	</li>
            <li data-id="129">Accessibility	</li>
            <li data-id="130">Advertising functionality	</li>
            <li data-id="182">Customised video player	</li>
            <li data-id="183">Featured content	</li>
            <li data-id="184">More episodes	</li>
            <li data-id="185">Device synchronisation	</li>
            <li data-id="186">Adaptive bitrate streaming	</li>
            <li data-id="187">Resume after stopping	</li>
            <li data-id="188">Download to device	</li>
            <li data-id="189">Live Restart	</li>
            <li data-id="190">Last Viewed	</li>
            <li data-id="191">Live Viewing	</li>
            <li data-id="192">Extended Archive	</li>
            <li data-id="193">Social recommendation	</li>
            <li data-id="194">Audio-described Shows	</li>
          </ul>
        <!--<div class="lightbox-nav"></div>-->
      </div>
      <div class="lightbox-gallery">
        <a class="lightbox-prev icon-chevron-left" href="#"></a>
        <div class="carousel"><ul></ul></div>
        <a class="lightbox-next icon-chevron-right" href="#"></a>
      </div>
    </div>
  </div>


    <span class="left-nav-template">
	  <a href="#" class="nav"> <!--  class="active vis" for first item -->
        <div class="thumb-info">
          <div class="player">[Service Name]</div>
          <div class="name">[Page Category]</div>
          <div class="device">[Platform / Device]</div>
          <div class="date">[Import Tag]</div>
        </div>
        <div class="thumb-img"><img src="images/tmp/lightbox/img-thumb1.jpg" alt="Category Homepage"></div>
      </a>
	</span>
    <span class="left-nav-reload-template">
      <a href="#" class="load-more">
        Load More...
      </a>
    </span>
	<span class="main-image-template">
	<li class="lightbox-item"> <!-- class="active" for first item -->
      <div class="lightbox-item-header">
		<div class="lightbox-item-panel">
		  <a class="print icon-print" href="#"></a><a class="clip icon-trash" href="#" title="Delete Image"></a><span class="count">[number of number]</span>
		</div>
        <div class="lightbox-item-package"></div>
		<div class="lightbox-item-title">[Page Category]</div>
	  </div>
	  <div class="lightbox-item-img">
        <div class="lightbox-item-img-inner" id="" data-id="">
		  <img src="images/tmp/lightbox/img2.jpg" alt="Category Homepage">
		</div>
	  </div>
	</li>
	</span>
  </div>      
<link href="/_inc/css/font-awesome.css" rel="stylesheet">  
<link rel="Stylesheet" type="text/css" href="/_inc/css/vui-admin-lightbox-styles.css?<%= DateTime.Now.ToString("hhmmss") %>" />
<script src="//code.jquery.com/jquery-1.10.1.min.js"></script>
<script src="//code.jquery.com/ui/1.10.3/jquery-ui.min.js"></script>
<script src="/_inc/js/jcarousellite.min.js"></script> 
<script src="/Scripts/jquery.lazyload.min.js" type="text/javascript"></script>
<script src="/_inc/js/vui-admin-lightbox.js?<%= DateTime.Now.ToString("hhmmss") %>" type="text/javascript"></script>
<script src="/_inc/js/jquery.ui.touch-punch.min.js"></script>
<script type="text/javascript">
    var numResults = 100;
    $(document).ready(function () {
        $(".open-lightbox").vuiLightbox({ "screenshots": "uncategorised", "pageTypeSelector": "#lb-pagetype", "deviceSelector": "#lb-device" });
    });
</script>