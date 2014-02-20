(function() {
  var NewsArticleItem, NewsArticleLoader, NewsFeedItem, NewsFeedLoader, NewsItem, NewsLoader, isArticleLoading, isFeedLoading, isNewsLoading, _ref, _ref1,
    __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  isArticleLoading = false;

  isFeedLoading = false;

  isNewsLoading = false;

  /*
  */


  NewsItem = (function() {
    function NewsItem(target) {
      this.target = target;
    }

    NewsItem.prototype.template = function(item) {
      return alert("Not implemented");
    };

    NewsItem.prototype.render = function(item) {
      return alert("Not implemented");
    };

    return NewsItem;

  })();

  /*
  */


  NewsFeedItem = (function(_super) {
    __extends(NewsFeedItem, _super);

    function NewsFeedItem() {
      _ref = NewsFeedItem.__super__.constructor.apply(this, arguments);
      return _ref;
    }

    NewsFeedItem.prototype.template = function(item) {
      var t;
      t = "    <article class=\"item feed-preview\" data-articleid=\"" + item.id + "\">      <header><a href=\"" + item.url + "\">" + item.headline + "</a></header>";
      if (item.image != null) {
        t += "<img src=\"" + item.image + "\"/>";
      }
      return t += "      <div class=\"text\">        <p>" + item.lead + "</p>      </div>      <div class=\"clearfix\"/>     </article>    ";
    };

    NewsFeedItem.prototype.render = function(item) {
      return this.target.append(this.template(item));
    };

    return NewsFeedItem;

  })(NewsItem);

  /*
  */


  NewsArticleItem = (function(_super) {
    __extends(NewsArticleItem, _super);

    function NewsArticleItem() {
      _ref1 = NewsArticleItem.__super__.constructor.apply(this, arguments);
      return _ref1;
    }

    NewsArticleItem.prototype.template = function(item) {
      return "      <article class=\"item article-preview\" data-articleid=\"" + item.id + "\">        <img src=\"" + item.image + "\"/>        <div class=\"text\">            <header><a href=\"" + item.url + "\">" + item.headline + "</a></header>            <p>" + item.lead + "</p>        </div>        <div class=\"clearfix\"></div>      </article>    ";
    };

    NewsArticleItem.prototype.render = function(item) {
      return this.target.append(this.template(item));
    };

    return NewsArticleItem;

  })(NewsItem);

  /*
  */


  NewsFeedLoader = (function() {
    function NewsFeedLoader(bottomDelta) {
      this.delta = bottomDelta;
    }

    NewsFeedLoader.prototype.register = function() {
      var _delta, _load;
      _delta = this.delta;
      _load = this.load;
      $(window).scroll(function() {
        if ($('#column-left').height() - $(window).height() - $(window).scrollTop() < _delta && !isFeedLoading) {
          return _load();
        }
      });
      $(document).bind('touchmove', function() {
        if ($('#column-left').height() - $(window).height() - $(window).scrollTop() < _delta && !isFeedLoading) {
          return _load();
        }
      });
      return _load();
    };

    NewsFeedLoader.prototype.load = function() {
      var jqXHR;
      isFeedLoading = true;
      if ($("#inner-left").find(".item").length < 280) {
        $('.pager-loading').show();
        jqXHR = $.getJSON("src/fixture/feed.json");
        jqXHR.done(function(json) {
          var item, _fn, _i, _len;
          _fn = function() {
            var obj;
            obj = new NewsFeedItem($("#inner-left"));
            return obj.render(item);
          };
          for (_i = 0, _len = json.length; _i < _len; _i++) {
            item = json[_i];
            _fn();
          }
          $('.pager-loading').hide();
          return isFeedLoading = false;
        });
        return jqXHR.error(function() {
          $("#main .pager").show();
          $('.pager-loading').hide();
          return isFeedLoading = false;
        });
      } else {
        $("#main .pager").show();
        return $('.pager-loading').hide();
      }
    };

    return NewsFeedLoader;

  })();

  /*
  */


  NewsLoader = (function() {
    function NewsLoader(bottomDelta) {
      this.delta = bottomDelta;
    }

    NewsLoader.prototype.register = function() {
      var _delta, _load;
      _delta = this.delta;
      _load = this.load;
      $(window).scroll(function() {
        if ($('#column-full').height() - $(window).height() - $(window).scrollTop() < _delta && !isNewsLoading) {
          return _load();
        }
      });
      $(document).bind('touchmove', function() {
        if ($('#column-full').height() - $(window).height() - $(window).scrollTop() < _delta && !isNewsLoading) {
          return _load();
        }
      });
      return _load();
    };

    NewsLoader.prototype.load = function() {
      var jqXHR;
      isNewsLoading = true;
      if ($("#inner-full").find(".item").length < 280) {
        $('.pager-loading').show();
        jqXHR = $.getJSON("src/fixture/articles.json");
        jqXHR.done(function(json) {
          var item, _fn, _i, _len;
          _fn = function() {
            var obj;
            obj = new NewsArticleItem($("#inner-full"));
            return obj.render(item);
          };
          for (_i = 0, _len = json.length; _i < _len; _i++) {
            item = json[_i];
            _fn();
          }
          $('.pager-loading').hide();
          return isNewsLoading = false;
        });
        return jqXHR.error(function() {
          $("#main .pager").show();
          $('.pager-loading').hide();
          return isNewsLoading = false;
        });
      } else {
        $("#main .pager").show();
        return $('.pager-loading').hide();
      }
    };

    return NewsLoader;

  })();

  /*
  */


  NewsArticleLoader = (function() {
    function NewsArticleLoader(bottomDelta) {
      this.delta = bottomDelta;
    }

    NewsArticleLoader.prototype.register = function() {
      var _delta, _load;
      _delta = this.delta;
      _load = this.load;
      $(window).scroll(function() {
        if ($('#column-center').height() - $(window).height() - $(window).scrollTop() < _delta && !isArticleLoading) {
          return _load();
        }
      });
      $(document).bind('touchmove', function() {
        if ($('#column-center').height() - $(window).height() - $(window).scrollTop() < _delta && !isArticleLoading) {
          return _load();
        }
      });
      return _load();
    };

    NewsArticleLoader.prototype.load = function() {
      var jqXHR;
      isArticleLoading = true;
      if ($('#inner-center').find(".item").length < 100) {
        $('.pager-loading').show();
        jqXHR = $.getJSON("src/fixture/articles.json");
        jqXHR.done(function(json) {
          var item, _fn, _i, _len;
          _fn = function() {
            var obj;
            obj = new NewsArticleItem($('#inner-center'));
            return obj.render(item);
          };
          for (_i = 0, _len = json.length; _i < _len; _i++) {
            item = json[_i];
            _fn();
          }
          $('.pager-loading').hide();
          return isArticleLoading = false;
        });
        return jqXHR.error(function() {
          $("#main .pager").show();
          $('.pager-loading').hide();
          return isArticleLoading = false;
        });
      } else {
        $("#main .pager").show();
        return $('.pager-loading').hide();
      }
    };

    return NewsArticleLoader;
  })();

  $(document).ready(function() {
    var newsArticleLoader, newsFeedLoader, newsLoader;
    newsFeedLoader = new NewsFeedLoader(500);
    newsFeedLoader.register();
    newsLoader = new NewsLoader(300);
    newsLoader.register();
    newsArticleLoader = new NewsArticleLoader(300);
    newsArticleLoader.register();
    $("#tab-news").click(function(e) {
      $("#column-center").removeClass("active");
      $("#tab-featured").removeClass("active");
      $("#tab-news").addClass("active");
      $("#column-left").addClass("active");
      return e.preventDefault;
    });
    return $("#tab-featured").click(function(e) {
      $("#column-left").removeClass("active");
      $("#tab-news").removeClass("active");
      $("#tab-featured").addClass("active");
      $("#column-center").addClass("active");
      return e.preventDefault;
    });
  });

}).call(this);
