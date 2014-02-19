$(document).ready(function () {
    //    LoadNews($("#VUI-news"));
    if ($(".news-dd-type").length > 0) {
        NewsForm($(".news-dd-type").val());
        $(".news-dd-type").change(function () {
            NewsForm($(this).val())
        });
    }


});


function NewsForm(selection) {
    if (selection == "system") {
        $(".news-dd-svc").attr("disabled", "disabled");
        $(".news-text-scrn").attr("disabled", "disabled");
        $(".news-text-vers").attr("disabled", "disabled");
        $(".news-dd-app").attr("disabled", "disabled");
        $(".news-dd-plat").attr("disabled", "disabled");
        $(".news-dd-dev").attr("disabled", "disabled");
        $(".news-text-desc").removeAttr("disabled");
    }
    if (selection == "bench") {
        $(".news-dd-svc").removeAttr("disabled");
        $(".news-text-scrn").attr("disabled", "disabled");
        $(".news-text-vers").attr("disabled", "disabled");
        $(".news-dd-app").attr("disabled", "disabled");
        $(".news-dd-plat").removeAttr("disabled");
        $(".news-dd-dev").removeAttr("disabled");
        $(".news-text-desc").attr("disabled", "disabled");
    }
    if (selection == "screen") {
        $(".news-dd-svc").removeAttr("disabled");
        $(".news-text-scrn").removeAttr("disabled");
        $(".news-text-vers").attr("disabled", "disabled");
        $(".news-dd-app").attr("disabled", "disabled");
        $(".news-dd-plat").removeAttr("disabled");
        $(".news-dd-dev").removeAttr("disabled");
        $(".news-text-desc").attr("disabled", "disabled");
    }
    if (selection == "version") {
        $(".news-dd-svc").removeAttr("disabled");
        $(".news-text-scrn").attr("disabled", "disabled");
        $(".news-text-vers").removeAttr("disabled");
        $(".news-dd-app").removeAttr("disabled");
        $(".news-dd-plat").removeAttr("disabled");
        $(".news-dd-dev").removeAttr("disabled");
        $(".news-text-desc").attr("disabled", "disabled");
    }
    if (selection == "new") {
        $(".news-dd-svc").removeAttr("disabled");
        $(".news-text-scrn").attr("disabled", "disabled");
        $(".news-text-vers").attr("disabled", "disabled");
        $(".news-dd-app").attr("disabled", "disabled");
        $(".news-dd-plat").attr("disabled", "disabled");
        $(".news-dd-dev").attr("disabled", "disabled");
        $(".news-text-desc").removeAttr("disabled");
    }
}

function LoadNews(target) {
    var xmlActionsURL = '/vui/vui-xml-actions/';
    var service = $("#search-service").val();
    var pagetype = '';
    var DataObject = { "numitems": 20 };
    $.ajax({
        url: xmlActionsURL + "?a=ln",
        type: "POST",
        dataType: 'json',
        data: DataObject,
        success: function (data) {
            for (i = 0; i < data.length; i++) {
                var result = $("#VUI-News-template div").clone();
                result
                    .find("span[item-prop=news-date]").html(data[i].DateCreated).end()
                    .find("span[item-prop=news-type]").html(data[i].NewsType).end()
                    .find("span[item-prop=description]").html(data[i].Description).end()
                    ;

                target.append(result);
            }
        }
    });
}
