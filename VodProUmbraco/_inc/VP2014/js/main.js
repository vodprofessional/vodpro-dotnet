(function() {
  var JobItem, JobsLoader, NewsArticleItem, NewsArticleLoader, NewsFeedItem, NewsFeedLoader, NewsItem, NewsLoader, RHSSignup, RegWall, Search, SearchLoader, bindEnter, isArticleLoading, isFeedLoading, isJobsLoading, isNewsLoading, _ref, _ref1, _ref2,
    __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  isArticleLoading = false;

  isFeedLoading = false;

  isNewsLoading = false;

  isJobsLoading = false;

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
      t = "    <article class=\"item feed-preview\" data-articleid=\"" + item.Id + "\">      <header><a href=\"" + item.Url + "\">" + item.Headline + "</a></header>";
      if (item.Image != null) {
        t += "<img src=\"" + item.Image + "\"/>";
      }
      return t += "      <div class=\"text\">        <p>" + item.Teaser + "</p>      </div>      <div class=\"clearfix\"/>     </article>    ";
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
      return "      <article class=\"item article-preview\" data-articleid=\"" + item.Id + "\">        <img src=\"" + item.Image + "\"/>        <div class=\"text\">            <header><a href=\"" + item.Url + "\">" + item.Headline + "</a></header>            <p>" + item.Teaser + "</p>        </div>        <div class=\"clearfix\"></div>      </article>    ";
    };

    NewsArticleItem.prototype.render = function(item) {
      return this.target.append(this.template(item));
    };

    return NewsArticleItem;

  })(NewsItem);

  /*
  */


  JobItem = (function(_super) {
    __extends(JobItem, _super);

    function JobItem() {
      _ref2 = JobItem.__super__.constructor.apply(this, arguments);
      return _ref2;
    }

    JobItem.prototype.template = function(item) {
      var description, metadata, _ref3, _ref4;
      description = "";
      metadata = "";
      if (((_ref3 = item.Description) != null ? _ref3.length : void 0)) {
        description = "<p>" + item.Description + "</p>";
      }
      if (((_ref4 = item.Metadata) != null ? _ref4.length : void 0)) {
        metadata = "<div class=\"meta\">" + item.Metadata + "</div>";
      }
      return "        <article class=\"item job\">            <header>                <a href=\"" + item.Url + "\" target=\"_blank\">" + item.Title + "</a>            </header>            <div class=\"text\">                <div class=\"post-date\">Posted on: " + item.Date + "</div>" + description + metadata + "<div></article>";
    };

    JobItem.prototype.render = function(item) {
      return this.target.append(this.template(item));
    };

    return JobItem;

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
      var data, jqXHR, numItems;
      isFeedLoading = true;
      numItems = $("#inner-left").find(".item").length;
      if (numItems < 280) {
        $('.pager-loading').show();
        data = {
          "a": "news",
          "c": 20,
          "s": numItems
        };
        jqXHR = $.getJSON("/ajax-actions", data);
        jqXHR.done(function(json) {
          var item, _fn, _i, _len, _ref3;
          _ref3 = json.data.Articles;
          _fn = function() {
            var obj;
            obj = new NewsFeedItem($("#inner-left"));
            return obj.render(item);
          };
          for (_i = 0, _len = _ref3.length; _i < _len; _i++) {
            item = _ref3[_i];
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
      var data, jqXHR, numItems, rootNodeId;
      isNewsLoading = true;
      numItems = $("#inner-full").find(".item").length;
      rootNodeId = $("#inner-full").attr("data-root");
      if (numItems < 280) {
        $('.pager-loading').show();
        data = {
          "a": "any",
          "r": rootNodeId,
          "c": 20,
          "s": numItems
        };
        jqXHR = $.getJSON("/ajax-actions", data);
        jqXHR.done(function(json) {
          var item, _fn, _i, _len, _ref3;
          _ref3 = json.data.Articles;
          _fn = function() {
            var obj;
            obj = new NewsArticleItem($("#inner-full"));
            return obj.render(item);
          };
          for (_i = 0, _len = _ref3.length; _i < _len; _i++) {
            item = _ref3[_i];
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
      var data, jqXHR, numItems;
      isArticleLoading = true;
      numItems = $("#inner-center").find(".item").length;
      if (numItems < 100) {
        $('.pager-loading').show();
        data = {
          "a": "features",
          "c": 20,
          "s": numItems
        };
        jqXHR = $.getJSON("/ajax-actions", data);
        jqXHR.done(function(json) {
          var item, _fn, _i, _len, _ref3;
          _ref3 = json.data.Articles;
          _fn = function() {
            var obj;
            obj = new NewsArticleItem($('#inner-center'));
            return obj.render(item);
          };
          for (_i = 0, _len = _ref3.length; _i < _len; _i++) {
            item = _ref3[_i];
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

  /*
  */


  JobsLoader = (function() {
    function JobsLoader(bottomDelta) {
      this.delta = bottomDelta;
    }

    JobsLoader.prototype.register = function() {
      var _delta, _load;
      _delta = this.delta;
      _load = this.load;
      $(window).scroll(function() {
        if ($('#column-full').height() - $(window).height() - $(window).scrollTop() < _delta && !isJobsLoading) {
          return _load();
        }
      });
      $(document).bind('touchmove', function() {
        if ($('#column-full').height() - $(window).height() - $(window).scrollTop() < _delta && !isJobsLoading) {
          return _load();
        }
      });
      return _load();
    };

    JobsLoader.prototype.load = function() {
      var data, jqXHR, numItems;
      isJobsLoading = true;
      numItems = $('#inner-job-listing').find(".item").length;
      if (numItems < 200) {
        $('.pager-loading').show();
        data = {
          "a": "jobs",
          "c": 20,
          "s": numItems
        };
        jqXHR = $.getJSON("/ajax-actions", data);
        jqXHR.done(function(json) {
          var item, _fn, _i, _len, _ref3;
          _ref3 = json.data.JobList;
          _fn = function() {
            var obj;
            obj = new JobItem($('#inner-job-listing'));
            return obj.render(item);
          };
          for (_i = 0, _len = _ref3.length; _i < _len; _i++) {
            item = _ref3[_i];
            _fn();
          }
          $('.pager-loading').hide();
          return isJobsLoading = false;
        });
        return jqXHR.error(function() {
          $("#main .pager").show();
          $('.pager-loading').hide();
          return isJobsLoading = false;
        });
      } else {
        $("#main .pager").show();
        return $('.pager-loading').hide();
      }
    };

    return JobsLoader;

  })();

  /*
  */


  RegWall = (function() {
    function RegWall() {}

    RegWall.prototype.register = function() {
      var _login, _reg;
      _login = this.login;
      _reg = this.reg;
      _login();
      return _reg();
    };

    RegWall.prototype.login = function() {
      bindEnter($('#signin'), $('#signin button[type="submit"]'));
      return $('#regwall-signin').click(function(e) {
        var data, jqXHR, pass, rem, u, user;
        e.preventDefault();
        u = document.location + '?loggedin#premium';
        user = $('#regwall-email').val();
        pass = $('#regwall-pwd').val();
        rem = false;
        data = {
          "a": "l",
          "user": user,
          "pass": pass,
          "rem": rem
        };
        jqXHR = $.ajax({
          type: 'POST',
          url: '/ajax-actions',
          data: data,
          dataType: 'JSON'
        });
        jqXHR.done(function(json) {
          if (json.response === 'valid') {
            document.location = u;
          }
          if (json.response === 'invalid') {
            return $('#regwall-pwd-error').html(json.data);
          }
        });
        return jqXHR.error(function() {
          return alert(error);
        });
      });
    };

    RegWall.prototype.reg = function() {
      bindEnter($('#register'), $('#register button[type="submit"]'));
      return $('#regwall-register').click(function(e) {
        e.preventDefault();
        return document.location = '/register?email=' + $('#regwall-email-reg').val();
      });
    };

    return RegWall;

  })();

  /*
  */


  RHSSignup = (function() {
    function RHSSignup() {}

    RHSSignup.prototype.register = function() {
      var _reg;
      _reg = this.reg;
      return _reg();
    };

    RHSSignup.prototype.reg = function() {
      bindEnter($('#rhs-form-signin'), $('#rhs-register'));
      return $('#rhs-register-btn').click(function(e) {
        e.preventDefault();
        return document.location = '/register?email=' + $('#rhs-email-reg').val();
      });
    };

    return RHSSignup;

  })();

  /*
  */


  Search = (function() {
    function Search() {}

    Search.prototype.register = function() {
      var _search;
      _search = this.search;
      return _search();
    };

    Search.prototype.search = function() {
      return $('#search-button').click(function(e) {
        var target, term;
        e.preventDefault();
        term = $("#search").val();
        target = $(this).attr("data-target");
        if (term != null ? term.length : void 0) {
          term = encodeURIComponent(term);
          return document.location.href = "" + target + "?search=" + term;
        }
      });
    };

    return Search;

  })();

  /*
  */


  SearchLoader = (function() {
    function SearchLoader(bottomDelta) {
      this.delta = bottomDelta;
    }

    SearchLoader.prototype.register = function() {
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

    SearchLoader.prototype.load = function() {
      var data, jqXHR, numItems, term;
      isNewsLoading = true;
      numItems = $('#inner-search-results').find(".item").length;
      term = $('#inner-search-results').attr('data-searchterm');
      if (numItems < 200) {
        $('.pager-loading').show();
        data = {
          "a": "sr",
          "t": term,
          "c": 20,
          "s": numItems
        };
        jqXHR = $.getJSON("/ajax-actions", data);
        jqXHR.done(function(json) {
          var item, _fn, _i, _len, _ref3;
          _ref3 = json.data.Articles;
          _fn = function() {
            var obj;
            obj = new NewsArticleItem($('#inner-search-results'));
            return obj.render(item);
          };
          for (_i = 0, _len = _ref3.length; _i < _len; _i++) {
            item = _ref3[_i];
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

    return SearchLoader;

  })();

  /*
  */


  bindEnter = function(panel, button) {
    return panel.keyup(function(event) {
      var code;
      code = event.which;
      if (code === 13) {
        button.trigger("click");
        return event.preventDefault();
      }
    });
  };

  /*
  */


  $(document).ready(function() {
    var jobsLoader, newsArticleLoader, newsFeedLoader, newsLoader, regwall, rhsReg, search, searchLoader;
    if ($('#vp2-regwall').length > 0) {
      regwall = new RegWall();
      regwall.register();
    }
    if ($("#inner-left").length > 0) {
      newsFeedLoader = new NewsFeedLoader(500);
      newsFeedLoader.register();
    }
    if ($("#inner-full").length > 0) {
      newsLoader = new NewsLoader(300);
      newsLoader.register();
    }
    if ($("#inner-center").length > 0) {
      newsArticleLoader = new NewsArticleLoader(300);
      newsArticleLoader.register();
    }
    if ($('#search-button').length > 0) {
      bindEnter($('nav .form-inline .form-group'), $("#search-button"));
      search = new Search();
      search.register();
    }
    if ($("#inner-search-results").length > 0) {
      searchLoader = new SearchLoader(300);
      searchLoader.register();
    }
    if ($("#inner-job-listing").length > 0) {
      jobsLoader = new JobsLoader(300);
      jobsLoader.register();
    }
    if ($('#rhs-register').length > 0) {
      rhsReg = new RHSSignup();
      rhsReg.register();
    }
    $("#tab-news").click(function(e) {
      $("#column-center").removeClass("active");
      $("#tab-featured").removeClass("active");
      $("#tab-news").addClass("active");
      $("#column-left").addClass("active");
      return e.preventDefault();
    });
    $("#tab-featured").click(function(e) {
      $("#column-left").removeClass("active");
      $("#tab-news").removeClass("active");
      $("#tab-featured").addClass("active");
      $("#column-center").addClass("active");
      return e.preventDefault();
    });
    if ($('.form-registration').length > 0) {
      $('.form-registration [type="submit"]').addClass("btn btn-lg btn-primary");
      if ($('.orgtype').length > 0) {
        $('.orgtype').change(function(e) {
          var orgtype;
          orgtype = $(this).val();
          if (orgtype.toLowerCase() === 'other') {
            return $('.orgtypeother').show();
          } else {
            return $('.orgtypeother').hide();
          }
        });
      }
    }
    if ($('.article-main img.vp-lightbox').length > 0) {
      $('.article-main img.vp-lightbox').each(function(index, e) {
        var imgurl;
        imgurl = $(e).attr('src');
        return $(e).wrap("<a href=\"" + imgurl + "\" data-lightbox=\"defaultgroup\"></a>");
      });
    }
    if (jQuery().lightBox) {
      return $('a.lightbox').lightBox();
    }
  });

}).call(this);
