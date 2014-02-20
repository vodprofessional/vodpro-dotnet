isArticleLoading = false
isFeedLoading = false
isNewsLoading = false

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
    <article class=\"item feed-preview\" data-articleid=\""+item.id+"\">
      <header><a href=\""+item.url+"\">"+item.headline+"</a></header>"
    t += "<img src=\""+item.image+"\"/>" if item.image?
    t += "
      <div class=\"text\">
        <p>"+item.lead+"</p>
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
      <article class=\"item article-preview\" data-articleid=\""+item.id+"\">
        <img src=\""+item.image+"\"/>
        <div class=\"text\">
            <header><a href=\""+item.url+"\">"+item.headline+"</a></header>
            <p>"+item.lead+"</p>
        </div>
        <div class=\"clearfix\"></div>
      </article>
    "

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
    if $("#inner-left").find(".item").length < 280
      $('.pager-loading').show()
      jqXHR = $.getJSON("src/fixture/feed.json")
      jqXHR.done (json) ->
        for item in json
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
    if $("#inner-full").find(".item").length < 280
      $('.pager-loading').show()
      jqXHR = $.getJSON("src/fixture/articles.json")
      jqXHR.done (json) ->
        for item in json
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
    if $('#inner-center').find(".item").length < 100
      $('.pager-loading').show()
      jqXHR = $.getJSON("src/fixture/articles.json")
      jqXHR.done (json) ->
        for item in json
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


$(document).ready () ->

  alert("OW test")

  newsFeedLoader = new NewsFeedLoader(500)
  newsFeedLoader.register()
  newsLoader = new NewsLoader(300)
  newsLoader.register()
  newsArticleLoader = new NewsArticleLoader(300)
  newsArticleLoader.register()

  $("#tab-news").click (e) ->
    $("#column-center").removeClass("active")
    $("#tab-featured").removeClass("active");
    $("#tab-news").addClass("active")
    $("#column-left").addClass("active")
    e.preventDefault

  $("#tab-featured").click (e) ->
    $("#column-left").removeClass("active")
    $("#tab-news").removeClass("active");
    $("#tab-featured").addClass("active")
    $("#column-center").addClass("active")
    e.preventDefault
