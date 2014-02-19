/* Author:

*/
var xmlActionsURL = '/vui/vui-xml-actions/';

var ACTION_GET_IMAGES_FOR_SERVICE = "si";
var ACTION_GET_SERVICE_LIST_BY_NAME = "sn";
var ACTION_GET_SERVICE_LIST_BY_PLATFORM = "sp";
var ACTION_GET_SERVICE_LIST_BY_NAME_AND_PLATFORM = "snp";
var ACTION_GET_ALL_SERVICE_LIST = "sa";
var ACTION_RATE_SERVICE = "rs";
var ACTION_FAVOURITE_IMAGE = "fi";
var ACTION_UNFAVOURITE_IMAGE = "ufi";

var RESPONSE_SUCCESS = "Success";

var TITLE_ALL_SERVICES = "All Services";
var TITLE_PLATFORM = "#platform#";
var TITLE_PLATFORM_SERVICE = '<a href="#" class="ui-link-platform">#platform#</a> | #service#';
var TITLE_PLATFORM_DEVICE_SERVICE = '<a href="#" class="ui-link-platform">#platform#</a> | <a href="#" class="ui-link-service">#service#</a> | #device#';
var TITLE_SERVICE = "#service#";
var TITLE_IMAGE = "#service# | #device# {#pagetype}";
var TITLE_FAVOURITE = "This is one of your favourite images";
var TITLE_RATING = "rated #n#/10 by #m# users";

var RESIZE_FIX = 0;
var RESIZE_SCROLLBARCORRECTION = 45;
var LEFT_NAV_SCROLL_CORRECT = 3;
var BORDER_CORRECT = 30;

var sizeOfStar = 15;
var starOriginalOffset = -150;
var starVerticalOffset = -45;

var headingHeightSmall = 40;
var headingHeightLarge = 95;

var UIROOT = 'vui/ui';
var currentDataRating = '0.0';

var BaseTitle = "";



$(function () {
    BaseTitle = document.title;




    var matrixTop = 290;
    $(document).scroll(function () {
        if ($("tr.matrix-header").length > 0) {
            if ($(document).scrollTop() > matrixTop && !$("tr.matrix-header").hasClass("fixed")) {
                $("tr.matrix-header").addClass("fixed");
            }
            if ($(document).scrollTop() <= matrixTop && $("tr.matrix-header").hasClass("fixed")) {
                $("tr.matrix-header").removeClass("fixed");
            }
        }
    });

    // Default view = tiles
    $(".ui-screenshot-view-selector li a.tiles").addClass("active");

    // Show the little bubble for the matrix view button
    $("#matrix-pop-out").fadeIn(1000).delay(5000).fadeOut(1000);
    $("#matrix-pop-out").click(function () { $(this).hide(); });
    $(".ui-screenshot-view-selector li a.tiles").click(function () {
        if (!$(".ui-screenshot-view-selector li a.tiles").hasClass("active")) {
            $(".ui-screenshot-view-selector li a.tiles").addClass("active")
        }
        if ($(".ui-screenshot-view-selector li a.matrix").hasClass("active")) {
            $(".ui-screenshot-view-selector li a.matrix").removeClass("active")
        }
        if ($("#main-ui-grid-wrapper").hasClass("hidden")) {
            $("html, body").animate({ scrollTop: 0 }, 500);
            $("#screenshot-matrix").fadeOut();
            $("#screenshot-matrix").addClass("hidden");
            $("#main-ui-grid-wrapper").fadeIn();
            $("#main-ui-grid-wrapper").removeClass("hidden");
        }
    });
    // Show the matrix view
    $(".ui-screenshot-view-selector li a.matrix").click(function () {

        $("#matrix-pop-out").hide();

        if (!$(".ui-screenshot-view-selector li a.matrix").hasClass("active")) {
            $(".ui-screenshot-view-selector li a.matrix").addClass("active")
        }
        if ($(".ui-screenshot-view-selector li a.tiles").hasClass("active")) {
            $(".ui-screenshot-view-selector li a.tiles").removeClass("active")
        }
        if ($("#screenshot-matrix").hasClass("hidden")) {
            $("html, body").animate({ scrollTop: 0 }, 500);
            $("#main-ui-grid-wrapper").fadeOut();
            $("#main-ui-grid-wrapper").addClass("hidden");
            $("#screenshot-matrix").fadeIn(500, function () {
                $("#screenshot-matrix").removeClass("hidden");
                matrixTop = $("tr.matrix-header").offset().top + $("tr.matrix-header").height();
            });

        }
    });
    // Show the search pane
    $(".ui-screenshot-view-selector li a.search").click(function () {
        $("#matrix-pop-out").hide();


    });


    $("#search-service").select2({
        placeholder: "Choose a service",
        minimumInputLength: 1
    });
    $("#search-service").on("change", function (e) {
        SearchImages();
    });

    // Back-button support
    var h = window.location.hash;
    if (h == "#matrix") {
        $(".ui-screenshot-view-selector li a.matrix").trigger('click');
    }

    // 





    $(".ui-member-message").show(800);

    $("img.lazy").lazyload();

    $(".ui-service-image a").on("click", function (event) {
        imageLinkOnClick2(event, this);
    });

    // New Top-Nav Stuff
    $("nav.top ul li").hover(
		function () {
		    $(this).find("ul.ui-nav-level-1").show(100);
		},
		function () {
		    $(this).find("ul.ui-nav-level-1").hide(100);
		}
	);
    $("nav.top ul li ul.ui-nav-level-1 li").hover(
		function () {
		    $(this).addClass("ui-hover-on");
		    $(this).find("ul").show(300);
		},
		function () {
		    $(this).removeClass("ui-hover-on");
		    $(this).find("ul").hide(300);
		}
	);
    $("nav.top ul li ul li").click(function () {
        $(this).mouseout();
    });


    $(".ui-print-link").on("click", function () {
        print();
    });

    $("a.ui-favourite-link:not(.ui-favourite)").off("click");
    $("a.ui-favourite-link:not(.ui-favourite)").on("click", function (event) {
        event.preventDefault();
        event.stopPropagation();
        event.stopImmediatePropagation();
        imageFavouriteClick(event, this);
    });

    $(".ui-item-rate a").on("mousemove", function (event) {
        rateItStarsHover(event, this);
    });

    $(".ui-item-rate a").on("mouseout", function (event) {
        var yoffset = 0
        currentDataRating = parseInt($(this).parent().attr("data-rating"));
        if (currentDataRating == '0.0') {
            yoffset = starVerticalOffset;
            $(this).parent().find("span").html('');
            $(this).css("background-position", (starOriginalOffset) + 'px ' + yoffset + 'px');
        }
        else {
            $(this).parent().find("span").html(Math.floor(currentDataRating) + '/10');
            $(this).css("background-position", (starOriginalOffset + (Math.floor(currentDataRating) * sizeOfStar)) + 'px ' + yoffset + 'px');
        }
    });
    $(".ui-item-rate a").on("click", function (event) {
        rateServiceClick(event, this);
    });

    $("#btnEUVatNumber").on("click", function (event) {
        $(this).attr('disabled', 'disabled');
        document.location = "/vui/subscribe/process/EUVAT/" + $("#euVATNumber").val();
    });

    $("#btnPONumber").on("click", function (event) {
        $(this).attr('disabled', 'disabled');
        document.location = "/vui/subscribe/process/PONUMBER/" + $("#purchaseOrderNumber").val();
    });

    $("#btnPromoCode").on("click", function (event) {
        $(this).attr('disabled', 'disabled');
        document.location = "/vui/subscribe/process/PROMO/" + $("#promoCode").val();
    });

    setTimeout(function () {
        $("body").scrollTop("1");
        $("#ui-col-container").resize();
    }, 100);
});


function SearchImages() 
{
    var service = $("#search-service").val();
    var pagetype = '';
    var DataObject = { "service": service, "pagetype": pagetype };
    $.ajax({
        url: xmlActionsURL + "?a=ss",
        type: "POST",
        dataType: 'json',
        data: DataObject,
        success: function (data) {

            for (i = 0; i < data.length; i++) {

                var result = $("#search-result-template li").clone();
                result
                    .find("dd[prop=service]").html(data[i].ServiceName).end()
                    .find("dd[prop=pagetype]").html(data[i].PageType).end()
                    .find("span.platform").html(data[i].Platform + data[i].Device).end()
                    .find("img").attr("src",data[i].ImageURL_th).end()
                    ;

                $("#search-results ul").append(result);
            }

        }
    });
}

function rateItStarsHover(event, selectedItem)
{
	var yoffset = 0
	if(currentDataRating == '0.0')
	{
		yoffset = starVerticalOffset;
	}
	var x = event.pageX - $(selectedItem).offset().left;
	var numStars = Math.floor(x/sizeOfStar)+1;
	$(selectedItem).css("background-position",(starOriginalOffset+(numStars*sizeOfStar))+'px ' + yoffset + 'px');
	$(selectedItem).parent().find("span").html(numStars + '/10')
}


function rateServiceClick(event, selectedItem) 
{
    var x = event.pageX - $(selectedItem).offset().left;
    var rating = Math.floor(x/sizeOfStar)+1;
	
    // var rating = $("#service-rating").val();
    var serviceid = $(selectedItem).parent().attr("data-serviceid");
    var DataObject = { "rating": rating, "serviceid": serviceid };
    $.ajax({
        url: xmlActionsURL + "?a=" + ACTION_RATE_SERVICE,
        type: "POST",
        dataType: 'json',
        data: DataObject,
        success: function (data) {
            if (data.response == RESPONSE_SUCCESS) {
                populateRatingData(data.servicedata);
            }
        }
    });
}


function imageFavouriteClick(event, selectedItem)
{
/*	$.each($(selectedItem).data('events'), function(i, e) {
		console.log(i, e);
	});
*/


	var id = $(selectedItem).attr("data-id");

	$(selectedItem).addClass("ui-favourite");

	var DataObject = { "imageid": id, "action": "ADD" };
	$.ajax({
	    url: xmlActionsURL + "?a=" + ACTION_FAVOURITE_IMAGE,
	    type: "POST",
	    dataType: "json",
	    data: DataObject,
	    success: function (data) {
	        if (data.response == RESPONSE_SUCCESS) {
	            $(selectedItem).addClass("ui-favourite");
	            $(".ui-image-info[data-id=" + id + "]")
					.addClass("ui-favourite").attr("title", TITLE_FAVOURITE);
	        }
	        else {
	            $(selectedItem).removeClass("ui-favourite");
	        }
	    }
	});
	return false;
}


function populateRatingData(servicedata) {
    $(".ui-heading")
        .find("div.ui-item-rate")
            .attr("data-serviceid", servicedata.id)
			.attr("data-rating", servicedata.ratingPersonal)
			.end()
        .find("div.ui-item-rating")
		    .html(servicedata.ratingOverall).end()
		.find("div.ui-item-overall-rating")
			.html(TITLE_RATING.replace("#n#", servicedata.ratingOverall).replace("#m#", servicedata.ratersOverall)).end();
			
	var yoffset =  starVerticalOffset;
	var xoffset =  starOriginalOffset;
	
	currentDataRating = servicedata.ratingPersonal
	
	if(currentDataRating != '0.0')
	{
		yoffset = 0;
		xoffset = xoffset + currentDataRating * sizeOfStar;
		$(".ui-heading")
			.find("div.ui-item-rate")
				.find("span")
					.html(Math.floor(currentDataRating) + '/10')
					.end()
	}
	else
	{
		$(".ui-heading")
			.find("div.ui-item-rate")
				.find("span")
					.html('')
					.end()
	}
	$(".ui-heading .ui-item-rate a").css("background-position", xoffset+'px ' + yoffset + 'px');
	
}

// !Lightbox
function imageLinkOnClick2(event, selectedItem) {
    event.preventDefault();

	$(".ui-heading").addClass("ui-hide-from-print");
	$("#wrapper").addClass("ui-hide-from-print");
	
	$(".ui-lightbox").fancybox({
		fitToView	: false,
		width		: '70%',
		height		: '70%',
		autoSize	: true,
		closeClick	: false,
		arrows      : false,
		helpers     : {
			buttons	: { position: "top"}
		},
		afterClose  : function() { 
			$(".ui-heading").removeClass("ui-hide-from-print");
			$("#wrapper").removeClass("ui-hide-from-print"); 
		}
	});
	
}


function imageLinkOnClick(event, selectedItem) 
{
    event.preventDefault();
    $("#main-ui-grid ul li").removeAttr("class");
    $(selectedItem).parent().attr("class", "ui-item-selected");
    toggleGridCarousel("carousel");
    thumbClicked(selectedItem);
}


function thumbClicked(clickedLink) 
{
	$("#main-ui-grid ul li").removeAttr("class");
	$(clickedLink).parent().attr("class", "ui-item-selected");
	var imageData = {
		img_lg: $(clickedLink).attr("rel"),
		img_full: $(clickedLink).attr("href"),
		img_id: $(clickedLink).find("img").attr("data-id"),
		uiPlatform: $(clickedLink).find("img").attr("data-platform"),
		uiDevice: $(clickedLink).find("img").attr("data-device"),
		uiService: $(clickedLink).find("img").attr("data-service"),
		uiPageType: $(clickedLink).find("img").attr("data-pagetype"),
        ratingPersonal: $(clickedLink).find("img").attr("data-ratingpersonal"),
        ratersPersonal: $(clickedLink).find("img").attr("data-raterspersonal"),
        ratingTeam: $(clickedLink).find("img").attr("data-ratingteam"),
        ratersTeam: $(clickedLink).find("img").attr("data-ratersteam"),
        ratingOverall: $(clickedLink).find("img").attr("data-ratingoverall"),
        ratersOverall: $(clickedLink).find("img").attr("data-ratersoverall")
	};
	openClickedImage(imageData);
}
