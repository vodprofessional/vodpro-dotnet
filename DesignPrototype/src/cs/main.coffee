isArticleLoading = false
isFeedLoading = false
isNewsLoading = false
isJobsLoading = false

###

###
class NewsItem
  constructor: (target) ->
    @target = target

  template: (item) ->
    alert("Not implemented")

  render: (item) ->
    alert("Not implemented")

###

###
class NewsFeedItem extends NewsItem
  template: (item) ->
    t = "
    <article class=\"item feed-preview\" data-articleid=\""+item.Id+"\">
      <header><a href=\""+item.Url+"\">"+item.Headline+"</a></header>"
    t += "<img src=\""+item.Image+"\"/>" if item.Image?
    t += "
      <div class=\"text\">
        <p>"+item.Teaser+"</p>
      </div>
      <div class=\"clearfix\"/>
     </article>
    "

  render: (item) ->
    @target.append this.template(item)
###

###
class NewsArticleItem extends NewsItem
  template: (item) ->
    "
      <article class=\"item article-preview\" data-articleid=\""+item.Id+"\">
        <img src=\""+item.Image+"\"/>
        <div class=\"text\">
            <header><a href=\""+item.Url+"\">"+item.Headline+"</a></header>
            <p>"+item.Teaser+"</p>
        </div>
        <div class=\"clearfix\"></div>
      </article>
    "

  render: (item) ->
    @target.append this.template(item)
###

###
class JobItem extends NewsItem
  template: (item) ->
  
    description = "" 
    metadata = ""
    if (item.Description?.length)
      description = "<p>" + item.Description + "</p>"
    if (item.Metadata?.length)
      metadata = "<div class=\"meta\">" + item.Metadata + "</div>"
      
    "
        <article class=\"item job\">
            <header>
                <a href=\"" + item.Url + "\" target=\"_blank\">" + item.Title + "</a>
            </header>
            <div class=\"text\">
                <div class=\"post-date\">Posted on: " + item.Date + "</div>" + description + metadata + "<div></article>"
                
  
  render: (item) ->
    @target.append this.template(item)
###


###
class NewsFeedLoader
  constructor: (bottomDelta) ->
    @delta = bottomDelta

  register: () ->
    _delta = @delta
    _load = this.load
    $(window).scroll () ->
      _load() if $('#column-left').height() - $(window).height() - $(window).scrollTop() < _delta and not isFeedLoading
    $(document).bind 'touchmove', () ->
      _load() if $('#column-left').height() - $(window).height() - $(window).scrollTop() < _delta and not isFeedLoading
    _load()

  load: ->
    isFeedLoading = true
    numItems = $("#inner-left").find(".item").length
    if numItems < 280
      $('.pager-loading').show()
      data = { "a":"news", "c":20, "s":numItems }
      jqXHR = $.getJSON("/ajax-actions", data)
      jqXHR.done (json) ->
        for item in json.data.Articles
          do () ->
            obj = new NewsFeedItem($("#inner-left"))
            obj.render(item)
        $('.pager-loading').hide()
        isFeedLoading = false
      jqXHR.error () ->
        $("#main .pager").show()
        $('.pager-loading').hide()
        isFeedLoading = false
    else
      $("#main .pager").show()
      $('.pager-loading').hide()

###

###
class NewsLoader
  constructor: (bottomDelta) ->
    @delta = bottomDelta

  register: () ->
    _delta = @delta
    _load = this.load
    $(window).scroll () ->
      _load() if $('#column-full').height() - $(window).height() - $(window).scrollTop() < _delta and not isNewsLoading
    $(document).bind 'touchmove', () ->
      _load() if $('#column-full').height() - $(window).height() - $(window).scrollTop() < _delta and not isNewsLoading
    _load()

  load: ->
    isNewsLoading = true
    numItems = $("#inner-full").find(".item").length
    rootNodeId = $("#inner-full").attr("data-root");
    if numItems < 280
      $('.pager-loading').show()
      data = { "a":"any", "r":rootNodeId, "c":20, "s":numItems }
      jqXHR = $.getJSON("/ajax-actions", data)
      jqXHR.done (json) ->
        for item in json.data.Articles
          do () ->
            obj = new NewsArticleItem($("#inner-full"))
            obj.render(item)
        $('.pager-loading').hide()
        isNewsLoading = false
      jqXHR.error () ->
        $("#main .pager").show()
        $('.pager-loading').hide()
        isNewsLoading = false
    else
      $("#main .pager").show()
      $('.pager-loading').hide()

###

###
class NewsArticleLoader
  constructor: (bottomDelta) ->
    @delta = bottomDelta

  register: () ->
    _delta = @delta
    _load = this.load
    $(window).scroll () ->
      _load() if $('#column-center').height() - $(window).height() - $(window).scrollTop() < _delta and not isArticleLoading
    $(document).bind 'touchmove', () ->
      _load() if $('#column-center').height() - $(window).height() - $(window).scrollTop() < _delta and not isArticleLoading
    _load()

  load: ->
    isArticleLoading = true
    numItems = $("#inner-center").find(".item").length
    if numItems < 100
      $('.pager-loading').show()
      data = { "a":"features", "c":20, "s":numItems }
      jqXHR = $.getJSON("/ajax-actions",data)
      jqXHR.done (json) ->
        for item in json.data.Articles
          do () ->
            obj = new NewsArticleItem($('#inner-center'))
            obj.render(item)
        $('.pager-loading').hide()
        isArticleLoading = false
      jqXHR.error () ->
        $("#main .pager").show()
        $('.pager-loading').hide()
        isArticleLoading = false
    else
      $("#main .pager").show()
      $('.pager-loading').hide()
###


###        
class JobsLoader
  constructor: (bottomDelta) ->
    @delta = bottomDelta

  register: () ->
    _delta = @delta
    _load = this.load
    $(window).scroll () ->
      _load() if $('#column-full').height() - $(window).height() - $(window).scrollTop() < _delta and not isJobsLoading
    $(document).bind 'touchmove', () ->
      _load() if $('#column-full').height() - $(window).height() - $(window).scrollTop() < _delta and not isJobsLoading
    _load()

  load: ->
    isJobsLoading = true
    numItems = $('#inner-job-listing').find(".item").length
    if numItems < 200
      $('.pager-loading').show()
      data = { "a":"jobs", "c":20, "s":numItems }
      jqXHR = $.getJSON("/ajax-actions", data)
      jqXHR.done (json) ->
        for item in json.data.JobList
          do () ->
            obj = new JobItem($('#inner-job-listing'))
            obj.render(item)
        $('.pager-loading').hide()
        isJobsLoading = false
      jqXHR.error () ->
        $("#main .pager").show()
        $('.pager-loading').hide()
        isJobsLoading = false
    else
      $("#main .pager").show()
      $('.pager-loading').hide()


###


###   
class RegWall
  register: () ->
    _login = this.login
    _reg = this.reg
    _login() 
    _reg()
    
  login: () ->
    bindEnter($('#signin'), $('#signin button[type="submit"]'));  
    $('#regwall-signin').click (e) ->
      e.preventDefault() 
      u = document.location + '?loggedin#premium';
      user = $('#regwall-email').val();
      pass = $('#regwall-pwd').val();
      #rem = $('#regwall-remember-me').is(":checked");
      rem = false
      data = { "a":"l", "user":user, "pass":pass, "rem":rem }
      jqXHR = $.ajax( {
        type: 'POST'
        url:'/ajax-actions',
        data: data,
        dataType: 'JSON' 
      })
      jqXHR.done (json) ->
        document.location = u if json.response == 'valid'
        $('#regwall-pwd-error').html(json.data) if json.response == 'invalid'
      jqXHR.error () ->
        alert(error)

  reg: () ->
    bindEnter($('#register'), $('#register button[type="submit"]'));  
    $('#regwall-register').click (e) ->
      e.preventDefault()
      document.location = '/register?email=' + $('#regwall-email-reg').val() 
###


###  
class RHSSignup
  register: () ->
    _reg = this.reg
    _reg()
    
  reg: () ->
    bindEnter($('#rhs-form-signin'), $('#rhs-register'));  
    $('#rhs-register-btn').click (e) ->
      e.preventDefault()
      document.location = '/register?email=' + $('#rhs-email-reg').val() 
###


###
class Search
  register: () ->
    _search = this.search
    _search()

  search: () ->
    $('#search-button').click (e) ->
      e.preventDefault()
      term = $("#search").val()
      target = $(this).attr("data-target")
      if term?.length
        term = encodeURIComponent(term)
        document.location.href = "#{target}?search=#{term}" 

###


###        
class SearchLoader
  constructor: (bottomDelta) ->
    @delta = bottomDelta

  register: () ->
    _delta = @delta
    _load = this.load
    $(window).scroll () ->
      _load() if $('#column-full').height() - $(window).height() - $(window).scrollTop() < _delta and not isNewsLoading
    $(document).bind 'touchmove', () ->
      _load() if $('#column-full').height() - $(window).height() - $(window).scrollTop() < _delta and not isNewsLoading
    _load()

  load: ->
    isNewsLoading = true
    numItems = $('#inner-search-results').find(".item").length
    term = $('#inner-search-results').attr('data-searchterm')
    if numItems < 200
      $('.pager-loading').show()
      data = { "a":"sr", "t":term, "c":20, "s":numItems }
      jqXHR = $.getJSON("/ajax-actions", data)
      jqXHR.done (json) ->
        for item in json.data.Articles
          do () ->
            obj = new NewsArticleItem($('#inner-search-results'))
            obj.render(item)
        $('.pager-loading').hide()
        isNewsLoading = false
      jqXHR.error () ->
        $("#main .pager").show()
        $('.pager-loading').hide()
        isNewsLoading = false
    else
      $("#main .pager").show()
      $('.pager-loading').hide()

###


###
bindEnter = (panel, button) ->
  panel.keyup (event) ->
    code = event.which;
    if code is 13
      button.trigger("click");
      event.preventDefault();
###

###
$(document).ready () ->

  if $('#vp2-regwall').length > 0
    regwall = new RegWall()
    regwall.register() 

  if $("#inner-left").length > 0
    newsFeedLoader = new NewsFeedLoader(500)
    newsFeedLoader.register()
  if $("#inner-full").length > 0
    newsLoader = new NewsLoader(300)
    newsLoader.register()
  if $("#inner-center").length > 0
    newsArticleLoader = new NewsArticleLoader(300)
    newsArticleLoader.register()
  if $('#search-button').length > 0
    bindEnter($('nav .form-inline .form-group'), $("#search-button"));
    search = new Search()
    search.register()
  if $("#inner-search-results").length > 0
    searchLoader = new SearchLoader(300)
    searchLoader.register()
  if $("#inner-job-listing").length > 0
    jobsLoader = new JobsLoader(300)
    jobsLoader.register()
  if $('#rhs-register').length > 0
    rhsReg = new RHSSignup()
    rhsReg.register()

  $("#tab-news").click (e) ->
    $("#column-center").removeClass("active")
    $("#tab-featured").removeClass("active");
    $("#tab-news").addClass("active")
    $("#column-left").addClass("active")
    e.preventDefault()

  $("#tab-featured").click (e) ->
    $("#column-left").removeClass("active")
    $("#tab-news").removeClass("active");
    $("#tab-featured").addClass("active")
    $("#column-center").addClass("active")
    e.preventDefault()

  if $('.form-registration').length > 0
    $('.form-registration [type="submit"]').addClass("btn btn-lg btn-primary")
    if $('.orgtype').length > 0
      $('.orgtype').change (e) ->
        orgtype = $(this).val()
        if orgtype.toLowerCase() == 'other'
          $('.orgtypeother').show()
        else
          $('.orgtypeother').hide()

  if $('.article-main img.vp-lightbox').length > 0
    $('.article-main img.vp-lightbox').each (index, e) ->
      imgurl = $(e).attr('src')
      $(e).wrap("<a href=\"#{imgurl}\" data-lightbox=\"defaultgroup\"></a>")

          
#  $('#forgotten-pwd-link a').click (e) ->
#    $('#signin-form-main').hide()
#    $('#forgotten-pwd').show()
#    e.preventDefault()
#    
#  $('#forgotten-pwd-cancel-link a').click (e) ->
#    $('#forgotten-pwd').hide()
#    $('#signin-form-main').show()
#    e.preventDefault()

  if jQuery().lightBox
    $('a.lightbox').lightBox();