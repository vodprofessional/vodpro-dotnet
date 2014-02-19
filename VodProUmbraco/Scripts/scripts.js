var isOpen = false;
var isDimmed = false;
var userClosed = false;
var scrollbar_size;
var fold;
var topfold;

var vp50data;
var vp50dataurl = '/media/VP50/vp50.js?';

var showSlider = true;

$(function () {
    /*  $(".imagereview").hover(function() {
    $(this).css("background-image-position", "-145px 0px");
    },
    function() {
    $(this).css("background-image-position", "0px 0px");
    });*/

    if (jQuery().lightBox) {
        $('a.lightbox').lightBox();
    }


    $('table[style]').removeAttr('style');
    if ($('#regform td[align=right] table tr td').length == 1) {
        $('#regform td[align=right] table tr td').addClass('align-right');
    }
    $('input[value=Previous]').addClass('plain-button');

    if ($("#RSSJobs").length) {
        var url = encodeURIComponent("http://www.labs.jobserve.com/services/v0.3.7/Core.svc/SiteGBR?source=39FB8404A11155&pagesize=3&sort=EXPLORER_DATE_DESC&skills=vod or \"video on demand\" or video or iptv or \"connected tv\" or television or transcode or ingest");

        $.get('/Proxy.ashx?u=' + url, function (data) {

            var html = '';
            var jobsToShow = 3;
            var i = 0;

            $(data).find("Job").each(function () {
                if (i < jobsToShow) {
                    html += '<div class="jobtitle">'
            + '<a href="'
            + $(this).find("JobURL").text()
            + '">'
            + $(this).find("Position").text()
            + '</a>'
            + '</div>';

                    html += '<div class="joblocation">'
            + $(this).find("Location").text()
            + '</div>';

                    html += '<div class="jobsalary">'
            + $(this).find("Rate").text()
            + '<hr/></div>';
                }
                i++;
            });
            $('#RSSJobs').append(html);
        });
    }


    setPageSizeVals();
    $(window).resize(function () {
        setPageSizeVals();
    });

    if ($("#VP50content").length) {
        showSlider = false;
    }

    $(window).scroll(function () {
        if (showSlider) {
            if (!userClosed && !isOpen && isScrolledIntoView('.storytext')) //getPageScroll() > fold)
            {
                open();
            }
            if (isOpen && getPageScroll() < topfold) {
                dim();
            }
        }
    });

    $('#slideInner').hover(function () {
        if (isDimmed) {
            open();
        }
    }, function () {
        if (isDimmed) {
            dim();
        }
    });

    $('#slideInner .next').click(function () {
        open();
    })

    $('#slideInner #close').click(function () {
        close();
    });


    $('#fakepassword').focus(function () {
        $('#fakepassword').hide();
        $('#VPPassword').show();
        $('#VPPassword').focus();
    });
    $('#VPUserName').focus(function () {
        if ($('#VPUserName').val() == "Enter email address") {
            $('#VPUserName').val("");
        }
    });
    $('#VPUserName').blur(function () {
        if ($('#VPUserName').val() == "") {
            $('#VPUserName').val("Enter email address");
        }
    });
    $('#VPReg_Email').focus(function () {
        if ($('#VPReg_Email').val() == "Enter email address") {
            $('#VPReg_Email').val("");
        }
    });
    $('#VPReg_Email').blur(function () {
        if ($('#VPReg_Email').val() == "") {
            $('#VPReg_Email').val("Enter email address");
        }
    });
    $('#VPPassword').blur(function () {
        if ($('#VPPassword').attr('value') == '') {
            $('#VPPassword').hide();
            $('#fakepassword').show();
        }
    });

    checkShowCookieMessage();

    $(".VP50loading").fadeIn(500, function () {

        var url = $("#VP50content").attr("data-url");
        if (url.length == 0) {
            url = vp50dataurl
        }
        $.getJSON(url, function (data) {
            vp50data = data;
            var list = [];
            $.each(vp50data, function (index, val) {
                var thumb = '/css/images/anon-sm.png';
                if (val.thumb != '') { thumb = val.thumb; }
                list.push('<li rel=\"' + index + '\"><div class=\"overlay\">' + val.rank + '</div><img src=\"' + thumb + '\" /></li>');
            });
            $("#VP50roller ul").html(list.join(''));
            $("#VP50roller ul li").on("mouseenter", function () {
                $(this).find(".overlay").css("visibility", "visible");
            });
            $("#VP50roller ul li").on("mouseleave", function () {
                $(this).find(".overlay").css("visibility", "hidden");
            });
            $("#VP50roller ul li").on("click", function () {
                VP50Person($(this).attr("rel"), true);
            });
            $(".VP50loading").fadeOut(500, function () {
                VP50Person('0', false);
            });
        });
    });
    $(".scrubleft").click(function () {
        var l = $("#VP50roller ul").position().left;
        if (l < 0) {
            $("#VP50roller ul").animate({
                left: '+=576'
            }, 2000);
        }
    });
    $(".scrubright").click(function () {
        var l = $("#VP50roller ul").position().left;
        if ((-1 * l) + $("#VP50roller").width() < $("#VP50roller ul").width()) {
            $("#VP50roller ul").animate({
                left: '-=576'
            }, 2000);
        }
    });


});



function setPageSizeVals() {
  scrollbar_size = ($(window).height() / $(document).height()) * $(window).height();
  fold = (scrollbar_size * 2) > $(window).height() ? $(window).height() / 3 : $(document).height() / 2;
  topfold = 200 > fold ? -1:200;
}

function open() {
  $('#slideInner').animate({
    right: '-30', bottom: '0'
    }, 1000, function() { ; }
  );
  isOpen = true;
}
function close() {
  $('#slideInner').animate({
    right: '-450', bottom: '-150'
    }, 1000, function() { ; }
  );
  userClosed = true;
}
function dim() {
  $('#slideInner').animate({
    bottom: '-80'
    }, 1000, function() { ; }
  );
  isDimmed = true;
  isOpen = false;
}
function getPageScroll() {
    var yScroll;
    if (self.pageYOffset) {
      yScroll = self.pageYOffset;
    } else if (document.documentElement && document.documentElement.scrollTop) {
      yScroll = document.documentElement.scrollTop;
    } else if (document.body) {// all other Explorers
      yScroll = document.body.scrollTop;
    }
    return yScroll;
}

function getPageHeight() {
    var windowHeight
    if (self.innerHeight) { // all except Explorer
      windowHeight = self.innerHeight;
    } else if (document.documentElement && document.documentElement.clientHeight) {
      windowHeight = document.documentElement.clientHeight;
    } else if (document.body) { // other Explorers
      windowHeight = document.body.clientHeight;
    }
    return windowHeight
}
      
function isScrolledIntoView(elem)
{
  if($(elem).length > 0)
  {
    var docViewTop = getPageScroll();
    var docViewBottom = docViewTop + $(window).height();

    var elemTop = $(elem).offset().top;
    var elemBottom = elemTop + $(elem).height();
    return ((elemBottom <= docViewBottom));
  }
  else {
    return false;
  }  
}

function TryLogin(submitBtn) {
    
    $("#VPLoginError_user").hide();
    $("#VPLoginError_password").hide();

    var id = submitBtn.id;
    var usernameid = id.substring(0, id.indexOf('btnSubmit')) + 'UserName';
    var pwdid = id.substring(0, id.indexOf('btnSubmit')) + 'Password';
    
    var userval = $('#' + usernameid).val();
    var pwdval = $('#' + pwdid).val();
    var retval = false;
   
    var DataObject = { username: userval , password: pwdval };
    var data = returnXML("/xml-actions?action=check", DataObject);
    $(data).find("loggedinok").each(function(){
      var loggedinok = $(this).text();
      if(loggedinok == 'N')
      {
        $(data).find("error").each(function(){
          $("#VPLoginError_"+$(this).text()).show(100);
        });
        retval = false;
      }
      else
      {
          retval = true;
      }
    });
  return retval;
}


function StartReg(submitBtn, pageid) {
  $("#VPRegError_email").hide();
  var id = submitBtn.id;
  var emailaddressid = id.substring(0, id.indexOf('btnSubmit')) + 'Email';
  var emailval = $('#' + emailaddressid).val();
  if( !isValidEmailAddress( emailval ) )
  {
     $("#VPRegError_email").show(100);
  }
  else 
  {
    var loc = window.location.href;
    if(loc.indexOf("/vui/") > 0)
    {
      window.location = "/vui/vuiregister?page=" + pageid + "&email=" + emailval;
    }
    else
    {
      window.location = "/register?page=" + pageid + "&email=" + emailval;
    }
  }
}
  
function isValidEmailAddress(emailAddress) {
  var pattern = new RegExp(/^(("[\w-+\s]+")|([\w-+]+(?:\.[\w-+]+)*)|("[\w-+\s]+")([\w-+]+(?:\.[\w-+]+)*))(@((?:[\w-+]+\.)*\w[\w-+]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][\d]\.|1[\d]{2}\.|[\d]{1,2}\.))((25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\.){2}(25[0-5]|2[0-4][\d]|1[\d]{2}|[\d]{1,2})\]?$)/i);
  return pattern.test(emailAddress);
};
  
function returnXML(url, DataObject) {
    var html = $.ajax({
        url: url,
        type: "POST",
        data: DataObject,
        error: function (x, e) {
            if (x.status == 0) {
                alert('You are offline!!\n Please Check Your Network.');
            } else if (x.status == 404) {
                alert('Requested URL not found.');
            } else if (x.status == 500) {
                alert('Internel Server Error.');
                //    $("#OTGerror").html(x.responseText);
            } else if (e == 'parsererror') {
                alert('Error.\nParsing JSON Request failed.');
            } else if (e == 'timeout') {
                alert('Request Time out.');
            } else {
                alert('Unknow Error.\n' + x.responseText);
            }
        }, async: false
    }).responseText;
    return html;
}


function VP50Person(i, fadeout) {
  var index = parseInt(i);
  var personData = vp50data[i];
  var fadeoutTime = 400;
  if(!fadeout) {
    fadeoutTime = 0;
  }
  $("#VP50content .content").fadeOut(fadeoutTime, function() {
  // Some fading
    var portrait = '/css/images/anon.png';
    if(personData.img != '') { var portrait = personData.img ; }
    $("#VP50content .content")
      .find("h2").html('#' + personData.rank  + ' ' + personData.name).end()
      .find("h3").html(personData.job).end()
      .find("p .brief").html(personData.brief).end()
      .find("p .long").html(personData.long).end()
      .find("img.portrait").attr("src",portrait).end();
    
    if(personData.link != null && personData.link != "") {
      $("#VP50content .content")
        .find("a.more").attr("href",personData.link).text(personData.relatedlinktext).css("display","block").end();
    }
    else {
      $("#VP50content .content")
        .find("a.more").attr("href","").css("display","none").end();
    }
    $("#VP50content .content").fadeIn(1000);
  });
}

function checkShowCookieMessage() {
    var seen_message = getCookie("seen_cookie_message");
    if (seen_message != null && seen_message != "") {
        // Do nothing
    }
    else {
        if ($("#CookieMessage")) {
            $("#CookieMessage").show(500);
            $("#CookieMessageAgree").on("click", function (event) {
                setCookie("seen_cookie_message", "yes", 3650);
                $("#CookieMessage").hide(500);
            });
        }
    }
}


function setCookie(c_name, value, exdays) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + exdays);
    var c_value = escape(value) + ((exdays == null) ? "" : "; expires=" + exdate.toUTCString());
    document.cookie = c_name + "=" + c_value + "; path=/";
}
function getCookie(c_name) {
    var i, x, y, ARRcookies = document.cookie.split(";");
    for (i = 0; i < ARRcookies.length; i++) {
        x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
        y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
        x = x.replace(/^\s+|\s+$/g, "");
        if (x == c_name) {
            return unescape(y);
        }
    }
}
function del_cookie(name) {
    document.cookie = name + '= ; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}