(function() {
  var JobItem, JobsLoader, NewsArticleItem, NewsArticleLoader, NewsFeedItem, NewsFeedLoader, NewsItem, NewsLoader, RHSSignup, RegWall, Screen, Search, SearchLoader, VP50Grid, VP50GridInfoPanel, VUIDailySnapshot, bindEnter, isArticleLoading, isFeedLoading, isJobsLoading, isNewsLoading, numberWithCommas, vpgrid, _ref, _ref1, _ref2,
    __hasProp = {}.hasOwnProperty,
    __extends = function(child, parent) { for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; } function ctor() { this.constructor = child; } ctor.prototype = parent.prototype; child.prototype = new ctor(); child.__super__ = parent.prototype; return child; };

  isArticleLoading = false;

  isFeedLoading = false;

  isNewsLoading = false;

  isJobsLoading = false;

  vpgrid = null;

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

    RegWall.prototype.login = function() {};

    $('#signin .form-signin').keyup(function(event) {
      var code;
      code = event.which;
      if (code === 13) {
        $('#regwall-signin').trigger("click");
        return event.preventDefault();
      }
    });

    $('#regwall-signin').click(function(e) {
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

    RegWall.prototype.reg = function() {
      $('#register .form-signin').keyup(function(event) {
        var code;
        code = event.which;
        if (code === 13) {
          $('#regwall-register').trigger("click");
          return event.preventDefault();
        }
      });
      return $('#regwall-register').click(function(e) {
        var data, jqXHR, u, user;
        e.preventDefault();
        $('#regwall-signin-error').addClass("hidden");
        u = '/register?email=' + $('#regwall-email-reg').val() + '&page=' + $('#regwall-page').val();
        user = $('#regwall-email-reg').val();
        data = {
          "a": "uc",
          "user": user
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
            return $('#regwall-signin-error').removeClass("hidden");
          }
        });
        return jqXHR.error(function() {
          return alert(error);
        });
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
          return document.location.href = "" + target + "?q=" + term;
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


  VP50GridInfoPanel = (function() {
    function VP50GridInfoPanel(target) {
      this.target = target;
    }

    VP50GridInfoPanel.prototype.template = function(item) {
      var t;
      t = "        <div id=\"info-panel\" data-id=\"" + item.id + "\">";
      t += item.content;
      t += "<div id=\"close-button\" class=\"nav-button\"><span class=\"fa fa-times\"></span></div>";
      if (item.nextid !== -1) {
        t += "<div id=\"nav-next\" class=\"nav-button\" data-nextid=\"" + item.nextid + "\"><span class=\"fa fa-angle-right\"></span></div>";
      }
      if (item.previd !== -1) {
        t += "<div id=\"nav-previous\" class=\"nav-button\" data-previd=\"" + item.previd + "\"><span class=\"fa fa-angle-left\"></span></div>";
      }
      return t += "<div id=\"info-panel-border\"></div>     </div>    ";
    };

    VP50GridInfoPanel.prototype.render = function(item) {
      var _this;
      this.target.after(this.template(item));
      _this = this;
      $('#info-panel #close-button').click(function() {
        _this.close();
        location.hash = 'topof_' + $('#vp50-grid li[data-id="' + item.id + '"]').prop('id');
        return vpgrid.unprepare();
      });
      $('#info-panel #nav-next').click(function() {
        var next, nextid, nextindex;
        nextid = $(this).data('nextid');
        next = $('#vp50-grid li[data-id="' + nextid + '"]');
        nextindex = $('#vp50-grid li').index($(next));
        vpgrid.prepare(nextid);
        return vpgrid.show(nextid, nextindex);
      });
      return $('#info-panel #nav-previous').click(function() {
        var prev, previd, previndex;
        previd = $(this).data('previd');
        prev = $('#vp50-grid li[data-id="' + previd + '"]');
        previndex = $('#vp50-grid li').index($(prev));
        vpgrid.prepare(previd);
        return vpgrid.show(previd, previndex);
      });
    };

    VP50GridInfoPanel.prototype.close = function() {
      if ($('#info-panel').length > 0) {
        return $('#info-panel').remove();
      }
    };

    return VP50GridInfoPanel;

  })();

  /*
  */


  VP50Grid = (function() {
    function VP50Grid(grid) {
      this.grid = grid;
      this.screen = new Screen();
      this.numPerRow = 4;
      this.panel;
    }

    VP50Grid.prototype.register = function() {
      var _this = this;
      this.load();
      return $(window).on('deviceWidthChange', function() {
        return _this.redraw();
      });
    };

    VP50Grid.prototype.unprepare = function() {
      return this.grid.find('li.cell').each(function(i, el) {
        $(el).removeClass('expanded');
        return $(el).removeClass('deselected');
      });
    };

    VP50Grid.prototype.prepare = function(id) {
      return this.grid.find('li.cell').each(function(i, el) {
        if ($(el).data('id') !== id) {
          $(el).removeClass('expanded');
          return $(el).addClass('deselected');
        } else {
          $(el).addClass('expanded');
          return $(el).removeClass('deselected');
        }
      });
    };

    VP50Grid.prototype.show = function(id, currentIndex) {
      var e, i, load, _content, _existingPanelId, _i, _nextid, _previd, _ref3, _rem, _target;
      load = true;
      _existingPanelId = -1;
      if ($('#info-panel').length > 0) {
        if ($('#info-panel').data('id') !== id) {
          _existingPanelId = $('#info-panel').data('id');
          this.panel.close();
          load = true;
        } else {
          load = false;
        }
      }
      if (load) {
        if (this.screen.screenSize === 'lg') {
          this.numPerRow = 5;
        } else if (this.screen.screenSize === 'md') {
          this.numPerRow = 4;
        } else if (this.screen.screenSize === 'sm') {
          this.numPerRow = 3;
        } else {
          this.numPerRow = 2;
        }
        _rem = (currentIndex + 1) % this.numPerRow;
        _target = $('#vp50-grid li[data-id="' + id + '"]');
        _content = $(_target).find('.cell-info-panel').html();
        _nextid = -1;
        if ($(_target).next().length > 0) {
          _nextid = $(_target).next().data('id');
        }
        _previd = -1;
        if ($(_target).prev().length > 0) {
          _previd = $(_target).prev().data('id');
        }
        if (_rem > 0) {
          for (i = _i = 1, _ref3 = this.numPerRow - _rem; _i <= _ref3; i = _i += 1) {
            try {
              if ($(_target).next().length > 0) {
                _target = $(_target).next();
              }
            } catch (_error) {
              e = _error;
            }
          }
        }
        this.panel = new VP50GridInfoPanel($(_target));
        this.panel.render({
          "id": id,
          "nextid": _nextid,
          "previd": _previd,
          "content": _content
        });
        return location.hash = 'jumpto_' + $('#vp50-grid li[data-id="' + id + '"]').prop('id');
      }
    };

    VP50Grid.prototype.load = function() {
      var $this, cells, _grid, _screen,
        _this = this;
      _grid = this.grid;
      _screen = this.screen;
      $this = this;
      $('#promotion').remove();
      $('#main').removeClass().addClass('xs-col-12');
      $(window).scroll(function() {
        return $this.showHideAds();
      });
      this.redraw();
      cells = _grid.find('li.cell');
      return cells.each(function(index, e) {
        $(e).mouseenter(function() {
          return $(this).find('img').removeClass('desaturate');
        });
        $(e).mouseleave(function() {
          return $(this).find('img').addClass('desaturate');
        });
        return $(e).click(function() {
          var _id, _index;
          _id = $(this).data('id');
          _index = $('#vp50-grid li').index($(this));
          $this.prepare(_id);
          $this.show(_id, _index);
          return showHideAds();
        });
      });
    };

    VP50Grid.prototype.showHideAds = function() {
      var $ads, $gridfoot, gridInView, gridfootInView, _grid, _screen;
      _screen = this.screen;
      _grid = this.grid;
      $ads = $('#vp50-ads');
      $gridfoot = $('#vp50-foot');
      gridInView = _screen.isScrolledIntoView($(_grid), false);
      gridfootInView = _screen.isScrolledIntoView($gridfoot, false);
      if (gridInView || gridfootInView) {
        $ads.addClass('on-screen');
        if (!gridfootInView) {
          return $ads.removeClass('inline').addClass('floating');
        } else {
          return $ads.addClass('inline').removeClass('floating');
        }
      } else {
        return $ads.removeClass('on-screen').removeClass('floating').removeClass('inline');
      }
    };

    VP50Grid.prototype.redraw = function() {
      var _currentRow, _grid, _numPerRow;
      _grid = this.grid;
      if (this.screen.screenSize === 'lg') {
        this.numPerRow = 5;
      } else if (this.screen.screenSize === 'md') {
        this.numPerRow = 4;
      } else if (this.screen.screenSize === 'sm') {
        this.numPerRow = 3;
      } else {
        this.numPerRow = 2;
      }
      _currentRow = 0;
      _numPerRow = this.numPerRow;
      _grid.find('li.cell').each(function(i, el) {
        $(el).removeClass('expanded');
        $(el).removeClass('deselected');
        if (i >= (_currentRow * _numPerRow) + _numPerRow) {
          _currentRow++;
        }
        return $(el).data('row', _currentRow);
      });
      if (this.panel) {
        return this.panel.close();
      }
    };

    return VP50Grid;

  })();

  /*
  */


  Screen = (function() {
    function Screen() {
      var $s;
      this.screenSize = 'xs';
      this.detectDeviceWidthChange();
      $s = this;
      $(window).resize(function() {
        return $s.detectDeviceWidthChange();
      });
    }

    Screen.prototype.detectDeviceWidthChange = function() {
      var dw, width;
      width = $(window).width();
      if (width < 768) {
        dw = 'xs';
      } else if (width < 992) {
        dw = 'sm';
      } else if (width < 1200) {
        dw = 'md';
      } else {
        dw = 'lg';
      }
      if (dw !== this.screenSize) {
        this.screenSize = dw;
        return $(window).trigger('deviceWidthChange', [this.screenSize]);
      }
    };

    Screen.prototype.isScrolledIntoView = function(elem, requireEntirelyVisible) {
      var $elem, $window, docViewBottom, docViewTop, elemBottom, elemTop;
      $elem = $(elem);
      $window = $(window);
      docViewTop = $window.scrollTop();
      docViewBottom = docViewTop + $window.height();
      elemTop = $elem.offset().top;
      elemBottom = elemTop + $elem.height();
      if (!requireEntirelyVisible) {
        return ((elemTop >= docViewTop) && (elemTop <= docViewBottom)) || (elemTop <= docViewTop && elemBottom >= docViewBottom) || ((elemBottom >= docViewTop) && (elemBottom <= docViewBottom));
      } else {
        return (elemBottom <= docViewBottom) && (elemTop >= docViewTop);
      }
    };

    return Screen;

  })();

  /*
  */


  VUIDailySnapshot = (function() {
    function VUIDailySnapshot() {}

    VUIDailySnapshot.prototype.register = function() {
      var _load;
      _load = this.load;
      return _load();
    };

    VUIDailySnapshot.prototype.load = function() {
      var data, jqXHR;
      data = {
        "a": "dailysnap"
      };
      jqXHR = $.getJSON("/vui/vui-xml-actions", data);
      return jqXHR.done(function(json) {
        var category, n, pad, payModel, screenshot, screenshotTemplate, snap, template, ulw, w, _i, _len, _ref3;
        snap = json.data.DailySnapshot;
        payModel = snap.PayModel;
        if (payModel !== '') {
          payModel = payModel.replace(/,/g, ', ');
        }
        category = snap.Category;
        if (category !== '') {
          category = category.replace(/,/g, ', ');
        }
        template = $('#vui-service-sheet-template').clone();
        template.find('article header h1').text(snap.ServiceName).end().find('article header .intro .b-info').find('img').attr('src', snap.IconURL).end().find('table tr td[data-name="availability"]').text(snap.Availability).end().find('table tr td[data-name="pay-model"]').text(payModel).end().find('table tr td[data-name="category"]').text(category).end().end();
        if (snap.Snapshot.numScreenshots > 0) {
          template.find('article .b-stats .inner li[data-name="screenshots"]').find('span.data-point').text(snap.Snapshot.numScreenshots).end().find('span.num-devices').text(snap.Snapshot.numScreenshotDevices).end();
        } else {
          template.remove('article .b-stats .inner li[data-name="screenshots"]').remove();
        }
        if (snap.Snapshot.benchmarkAverage > 0) {
          template.find('article .b-stats .inner li[data-name="benchmark"]').find('span.data-point').text(snap.Snapshot.benchmarkAverage).end();
        } else {
          template.find('article .b-stats .inner li[data-name="benchmark"]').remove();
        }
        if (snap.Snapshot.ratingAverage > 0) {
          template.find('article .b-stats .inner li[data-name="rating"]').find('span.data-point').text(snap.Snapshot.ratingAverage).end();
        } else {
          template.find('article .b-stats .inner li[data-name="rating"]').remove();
        }
        if (snap.Snapshot.twitterFollowers > 0) {
          template.find('article .b-stats .inner li[data-name="twitter"]').find('span.data-point').text(numberWithCommas(snap.Snapshot.twitterFollowers)).end();
        } else {
          template.find('article .b-stats .inner li[data-name="twitter"]').remove();
        }
        if (snap.Snapshot.facebookLikes > 0) {
          template.find('article .b-stats .inner li[data-name="facebook"]').find('span.data-point').text(numberWithCommas(snap.Snapshot.facebookLikes)).end();
        } else {
          template.find('article .b-stats .inner li[data-name="facebook"]').remove();
        }
        if (snap.Snapshot.ytSubscriberCount > 0) {
          template.find('article .b-stats .inner li[data-name="youtube"]').find('span.data-point').text(numberWithCommas(snap.Snapshot.ytSubscriberCount)).end();
        } else {
          template.find('article .b-stats .inner li[data-name="youtube"]').remove();
        }
        if (snap.Screenshots.resultCount > 0) {
          _ref3 = snap.Screenshots.screenshots;
          for (_i = 0, _len = _ref3.length; _i < _len; _i++) {
            screenshot = _ref3[_i];
            screenshotTemplate = template.find('#vui-service-screenshot-template').clone();
            screenshotTemplate.find('.name').text(screenshot.PageType).end().find('.device').text(screenshot.Device).end().find('.img a').attr('href', screenshot.ImageURL_lg).attr('data-lightbox', 'vui-images').end().find('.img img').attr('src', screenshot.ImageURL_th).end().end();
            template.find('article .b-screenshots .carousel ul').append(screenshotTemplate.find('li'));
          }
        }
        template.find('#vui-service-screenshot-template').remove();
        $('#vui-service-sheet .loading').hide();
        template.find('article').appendTo('#vui-service-sheet');
        w = $('#vui-service-sheet article .b-screenshots .carousel ul li').first().outerWidth(true);
        n = $('#vui-service-sheet article .b-screenshots .carousel ul li').length;
        pad = parseInt($('#vui-service-sheet article .b-screenshots .carousel ul').css('padding-left').replace('px', '')) + parseInt($('#vui-service-sheet article .b-screenshots .carousel ul').css('padding-right').replace('px', ''));
        ulw = (w * n) + pad;
        $('#vui-service-sheet article .b-screenshots .carousel ul').css('width', ulw + 'px');
        $('#vui-service-sheet article .b-screenshots .carousel').jCarouselLite({
          circular: true,
          auto: false,
          timeout: 2500,
          speed: 500,
          btnNext: "#vui-service-sheet article .b-screenshots .lightbox-next",
          btnPrev: "#vui-service-sheet article .b-screenshots .lightbox-prev",
          swipe: true
        });
        return true;
      });
    };

    return VUIDailySnapshot;

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


  numberWithCommas = function(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
  };

  /*
  */


  $(document).ready(function() {
    var jobsLoader, newsLoader, regwall, rhsReg, search, searchLoader, snap;
    if ($('#vp2-regwall').length > 0) {
      regwall = new RegWall();
      regwall.register();
    }
    if ($("#inner-full").length > 0) {
      newsLoader = new NewsLoader(300);
      newsLoader.register();
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
      if ($('.form-registration span.error').length > 0) {
        $('.alert-warning').removeClass('hidden');
      }
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
    if ($('.article-main img').not('.vp-nolightbox').length > 0) {
      $('.article-main img').not('.vp-nolightbox').each(function(index, e) {
        var imgurl;
        if ($(e).parents('a, .vp-nolightbox').length === 0) {
          imgurl = $(e).attr('src');
          return $(e).wrap("<a href=\"" + imgurl + "\" data-lightbox=\"defaultgroup\" title=\"Open in lightbox\"></a>");
        }
      });
    }
    if ($('.article-main a.lightbox').not('.vp-nolightbox').length > 0) {
      $('.article-main a.lightbox').not('.vp-nolightbox').each(function(index, e) {
        return $(e).removeClass('lightbox').attr('data-lightbox', 'defaultgroup').attr('title', 'Open in lightbox');
      });
    }
    if ($('#vui-service-sheet').length > 0) {
      snap = new VUIDailySnapshot();
      snap.register();
    }
    if ($('#vp50-grid').length > 0) {
      vpgrid = new VP50Grid($('#vp50-grid'));
      vpgrid.register();
    }
    jQuery.easing.def = 'easeOutQuart';
    return true;
  });

}).call(this);
